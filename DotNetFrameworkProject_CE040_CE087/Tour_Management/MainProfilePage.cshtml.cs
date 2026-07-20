using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    public class MainProfilePageModel : PageModel
    {
        public string WelcomeMessage { get; set; }

        public void OnGet()
        {
            WelcomeMessage = "Welcome to Tour Management";
        }
    }
}
