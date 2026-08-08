using System;
using System.Web.UI;

namespace Tour_Management
{
    public partial class userlogin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Btn_Submit(object sender, EventArgs e)
        {
            Response.Write("Authentication flow requires migration to ASP.NET Core identity-backed authentication for Azure deployment.");
            Response.Redirect("MainProfilePage.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void Btn_reg(object sender, EventArgs e)
        {
            Response.Redirect("SignUpForm.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}