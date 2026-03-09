using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tour_Management
{
    public partial class AdminLogin2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Use environment variables for admin credentials instead of hardcoded values
            string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? ConfigurationManager.AppSettings["AdminPassword"];
            string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? ConfigurationManager.AppSettings["AdminEmail"];

            if (!string.IsNullOrEmpty(adminPassword) && !string.IsNullOrEmpty(adminEmail) &&
                password.Text == adminPassword && name.Text == adminEmail)
            {
                Response.Redirect("AdminProfile.aspx");
            }
            else if (!string.IsNullOrEmpty(password.Text) || !string.IsNullOrEmpty(name.Text))
            {
                Response.Write("Invalid admin credentials");
            }
        }
    }
}