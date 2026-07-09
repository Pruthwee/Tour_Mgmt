using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using Xunit;

namespace TourManagement.Web.Controllers.Tests
{
    public class ToursControllerTests
    {
        private readonly Mock<ITourService> _mockTourService;
        private readonly ToursController _controller;

        public ToursControllerTests()
        {
            _mockTourService = new Mock<ITourService>();
            _controller = new ToursController(_mockTourService.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithTours()
        {
            // Arrange
            var tours = new List<Tour> { new Tour { Id = 1, Name = "Tour 1" } };
            _mockTourService.Setup(s => s.GetAllToursAsync()).ReturnsAsync(tours);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(tours, viewResult.Model);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsViewWithTour()
        {
            // Arrange
            var tour = new Tour { Id = 1, Name = "Tour 1" };
            _mockTourService.Setup(s => s.GetTourByIdAsync(1)).ReturnsAsync(tour);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(tour, viewResult.Model);
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockTourService.Setup(s => s.GetTourByIdAsync(It.IsAny<int>())).ReturnsAsync((Tour)null!);

            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_Get_ReturnsView()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ValidModel_ReturnsRedirectToIndex()
        {
            // Arrange
            var tour = new Tour { Id = 1, Name = "Tour 1" };
            _controller.ModelState.AddModelError("Name", "Required");
            _controller.ModelState.Clear(); // Simulate valid model

            // Act
            var result = await _controller.Create(tour);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockTourService.Verify(s => s.CreateTourAsync(tour), Times.Once);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsView()
        {
            // Arrange
            var tour = new Tour { Id = 1 };
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.Create(tour);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(tour, viewResult.Model);
            _mockTourService.Verify(s => s.CreateTourAsync(It.IsAny<Tour>()), Times.Never);
        }

        [Fact]
        public async Task Edit_Get_WithValidId_ReturnsViewWithTour()
        {
            // Arrange
            var tour = new Tour { Id = 1, Name = "Tour 1" };
            _mockTourService.Setup(s => s.GetTourByIdAsync(1)).ReturnsAsync(tour);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(tour, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockTourService.Setup(s => s.GetTourByIdAsync(It.IsAny<int>())).ReturnsAsync((Tour)null!);

            // Act
            var result = await _controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_ReturnsRedirectToIndex()
        {
            // Arrange
            var tour = new Tour { Id = 1, Name = "Tour 1" };
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.Edit(tour);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockTourService.Verify(s => s.UpdateTourAsync(tour), Times.Once);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsView()
        {
            // Arrange
            var tour = new Tour { Id = 1 };
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.Edit(tour);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(tour, viewResult.Model);
            _mockTourService.Verify(s => s.UpdateTourAsync(It.IsAny<Tour>()), Times.Never);
        }

        [Fact]
        public async Task Delete_Get_WithValidId_ReturnsViewWithTour()
        {
            // Arrange
            var tour = new Tour { Id = 1, Name = "Tour 1" };
            _mockTourService.Setup(s => s.GetTourByIdAsync(1)).ReturnsAsync(tour);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(tour, viewResult.Model);
        }

        [Fact]
        public async Task Delete_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockTourService.Setup(s => s.GetTourByIdAsync(It.IsAny<int>())).ReturnsAsync((Tour)null!);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToIndex()
        {
            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockTourService.Verify(s => s.DeleteTourAsync(1), Times.Once);
        }
    }
}
