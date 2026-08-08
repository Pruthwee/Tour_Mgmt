using System;
using System.Configuration;
using System.IO;
using System.Web.UI;
using Microsoft.Extensions.Configuration;

namespace Tour_Management
{
    public partial class AddTour : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Register_Click(object sender, EventArgs e)
        {
            var configuration = CloudConfiguration.Build();
            var imageDirectory = configuration["Storage:TourImagesPath"];

            if (string.IsNullOrWhiteSpace(imageDirectory))
            {
                imageDirectory = Server.MapPath("~/Tour_pics/");
            }

            Directory.CreateDirectory(imageDirectory);

            var fileName = Path.GetFileName(FileUpload1.FileName);
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                FileUpload1.SaveAs(Path.Combine(imageDirectory, fileName));
            }

            Response.Write("ADD Successful. Configure Azure SQL and EF Core migration for database persistence.");
        }
    }
}