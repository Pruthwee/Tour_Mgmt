using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Tour_Management.Pages
{
    public class SignUpFormModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public SignUpFormModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string FirstName { get; set; }
        [BindProperty]
        public string LastName { get; set; }
        [BindProperty]
        public string Gender { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string PasswordConfirm { get; set; }
        [BindProperty]
        public string Dob { get; set; }
        [BindProperty]
        public string Street { get; set; }
        [BindProperty]
        public string City { get; set; }
        [BindProperty]
        public string State { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Password != PasswordConfirm)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match");
                return Page();
            }

            string connectionString = _configuration.GetConnectionString("dbconnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string insertQuery = "insert into UserInfo(Email,FirstName,LastName,Gender,Password,dob,Street,City,State) values(@email,@FirstName,@LastName,@Gender,@Password,@dob,@Street,@City,@State)";
                using (SqlCommand com = new SqlCommand(insertQuery, conn))
                {
                    com.Parameters.AddWithValue("@Email", Email ?? (object)DBNull.Value);
                    com.Parameters.AddWithValue("@FirstName", FirstName ?? (object)DBNull.Value);
                    com.Parameters.AddWithValue("@LastName", LastName ?? (object)DBNull.Value);
                    com.Parameters.AddWithValue("@Gender", Gender ?? (object)DBNull.Value);
                    com.Parameters.AddWithValue("@Password", Password ?? (object)DBNull.Value);
                    com.Parameters.AddWithValue("@dob", Dob ?? (object)DBNull.Value);
                    com.Parameters.AddWithValue("@Street", Street ?? (object)DBNull.Value);
                    com.Parameters.AddWithValue("@City", City ?? (object)DBNull.Value);
                    com.Parameters.AddWithValue("@State", State ?? (object)DBNull.Value);

                    await conn.OpenAsync();
                    await com.ExecuteNonQueryAsync();
                }
            }
            return RedirectToPage("userlogin");
        }
    }
}
