using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Tour_Management.Pages
{
    public class AddTourModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public AddTourModel(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        [BindProperty]
        public string TourName { get; set; }
        [BindProperty]
        public string Place { get; set; }
        [BindProperty]
        public string Days { get; set; }
        [BindProperty]
        public string Locations { get; set; }
        [BindProperty]
        public string Price { get; set; }
        [BindProperty]
        public string TourInfo { get; set; }
        [BindProperty]
        public IFormFile TourPic { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string fileName = "";
            if (TourPic != null)
            {
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(TourPic.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "Tour_pics", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await TourPic.CopyToAsync(stream);
                }
            }

            string connectionString = _configuration.GetConnectionString("dbconnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Tours (TourName, Place, Days, Locations, Price, TourInfo, Pic) VALUES (@TOUR_NAME, @PLACE, @DAYS, @LOCATIONS, @PRICE, @TOUR_INFO, @PIC)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TOUR_NAME", TourName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PLACE", Place ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DAYS", Days ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LOCATIONS", Locations ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PRICE", Price ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TOUR_INFO", TourInfo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PIC", fileName ?? (object)DBNull.Value);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return RedirectToPage("Index");
        }
    }
}
