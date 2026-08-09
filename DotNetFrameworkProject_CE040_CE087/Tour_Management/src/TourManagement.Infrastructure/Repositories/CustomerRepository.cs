using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories;

public sealed class CustomerRepository(TourManagementDbContext dbContext) : ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Customers.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.FirstName).ToListAsync(cancellationToken);

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email && c.IsActive, cancellationToken);

    public async Task<Customer> AddAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        dbContext.Customers.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        entity.ModifiedDate = DateTime.UtcNow;
        dbContext.Customers.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Customers.FindAsync([id], cancellationToken);
        if (entity is null) return;
        entity.IsActive = false;
        entity.ModifiedDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) => await dbContext.Customers.AnyAsync(c => c.Id == id && c.IsActive, cancellationToken);

    public async Task<IReadOnlyList<Customer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        searchTerm = searchTerm.Trim();
        return await dbContext.Customers.AsNoTracking().Where(c => c.IsActive && (c.Email.Contains(searchTerm) || c.FirstName.Contains(searchTerm) || c.LastName.Contains(searchTerm))).ToListAsync(cancellationToken);
    }
}
