using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iHospital.UserControl
{
    public partial class InputUserControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string QuestionText
        {
            get { return questionText.Text; }
            set { questionText.Text = value; }
        }

        public string TextFieldText
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }
        
     
    }
}