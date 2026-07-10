using System;
using System.Web;

namespace Tour_Management
{
    public partial class HealthCheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            Response.Write("{\"status\": \"Healthy\"}");
        }
    }
}