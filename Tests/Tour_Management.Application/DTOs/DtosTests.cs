using System;
using System.Collections.Generic;
using Xunit;
using Tour_Management.Application.DTOs;

namespace Tour_Management.Application.DTOs.Tests
{
    public class DtosTests
    {
        // UserDto Tests
        [Fact]
        public void UserDto_DefaultConstructor_SetsDefaultValues()
        {
            var dto = new UserDto();
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(string.Empty, dto.FirstName);
            Assert.Equal(string.Empty, dto.LastName);
            Assert.Equal(string.Empty, dto.Gender);
            Assert.Equal(string.Empty, dto.Street);
            Assert.Equal(string.Empty, dto.City);
            Assert.Equal(string.Empty, dto.State);
            Assert.Equal("User", dto.Role);
        }

        [Fact]
        public void UserDto_SetProperties_ReturnsCorrectValues()
        {
            var dob = new DateTime(1990, 1, 1);
            var created = new DateTime(2024, 1, 1);
            var dto = new UserDto
            {
                Email = "john@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Dob = dob,
                Street = "123 Main St",
                City = "NYC",
                State = "NY",
                Role = "Admin",
                CreatedDate = created
            };

            Assert.Equal("john@example.com", dto.Email);
            Assert.Equal("John", dto.FirstName);
            Assert.Equal("Doe", dto.LastName);
            Assert.Equal("Male", dto.Gender);
            Assert.Equal(dob, dto.Dob);
            Assert.Equal("123 Main St", dto.Street);
            Assert.Equal("NYC", dto.City);
            Assert.Equal("NY", dto.State);
            Assert.Equal("Admin", dto.Role);
            Assert.Equal(created, dto.CreatedDate);
        }

        // UserCreateDto Tests
        [Fact]
        public void UserCreateDto_DefaultConstructor_SetsDefaultValues()
        {
            var dto = new UserCreateDto();
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(string.Empty, dto.FirstName);
            Assert.Equal(string.Empty, dto.LastName);
            Assert.Equal(string.Empty, dto.Gender);
            Assert.Equal(string.Empty, dto.Password);
            Assert.Equal(string.Empty, dto.Street);
            Assert.Equal(string.Empty, dto.City);
            Assert.Equal(string.Empty, dto.State);
        }

        [Fact]
        public void UserCreateDto_SetProperties_ReturnsCorrectValues()
        {
            var dto = new UserCreateDto
            {
                Email = "test@test.com",
                FirstName = "Jane",
                LastName = "Smith",
                Gender = "Female",
                Password = "password123",
                Dob = new DateTime(1995, 6, 20),
                Street = "456 Oak Ave",
                City = "Chicago",
                State = "IL"
            };

            Assert.Equal("test@test.com", dto.Email);
            Assert.Equal("Jane", dto.FirstName);
            Assert.Equal("Smith", dto.LastName);
            Assert.Equal("Female", dto.Gender);
            Assert.Equal("password123", dto.Password);
            Assert.Equal("456 Oak Ave", dto.Street);
            Assert.Equal("Chicago", dto.City);
            Assert.Equal("IL", dto.State);
        }

        // UserUpdateDto Tests
        [Fact]
        public void UserUpdateDto_DefaultConstructor_SetsDefaultValues()
        {
            var dto = new UserUpdateDto();
            Assert.Equal(string.Empty, dto.FirstName);
            Assert.Equal(string.Empty, dto.LastName);
            Assert.Equal(string.Empty, dto.Gender);
            Assert.Equal(string.Empty, dto.Street);
            Assert.Equal(string.Empty, dto.City);
            Assert.Equal(string.Empty, dto.State);
        }

        // TourDto Tests
        [Fact]
        public void TourDto_DefaultConstructor_SetsDefaultValues()
        {
            var dto = new TourDto();
            Assert.Equal(0, dto.TourId);
            Assert.Equal(string.Empty, dto.TourName);
            Assert.Equal(string.Empty, dto.Place);
            Assert.Equal(0, dto.Days);
            Assert.Equal(0m, dto.Price);
            Assert.Equal(string.Empty, dto.Locations);
            Assert.Equal(string.Empty, dto.TourInfo);
            Assert.Null(dto.Pic);
            Assert.Null(dto.ModifiedDate);
        }

        [Fact]
        public void TourDto_SetProperties_ReturnsCorrectValues()
        {
            var created = new DateTime(2024, 1, 1);
            var modified = new DateTime(2024, 2, 1);
            var dto = new TourDto
            {
                TourId = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500m,
                Locations = "Eiffel Tower",
                TourInfo = "Great tour",
                Pic = "paris.jpg",
                IsActive = true,
                CreatedDate = created,
                ModifiedDate = modified
            };

            Assert.Equal(1, dto.TourId);
            Assert.Equal("Paris Tour", dto.TourName);
            Assert.Equal("Paris", dto.Place);
            Assert.Equal(7, dto.Days);
            Assert.Equal(1500m, dto.Price);
            Assert.Equal("Eiffel Tower", dto.Locations);
            Assert.Equal("Great tour", dto.TourInfo);
            Assert.Equal("paris.jpg", dto.Pic);
            Assert.True(dto.IsActive);
            Assert.Equal(created, dto.CreatedDate);
            Assert.Equal(modified, dto.ModifiedDate);
        }

        // TourCreateDto Tests
        [Fact]
        public void TourCreateDto_DefaultConstructor_SetsDefaultValues()
        {
            var dto = new TourCreateDto();
            Assert.Equal(string.Empty, dto.TourName);
            Assert.Equal(string.Empty, dto.Place);
            Assert.Equal(0, dto.Days);
            Assert.Equal(0m, dto.Price);
            Assert.Equal(string.Empty, dto.Locations);
            Assert.Equal(string.Empty, dto.TourInfo);
            Assert.Null(dto.Pic);
        }

        // TourUpdateDto Tests
        [Fact]
        public void TourUpdateDto_DefaultConstructor_SetsDefaultValues()
        {
            var dto = new TourUpdateDto();
            Assert.Equal(string.Empty, dto.TourName);
            Assert.Equal(string.Empty, dto.Place);
            Assert.Equal(0, dto.Days);
            Assert.Equal(0m, dto.Price);
            Assert.Equal(string.Empty, dto.Locations);
            Assert.Equal(string.Empty, dto.TourInfo);
            Assert.Null(dto.Pic);
        }

        // BookingDto Tests
        [Fact]
        public void BookingDto_DefaultConstructor_SetsDefaultValues()
        {
            var dto = new BookingDto();
            Assert.Equal(0, dto.BookingId);
            Assert.Equal(string.Empty, dto.TourName);
            Assert.Equal(string.Empty, dto.Place);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(string.Empty, dto.FirstName);
            Assert.Null(dto.TourId);
        }

        [Fact]
        public void BookingDto_SetProperties_ReturnsCorrectValues()
        {
            var bookingDate = new DateTime(2024, 3, 15);
            var dto = new BookingDto
            {
                BookingId = 1,
                TourName = "Rome Tour",
                Place = "Rome",
                Email = "user@example.com",
                FirstName = "Alice",
                TourId = 3,
                BookingDate = bookingDate,
                IsActive = true
            };

            Assert.Equal(1, dto.BookingId);
            Assert.Equal("Rome Tour", dto.TourName);
            Assert.Equal("Rome", dto.Place);
            Assert.Equal("user@example.com", dto.Email);
            Assert.Equal("Alice", dto.FirstName);
            Assert.Equal(3, dto.TourId);
            Assert.Equal(bookingDate, dto.BookingDate);
            Assert.True(dto.IsActive);
        }

        // BookingCreateDto Tests
        [Fact]
        public void BookingCreateDto_DefaultConstructor_SetsDefaultValues()
        {
            var dto = new BookingCreateDto();
            Assert.Equal(string.Empty, dto.TourName);
            Assert.Equal(string.Empty, dto.Place);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(string.Empty, dto.FirstName);
            Assert.Null(dto.TourId);
        }

        // BookingUpdateDto Tests
        [Fact]
        public void BookingUpdateDto_DefaultConstructor_SetsDefaultValues()
        {
            var dto = new BookingUpdateDto();
            Assert.Equal(string.Empty, dto.TourName);
            Assert.Equal(string.Empty, dto.Place);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(string.Empty, dto.FirstName);
        }
    }
}
