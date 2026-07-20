using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Tour_Management.Pages
{
    public class UserLoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public UserLoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            string connectionString = _configuration.GetConnectionString("dbconnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string checkPasswordQuery = "SELECT password FROM Userinfo WHERE email = @email";
                using (SqlCommand passComm = new SqlCommand(checkPasswordQuery, conn))
                {
                    passComm.Parameters.AddWithValue("@email", Email);
                    await conn.OpenAsync();
                    var result = await passComm.ExecuteScalarAsync();
                    string dbPassword = result?.ToString() ?? "";
                    if (dbPassword == Password)
                    {
                        return RedirectToPage("MainProfilePage");
                    }
                }
            }
            ModelState.AddModelError(string.Empty, "Password is not correct");
            return Page();
        }

        public IActionResult OnPostSignUp()
        {
            return RedirectToPage("SignUpForm");
        }
    }
}
