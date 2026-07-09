using Xunit;
using TourManagement.Domain.Entities;
using System;

namespace TourManagement.Domain.Entities.Tests
{
    public class TourTests
    {
        [Fact]
        public void Tour_Properties_ShouldSetAndGet()
        {
            // Arrange
            var tour = new Tour
            {
                TourId = 1,
                TourName = "Paris Getaway",
                Description = "A beautiful tour of Paris",
                Price = 1200.00m,
                Location = "France",
                DurationDays = 5,
                ImagePath = "paris.jpg"
            };

            // Assert
            Assert.Equal(1, tour.TourId);
            Assert.Equal("Paris Getaway", tour.TourName);
            Assert.Equal("A beautiful tour of Paris", tour.Description);
            Assert.Equal(1200.00m, tour.Price);
            Assert.Equal("France", tour.Location);
            Assert.Equal(5, tour.DurationDays);
            Assert.Equal("paris.jpg", tour.ImagePath);
        }
    }

    public class UserTests
    {
        [Fact]
        public void User_Properties_ShouldSetAndGet()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                Password = "password123",
                Email = "test@example.com",
                Role = "Admin",
                FullName = "Test User"
            };

            // Assert
            Assert.Equal(1, user.UserId);
            Assert.Equal("testuser", user.Username);
            Assert.Equal("password123", user.Password);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("Admin", user.Role);
            Assert.Equal("Test User", user.FullName);
        }
    }

    public class BookingTests
    {
        [Fact]
        public void Booking_Properties_ShouldSetAndGet()
        {
            // Arrange
            var tour = new Tour { TourId = 1, TourName = "Tour 1" };
            var user = new User { UserId = 1, Username = "User 1" };
            var booking = new Booking
            {
                BookingId = 1,
                UserId = 1,
                TourId = 1,
                BookingDate = DateTime.Now,
                NumberOfPeople = 2,
                TotalPrice = 2400.00m,
                Status = "Confirmed",
                User = user,
                Tour = tour
            };

            // Assert
            Assert.Equal(1, booking.BookingId);
            Assert.Equal(1, booking.UserId);
            Assert.Equal(1, booking.TourId);
            Assert.Equal(2, booking.NumberOfPeople);
            Assert.Equal(2400.00m, booking.TotalPrice);
            Assert.Equal("Confirmed", booking.Status);
            Assert.NotNull(booking.User);
            Assert.NotNull(booking.Tour);
        }
    }
}
