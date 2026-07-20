using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Tour_Management.Pages
{
    public class UserInfo
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
    }

    public class UserCrudModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<UserInfo> Users { get; set; } = new List<UserInfo>();

        public UserCrudModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            string connectionString = _configuration.GetConnectionString("dbconnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM UserInfo";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Users.Add(new UserInfo
                            {
                                Email = reader["Email"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Password = reader["Password"].ToString(),
                                City = reader["City"].ToString()
                            });
                        }
                    }
                }
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync([Microsoft.AspNetCore.Mvc import="Microsoft.AspNetCore.Mvc" ] UserInfo user)
        {
            string connectionString = _configuration.GetConnectionString("dbconnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE [UserInfo] Set [FirstName]=@FirstName,[LastName]=@LastName,[Gender]=@Gender,[Password]=@Password,[City]=@City Where [Email]=@Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@Gender", user.Gender);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@City", user.City);
                    cmd.Parameters.AddWithValue("@Email", user.Email);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return RedirectToPage();
        }
    }
}
