using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Tour_Management.Pages
{
    public class OrderModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public OrderModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string City { get; set; }
        [BindProperty]
        public string TourName { get; set; }
        [BindProperty]
        public string Number { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string connectionString = _configuration.GetConnectionString("dbconnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string insertQuery = "insert into booking(TOUR_NAME,PLACE,Email,FirstName) values(@TOUR_NAME,@PLACE,@Email,@FirstName)";
                using (SqlCommand com = new SqlCommand(insertQuery, conn))
                {
                    com.Parameters.AddWithValue("@TOUR_NAME", TourName ?? (object)DBNull.Value);
                    com.Parameters.AddWithValue("@PLACE", City ?? (object)DBNull.Value);
                    com.Parameters.AddWithValue("@Email", Number ?? (object)DBNull.Value);
                    com.Parameters.AddWithValue("@FirstName", Name ?? (object)DBNull.Value);

                    await conn.OpenAsync();
                    await com.ExecuteNonQueryAsync();
                }
            }
            return RedirectToPage("mybooking");
        }
    }
}
