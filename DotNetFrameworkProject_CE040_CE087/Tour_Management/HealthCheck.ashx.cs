using System;
using System.Web;

namespace Tour_Management
{
    public class HealthCheck : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write("{\"status\": \"Healthy\"}");
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}