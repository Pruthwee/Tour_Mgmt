using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Tour_Management.Pages
{
    public class TourCrudModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<TourItem> Tours { get; set; } = new List<TourItem>();

        public TourCrudModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class TourItem
        {
            public int TOUR_ID { get; set; }
            public string TOUR_NAME { get; set; }
            public string PLACE { get; set; }
            public string DAYS { get; set; }
            public string PRICE { get; set; }
            public string LOCATIONS { get; set; }
            public string TOUR_INFO { get; set; }
            public string pic { get; set; }
        }

        public async Task OnGetAsync()
        {
            string connectionString = _configuration.GetConnectionString("dbconnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM [Tour]";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Tours.Add(new TourItem
                            {
                                TOUR_ID = (int)reader["TOUR_ID"],
                                TOUR_NAME = reader["TOUR_NAME"].ToString(),
                                PLACE = reader["PLACE"].ToString(),
                                DAYS = reader["DAYS"].ToString(),
                                PRICE = reader["PRICE"].ToString(),
                                LOCATIONS = reader["LOCATIONS"].ToString(),
                                TOUR_INFO = reader["TOUR_INFO"].ToString(),
                                pic = reader["pic"].ToString()
                            });
                        }
                    }
                }
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            string connectionString = _configuration.GetConnectionString("dbconnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM [Tour] WHERE [TOUR_ID] = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return RedirectToPage();
        }
    }
}
