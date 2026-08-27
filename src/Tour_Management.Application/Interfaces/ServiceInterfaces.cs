using Tour_Management.Application.DTOs;

namespace Tour_Management.Application.Interfaces;

/// <summary>
/// Service interface for UserInfo operations.
/// </summary>
public interface IUserService
{
    /// <summary>Gets all users asynchronously.</summary>
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a user by email asynchronously.</summary>
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Creates a new user asynchronously.</summary>
    Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing user asynchronously.</summary>
    Task<UserDto> UpdateAsync(string email, UserUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Deletes a user by email asynchronously.</summary>
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Validates user login credentials asynchronously.</summary>
    Task<UserDto?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>Searches users by name or email asynchronously.</summary>
    Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for Tour operations.
/// </summary>
public interface ITourService
{
    /// <summary>Gets all active tours asynchronously.</summary>
    Task<IEnumerable<TourDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a tour by ID asynchronously.</summary>
    Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new tour asynchronously.</summary>
    Task<TourDto> CreateAsync(TourCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing tour asynchronously.</summary>
    Task<TourDto> UpdateAsync(int id, TourUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Deletes a tour by ID asynchronously.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Searches tours by name or place asynchronously.</summary>
    Task<IEnumerable<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for Booking operations.
/// </summary>
public interface IBookingService
{
    /// <summary>Gets all bookings asynchronously.</summary>
    Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a booking by ID asynchronously.</summary>
    Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets all bookings for a specific user asynchronously.</summary>
    Task<IEnumerable<BookingDto>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Creates a new booking asynchronously.</summary>
    Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing booking asynchronously.</summary>
    Task<BookingDto> UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Deletes a booking by ID asynchronously.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Searches bookings by tour name or user email asynchronously.</summary>
    Task<IEnumerable<BookingDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
