using iHospital.config;
using iHospital.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        string myConnectionString;
        static int currentDependentQuestionNumber = 0;


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
            DisplayCurrentQuestion();
        }

        private List<Question> nextDependantQuestions
        {
            get
            {
                if (ViewState["NextDependantQuestions"] == null)
                {
                    ViewState["NextDependantQuestions"] = new List<Question>();
                }
                return (List<Question>)ViewState["NextDependantQuestions"];
            }
            set
            {
                ViewState["NextDependantQuestions"] = value;
            }
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

                            if (tempQuestion.QuestionOrder != 999 && tempQuestion.QuestionOrder != 0)
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
            if (currentQuestionNumber == 0)
            {
                previousButton.Visible = false;
            }
            else
            {
                previousButton.Visible = true;
            }
            surveyPlaceHolder.Controls.Clear();



            if (currentQuestionNumber >= 0 && currentQuestionNumber < questions.Count)
            {
                Question currentQuestion = questions[currentQuestionNumber];

                if (nextDependantQuestions.Count > 0)
                {
                    currentQuestion = nextDependantQuestions.FirstOrDefault();

                    currentDependentQuestionNumber = currentQuestion.Id;
                }

                var items = options
                    .Where(o => o.QuestionId == currentQuestion.Id)
                    .Select(o => new KeyValuePair<string, int>(o.Option_Value, o.Id))
                    .ToList();

                Control questionControl = null;
                var questionType = currentQuestion.QuestionType;

                if (currentQuestion.QuestionType.Contains("select"))
                {
                    questionType = "select";
                }

                switch (questionType)
                {
                    case "choose_one":
                        Label chooseQuestionLabel = new Label
                        {
                            Text = currentQuestion.QuestionText,
                            CssClass = "question-label"
                        };

                        RadioButtonList radioButtonList = new RadioButtonList
                        {
                            ID = "radioList",
                            DataSource = items,
                            DataTextField = "Key",
                            DataValueField = "Value",
                            CssClass = "radio-list" // Apply CSS class for styling
                        };
                        radioButtonList.DataBind();
                        surveyPlaceHolder.Controls.Add(chooseQuestionLabel);
                        surveyPlaceHolder.Controls.Add(radioButtonList);
                        break;

                    case "drop_down":
                        Label dropDownQuestionLabel = new Label
                        {
                            Text = currentQuestion.QuestionText,
                            CssClass = "question-label"
                        };

                        DropDownList dropDownList = new DropDownList
                        {
                            ID = "dropDownList",
                            DataSource = items,
                            DataTextField = "Key",
                            DataValueField = "Value",
                            CssClass = "drop-down-list" // Apply CSS class for styling
                        };
                        dropDownList.DataBind();
                        surveyPlaceHolder.Controls.Add(dropDownQuestionLabel);
                        surveyPlaceHolder.Controls.Add(dropDownList);
                        break;

                    case "select":
                        Label checkBoxQuestionLabel = new Label
                        {
                            Text = currentQuestion.QuestionText,
                            CssClass = "question-label"
                        };

                        CheckBoxList checkBoxList = new CheckBoxList
                        {
                            ID = "optionsCheckBoxList",
                            DataSource = items,
                            DataTextField = "Key",
                            DataValueField = "Value",
                            CssClass = "check-box-list" // Apply CSS class for styling
                        };
                        checkBoxList.DataBind();
                        surveyPlaceHolder.Controls.Add(checkBoxQuestionLabel);
                        surveyPlaceHolder.Controls.Add(checkBoxList);

                        // Adding a hidden field to store the maximum selection count
                        var splitedQuestion = currentQuestion.QuestionType.Split('-');
                        var maxField = splitedQuestion.Last();

                        HiddenField maxSelectionsHiddenField = new HiddenField
                        {
                            ID = "maxSelectionsHiddenField",
                            Value = maxField    // Maximum selection count
                        };
                        surveyPlaceHolder.Controls.Add(maxSelectionsHiddenField);

                        break;

                    case "input":
                    case "email":
                        Label inputQuestionLabel = new Label
                        {
                            Text = currentQuestion.QuestionText,
                            CssClass = "question-label"
                        };

                        TextBox inputTextBox = new TextBox
                        {
                            ID = "inputTextBox",
                            CssClass = "text-box" // Apply CSS class for styling
                        };
                        surveyPlaceHolder.Controls.Add(inputQuestionLabel);
                        surveyPlaceHolder.Controls.Add(inputTextBox);
                        break;

                    case "date":
                        Label dateQuestionLabel = new Label
                        {
                            Text = currentQuestion.QuestionText,
                            CssClass = "question-label"
                        };

                        TextBox dateTextBox = new TextBox
                        {
                            ID = "dateTextBox",
                            CssClass = "date-text-box" // Apply CSS class for styling
                        };
                        surveyPlaceHolder.Controls.Add(dateQuestionLabel);
                        surveyPlaceHolder.Controls.Add(dateTextBox);
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

            if(nextDependantQuestions.Count > 0 && currentDependentQuestionNumber > 0)
            {
                nextDependantQuestions.Clear();
                currentDependentQuestionNumber = 0;
                DisplayCurrentQuestion();
                int nextQuestionId = questions[currentQuestionNumber ].Id;
                // Remove answers for the current question to prevent accumulation
                answers.RemoveAll(a => a.QuestionId == nextQuestionId);
                return;
            }

            // Ensure that currentQuestionNumber is greater than 0 to move to the previous question
            if (currentQuestionNumber > 0 && nextDependantQuestions.Count == 0)
            {
                    int questionId = questions[currentQuestionNumber -1 ].Id;
                    answers.RemoveAll(a => a.QuestionId == questionId);
         

                // Move to the previous question
                currentQuestionNumber--;
                DisplayCurrentQuestion();
            }

}


        protected void nextButton_Click(object sender, EventArgs e)
        {
            if (currentDependentQuestionNumber != 0 && nextDependantQuestions.Count > 0)
            {

                nextDependantQuestions.Remove(nextDependantQuestions.First());
            }

            foreach (Control control in surveyPlaceHolder.Controls)
            {
                ProcessCurrentAnswer(control);
            }

            if (currentQuestionNumber == questions.Count - 1)
            {
                Session["Answers"] = answers;
                Response.Redirect("~/Register.aspx");
            }

            foreach (Answer answer in answers)
            {

                if (currentDependentQuestionNumber == 0)
                {
                    if (answer.QuestionId == questions[currentQuestionNumber].Id)
                    {
                        foreach (DependentQuestion question in dependentQuestions)
                        {
                            if (question.OptionID == answer.OptionId)
                            {
                                // Check if the question is already in the nextDependantQuestions list
                                var dependentQuestion = optionalQuestions.FirstOrDefault(q => q.Id == question.QuestionId);
                                if (dependentQuestion != null)
                                {
                                    // Only add if it's not already present
                                    if (!nextDependantQuestions.Any(q => q.Id == dependentQuestion.Id))
                                    {
                                        nextDependantQuestions.Add(dependentQuestion);
                                    }
                                }
                            }
                        }

                    }
                }
                else
                {
                    if (answer.QuestionId == currentDependentQuestionNumber)
                    {
                        foreach (DependentQuestion question in dependentQuestions)
                        {
                            if (question.OptionID == answer.OptionId )
                            {
                                nextDependantQuestions.Add(optionalQuestions.Where(q => q.Id == question.QuestionId).FirstOrDefault());
                            }
                        }
                    }
                }


            }

            if (currentQuestionNumber < questions.Count && nextDependantQuestions.Count == 0)
            {
                currentQuestionNumber++;
            }
            currentDependentQuestionNumber = 0;

            currentDependentQuestionNumber = 0;
            DisplayCurrentQuestion();

            foreach (var answer in answers)
            {
                System.Diagnostics.Debug.WriteLine($"QuestionId: {answer.QuestionId}, OptionId: {answer.OptionId}, AnswerText: {answer.AnswerText}");
            }

           

        }

        private void ProcessCurrentAnswer(Control control)
        {
            if (control is RadioButtonList radioButtonList)
            {
                if (radioButtonList.SelectedItem != null)
                {
                    if (currentDependentQuestionNumber != 0)
                    {
                        answers.Add(new Answer
                        {
                            OptionId = Convert.ToInt32(radioButtonList.SelectedValue),
                            QuestionId = currentDependentQuestionNumber
                        });
                    }
                    else
                    {
                        answers.Add(new Answer
                        {
                            OptionId = Convert.ToInt32(radioButtonList.SelectedValue),
                            QuestionId = questions[currentQuestionNumber].Id
                        });
                    }
                }
            }
            else if (control is DropDownList dropDownList)
            {
                if (dropDownList.SelectedItem != null)
                {
                    if (currentDependentQuestionNumber != 0)
                    {
                        answers.Add(new Answer
                        {
                            OptionId = Convert.ToInt32(dropDownList.SelectedValue),
                            QuestionId = currentDependentQuestionNumber
                        });
                    }
                    else
                    {
                        answers.Add(new Answer
                        {
                            OptionId = Convert.ToInt32(dropDownList.SelectedValue),
                            QuestionId = questions[currentQuestionNumber].Id
                        });
                    }
                }
            }
            else if (control is CheckBoxList checkBoxList)
            {
                foreach (ListItem item in checkBoxList.Items)
                {
                    if (item.Selected)
                    {
                        if (currentDependentQuestionNumber != 0)
                        {
                            answers.Add(new Answer
                            {
                                OptionId = Convert.ToInt32(item.Value),
                                QuestionId = currentDependentQuestionNumber
                            });
                        }
                        else
                        {
                            answers.Add(new Answer
                            {
                                OptionId = Convert.ToInt32(item.Value),
                                QuestionId = questions[currentQuestionNumber].Id
                            });
                        }
                    }
                }
            }
            else if (control is TextBox textBox)
            {
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    if (currentDependentQuestionNumber != 0)
                    {
                        answers.Add(new Answer
                        {
                            OptionId = 0,
                            QuestionId = currentDependentQuestionNumber,
                            AnswerText = textBox.Text
                        });
                    }
                    else
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
}

