using iHospital.config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital
{
    public partial class Login : System.Web.UI.Page
    {
        string myConnectionString ;
        protected void Page_Load(object sender, EventArgs e)
        {

            // Get the connection string from the web.config file

            myConnectionString = ConfigurationManager.ConnectionStrings["CurrentConnection"].ConnectionString;

            if (myConnectionString.Equals("dev"))
            {
                myConnectionString = AppConst.DBServerConnection.testConnectionString;
            }

        }


       
        protected void loginButton_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (ValidateUser(username, password))
            {
                // Redirect to admin page if login is successful
                Response.Redirect("Admin.aspx");
            }
            else
            {
                // Show error message if login fails
                lblMessage.Text = "Invalid username or password.";
            }
        }


        // This method will validate the user credentials
        private bool ValidateUser(string username, string password)
        {
            bool isValid = false;
          
            

            using (SqlConnection connection = new SqlConnection(myConnectionString))
            {
                string query = "SELECT COUNT(*) FROM Staff WHERE userName = @Username AND password = @Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                try
                {
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    if (count == 1)
                    {
                        isValid = true;
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (not shown here for brevity)
                    lblMessage.Text = "An error occurred while trying to log in.";
                }
            }

            return isValid;
        }

    }



}