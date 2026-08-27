using System;
using Xunit;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.ViewModels.Tests
{
    public class ViewModelsTests
    {
        // LoginViewModel Tests
        [Fact]
        public void LoginViewModel_DefaultConstructor_SetsDefaultValues()
        {
            var vm = new LoginViewModel();
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.Password);
        }

        [Fact]
        public void LoginViewModel_SetProperties_ReturnsCorrectValues()
        {
            var vm = new LoginViewModel
            {
                Email = "user@example.com",
                Password = "password123"
            };
            Assert.Equal("user@example.com", vm.Email);
            Assert.Equal("password123", vm.Password);
        }

        // RegisterViewModel Tests
        [Fact]
        public void RegisterViewModel_DefaultConstructor_SetsDefaultValues()
        {
            var vm = new RegisterViewModel();
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.FirstName);
            Assert.Equal(string.Empty, vm.LastName);
            Assert.Equal(string.Empty, vm.Gender);
            Assert.Equal(string.Empty, vm.Password);
            Assert.Equal(string.Empty, vm.Street);
            Assert.Equal(string.Empty, vm.City);
            Assert.Equal(string.Empty, vm.State);
        }

        [Fact]
        public void RegisterViewModel_SetProperties_ReturnsCorrectValues()
        {
            var dob = new DateTime(1990, 5, 15);
            var vm = new RegisterViewModel
            {
                Email = "john@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "password123",
                Dob = dob,
                Street = "123 Main St",
                City = "New York",
                State = "NY"
            };

            Assert.Equal("john@example.com", vm.Email);
            Assert.Equal("John", vm.FirstName);
            Assert.Equal("Doe", vm.LastName);
            Assert.Equal("Male", vm.Gender);
            Assert.Equal("password123", vm.Password);
            Assert.Equal(dob, vm.Dob);
            Assert.Equal("123 Main St", vm.Street);
            Assert.Equal("New York", vm.City);
            Assert.Equal("NY", vm.State);
        }

        // AdminLoginViewModel Tests
        [Fact]
        public void AdminLoginViewModel_DefaultConstructor_SetsDefaultValues()
        {
            var vm = new AdminLoginViewModel();
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.Password);
        }

        [Fact]
        public void AdminLoginViewModel_SetProperties_ReturnsCorrectValues()
        {
            var vm = new AdminLoginViewModel
            {
                Email = "admin@example.com",
                Password = "adminpass"
            };
            Assert.Equal("admin@example.com", vm.Email);
            Assert.Equal("adminpass", vm.Password);
        }

        // TourListViewModel Tests
        [Fact]
        public void TourListViewModel_DefaultConstructor_SetsDefaultValues()
        {
            var vm = new TourListViewModel();
            Assert.Equal(0, vm.TourId);
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(0, vm.Days);
            Assert.Equal(0m, vm.Price);
            Assert.Equal(string.Empty, vm.Locations);
            Assert.Equal(string.Empty, vm.TourInfo);
            Assert.Null(vm.Pic);
        }

        [Fact]
        public void TourListViewModel_SetProperties_ReturnsCorrectValues()
        {
            var vm = new TourListViewModel
            {
                TourId = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500m,
                Locations = "Eiffel Tower",
                TourInfo = "Great tour",
                Pic = "paris.jpg"
            };

            Assert.Equal(1, vm.TourId);
            Assert.Equal("Paris Tour", vm.TourName);
            Assert.Equal("Paris", vm.Place);
            Assert.Equal(7, vm.Days);
            Assert.Equal(1500m, vm.Price);
            Assert.Equal("Eiffel Tower", vm.Locations);
            Assert.Equal("Great tour", vm.TourInfo);
            Assert.Equal("paris.jpg", vm.Pic);
        }

        // TourFormViewModel Tests
        [Fact]
        public void TourFormViewModel_DefaultConstructor_SetsDefaultValues()
        {
            var vm = new TourFormViewModel();
            Assert.Equal(0, vm.TourId);
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(0, vm.Days);
            Assert.Equal(0m, vm.Price);
            Assert.Equal(string.Empty, vm.Locations);
            Assert.Equal(string.Empty, vm.TourInfo);
            Assert.Null(vm.PicFile);
            Assert.Null(vm.ExistingPic);
        }

        [Fact]
        public void TourFormViewModel_SetProperties_ReturnsCorrectValues()
        {
            var vm = new TourFormViewModel
            {
                TourId = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500m,
                Locations = "Eiffel Tower",
                TourInfo = "Great tour",
                ExistingPic = "paris.jpg"
            };

            Assert.Equal(1, vm.TourId);
            Assert.Equal("Paris Tour", vm.TourName);
            Assert.Equal("Paris", vm.Place);
            Assert.Equal(7, vm.Days);
            Assert.Equal(1500m, vm.Price);
            Assert.Equal("Eiffel Tower", vm.Locations);
            Assert.Equal("Great tour", vm.TourInfo);
            Assert.Equal("paris.jpg", vm.ExistingPic);
        }

        // BookingFormViewModel Tests
        [Fact]
        public void BookingFormViewModel_DefaultConstructor_SetsDefaultValues()
        {
            var vm = new BookingFormViewModel();
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.FirstName);
            Assert.Null(vm.TourId);
        }

        [Fact]
        public void BookingFormViewModel_SetProperties_ReturnsCorrectValues()
        {
            var vm = new BookingFormViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@example.com",
                FirstName = "John",
                TourId = 1
            };

            Assert.Equal("Paris Tour", vm.TourName);
            Assert.Equal("Paris", vm.Place);
            Assert.Equal("user@example.com", vm.Email);
            Assert.Equal("John", vm.FirstName);
            Assert.Equal(1, vm.TourId);
        }

        // BookingListViewModel Tests
        [Fact]
        public void BookingListViewModel_DefaultConstructor_SetsDefaultValues()
        {
            var vm = new BookingListViewModel();
            Assert.Equal(0, vm.BookingId);
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.FirstName);
        }

        [Fact]
        public void BookingListViewModel_SetProperties_ReturnsCorrectValues()
        {
            var bookingDate = new DateTime(2024, 3, 15);
            var vm = new BookingListViewModel
            {
                BookingId = 1,
                TourName = "Rome Tour",
                Place = "Rome",
                Email = "user@example.com",
                FirstName = "Alice",
                BookingDate = bookingDate
            };

            Assert.Equal(1, vm.BookingId);
            Assert.Equal("Rome Tour", vm.TourName);
            Assert.Equal("Rome", vm.Place);
            Assert.Equal("user@example.com", vm.Email);
            Assert.Equal("Alice", vm.FirstName);
            Assert.Equal(bookingDate, vm.BookingDate);
        }

        // UserProfileViewModel Tests
        [Fact]
        public void UserProfileViewModel_DefaultConstructor_SetsDefaultValues()
        {
            var vm = new UserProfileViewModel();
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.FirstName);
            Assert.Equal(string.Empty, vm.LastName);
            Assert.Equal(string.Empty, vm.Gender);
            Assert.Equal(string.Empty, vm.Street);
            Assert.Equal(string.Empty, vm.City);
            Assert.Equal(string.Empty, vm.State);
            Assert.NotNull(vm.Bookings);
            Assert.Empty(vm.Bookings);
        }

        [Fact]
        public void UserProfileViewModel_SetProperties_ReturnsCorrectValues()
        {
            var dob = new DateTime(1990, 1, 1);
            var vm = new UserProfileViewModel
            {
                Email = "john@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Dob = dob,
                Street = "123 Main St",
                City = "NYC",
                State = "NY"
            };

            Assert.Equal("john@example.com", vm.Email);
            Assert.Equal("John", vm.FirstName);
            Assert.Equal("Doe", vm.LastName);
            Assert.Equal("Male", vm.Gender);
            Assert.Equal(dob, vm.Dob);
            Assert.Equal("123 Main St", vm.Street);
            Assert.Equal("NYC", vm.City);
            Assert.Equal("NY", vm.State);
        }

        [Fact]
        public void UserProfileViewModel_Bookings_CanBeSet()
        {
            var bookings = new[]
            {
                new BookingListViewModel { BookingId = 1, TourName = "Paris Tour" },
                new BookingListViewModel { BookingId = 2, TourName = "Rome Tour" }
            };
            var vm = new UserProfileViewModel { Bookings = bookings };

            Assert.Equal(2, new System.Collections.Generic.List<BookingListViewModel>(vm.Bookings).Count);
        }
    }
}
