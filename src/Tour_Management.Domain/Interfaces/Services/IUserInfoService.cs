using Tour_Management.Domain.DTOs;

namespace Tour_Management.Domain.Interfaces.Services;

/// <summary>
/// Service interface for UserInfo business operations.
/// </summary>
public interface IUserInfoService
{
    /// <summary>Gets all active users.</summary>
    Task<IEnumerable<UserInfoDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a user by their identifier.</summary>
    Task<UserInfoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets a user by their email address.</summary>
    Task<UserInfoDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Registers a new user.</summary>
    Task<UserInfoDto> RegisterAsync(UserInfoCreateDto createDto, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing user.</summary>
    Task UpdateAsync(int id, UserInfoUpdateDto updateDto, CancellationToken cancellationToken = default);

    /// <summary>Deletes a user by their identifier.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Validates user credentials and returns the user if valid.</summary>
    Task<UserInfoDto?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>Searches users by name or email.</summary>
    Task<IEnumerable<UserInfoDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
