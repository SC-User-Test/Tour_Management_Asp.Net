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
            // Use environment variable for upload path for cloud compatibility
            string uploadPath = Environment.GetEnvironmentVariable("UPLOAD_PATH") ??
                               ConfigurationManager.AppSettings["UploadPath"] ??
                               Server.MapPath("~/Tour_pics/");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString))
            {
                conn.Open();
                string insertQuery = "INSERT INTO Tour(TOUR_NAME,PLACE,DAYS,PRICE,LOCATIONS,TOUR_INFO,pic) VALUES(@TOUR_NAME,@PLACE,@DAYS,@PRICE,@LOCATIONS,@TOUR_INFO,@pic)";

                using (SqlCommand com = new SqlCommand(insertQuery, conn))
                {
                    com.Parameters.AddWithValue("@TOUR_NAME", tour_name.Text);
                    com.Parameters.AddWithValue("@PLACE", place.Text);
                    com.Parameters.AddWithValue("@DAYS", days.Text);
                    com.Parameters.AddWithValue("@PRICE", price.Text);
                    com.Parameters.AddWithValue("@LOCATIONS", locations.Text);
                    com.Parameters.AddWithValue("@TOUR_INFO", tour_info.Text);

                    // Save file to configurable upload path
                    if (FileUpload1.HasFile)
                    {
                        string fileName = System.IO.Path.GetFileName(FileUpload1.FileName);
                        string filePath = System.IO.Path.Combine(uploadPath, fileName);
                        FileUpload1.SaveAs(filePath);
                        com.Parameters.AddWithValue("@pic", fileName);
                    }
                    else
                    {
                        com.Parameters.AddWithValue("@pic", DBNull.Value);
                    }

                    com.ExecuteNonQuery();
                    Response.Write("ADD Successful");
                }
            }
        }
    }
}