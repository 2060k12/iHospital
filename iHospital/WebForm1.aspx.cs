using iHospital.Data;
using iHospital.UserControl;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
             LoadQuestions();

                DisplayCurrentQuestion();
            
        }

        // list of questions
        // FIrst list "question" will hold the list of questions that are not optional and "optionalQuestions" will hold optional questions
        List<Question> questions = new List<Question>();
        List<Question> optionalQuestions = new List<Question>();



 // Current Questions 
        private int currentQuestionNumber
        {
            get
            {
                return (int)(ViewState["CurrentQuestionNumber"] ?? 0);
            }
            set
            {
                ViewState["CurrentQuestionNumber"] = value;
            }
        }

        // list of all options
        List<Option> options = new List<Option>();

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

                            if (tempQuestion.QuestionOrder != 0)
                            {
                                questions.Add(tempQuestion);
                                questions.Sort((x, y) => x.QuestionOrder.CompareTo(y.QuestionOrder));
                            }
                            else
                            {
                                optionalQuestions.Add(tempQuestion);
                            }
                        }
                    }

                    // Load options
                    string optionQuery = "SELECT * FROM [Option]";
                    using (SqlCommand cmd = new SqlCommand(optionQuery, conn))
                    using (SqlDataReader optionReader = cmd.ExecuteReader())
                    {
                        while (optionReader.Read())
                        {
                            Option tempOption = new Option
                            {
                                Id = Convert.ToInt32(optionReader["id"]),
                                QuestionId = Convert.ToInt32(optionReader["question_id"]),
                                Option_Value = optionReader["option_value"].ToString()
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
        private void DisplayCurrentQuestion()
        {
            PlaceHolder1.Controls.Clear();
            if (currentQuestionNumber >=0 && currentQuestionNumber < questions.Count)
            {
                Question currentQuestion = questions[currentQuestionNumber];
                string[] items = options
                    .Where(o => o.QuestionId == currentQuestion.Id)
                    .Select(o => o.Option_Value)
                    .ToArray();

                Control questionControl = null;

                switch (currentQuestion.QuestionType)
                {
                    case "choose_one":
                        ChooseUserControl chooseList = (ChooseUserControl)LoadControl("~/UserControl/ChooseUserControl.ascx");
                        chooseList.QuestionText = currentQuestion.QuestionText;
                        chooseList.SetCheckBoxListItems(items);
                        //questionContainer.Controls.Add(chooseList);
                        questionControl = chooseList;

                        break;

                    case "drop_down":
                        DropDownUserControl dropDown = (DropDownUserControl)LoadControl("~/UserControl/DropDownUserControl.ascx");
                        dropDown.QuestionText = currentQuestion.QuestionText;
                        dropDown.SetCheckBoxListItems(items);
                        //questionContainer.Controls.Add(dropDown);
                        questionControl = dropDown;
                        break;

                    case "select":
                        CheckListControl checkList = (CheckListControl)LoadControl("~/UserControl/CheckListUserControl.ascx");
                        checkList.QuestionText = currentQuestion.QuestionText;
                        checkList.SetCheckBoxListItems(items);
                        //questionContainer.Controls.Add(checkList);
                        questionControl = checkList;
                        break;

                    case "input":
                        InputUserControl inputControl = (InputUserControl)LoadControl("~/UserControl/InputUserControl.ascx");
                        inputControl.QuestionText = currentQuestion.QuestionText;
                        //questionContainer.Controls.Add(inputControl);
                        questionControl = inputControl;
                        break;
                }
                if (questionControl != null)
                {
                    PlaceHolder1.Controls.Add(questionControl);
                }

            }
        }

        protected void previousButton_Click(object sender, EventArgs e)
        {
            if(currentQuestionNumber > 0)
            {
                currentQuestionNumber--;
                DisplayCurrentQuestion();
            }
        }

        protected void nextButton_Click(object sender, EventArgs e)
        {
               if(currentQuestionNumber < questions.Count - 1)
            {
                currentQuestionNumber++;
                DisplayCurrentQuestion();
            }
        }





       
    }
}