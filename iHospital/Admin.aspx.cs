using iHospital.config;
using iHospital.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        string myConnectionString;

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

        private List<Option> options
        {
            get
            {
                if (ViewState["Options"] == null)
                {
                    ViewState["Options"] = new List<Option>();
                }
                return (List<Option>)ViewState["Options"];
            }
            set
            {
                ViewState["Options"] = value;
            }
        }

        private List<Answer> answerList
        {
            get
            {
                if (ViewState["AnswerList"] == null)
                {
                    ViewState["AnswerList"] = new List<Answer>();
                }
                return (List<Answer>)ViewState["AnswerList"];
            }
            set
            {
                ViewState["AnswerList"] = value;
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
                LoadOptions();
                LoadAnswers();
                PopulateQuestionsDropDown();
                BindQuestionsToGridView();
            }
        }

        private void LoadQuestions()
        {
            string connectionString = myConnectionString;

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

        private void LoadOptions()
        {
            string connectionString = myConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load options
                    string optionQuery = "SELECT * FROM [Option]";
                    using (SqlCommand cmd = new SqlCommand(optionQuery, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Option tempOption = new Option
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                QuestionId = Convert.ToInt32(reader["question_id"]),
                                Option_Value = reader["option_value"].ToString()
                            };
                            options.Add(tempOption);
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

        private void LoadAnswers()
        {
            string connectionString = myConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load answers
                    string answerQuery = "SELECT * FROM Answer";
                    using (SqlCommand cmd = new SqlCommand(answerQuery, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Answer tempAnswer = new Answer
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                AnswerText = reader["answer"].ToString(),
                                QuestionId = Convert.ToInt32(reader["question_id"]),
                                OptionId = reader["option_id"] != DBNull.Value ? (int?)Convert.ToInt32(reader["option_id"]) : null,
                                RespondantId = Convert.ToInt32(reader["respondant_id"])
                            };
                            answerList.Add(tempAnswer);
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

        private void PopulateQuestionsDropDown()
        {
            ddlQuestions.Items.Clear();
            ddlQuestions.Items.Add(new ListItem("Select Question", ""));

            foreach (var question in questions)
            {
                ddlQuestions.Items.Add(new ListItem(question.QuestionText, question.Id.ToString()));
            }
        }
        protected void ddlQuestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedQuestionId;
            if (int.TryParse(ddlQuestions.SelectedValue, out selectedQuestionId))
            {
                PopulateOptionsDropDown(selectedQuestionId);
            }
            else
            {
                ddlOptions.Items.Clear();
                ddlOptions.Items.Add(new ListItem("Select Option", ""));
            }

            // Re-bind the GridView with the filtered data
            BindQuestionsToGridView();
        }

        protected void ddlOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Re-bind the GridView with the filtered data
            BindQuestionsToGridView();
        }



        private void PopulateOptionsDropDown(int questionId)
        {
            ddlOptions.Items.Clear();
            ddlOptions.Items.Add(new ListItem("Select Option", ""));

            var filteredOptions = options.Where(o => o.QuestionId == questionId).ToList();

            foreach (var option in filteredOptions)
            {
                ddlOptions.Items.Add(new ListItem(option.Option_Value, option.Id.ToString()));
            }
        }



        private List<int> ApplyFilters()
        {
            var filteredAnswers = answerList;

            if (!string.IsNullOrEmpty(ddlQuestions.SelectedValue))
            {
                int selectedQuestionId = int.Parse(ddlQuestions.SelectedValue);
                filteredAnswers = filteredAnswers.Where(a => a.QuestionId == selectedQuestionId).ToList();
            }

            if (!string.IsNullOrEmpty(ddlOptions.SelectedValue))
            {
                int selectedOptionId = int.Parse(ddlOptions.SelectedValue);
                filteredAnswers = filteredAnswers.Where(a => a.OptionId == selectedOptionId).ToList();
            }

            // Return the list of respondent IDs that match the selected criteria
            return filteredAnswers.Select(a => a.RespondantId).Distinct().ToList();
        }




        private void BindQuestionsToGridView()
        {
            DataTable dt = new DataTable();

            // Add columns to DataTable based on questions
            foreach (Question question in questions)
            {
                dt.Columns.Add(question.QuestionText);
            }

            // Apply filters to get the list of respondent IDs that match the selected criteria
            var filteredRespondentIds = ApplyFilters();

            // Create rows for each filtered respondent
            foreach (var respondentId in filteredRespondentIds)
            {
                DataRow row = dt.NewRow();

                // Fill in the answers for this respondent
                foreach (Question question in questions)
                {
                    // Get the answers for this respondent and question
                    var answers = answerList.Where(a => a.QuestionId == question.Id && a.RespondantId == respondentId).ToList();

                    if (answers.Any())
                    {
                        var concatenatedAnswers = string.Join(", ", answers.Select(a =>
                        {
                            if (string.IsNullOrEmpty(a.AnswerText))
                            {
                                var option = options.FirstOrDefault(o => o.Id == a.OptionId);
                                return option != null ? option.Option_Value : "No Answer";
                            }
                            else
                            {
                                return a.AnswerText;
                            }
                        }));
                        row[question.QuestionText] = concatenatedAnswers;
                    }
                    else
                    {
                        row[question.QuestionText] = "No Answer"; // Or handle as needed
                    }
                }

                dt.Rows.Add(row);
            }

            // Bind DataTable to GridView
            respondantGridView.DataSource = dt;
            respondantGridView.DataBind();
        }




    }
}
