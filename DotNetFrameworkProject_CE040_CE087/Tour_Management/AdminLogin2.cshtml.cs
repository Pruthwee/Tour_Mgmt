using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Tour_Management.Pages
{
    public class AdminLogin2Model : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (Password == "admin" && Email == "admin@gmail.com")
            {
                return RedirectToPage("AdminProfile");
            }
            return Page();
        }
    }
}
