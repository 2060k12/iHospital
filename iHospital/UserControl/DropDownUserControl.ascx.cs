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

        public void SetCheckBoxListItems(string[] items)
        {
            dropDownListUC.DataSource = items;
            dropDownListUC.DataBind();
        }
    }
}