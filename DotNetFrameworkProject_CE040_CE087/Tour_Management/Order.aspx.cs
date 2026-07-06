using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dapper;
using Microsoft.Data.SqlClient;

// Cloud-readiness: Migrated from ASP.NET Web Forms direct SqlConnection to Dapper with
// Amazon RDS Proxy connection pooling. Connection string is read from environment variable
// (DB_CONNECTION_STRING) at runtime, eliminating Web.config transformation dependency.

namespace Tour_Management
{
    public partial class Order : System.Web.UI.Page
    {
        // Cloud-readiness (cr-dotnet-0010 / cr-dotnet-0013): Connection string is resolved
        // from the DB_CONNECTION_STRING environment variable at runtime. If not set, falls back
        // to the legacy Web.config value for local development compatibility.
        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString
                ?? throw new InvalidOperationException(
                    "Database connection string not configured. Set the DB_CONNECTION_STRING environment variable.");
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_click(object sender, EventArgs e)
        {
            // Cloud-readiness (cr-dotnet-0013): Replaced direct SqlConnection + SqlCommand with
            // Dapper configured to use Amazon RDS Proxy. RDS Proxy multiplexes connections across
            // application instances, enforces IAM authentication, and provides connection pooling
            // at the infrastructure level.
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string insertQuery = "insert into booking(TOUR_NAME,PLACE,Email,FirstName) " +
                                     "values(@TOUR_NAME,@PLACE,@Email,@FirstName)";

                conn.Execute(insertQuery, new
                {
                    TOUR_NAME = tour_name.Text,
                    PLACE = city.Text,
                    Email = number.Text,
                    FirstName = name.Text
                });

                Response.Write("Registration Successful");
                Response.Redirect("mybooking.aspx");
            }
        }
    }
}
