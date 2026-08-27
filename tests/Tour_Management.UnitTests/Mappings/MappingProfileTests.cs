using System;
using AutoMapper;
using Xunit;
using Tour_Management.Application.DTOs;
using Tour_Management.Application.Mappings;
using Tour_Management.Domain.Entities;

namespace Tour_Management.UnitTests.Mappings;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void MappingProfile_CanBeCreated_Successfully()
    {
        // Verify the mapper can be created without errors
        Assert.NotNull(_mapper);
    }

    [Fact]
    public void Map_UserInfo_To_UserDto_MapsAllProperties()
    {
        var user = new UserInfo
        {
            Email = "john@example.com", FirstName = "John", LastName = "Doe",
            Gender = "Male", Password = "hashedpassword", Dob = new DateTime(1990, 5, 15),
            Street = "123 Main St", City = "New York", State = "NY",
            Role = "User", CreatedDate = new DateTime(2024, 1, 1)
        };

        var dto = _mapper.Map<UserDto>(user);

        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.FirstName, dto.FirstName);
        Assert.Equal(user.LastName, dto.LastName);
        Assert.Equal(user.Gender, dto.Gender);
        Assert.Equal(user.Dob, dto.Dob);
        Assert.Equal(user.Street, dto.Street);
        Assert.Equal(user.City, dto.City);
        Assert.Equal(user.State, dto.State);
        Assert.Equal(user.Role, dto.Role);
        Assert.Equal(user.CreatedDate, dto.CreatedDate);
    }

    [Fact]
    public void Map_UserCreateDto_To_UserInfo_MapsAllProperties()
    {
        var dto = new UserCreateDto
        {
            Email = "jane@example.com", FirstName = "Jane", LastName = "Smith",
            Gender = "Female", Password = "password123", Dob = new DateTime(1995, 3, 20),
            Street = "456 Oak Ave", City = "Chicago", State = "IL"
        };

        var user = _mapper.Map<UserInfo>(dto);

        Assert.Equal(dto.Email, user.Email);
        Assert.Equal(dto.FirstName, user.FirstName);
        Assert.Equal(dto.LastName, user.LastName);
        Assert.Equal(dto.Gender, user.Gender);
        Assert.Equal(dto.Dob, user.Dob);
        Assert.Equal(dto.Street, user.Street);
        Assert.Equal(dto.City, user.City);
        Assert.Equal(dto.State, user.State);
        Assert.Equal("User", user.Role);
    }

    [Fact]
    public void Map_UserUpdateDto_To_UserInfo_UpdatesProperties()
    {
        var existing = new UserInfo
        {
            Email = "john@example.com", Password = "hashedpassword",
            Role = "User", CreatedDate = new DateTime(2024, 1, 1)
        };
        var dto = new UserUpdateDto
        {
            FirstName = "Johnny", LastName = "Doe", Gender = "Male",
            Dob = new DateTime(1990, 1, 1), Street = "New St", City = "New City", State = "NS"
        };

        _mapper.Map(dto, existing);

        Assert.Equal("Johnny", existing.FirstName);
        Assert.Equal("Doe", existing.LastName);
        Assert.Equal("Male", existing.Gender);
        Assert.Equal("john@example.com", existing.Email);
        Assert.Equal("hashedpassword", existing.Password);
        Assert.Equal("User", existing.Role);
    }

    [Fact]
    public void Map_Tour_To_TourDto_MapsAllProperties()
    {
        var tour = new Tour
        {
            TourId = 1, TourName = "Paris Tour", Place = "Paris", Days = 7,
            Price = 1500m, Locations = "Eiffel Tower", TourInfo = "Great tour",
            Pic = "paris.jpg", IsActive = true, CreatedDate = new DateTime(2024, 1, 1),
            ModifiedDate = new DateTime(2024, 2, 1)
        };

        var dto = _mapper.Map<TourDto>(tour);

        Assert.Equal(tour.TourId, dto.TourId);
        Assert.Equal(tour.TourName, dto.TourName);
        Assert.Equal(tour.Place, dto.Place);
        Assert.Equal(tour.Days, dto.Days);
        Assert.Equal(tour.Price, dto.Price);
        Assert.Equal(tour.Locations, dto.Locations);
        Assert.Equal(tour.TourInfo, dto.TourInfo);
        Assert.Equal(tour.Pic, dto.Pic);
        Assert.Equal(tour.IsActive, dto.IsActive);
        Assert.Equal(tour.CreatedDate, dto.CreatedDate);
        Assert.Equal(tour.ModifiedDate, dto.ModifiedDate);
    }

    [Fact]
    public void Map_TourCreateDto_To_Tour_MapsAllProperties()
    {
        var dto = new TourCreateDto
        {
            TourName = "London Tour", Place = "London", Days = 5, Price = 1200m,
            Locations = "Big Ben", TourInfo = "Great London tour", Pic = "london.jpg"
        };

        var tour = _mapper.Map<Tour>(dto);

        Assert.Equal(dto.TourName, tour.TourName);
        Assert.Equal(dto.Place, tour.Place);
        Assert.Equal(dto.Days, tour.Days);
        Assert.Equal(dto.Price, tour.Price);
        Assert.Equal(dto.Locations, tour.Locations);
        Assert.Equal(dto.TourInfo, tour.TourInfo);
        Assert.Equal(dto.Pic, tour.Pic);
        Assert.True(tour.IsActive);
        Assert.Equal(0, tour.TourId);
    }

    [Fact]
    public void Map_TourUpdateDto_To_Tour_UpdatesProperties()
    {
        var existing = new Tour { TourId = 1, CreatedDate = new DateTime(2024, 1, 1) };
        var dto = new TourUpdateDto
        {
            TourName = "Updated Tour", Place = "Updated Place", Days = 10,
            Price = 2000m, Locations = "Updated Loc", TourInfo = "Updated Info", IsActive = true
        };

        _mapper.Map(dto, existing);

        Assert.Equal("Updated Tour", existing.TourName);
        Assert.Equal("Updated Place", existing.Place);
        Assert.Equal(10, existing.Days);
        Assert.Equal(2000m, existing.Price);
        Assert.Equal(1, existing.TourId);
        Assert.Equal(new DateTime(2024, 1, 1), existing.CreatedDate);
    }

    [Fact]
    public void Map_Booking_To_BookingDto_MapsAllProperties()
    {
        var booking = new Booking
        {
            BookingId = 1, TourName = "Paris Tour", Place = "Paris",
            Email = "user@example.com", FirstName = "John", TourId = 5,
            BookingDate = new DateTime(2024, 3, 15), IsActive = true
        };

        var dto = _mapper.Map<BookingDto>(booking);

        Assert.Equal(booking.BookingId, dto.BookingId);
        Assert.Equal(booking.TourName, dto.TourName);
        Assert.Equal(booking.Place, dto.Place);
        Assert.Equal(booking.Email, dto.Email);
        Assert.Equal(booking.FirstName, dto.FirstName);
        Assert.Equal(booking.TourId, dto.TourId);
        Assert.Equal(booking.BookingDate, dto.BookingDate);
        Assert.Equal(booking.IsActive, dto.IsActive);
    }

    [Fact]
    public void Map_BookingCreateDto_To_Booking_MapsAllProperties()
    {
        var dto = new BookingCreateDto
        {
            TourName = "Rome Tour", Place = "Rome",
            Email = "user@example.com", FirstName = "Alice", TourId = 3
        };

        var booking = _mapper.Map<Booking>(dto);

        Assert.Equal(dto.TourName, booking.TourName);
        Assert.Equal(dto.Place, booking.Place);
        Assert.Equal(dto.Email, booking.Email);
        Assert.Equal(dto.FirstName, booking.FirstName);
        Assert.Equal(dto.TourId, booking.TourId);
        Assert.True(booking.IsActive);
        Assert.Equal(0, booking.BookingId);
    }

    [Fact]
    public void Map_BookingUpdateDto_To_Booking_UpdatesProperties()
    {
        var existing = new Booking { BookingId = 1, BookingDate = new DateTime(2024, 1, 1) };
        var dto = new BookingUpdateDto
        {
            TourName = "Updated Tour", Place = "Updated Place",
            Email = "updated@example.com", FirstName = "Updated", IsActive = false
        };

        _mapper.Map(dto, existing);

        Assert.Equal("Updated Tour", existing.TourName);
        Assert.Equal("Updated Place", existing.Place);
        Assert.Equal("updated@example.com", existing.Email);
        Assert.Equal("Updated", existing.FirstName);
        Assert.False(existing.IsActive);
        Assert.Equal(1, existing.BookingId);
        Assert.Equal(new DateTime(2024, 1, 1), existing.BookingDate);
    }
}
