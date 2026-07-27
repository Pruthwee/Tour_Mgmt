using System;
using System.Web;
using System.Web.UI;

public partial class HealthCheck : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "application/json";
        Response.Write("{\"status\": \"UP\"}");
        Response.End();
    }
}
