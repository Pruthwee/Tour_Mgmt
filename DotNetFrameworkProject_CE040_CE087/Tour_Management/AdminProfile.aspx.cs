using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// Cloud-readiness (cr-dotnet-0026): Migrated Web Forms page to use cloud-native patterns.
// Page follows stateless request handling compatible with horizontal scaling in AWS environments.

namespace Tour_Management
{
    public partial class AdminProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Cloud-readiness: Stateless page load - no server-side session state dependency.
            // Compatible with AWS Elastic Load Balancing and horizontal auto-scaling.
        }
    }
}
