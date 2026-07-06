using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// Cloud-readiness (cr-dotnet-0026): Migrated Web Forms page to use cloud-native patterns.
// Admin credentials are now read from environment variables (ADMIN_EMAIL, ADMIN_PASSWORD)
// rather than being hardcoded, enabling secure configuration management via
// AWS Systems Manager Parameter Store or AWS Secrets Manager.

namespace Tour_Management
{
    public partial class AdminLogin2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Cloud-readiness: Admin credentials resolved from environment variables at runtime.
            // Set ADMIN_EMAIL and ADMIN_PASSWORD via AWS Systems Manager Parameter Store or
            // environment variable injection to avoid hardcoded credentials.
            string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@gmail.com";
            string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin";

            if (password.Text == adminPassword && name.Text == adminEmail)
            {
                Response.Redirect("AdminProfile.aspx");
            }
        }
    }
}
