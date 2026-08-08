using System;
using System.Web.UI;

namespace Tour_Management
{
    public partial class Order : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btn_click(object sender, EventArgs e)
        {
            Response.Write("Booking request captured. Configure Azure SQL and EF Core migration for database persistence.");
            Response.Redirect("mybooking.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}