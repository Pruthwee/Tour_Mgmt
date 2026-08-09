using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Application.Validators;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

public sealed class CustomerService(ICustomerRepository repository, ILogger<CustomerService> logger) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default) => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => (await repository.GetByIdAsync(id, cancellationToken)) is { } customer ? Map(customer) : null;
    public async Task<IReadOnlyList<CustomerDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default) => (await repository.SearchAsync(searchTerm ?? string.Empty, cancellationToken)).Select(Map).ToList();
    public async Task<CustomerDto> CreateAsync(CustomerCreateDto dto, CancellationToken cancellationToken = default)
    {
        ValidationHelper.Validate(dto);
        if (await repository.GetByEmailAsync(dto.Email, cancellationToken) is not null) throw new InvalidOperationException("A customer with this email already exists.");
        var entity = new Customer { Email = dto.Email.Trim(), FirstName = dto.FirstName.Trim(), LastName = dto.LastName.Trim(), Gender = dto.Gender, DateOfBirth = dto.DateOfBirth, Street = dto.Street, City = dto.City, State = dto.State };
        entity = await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("Created customer {CustomerId}", entity.Id);
        return Map(entity);
    }
    public async Task UpdateAsync(int id, CustomerUpdateDto dto, CancellationToken cancellationToken = default)
    {
        ValidationHelper.Validate(dto);
        var entity = await repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Customer {id} was not found.");
        entity.Email = dto.Email.Trim(); entity.FirstName = dto.FirstName.Trim(); entity.LastName = dto.LastName.Trim(); entity.Gender = dto.Gender; entity.DateOfBirth = dto.DateOfBirth; entity.Street = dto.Street; entity.City = dto.City; entity.State = dto.State;
        await repository.UpdateAsync(entity, cancellationToken);
    }
    public Task DeleteAsync(int id, CancellationToken cancellationToken = default) => repository.DeleteAsync(id, cancellationToken);
    private static CustomerDto Map(Customer customer) => new(customer.Id, customer.Email, customer.FirstName, customer.LastName, customer.City, customer.State);
}
