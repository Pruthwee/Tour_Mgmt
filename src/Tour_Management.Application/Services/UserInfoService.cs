using AutoMapper;
using Microsoft.Extensions.Logging;
using Tour_Management.Domain.DTOs;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Exceptions;
using Tour_Management.Domain.Interfaces.Repositories;
using Tour_Management.Domain.Interfaces.Services;

namespace Tour_Management.Application.Services;

/// <summary>
/// Service implementation for UserInfo business operations.
/// </summary>
public class UserInfoService : IUserInfoService
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserInfoService> _logger;

    public UserInfoService(IUserInfoRepository userInfoRepository, IMapper mapper, ILogger<UserInfoService> logger)
    {
        _userInfoRepository = userInfoRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserInfoDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            var users = await _userInfoRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<UserInfoDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with ID {UserId}", id);
            var user = await _userInfoRepository.GetByIdAsync(id, cancellationToken);
            return user is null ? null : _mapper.Map<UserInfoDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfoDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email {Email}", email);
            var user = await _userInfoRepository.GetByEmailAsync(email, cancellationToken);
            return user is null ? null : _mapper.Map<UserInfoDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfoDto> RegisterAsync(UserInfoCreateDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Registering new user with email {Email}", createDto.Email);
            var existing = await _userInfoRepository.GetByEmailAsync(createDto.Email, cancellationToken);
            if (existing is not null)
                throw new DuplicateEntityException(nameof(UserInfo), "Email", createDto.Email);

            var user = _mapper.Map<UserInfo>(createDto);
            // Hash the password before storing
            user.Password = BCrypt.Net.BCrypt.HashPassword(createDto.Password);
            var created = await _userInfoRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("User registered successfully with ID {UserId}", created.UserInfoId);
            return _mapper.Map<UserInfoDto>(created);
        }
        catch (DuplicateEntityException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user with email {Email}", createDto.Email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(int id, UserInfoUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with ID {UserId}", id);
            var existing = await _userInfoRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(UserInfo), id);
            _mapper.Map(updateDto, existing);
            await _userInfoRepository.UpdateAsync(existing, cancellationToken);
            _logger.LogInformation("User with ID {UserId} updated successfully", id);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID {UserId}", id);
            var exists = await _userInfoRepository.ExistsAsync(id, cancellationToken);
            if (!exists)
                throw new NotFoundException(nameof(UserInfo), id);
            await _userInfoRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("User with ID {UserId} deleted successfully", id);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfoDto?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for email {Email}", email);
            var user = await _userInfoRepository.GetByEmailAsync(email, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning("Login failed: user with email {Email} not found", email);
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isValid)
            {
                _logger.LogWarning("Login failed: invalid password for email {Email}", email);
                return null;
            }

            _logger.LogInformation("Login successful for email {Email}", email);
            return _mapper.Map<UserInfoDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserInfoDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            var users = await _userInfoRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<UserInfoDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
