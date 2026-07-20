using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Tour_Management.Pages
{
    public class Booking
    {
        public int TOUR_ID { get; set; }
        public string TOUR_NAME { get; set; }
        public string PLACE { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
    }

    public class AllBookingModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Booking> Bookings { get; set; } = new List<Booking>();

        public AllBookingModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            string connectionString = _configuration.GetConnectionString("dbconnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM [booking]";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Bookings.Add(new Booking
                            {
                                TOUR_ID = (int)reader["TOUR_ID"],
                                TOUR_NAME = reader["TOUR_NAME"].ToString(),
                                PLACE = reader["PLACE"].ToString(),
                                Email = reader["Email"].ToString(),
                                FirstName = reader["FirstName"].ToString()
                            });
                        }
                    }
                }
            }
        }
    }
}
