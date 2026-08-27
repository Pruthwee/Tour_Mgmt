using System;
using System.Collections.Generic;
using Xunit;
using Tour_Management.Domain.Entities;

namespace Tour_Management.UnitTests.Entities;

public class TourEntityTests
{
    [Fact]
    public void Tour_DefaultConstructor_SetsDefaultValues()
    {
        var tour = new Tour();
        Assert.Equal(0, tour.TourId);
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(0, tour.Days);
        Assert.Equal(0m, tour.Price);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Equal(string.Empty, tour.TourInfo);
        Assert.Null(tour.Pic);
        Assert.True(tour.IsActive);
        Assert.Null(tour.ModifiedDate);
        Assert.NotNull(tour.Bookings);
    }

    [Fact]
    public void Tour_CreatedDate_DefaultsToUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var tour = new Tour();
        var after = DateTime.UtcNow.AddSeconds(1);
        Assert.True(tour.CreatedDate >= before && tour.CreatedDate <= after);
    }

    [Fact]
    public void Tour_SetProperties_ReturnsCorrectValues()
    {
        var tour = new Tour
        {
            TourId = 1,
            TourName = "Paris Tour",
            Place = "Paris",
            Days = 7,
            Price = 1500.00m,
            Locations = "Eiffel Tower, Louvre",
            TourInfo = "A wonderful tour of Paris",
            Pic = "paris.jpg",
            IsActive = true,
            CreatedDate = new DateTime(2024, 1, 1),
            ModifiedDate = new DateTime(2024, 2, 1)
        };
        Assert.Equal(1, tour.TourId);
        Assert.Equal("Paris Tour", tour.TourName);
        Assert.Equal("Paris", tour.Place);
        Assert.Equal(7, tour.Days);
        Assert.Equal(1500.00m, tour.Price);
        Assert.Equal("Eiffel Tower, Louvre", tour.Locations);
        Assert.Equal("A wonderful tour of Paris", tour.TourInfo);
        Assert.Equal("paris.jpg", tour.Pic);
        Assert.True(tour.IsActive);
    }

    [Fact]
    public void Tour_IsActive_CanBeSetToFalse()
    {
        var tour = new Tour { IsActive = false };
        Assert.False(tour.IsActive);
    }

    [Fact]
    public void Tour_Bookings_DefaultsToEmptyList()
    {
        var tour = new Tour();
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_Bookings_CanAddBookings()
    {
        var tour = new Tour();
        var booking = new Booking { BookingId = 1, TourName = "Paris Tour" };
        tour.Bookings.Add(booking);
        Assert.Single(tour.Bookings);
    }

    [Fact]
    public void Tour_Pic_CanBeNull()
    {
        var tour = new Tour { Pic = null };
        Assert.Null(tour.Pic);
    }

    [Fact]
    public void Tour_Price_CanBeDecimal()
    {
        var tour = new Tour { Price = 999.99m };
        Assert.Equal(999.99m, tour.Price);
    }

    [Fact]
    public void Tour_ModifiedDate_CanBeSet()
    {
        var modifiedDate = new DateTime(2024, 6, 15);
        var tour = new Tour { ModifiedDate = modifiedDate };
        Assert.Equal(modifiedDate, tour.ModifiedDate);
    }

    [Fact]
    public void Tour_Days_CanBeSet()
    {
        var tour = new Tour { Days = 14 };
        Assert.Equal(14, tour.Days);
    }
}
