using Microsoft.AspNetCore.Mvc;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;

namespace TourManagement.Web.Controllers
{
    public class ToursController : Controller
    {
        private readonly ITourService _tourService;

        public ToursController(ITourService tourService)
        {
            _tourService = tourService;
        }

        public async Task<IActionResult> Index()
        {
            var tours = await _tourService.GetAllToursAsync();
            return View(tours);
        }

        public async Task<IActionResult> Details(int id)
        {
            var tour = await _tourService.GetTourByIdAsync(id);
            if (tour == null) return NotFound();
            return View(tour);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Tour tour)
        {
            if (ModelState.IsValid)
            {
                await _tourService.CreateTourAsync(tour);
                return RedirectToAction(nameof(Index));
            }
            return View(tour);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tour = await _tourService.GetTourByIdAsync(id);
            if (tour == null) return NotFound();
            return View(tour);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Tour tour)
        {
            if (ModelState.IsValid)
            {
                await _tourService.UpdateTourAsync(tour);
                return RedirectToAction(nameof(Index));
            }
            return View(tour);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tour = await _tourService.GetTourByIdAsync(id);
            if (tour == null) return NotFound();
            return View(tour);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _tourService.DeleteTourAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
