using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tour_Management
{
    public partial class AddTour : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
       
        protected void Register_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__dbconnection"));
            conn.Open();
            string insertQuery = "insert into Tour(TOUR_NAME,PLACE,DAYS,PRICE,LOCATIONS,TOUR_INFO,pic) values(@TOUR_NAME,@PLACE,@DAYS,@PRICE,@LOCATIONS,@TOUR_INFO,@pic)";
            SqlCommand com = new SqlCommand(insertQuery, conn);
            
            com.Parameters.AddWithValue("@TOUR_NAME", tour_name.Text);
            com.Parameters.AddWithValue("@PLACE", place.Text);
            com.Parameters.AddWithValue("@DAYS", days.Text); 
            com.Parameters.AddWithValue("@PRICE", price.Text);
            com.Parameters.AddWithValue("@LOCATIONS", locations.Text);
            com.Parameters.AddWithValue("@TOUR_INFO", tour_info.Text);

            using (var s3Client = new AmazonS3Client())
            using (var transferUtility = new TransferUtility(s3Client))
            {
                transferUtility.Upload(FileUpload1.FileContent, Environment.GetEnvironmentVariable("S3_BUCKET_NAME"), "Tour_pics/" + FileUpload1.FileName);
            }

             com.Parameters.AddWithValue("@pic", FileUpload1.FileName);


            com.ExecuteNonQuery();
            Response.Write("ADD  Successful");
            //Response.Redirect("a.aspx");
            //Server.Transfer("a.aspx");
            conn.Close();
        }
    }
}