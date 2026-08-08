using System;
using System.Web.UI;

namespace Tour_Management
{
    public partial class SignUpForm : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Register_Click(object sender, EventArgs e)
        {
            Response.Write("Registration request captured. Configure Azure SQL and EF Core migration for database persistence.");
            Response.Redirect("userlogin.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}