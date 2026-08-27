using System;
using System.Collections.Generic;
using Xunit;
using Tour_Management.Domain.Entities;

namespace Tour_Management.Domain.Entities.Tests
{
    public class UserInfoTests
    {
        [Fact]
        public void UserInfo_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var user = new UserInfo();

            // Assert
            Assert.Equal(string.Empty, user.Email);
            Assert.Equal(string.Empty, user.FirstName);
            Assert.Equal(string.Empty, user.LastName);
            Assert.Equal(string.Empty, user.Gender);
            Assert.Equal(string.Empty, user.Password);
            Assert.Equal(string.Empty, user.Street);
            Assert.Equal(string.Empty, user.City);
            Assert.Equal(string.Empty, user.State);
            Assert.Equal("User", user.Role);
            Assert.NotNull(user.Bookings);
        }

        [Fact]
        public void UserInfo_CreatedDate_DefaultsToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var user = new UserInfo();
            var after = DateTime.UtcNow.AddSeconds(1);

            // Assert
            Assert.True(user.CreatedDate >= before && user.CreatedDate <= after);
        }

        [Fact]
        public void UserInfo_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var dob = new DateTime(1990, 5, 15);
            var user = new UserInfo
            {
                Email = "john@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "hashedpassword",
                Dob = dob,
                Street = "123 Main St",
                City = "New York",
                State = "NY",
                Role = "Admin"
            };

            // Assert
            Assert.Equal("john@example.com", user.Email);
            Assert.Equal("John", user.FirstName);
            Assert.Equal("Doe", user.LastName);
            Assert.Equal("Male", user.Gender);
            Assert.Equal("hashedpassword", user.Password);
            Assert.Equal(dob, user.Dob);
            Assert.Equal("123 Main St", user.Street);
            Assert.Equal("New York", user.City);
            Assert.Equal("NY", user.State);
            Assert.Equal("Admin", user.Role);
        }

        [Fact]
        public void UserInfo_Role_DefaultsToUser()
        {
            // Arrange & Act
            var user = new UserInfo();

            // Assert
            Assert.Equal("User", user.Role);
        }

        [Fact]
        public void UserInfo_Role_CanBeSetToAdmin()
        {
            // Arrange
            var user = new UserInfo { Role = "Admin" };

            // Assert
            Assert.Equal("Admin", user.Role);
        }

        [Fact]
        public void UserInfo_Bookings_DefaultsToEmptyList()
        {
            // Arrange & Act
            var user = new UserInfo();

            // Assert
            Assert.NotNull(user.Bookings);
            Assert.Empty(user.Bookings);
        }

        [Fact]
        public void UserInfo_Bookings_CanAddBookings()
        {
            // Arrange
            var user = new UserInfo();
            var booking = new Booking { BookingId = 1, Email = "john@example.com" };

            // Act
            user.Bookings.Add(booking);

            // Assert
            Assert.Single(user.Bookings);
        }

        [Fact]
        public void UserInfo_Email_CanBeAssigned()
        {
            // Arrange
            var user = new UserInfo { Email = "test@test.com" };

            // Assert
            Assert.Equal("test@test.com", user.Email);
        }

        [Fact]
        public void UserInfo_Dob_CanBeSet()
        {
            // Arrange
            var dob = new DateTime(1985, 3, 20);
            var user = new UserInfo { Dob = dob };

            // Assert
            Assert.Equal(dob, user.Dob);
        }

        [Fact]
        public void UserInfo_Gender_CanBeSetToFemale()
        {
            // Arrange
            var user = new UserInfo { Gender = "Female" };

            // Assert
            Assert.Equal("Female", user.Gender);
        }
    }
}
