using System;
using System.Collections.Generic;
using Xunit;
using Tour_Management.Domain.Entities;

namespace Tour_Management.UnitTests.Entities;

public class BookingTests
{
    [Fact]
    public void Booking_DefaultConstructor_SetsDefaultValues()
    {
        var booking = new Booking();
        Assert.Equal(0, booking.BookingId);
        Assert.Equal(string.Empty, booking.TourName);
        Assert.Equal(string.Empty, booking.Place);
        Assert.Equal(string.Empty, booking.Email);
        Assert.Equal(string.Empty, booking.FirstName);
        Assert.Null(booking.TourId);
        Assert.True(booking.IsActive);
        Assert.Null(booking.Tour);
        Assert.Null(booking.User);
    }

    [Fact]
    public void Booking_BookingDate_DefaultsToUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var booking = new Booking();
        var after = DateTime.UtcNow.AddSeconds(1);
        Assert.True(booking.BookingDate >= before && booking.BookingDate <= after);
    }

    [Fact]
    public void Booking_SetProperties_ReturnsCorrectValues()
    {
        var booking = new Booking
        {
            BookingId = 1,
            TourName = "Paris Tour",
            Place = "Paris",
            Email = "user@example.com",
            FirstName = "John",
            TourId = 5,
            BookingDate = new DateTime(2024, 1, 15),
            IsActive = true
        };
        Assert.Equal(1, booking.BookingId);
        Assert.Equal("Paris Tour", booking.TourName);
        Assert.Equal("Paris", booking.Place);
        Assert.Equal("user@example.com", booking.Email);
        Assert.Equal("John", booking.FirstName);
        Assert.Equal(5, booking.TourId);
        Assert.Equal(new DateTime(2024, 1, 15), booking.BookingDate);
        Assert.True(booking.IsActive);
    }

    [Fact]
    public void Booking_IsActive_CanBeSetToFalse()
    {
        var booking = new Booking { IsActive = false };
        Assert.False(booking.IsActive);
    }

    [Fact]
    public void Booking_TourId_CanBeNull()
    {
        var booking = new Booking { TourId = null };
        Assert.Null(booking.TourId);
    }

    [Fact]
    public void Booking_NavigationProperty_Tour_CanBeSet()
    {
        var tour = new Tour { TourId = 1, TourName = "Paris Tour" };
        var booking = new Booking { Tour = tour };
        Assert.NotNull(booking.Tour);
        Assert.Equal(1, booking.Tour.TourId);
    }

    [Fact]
    public void Booking_NavigationProperty_User_CanBeSet()
    {
        var user = new UserInfo { Email = "user@example.com", FirstName = "John" };
        var booking = new Booking { User = user };
        Assert.NotNull(booking.User);
        Assert.Equal("user@example.com", booking.User.Email);
    }

    [Fact]
    public void Booking_TourName_DefaultIsEmptyString()
    {
        var booking = new Booking();
        Assert.NotNull(booking.TourName);
        Assert.Equal(string.Empty, booking.TourName);
    }

    [Fact]
    public void Booking_Email_CanBeAssigned()
    {
        var booking = new Booking { Email = "test@test.com" };
        Assert.Equal("test@test.com", booking.Email);
    }
}
