using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Repositories;
using Tour_Management.Infrastructure.Data;

namespace Tour_Management.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for UserInfo entity.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly TourManagementDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    /// <summary>Initializes a new instance of <see cref="UserRepository"/>.</summary>
    public UserRepository(TourManagementDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserInfos.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.UserInfos.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<UserInfo> AddAsync(UserInfo user, CancellationToken cancellationToken = default)
    {
        _context.UserInfos.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    /// <inheritdoc/>
    public async Task<UserInfo> UpdateAsync(UserInfo user, CancellationToken cancellationToken = default)
    {
        _context.UserInfos.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _context.UserInfos.FindAsync(new object[] { email }, cancellationToken);
        if (user != null)
        {
            _context.UserInfos.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.UserInfos.AnyAsync(u => u.Email == email, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<UserInfo?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        return await _context.UserInfos.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var term = searchTerm.ToLower();
        return await _context.UserInfos.AsNoTracking()
            .Where(u => u.Email.ToLower().Contains(term)
                     || u.FirstName.ToLower().Contains(term)
                     || u.LastName.ToLower().Contains(term))
            .ToListAsync(cancellationToken);
    }
}
