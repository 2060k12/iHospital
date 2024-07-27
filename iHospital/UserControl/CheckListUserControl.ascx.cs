using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital
{
    public partial class CheckListControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        public string QuestionText
        {
            get { return questionValueLabel.Text; }
            set { questionValueLabel.Text = value; }
        }

        public void SetCheckBoxListItems(string[] items)
        {
            optionsCheckBoxList.DataSource = items;
            optionsCheckBoxList.DataBind();
        }

    }
}