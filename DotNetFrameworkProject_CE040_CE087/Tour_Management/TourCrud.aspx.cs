using System;
using System.Web.UI;

namespace Tour_Management
{
    public partial class TourCrud : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                refreshdata();
            }
        }

        public void refreshdata()
        {
            GridView1.DataBind();
        }
    }
}