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
        List<DependentQuestion> dependentQuestions = new List<DependentQuestion>();
        private List<Answer> answers
        {
            get
            {
                if (ViewState["Answers"] == null)
                {
                    ViewState["Answers"] = new List<Answer>();
                }
                return (List<Answer>)ViewState["Answers"];
            }
            set
            {
                ViewState["Answers"] = value;
            }
        }


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


                    // Load options
                    string dependentQuery = "SELECT * FROM Contingent_Questions";
                    using (SqlCommand cmd = new SqlCommand(dependentQuery, conn))
                    using (SqlDataReader dependentReader = cmd.ExecuteReader())
                    {
                        while (dependentReader.Read())
                        {
                            DependentQuestion tempDep = new DependentQuestion
                            {
                                Id = Convert.ToInt32(dependentReader["id"]),
                                QuestionId = Convert.ToInt32(dependentReader["question_id"]),
                                OptionID = Convert.ToInt32(dependentReader["option_id"])
                            };
                            dependentQuestions.Add(tempDep);
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
                var items = options
                    .Where(o => o.QuestionId == currentQuestion.Id)
                    .Select(o => new KeyValuePair<string, int>(o.Option_Value, o.Id))
                    .ToList();

                Control questionControl = null;

                switch (currentQuestion.QuestionType)
                {
                    case "choose_one":
                        ChooseUserControl chooseList = (ChooseUserControl)LoadControl("~/UserControl/ChooseUserControl.ascx");
                        chooseList.QuestionText = currentQuestion.QuestionText;
                        chooseList.SetChooseBoxListItems(items);
                        //questionContainer.Controls.Add(chooseList);
                        questionControl = chooseList;

                        break;

                    case "drop_down":
                        DropDownUserControl dropDown = (DropDownUserControl)LoadControl("~/UserControl/DropDownUserControl.ascx");
                        dropDown.QuestionText = currentQuestion.QuestionText;
                        dropDown.SetDropDownListItems(items);
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

                Answer answer = new Answer();
                
                if(PlaceHolder1.Controls.Count > 0)
                {
                    var control = PlaceHolder1.Controls[0];
                    if(control is ChooseUserControl)
                    {
                        var chooseControl = (ChooseUserControl)control;
                        var selectedOption = chooseControl.GetChooseBoxListItems();
                        if(selectedOption != null)
                        {
                            answer.OptionId = selectedOption.Value.Value;
                            answer.QuestionId = questions[currentQuestionNumber].Id;
                            answers.Add(answer);
                        }
                    }
                    else if(control is DropDownUserControl)
                    {
                        var dropDownControl = (DropDownUserControl)control;
                        var selectedOption = dropDownControl.GetDropDownListSelectedItem();
                        if(selectedOption != null)
                        {
                            answer.OptionId = selectedOption.Value.Value;
                            answer.QuestionId = questions[currentQuestionNumber].Id;
                            answers.Add(answer);
                        }
                    }
                    else if(control is CheckListControl)
                    {
                        var checkListControl = (CheckListControl)control;
                        var selectedOptions = checkListControl.GetCheckBoxListItems();
                        if(selectedOptions.Count > 0)
                        {
                            foreach (var item in selectedOptions)
                            {
                                answer.OptionId = item.Value ;
                                answer.QuestionId = questions[currentQuestionNumber].Id;
                                answers.Add(answer);
                            }
                        }
                    }
                    else if(control is InputUserControl)
                    {
                        var inputControl = (InputUserControl)control;
                        answer.OptionId = 0;
                        answer.QuestionId = questions[currentQuestionNumber].Id;
                        answer.AnswerText = inputControl.TextFieldText;
                        answers.Add(answer);
                    }
                }

                if(dependentQuestions.Count > 0)
                {
                    foreach (var item in dependentQuestions)
                    {
                        if( item.OptionID == answer.OptionId)
                        {
                            questions.Insert(currentQuestionNumber + 1, optionalQuestions.Find(ques => ques.Id == item.QuestionId));
                            DisplayCurrentQuestion();
                        }      
                    }
                }
               
            }
        }

        private void SaveAnswerToDatabase(Answer answer)
        {
            string connectionString = "Data Source=SQL5111.site4now.net;Initial Catalog=db_9ab8b7_224dda12275;User Id=db_9ab8b7_224dda12275_admin;Password=vWHVw5VW";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Answer ( question_id, option_id, answer, respondant_id) VALUES (@question_id, @optionId, @answer, @RespondantId)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@question_id", answer.QuestionId);
                        cmd.Parameters.AddWithValue("@optionId", (object)answer.OptionId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@answer", (object)answer.AnswerText ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RespondantId", 1);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }


        protected void StartSurveyButton_Click(object sender, EventArgs e)
        {

            ViewState["Answers"] = answers;

            foreach (var item in answers)
            {
                SaveAnswerToDatabase(item);
            }
            Label label = new Label();
            label.Text = "Successfully Submitted";
              
                PlaceHolder1.Controls.Add(label);
            }
        

    }
}