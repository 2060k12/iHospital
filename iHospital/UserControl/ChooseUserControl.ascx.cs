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

        public void SetCheckBoxListItems(string[] items)
        {
            radioList.DataSource = items;
            radioList.DataBind();
        }
    }
}