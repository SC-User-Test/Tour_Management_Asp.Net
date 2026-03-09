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
    public partial class userlogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

  
            protected void Btn_Submit(object sender, EventArgs e)
            {
                // Use parameterized query to prevent SQL injection
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString))
                {
                    conn.Open();
                    string checkPasswordQuery = "SELECT password FROM Userinfo WHERE email = @Email";
                    using (SqlCommand passComm = new SqlCommand(checkPasswordQuery, conn))
                    {
                        passComm.Parameters.AddWithValue("@Email", txtEmail.Text);
                        string password = passComm.ExecuteScalar()?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(password) && password == txtPassword.Text)
                        {
                            Session["UserEmail"] = txtEmail.Text;
                            Response.Redirect("MainProfilePage.aspx");
                        }
                        else
                        {
                            Response.Write("Invalid email or password");
                        }
                    }
                }
            }

        protected void Btn_reg(object sender, EventArgs e)
        {
            Response.Redirect("SignUpForm.aspx");
            Server.Transfer("SignUpForm.aspx");
        }
    }
   
}