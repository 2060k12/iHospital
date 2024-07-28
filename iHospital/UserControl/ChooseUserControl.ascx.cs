using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital.UserControl
{
    public partial class ChooseUserControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string QuestionText
        {
            get { return radioQuestionLabel.Text; }
            set { radioQuestionLabel.Text = value; }
        }

        public void SetChooseBoxListItems(List<KeyValuePair<string, int>> items)
        {
            radioList.DataSource = items;
            radioList.DataTextField = "Key";   // The text displayed in the RadioButtonList
            radioList.DataValueField = "Value"; // The value associated with the item
            radioList.DataBind();
        }

        public KeyValuePair<string, int>? GetChooseBoxListItems()
        {
            if (radioList.SelectedItem != null)
            {
                return new KeyValuePair<string, int>(
                    radioList.SelectedItem.Text,
                    Convert.ToInt32(radioList.SelectedItem.Value)
                );
            }
            return null;
        }
    }
}