using iHospital.config;
using iHospital.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital
{
    public partial class Register : System.Web.UI.Page
    {
        string myConnectionString;




        private void registerAsUser()
        {

        }


        private List<Question> questions
        {
            get
            {
                if (ViewState["registerQuestions"] == null)
                {
                    ViewState["registerQuestions"] = new List<Question>();
                }
                return (List<Question>)ViewState["registerQuestions"];
            }
            set
            {
                ViewState["registerQuestions"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            myConnectionString = ConfigurationManager.ConnectionStrings["CurrentConnection"].ConnectionString;

            if (myConnectionString.Equals("dev"))
            {
                myConnectionString = AppConst.DBServerConnection.testConnectionString;
            }

            if (!IsPostBack)
            {
                LoadQuestions();
            }

            LoadControls();
        }

        private void LoadControls()
        {
            registerPlaceHolder.Controls.Clear();

            foreach (Question question in questions)
            {
                Label label = new Label
                {
                    Text = question.QuestionText,
                    ID = "label_" + question.Id,
                    CssClass = "question-label"
                };

                TextBox textBox = new TextBox
                {
                    ID = "textBox_" + question.Id,
                      CssClass = "text-box"
                };

                registerPlaceHolder.Controls.Add(label);
                registerPlaceHolder.Controls.Add(new LiteralControl("<br/>"));
                registerPlaceHolder.Controls.Add(textBox);
                registerPlaceHolder.Controls.Add(new LiteralControl("<br/><br/>"));
            }
        }

        private void SaveAnswerToDatabase(Answer answer, int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(myConnectionString))
                {
                    conn.Open();

                    // Check if AnswerText is provided and OptionId is not
                    if (answer.OptionId == 0 && !string.IsNullOrWhiteSpace(answer.AnswerText))
                    {
                        string query = "INSERT INTO Answer (question_id, answer, respondant_id) VALUES (@question_id, @answer, @respondantId)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@question_id", answer.QuestionId);
                            cmd.Parameters.AddWithValue("@answer", answer.AnswerText ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@respondantId", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // Check if OptionId is provided and AnswerText is not
                    else if (answer.OptionId != 0)
                    {
                        string query = "INSERT INTO Answer (question_id, option_id, respondant_id) VALUES (@question_id, @optionId, @respondantId)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@question_id", answer.QuestionId);
                            cmd.Parameters.AddWithValue("@optionId", answer.OptionId);
                            cmd.Parameters.AddWithValue("@respondantId", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Log or handle the case where neither OptionId nor AnswerText is provided
                        System.Diagnostics.Debug.WriteLine("Error: Both OptionId and AnswerText are missing or invalid.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
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

                    // Load questions
                    string questionQuery = "SELECT * FROM Question";
                    using (SqlCommand cmd = new SqlCommand(questionQuery, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Question tempQuestion = new Question
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                QuestionText = reader["question"].ToString(),
                                QuestionType = reader["question_type"].ToString(),
                                QuestionOrder = Convert.ToInt32(reader["question_order"])
                            };

                            // anything with number 999 in question order is a register type question
                            if (tempQuestion.QuestionOrder == 999)
                            {
                                questions.Add(tempQuestion);
                            }
                        }
                    }
                }

                // Sort questions by QuestionOrder
                questions.Sort((x, y) => x.QuestionOrder.CompareTo(y.QuestionOrder));
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }

        protected void registerButton_Click(object sender, EventArgs e)
        {
            int registeredId = registerUser();
            var answers = Session["Answers"] as List<Answer> ?? new List<Answer>();

            // Add answers from questions
            foreach (Question question in questions)
            {
                TextBox textBox = (TextBox)registerPlaceHolder.FindControl("textBox_" + question.Id);
                if (textBox != null)
                {
                    string answerText = textBox.Text;
                    if (!string.IsNullOrEmpty(answerText))
                    {
                        answers.Add(new Answer
                        {
                            QuestionId = question.Id,
                            AnswerText = answerText,
                            RespondantId = registeredId,
                            OptionId = 0 //  OptionId is 0 for text answers
                        });
                    }
                }
            }

            // Save all answers to the database
            foreach (var item in answers)
            {
                SaveAnswerToDatabase(item, registeredId);
            }

            Label label = new Label
            {
                Text = "Successfully Submitted"
            };
            registerPlaceHolder.Controls.Add(label);
            answers.Clear();
            Session["Answers"] = answers;
            Response.Redirect("~/Homepage.aspx");


        }


        private int registerUser()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(myConnectionString))
                {
                    conn.Open();

                    // Retrieving maximum id from the table
                    int newRespondantId = 0;
                    string getMaxIdQuery = "SELECT ISNULL(MAX(id), 0) + 1 FROM Respondant";
                    using (SqlCommand getMaxIdCmd = new SqlCommand(getMaxIdQuery, conn))
                    {
                        newRespondantId = (int)getMaxIdCmd.ExecuteScalar();
                    }

                    // Insert a user into Respondant table
                    string insertRespondantQuery = "INSERT INTO Respondant (id, first_Name) VALUES (@id, @firstName)";
                    using (SqlCommand insertRespondantCmd = new SqlCommand(insertRespondantQuery, conn))
                    {
                        insertRespondantCmd.Parameters.AddWithValue("@id", newRespondantId);
                        insertRespondantCmd.Parameters.AddWithValue("@firstName", "User"); // Replace with actual user data
                        insertRespondantCmd.ExecuteNonQuery();
                    }

                    var macAddress = (
                        from nic in NetworkInterface.GetAllNetworkInterfaces()
                        where nic.OperationalStatus == OperationalStatus.Up
                        select nic.GetPhysicalAddress().ToString()
                    ).FirstOrDefault();

                    string insertSessionQuery = "INSERT INTO Session (id, mac_Address) VALUES (@id, @macAddress)";
                    using (SqlCommand insertSessionCmd = new SqlCommand(insertSessionQuery, conn))
                    {
                        insertSessionCmd.Parameters.AddWithValue("@id", newRespondantId);
                        insertSessionCmd.Parameters.AddWithValue("@macAddress", macAddress);
                        insertSessionCmd.ExecuteNonQuery();
                    }

                    return newRespondantId;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
            return 0;
        }

        private int registerifAnonymous()
        {

           
            try
            {
                using (SqlConnection conn = new SqlConnection(myConnectionString))
                {
                    conn.Open();

                    // retriving maximum id from the table
                    int newRespondantId = 0;
                    string getMaxIdQuery = "SELECT ISNULL(MAX(id), 0) + 1 FROM Respondant";
                    using (SqlCommand getMaxIdCmd = new SqlCommand(getMaxIdQuery, conn))
                    {
                        newRespondantId = (int)getMaxIdCmd.ExecuteScalar();
                    }

                    //  Insert an anonymous user into Respondant table
                    string insertRespondantQuery = "INSERT INTO Respondant (id, first_Name) VALUES (@id, @firstName)";
                    using (SqlCommand insertRespondantCmd = new SqlCommand(insertRespondantQuery, conn))
                    {
                        insertRespondantCmd.Parameters.AddWithValue("@id", newRespondantId);
                        insertRespondantCmd.Parameters.AddWithValue("@firstName", "Anonymous");
                        insertRespondantCmd.ExecuteNonQuery();
                    }
                    DateTime myDateTime = DateTime.Now;
                    


                    var macAddress =
                        (
                            from nic in NetworkInterface.GetAllNetworkInterfaces()
                            where nic.OperationalStatus == OperationalStatus.Up
                            select nic.GetPhysicalAddress().ToString()
                            ).FirstOrDefault();

                   //string insertSessionQuery = "INSERT INTO Session (id, date_time, mac_address) VALUES (@id, @macAddress, @dateTime)";

                    string insertSessionQuery = "INSERT INTO Session (id, mac_Address) VALUES (@id, @macAddress)";
               
                    using (SqlCommand insertRespondantCmd = new SqlCommand(insertSessionQuery, conn))
                    {

                        insertRespondantCmd.Parameters.AddWithValue("@id", newRespondantId);
                        insertRespondantCmd.Parameters.AddWithValue("@macAddress", macAddress);
                        insertRespondantCmd.ExecuteNonQuery();
                    }

                    return newRespondantId;

                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
            return 0;

        }


        protected void anonymousRegisterButton_Click(object sender, EventArgs e)
        {
            int registeredId = registerifAnonymous();
            var answers = Session["Answers"] as List<Answer>;


            foreach (Question question in questions)
            {
                TextBox textBox = (TextBox)registerPlaceHolder.FindControl("textBox_" + question.Id);
                if (textBox != null)
                {
                    string answerText = textBox.Text;
                    if (!string.IsNullOrEmpty(answerText))
                    {
                        answers.Add(new Answer
                        {
                            QuestionId = question.Id,
                            AnswerText = answerText,
                            RespondantId = registeredId
                        });
                    }
                }
            }

            foreach (var item in answers)
            {
                SaveAnswerToDatabase(item, registeredId);
            }

            Label label = new Label
            {
                Text = "Successfully Submitted"
            };
            registerPlaceHolder.Controls.Add(label);


            answers.Clear();
            Session["Answers"] = answers;
            Response.Redirect("~/Homepage.aspx");
 
        }

        protected void cancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("Homepage.aspx");
        }
    }
}
