using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Tour_Management.Pages
{
    public class MyBooking
    {
        public string TOUR_NAME { get; set; }
        public int TOUR_ID { get; set; }
    }

    public class MyBookingModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<MyBooking> Bookings { get; set; } = new List<MyBooking>();

        public MyBookingModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            string connectionString = _configuration.GetConnectionString("dbconnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT [TOUR_NAME], [TOUR_ID] FROM [booking]";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Bookings.Add(new MyBooking
                            {
                                TOUR_NAME = reader["TOUR_NAME"].ToString(),
                                TOUR_ID = (int)reader["TOUR_ID"]
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
                string query = "DELETE FROM [booking] WHERE [TOUR_ID] = @id";
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
