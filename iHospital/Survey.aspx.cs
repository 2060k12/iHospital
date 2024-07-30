using iHospital.Data;
using iHospital.UserControl;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
            DisplayCurrentQuestion();
        }

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

        private List<Question> questions
        {
            get
            {
                if (ViewState["Questions"] == null)
                {
                    ViewState["Questions"] = new List<Question>();
                }
                return (List<Question>)ViewState["Questions"];
            }
            set
            {
                ViewState["Questions"] = value;
            }
        }

        private List<Question> optionalQuestions
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

        private List<DependentQuestion> dependentQuestions
        {
            get
            {
                if (ViewState["DependentQuestions"] == null)
                {
                    ViewState["DependentQuestions"] = new List<DependentQuestion>();
                }
                return (List<DependentQuestion>)ViewState["DependentQuestions"];
            }
            set
            {
                ViewState["DependentQuestions"] = value;
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

                            if (tempQuestion.QuestionOrder != 0 && tempQuestion.QuestionOrder !=999)
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

                    // Load dependent questions
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
            surveyPlaceHolder.Controls.Clear();
            if (currentQuestionNumber >= 0 && currentQuestionNumber < questions.Count)
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
                        Label chooseQuestionLabel = new Label { Text = currentQuestion.QuestionText };
                        RadioButtonList radioButtonList = new RadioButtonList
                        {
                            ID = "radioList",
                            DataSource = items,
                            DataTextField = "Key",
                            DataValueField = "Value",
                            SelectedIndex = 0
                        };
                        radioButtonList.DataBind();
                        surveyPlaceHolder.Controls.Add(chooseQuestionLabel);
                        surveyPlaceHolder.Controls.Add(radioButtonList);
                        break;

                    case "drop_down":
                        Label dropDownQuestionLabel = new Label { Text = currentQuestion.QuestionText };
                        DropDownList dropDownList = new DropDownList
                        {
                            ID = "dropDownList",
                            DataSource = items,
                            DataTextField = "Key",
                            DataValueField = "Value"
                        };
                        dropDownList.DataBind();
                        surveyPlaceHolder.Controls.Add(dropDownQuestionLabel);
                        surveyPlaceHolder.Controls.Add(dropDownList);
                        break;

                    case "select":
                        Label checkBoxQuestionLabel = new Label { Text = currentQuestion.QuestionText };
                        CheckBoxList checkBoxList = new CheckBoxList
                        {
                            ID = "optionsCheckBoxList",
                            DataSource = items,
                            DataTextField = "Key",
                            DataValueField = "Value"
                        };
                        checkBoxList.DataBind();
                        surveyPlaceHolder.Controls.Add(checkBoxQuestionLabel);
                        surveyPlaceHolder.Controls.Add(checkBoxList);
                        break;

                    case "input":
                        Label inputQuestionLabel = new Label { Text = currentQuestion.QuestionText };
                        TextBox inputTextBox = new TextBox { ID = "inputTextBox" };
                        surveyPlaceHolder.Controls.Add(inputQuestionLabel);
                        surveyPlaceHolder.Controls.Add(inputTextBox);
                        break;


                    case "date":
                        Label dateQuestionLabel = new Label { Text = currentQuestion.QuestionText };
                        TextBox dateTextBox = new TextBox { ID = "dateTextBox" };
                        surveyPlaceHolder.Controls.Add(dateQuestionLabel);
                        surveyPlaceHolder.Controls.Add(dateTextBox);
                        // Optionally add date picker initialization script
                        break;
                }

                if (questionControl != null)
                {
                    surveyPlaceHolder.Controls.Add(questionControl);
                }
            }
        }


        protected void previousButton_Click(object sender, EventArgs e)
        {
            if (currentQuestionNumber > 0)
            {

                foreach (DependentQuestion dependentQuestion in dependentQuestions)
                {
                    if(currentQuestionNumber == dependentQuestion.QuestionId)
                    {
                        questions.RemoveAt(currentQuestionNumber );
                    }


                }
            
                currentQuestionNumber--;
                DisplayCurrentQuestion();
               

                  
              
            }
        }

        protected void nextButton_Click(object sender, EventArgs e)
        {

            if (currentQuestionNumber == questions.Count -1)
            {
                Session["Answers"] = answers;
                Response.Redirect("~/Register.aspx");
            }

            foreach (Control control in surveyPlaceHolder.Controls)
            {
                ProcessCurrentAnswer(control);
            }

            if (dependentQuestions.Count > 0)
            {
                HandleDependentQuestions();
            }

            if (currentQuestionNumber < questions.Count - 1)
            {
                currentQuestionNumber++;
                DisplayCurrentQuestion();
            }
            else
            {
                
            }

            foreach (var answer in answers)
            {
                System.Diagnostics.Debug.WriteLine($"QuestionId: {answer.QuestionId}, OptionId: {answer.OptionId}, AnswerText: {answer.AnswerText}");
            }
        }


        private void HandleDependentQuestions()
        {
            System.Diagnostics.Debug.WriteLine($"question count :" + questions.Count);

            List<Question> tempQuestion = questions;

            foreach (var depQuestion in dependentQuestions)
            {
                foreach (var answer in answers)
                {
                    if (depQuestion.OptionID == answer.OptionId)
                    {
                        var optionalQuestion = optionalQuestions.Find(ques => ques.Id == depQuestion.QuestionId);
                          if (optionalQuestion != null)
                        {
                            tempQuestion.Insert(currentQuestionNumber + 1, optionalQuestion);
                        }

                        questions = tempQuestion;
                        DisplayCurrentQuestion();
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine($"question count :" + questions.Count);

        }
        private void ProcessCurrentAnswer(Control control)
        {
            if (control is RadioButtonList radioButtonList)
            {
                if (radioButtonList.SelectedItem != null)
                {
                    answers.Add(new Answer
                    {
                        OptionId = Convert.ToInt32(radioButtonList.SelectedValue),
                        QuestionId = questions[currentQuestionNumber].Id
                    });
                }
            }
            else if (control is DropDownList dropDownList)
            {
                if (dropDownList.SelectedItem != null)
                {
                    answers.Add(new Answer
                    {
                        OptionId = Convert.ToInt32(dropDownList.SelectedValue),
                        QuestionId = questions[currentQuestionNumber].Id
                    });
                }
            }
            else if (control is CheckBoxList checkBoxList)
            {
                foreach (ListItem item in checkBoxList.Items)
                {
                    if (item.Selected)
                    {
                        answers.Add(new Answer
                        {
                            OptionId = Convert.ToInt32(item.Value),
                            QuestionId = questions[currentQuestionNumber].Id
                        });
                    }
                }
            }
            else if (control is TextBox textBox)
            {
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    answers.Add(new Answer
                    {
                        OptionId = 0,
                        QuestionId = questions[currentQuestionNumber].Id,
                        AnswerText = textBox.Text
                    });
                }
            }
        }


       
    }
}
