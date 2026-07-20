using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Tour_Management.Pages
{
    public class Tour
    {
        public int TOUR_ID { get; set; }
        public string TOUR_NAME { get; set; }
        public string pic { get; set; }
        public string PRICE { get; set; }
        public string DAYS { get; set; }
        public string LOCATIONS { get; set; }
    }

    public class DisplayToursModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Tour> Tours { get; set; } = new List<Tour>();

        public DisplayToursModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            string connectionString = _configuration.GetConnectionString("dbconnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT [TOUR_NAME], [pic], [PRICE], [DAYS], [LOCATIONS], [TOUR_ID] FROM [Tour]";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Tours.Add(new Tour
                            {
                                TOUR_NAME = reader["TOUR_NAME"].ToString(),
                                pic = reader["pic"].ToString(),
                                PRICE = reader["PRICE"].ToString(),
                                DAYS = reader["DAYS"].ToString(),
                                LOCATIONS = reader["LOCATIONS"].ToString(),
                                TOUR_ID = (int)reader["TOUR_ID"]
                            });
                        }
                    }
                }
            }
        }
    }
}
