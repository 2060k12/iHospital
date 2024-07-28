using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital.UserControl
{
    public partial class DropDownUserControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        public string QuestionText
        {
            get { return dropDownQuestionName.Text; }
            set { dropDownQuestionName.Text = value; }
        }

        public void SetDropDownListItems(List<KeyValuePair<string, int>> items)
        {
            dropDownListUC.DataSource = items;
            dropDownListUC.DataTextField = "Key";   // The text displayed in the RadioButtonList
            dropDownListUC.DataValueField = "Value"; // The value associated with the item
            dropDownListUC.DataBind();
        }


        public KeyValuePair<string, int>? GetDropDownListSelectedItem()
        {
            if (dropDownListUC.SelectedItem != null)
            {
                return new KeyValuePair<string, int>(
                    dropDownListUC.SelectedItem.Text,
                    Convert.ToInt32(dropDownListUC.SelectedItem.Value)
                );
            }
            return null;
        }
    }
}