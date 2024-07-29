using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital
{
    public partial class Register : System.Web.UI.Page
    {
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

            if (!IsPostBack)
            {
                LoadQuestions();

            }

                registerPagePlaceHolder.Controls.Clear();

                foreach (Question question in questions)
                {
                    Label label = new Label();
                    label.Text = question.QuestionText;
                    label.ID = "Label_" + question.Id;

                    TextBox textBox = new TextBox();
                    textBox.ID = "TextBox_" + question.Id;

               

                    registerPagePlaceHolder.Controls.Add(label);
                    registerPagePlaceHolder.Controls.Add(textBox);
                }

                Button button = new Button();
                button.Text = "Submit";
   
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

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load respondants
                    string questionQuery = "SELECT * FROM Respondant";
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
                            questions.Add(tempQuestion);
                            questions.Sort((x, y) => x.QuestionOrder.CompareTo(y.QuestionOrder));
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }




        }

    } }