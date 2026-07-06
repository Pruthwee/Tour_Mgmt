using System;
using System.Web;

namespace Tour_Management
{
    /// <summary>
    /// Health check endpoint for container liveness/readiness probes.
    /// Accessible at /HealthCheck.ashx
    /// Returns HTTP 200 with JSON status when the application is healthy.
    /// </summary>
    public class HealthCheck : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            context.Response.Write("{\"status\":\"healthy\",\"application\":\"Tour_Management\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
