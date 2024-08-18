using iHospital.config;
using iHospital.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        string myConnectionString;
        static int currentDependentQuestionNumber = 0;


        // when the page loads for the  first time
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


        // List of questions, options, answers, dependent questions and optional questions
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


        // current question number

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

        // function which loads the questions from the database
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


        // function that will display the current question in the screen
        private void DisplayCurrentQuestion()
        {

            // Hide the previous button if the current question is the first question
            if (currentQuestionNumber == 0)
            {
                previousButton.Visible = false;
            }
            else
            {
                previousButton.Visible = true;
            }
            surveyPlaceHolder.Controls.Clear();


            // Display the current question

            if (currentQuestionNumber >= 0 && currentQuestionNumber < questions.Count)
            {
                Question currentQuestion = questions[currentQuestionNumber];

                if (nextDependantQuestions.Count > 0)
                {
                    currentQuestion = nextDependantQuestions.FirstOrDefault();

                    currentDependentQuestionNumber = currentQuestion.Id;
                }

                // Get the options for the current question

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
                            CssClass = "radio-list" 
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
                            CssClass = "drop-down-list" 
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
                            CssClass = "check-box-list" 
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
                    case "number":
                        Label inputQuestionLabel = new Label
                        {
                            Text = currentQuestion.QuestionText,
                            CssClass = "question-label"
                        };

                        TextBox inputTextBox = new TextBox
                        {
                            ID = "inputTextBox",
                            CssClass = "text-box" 
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
                            CssClass = "date-text-box" 
                        };
                        surveyPlaceHolder.Controls.Add(dateQuestionLabel);
                        surveyPlaceHolder.Controls.Add(dateTextBox);
                        break;
                }

                // Add the question control to the surveyPlaceHolder

                if (questionControl != null)
                {
                    surveyPlaceHolder.Controls.Add(questionControl);
                }
            }
        }

        // function that will be called when the previous button is clicked
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

           
            if (currentQuestionNumber > 0 && nextDependantQuestions.Count == 0)
            {
                    int questionId = questions[currentQuestionNumber -1 ].Id;
                    answers.RemoveAll(a => a.QuestionId == questionId);
         

                // Move to the previous question 
                currentQuestionNumber--;
                DisplayCurrentQuestion();
            }

}

        // function that will be called when the next button is clicked
        protected void nextButton_Click(object sender, EventArgs e)
        {
            bool hasError = false;

            foreach (Control control in surveyPlaceHolder.Controls)
            {
                if (control is TextBox textBox)
                {
                    string errorMessage;

                    // Validate the input based on the question type

                    switch (questions[currentQuestionNumber].QuestionType)
                    {
                        case "email":
                            if (!Validator.IsValidEmail(textBox.Text, out errorMessage))
                            {
                                if (textBox.Text == "")
                                {
                                    hasError = false;
                                    break;
                                }

                                else
                                {
                                    errorLbl.Text = errorMessage;
                                    errorLbl.Visible = true;
                                    hasError = true;
                                    break;  // Exit loop since there's an error
                                }
                            }
                            break;

                        case "date":
                            if (!Validator.IsValidDate(textBox.Text, out errorMessage) )
                            {
                                if (textBox.Text == "")
                                {
                                    hasError = false;
                                    break;
                                }

                                else
                                {
                                    errorLbl.Text = errorMessage;
                                    errorLbl.Visible = true;
                                    hasError = true;
                                    break;  // Exit loop since there's an error
                                }
                            }
                            break;

                        case "number":
                            if (!Validator.IsValidNumber(textBox.Text, out errorMessage))
                            {
                                if (textBox.Text == "") {
                                    hasError = false;
                                    break; }

                                else
                                {
                                    errorLbl.Text = errorMessage;
                                    errorLbl.Visible = true;
                                    hasError = true;
                                    break;  // Exit loop since there's an error
                                }
                            }
                            break;
                    }

                    if (hasError)
                    {
                        return;  // Prevent moving to the next question if there's an error
                    }
                    else
                    {
                        errorLbl.Visible = false;
                    }
                }
            }

            // Remove the current question from the nextDependantQuestions list

            if (currentDependentQuestionNumber != 0 && nextDependantQuestions.Count > 0)
            {

                nextDependantQuestions.Remove(nextDependantQuestions.First());
            }

            // Process the current answer

            foreach (Control control in surveyPlaceHolder.Controls)
            {
                ProcessCurrentAnswer(control);
            }

            if (currentQuestionNumber == questions.Count - 1)
            {
                Session["Answers"] = answers;
                Response.Redirect("~/Register.aspx");
            }

            // Check if there are dependent questions for the current question

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

            // Move to the next question

            if (currentQuestionNumber < questions.Count && nextDependantQuestions.Count == 0)
            {
                currentQuestionNumber++;
            }
            // Display the current question

            currentDependentQuestionNumber = 0;

            DisplayCurrentQuestion();

            foreach (var answer in answers)
            {
                System.Diagnostics.Debug.WriteLine($"QuestionId: {answer.QuestionId}, OptionId: {answer.OptionId}, AnswerText: {answer.AnswerText}");
            }

        }

        // function that will process the current answer

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

