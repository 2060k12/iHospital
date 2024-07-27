using iHospital.UserControl;
using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadQuestions();
            }
        }

        private void LoadQuestions()
        {
            string connectionString = "Data Source=SQL5111.site4now.net;Initial Catalog=db_9ab8b7_224dda12275;User Id=db_9ab8b7_224dda12275_admin;Password=vWHVw5VW";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * from Question";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {   
                            switch (reader["question_type"].ToString())
                            {

                                // if the question type of "choose_one" this user control will work
                                case "choose_one":
                                    ChooseUserControl chooseList = (ChooseUserControl)LoadControl("~/UserControl/ChooseUserControl.ascx");
                                    chooseList.QuestionText = reader["question"].ToString();
                                    string[] items = { "Item 1", "Item 2", "Item 3" };
                                    chooseList.SetCheckBoxListItems(items);
                                    questionContainer.Controls.Add(chooseList);
                                    break;


                                // if the question type of "drop_down" this user control will work

                                case "drop_down":
                                    DropDownUserControl dropDown = (DropDownUserControl)LoadControl("~/UserControl/DropDownUserControl.ascx");
                                    dropDown.QuestionText = reader["question"].ToString();
                                    string[] items1 = { "Item 1", "Item 2", "Item 3" };
    
                                    dropDown.SetCheckBoxListItems(items1);
                                    questionContainer.Controls.Add(dropDown);
                                    break;

                                // if the question type of "select" this user control will work

                                case "select":
                                    CheckListControl checkList = (CheckListControl)LoadControl("~/UserControl/CheckListUserControl.ascx");
                                    checkList.QuestionText = reader["question"].ToString();
                                    string[] items3 = { "Item 1", "Item 2", "Item 3" };
                                    checkList.SetCheckBoxListItems(items3);
                                    questionContainer.Controls.Add(checkList);
                                    break;


                                    // if the question type of "input" this user control will work
                                case "input":
                                    InputUserControl inputControl = (InputUserControl)LoadControl("~/UserControl/InputUserControl.ascx");
                                    inputControl.QuestionText = reader["question"].ToString();
                                    string input = inputControl.TextFieldText;
                                    questionContainer.Controls.Add(inputControl);
                                    break;

                            }


                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);

                // Display a user-friendly error message
                // Assuming you have a label for error messages in your ASPX page
              
            }
        }
    }
}