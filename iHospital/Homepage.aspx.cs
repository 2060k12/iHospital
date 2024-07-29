using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital
{
    public partial class Homepage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void loginButton_Click(object sender, EventArgs e)
        {

            // Redirects to admin login page 
            Response.Redirect("Login.aspx");


        }

        protected void startSurveyButton_Click(object sender, EventArgs e)
        {
            // redirects to survey page
            Response.Redirect("Survey.aspx");


        }

        protected void registerButton_Click(object sender, EventArgs e)
        {
            // redirects to register page
            Response.Redirect("Register.aspx");

        }
    }
}