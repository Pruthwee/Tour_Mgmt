using System;
using System.Web;

namespace Tour_Management
{
    public class HealthCheck : IHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write("{\"status\": \"Healthy\"}");
        }

        public string ProcessRequest(HttpContext context)
        {
            return "{\"status\": \"Healthy\"}";
        }
    }
}
