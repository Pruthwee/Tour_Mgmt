using System.Web;

namespace Tour_Management
{
    // Health check endpoint: GET /health answers {"status":"healthy"} while the application is running.
    public class HealthHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Write("{\"status\":\"healthy\"}");
        }
    }
}
