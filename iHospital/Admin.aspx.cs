using iHospital.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital
{
    public partial class Admin : System.Web.UI.Page
    {

        private List<Question> questions
        {
            get
            {
                if (ViewState["OptionalQuestions"] == null)
                {
                    ViewState["OptionalQuestions"] = new List<Question>();
                }
                return (List<Question>)ViewState["OptionalQuestions"];
            }
            set
            {
                ViewState["OptionalQuestions"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadQuestions(); // Ensure questions are loaded
                BindQuestionsToGridView();
            }
        }

        private void BindQuestionsToGridView()
        {
            DataTable dt = new DataTable();

            // Add columns to DataTable based on questions
            foreach (Question question in questions)
            {
                dt.Columns.Add(question.QuestionText);
            }

            // Add a sample row for demonstration purposes
            // Replace this with actual data retrieval logic
            DataRow row = dt.NewRow();
            foreach (Question question in questions)
            {
                row[question.QuestionText] = "Sample Answer"; // Replace with actual answers if available
            }
            dt.Rows.Add(row);

            // Bind DataTable to GridView
            respondantGridView.DataSource = dt;
            respondantGridView.DataBind();
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
                            questions.Add(tempQuestion);
                            questions.Sort((x, y) => x.QuestionOrder.CompareTo(y.QuestionOrder));
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
    }
}
