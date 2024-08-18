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
using static System.Collections.Specialized.BitVector32;

namespace iHospital
{
    public partial class Admin : System.Web.UI.Page
    {
        string myConnectionString;


        // Store the list of Respondants, Sessions, Questions, Options, and Answers in ViewState
        private List<Respondant> respondants
        {
            get
            {
                if (ViewState["Respondants"] == null)
                {
                    ViewState["Respondants"] = new List<Respondant>();
                }
                return (List<Respondant>)ViewState["Respondants"];
            }
            set
            {
                ViewState["Respondants"] = value;
            }
        }
        private List<Session> sessions
        {

            get
            {
                if (ViewState["Sessions"] == null)
                {
                    ViewState["Sessions"] = new List<Session>();
                }
                return (List<Session>)ViewState["Sessions"];
            }
            set
            {
                ViewState["Sessions"] = value;
            }
        }
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





        // loads the session information from the databse

        private void LoadSessions()


        {
            string connectionString = myConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load sessions
                    string sessionQuery = "SELECT * FROM Session";
                    using (SqlCommand cmd = new SqlCommand(sessionQuery, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        sessions = new List<Session>();
                        while (reader.Read())
                        {
                            Session tempSession = new Session
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                DateTime = Convert.ToDateTime(reader["date_time"]),
                                MacAddress = reader["mac_address"].ToString()
                            };
                            sessions.Add(tempSession);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }

            // Ensure the list is not null
            if (sessions == null)
            {
                sessions = new List<Session>();
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
                // Load the data from the database and display them in a grid view

                LoadQuestions();
                LoadOptions();
                LoadAnswers();
                LoadSessions();
                PopulateQuestionsDropDown();
                BindQuestionsToGridView();
            }
        }



        // Load the questions from the database
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


        // Load the options from the database
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

        // Load the answers from the database
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

        // Populate the questions dropdown list
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
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // Re-bind the GridView with the filtered data
            BindQuestionsToGridView();
        }

        // Populate the options dropdown list based on the selected question
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


        // Apply filters to the data and return the list of respondent that match the selected criteria

        private List<int> ApplyFilters()
        {
            var filteredAnswers = answerList;

            // Filter by Question
            if (!string.IsNullOrEmpty(ddlQuestions.SelectedValue))
            {
                int selectedQuestionId = int.Parse(ddlQuestions.SelectedValue);
                filteredAnswers = filteredAnswers.Where(a => a.QuestionId == selectedQuestionId).ToList();
            }

            // Filter by Option
            if (!string.IsNullOrEmpty(ddlOptions.SelectedValue))
            {
                int selectedOptionId = int.Parse(ddlOptions.SelectedValue);
                filteredAnswers = filteredAnswers.Where(a => a.OptionId == selectedOptionId).ToList();
            }

            // Filter by Answer Text
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                filteredAnswers = filteredAnswers.Where(a => a.AnswerText != null && a.AnswerText.Contains(txtSearch.Text)).ToList();
            }

            // New Filter by Respondant ID
            if (!string.IsNullOrEmpty(txtRespondantId.Text))
            {
                int respondentId;
                if (int.TryParse(txtRespondantId.Text, out respondentId))
                {
                    filteredAnswers = filteredAnswers.Where(a => a.RespondantId == respondentId).ToList();
                }
            }

            // New Filter by Session Date
            if (!string.IsNullOrEmpty(txtSessionDate.Text))
            {
                DateTime sessionDate;
                if (DateTime.TryParse(txtSessionDate.Text, out sessionDate))
                {
                    var respondentIdsWithSessionDate = sessions
                        .Where(s => s.DateTime.Date == sessionDate.Date)
                        .Select(s => s.Id)
                        .ToList();

                    filteredAnswers = filteredAnswers.Where(a => respondentIdsWithSessionDate.Contains(a.RespondantId)).ToList();
                }
            }

            // Return the list of respondent IDs that match the selected criteria
            return filteredAnswers.Select(a => a.RespondantId).Distinct().ToList();
        }

        protected void btnApplyFilters_Click(object sender, EventArgs e)
        {
            BindQuestionsToGridView();
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            // Clear all filter controls
            ddlQuestions.SelectedIndex = 0;
            ddlOptions.SelectedIndex = 0;
            txtSearch.Text = string.Empty;
            txtRespondantId.Text = string.Empty;
            txtSessionDate.Text = string.Empty;

            // Re-bind GridView with unfiltered data
            BindQuestionsToGridView();
        }





        private void BindQuestionsToGridView()
{
    DataTable dt = new DataTable();

    // Add Respondant ID as the first column
    dt.Columns.Add("Respondant ID");

    // Add columns for Session data
    dt.Columns.Add("Session DateTime");
    dt.Columns.Add("Session MacAddress");

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

        // Add Respondant ID
        row["Respondant ID"] = respondentId;

        // Add Session data
        if (sessions != null)
        {
            var session = sessions.FirstOrDefault(s => s.Id == respondentId); 
            if (session != null)
            {
                row["Session DateTime"] = session.DateTime;
                row["Session MacAddress"] = session.MacAddress;
            }
            else
            {
                row["Session DateTime"] = "N/A";
                row["Session MacAddress"] = "N/A";
            }
        }
        else
        {
            row["Session DateTime"] = "N/A";
            row["Session MacAddress"] = "N/A";
        }

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
                row[question.QuestionText] = "No Answer";
            }
        }

        dt.Rows.Add(row);
    }

    // Bind DataTable to GridView
    respondantGridView.DataSource = dt;
    respondantGridView.DataBind();

    // Ensure Respondant ID and Session columns are always visible
    if (respondantGridView.Columns.Count > 0)
    {
        respondantGridView.Columns[0].Visible = true; // Respondant ID
        respondantGridView.Columns[1].Visible = true; // Session DateTime
        respondantGridView.Columns[2].Visible = true; // Session MacAddress
    }
}



        protected void btnSignOut_Click(object sender, EventArgs e)
        {// Clear the session
            Session.Clear();
            Session.Abandon();


            // Redirect to the login page
            Response.Redirect("~/Homepage.aspx");
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            // Apply filters and re-bind the GridView
            BindQuestionsToGridView();
        }







    }
}
