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
            set { questionValueLabel.Text = value ; }
        }

        public void SetCheckBoxListItems(List<KeyValuePair<string, int>> items)
        {
            optionsCheckBoxList.DataSource = items;
            optionsCheckBoxList.DataTextField = "Key";   // The text displayed in the CheckBoxList
            optionsCheckBoxList.DataValueField = "Value"; // The value associated with the item
            optionsCheckBoxList.DataBind();
        }

      

        public List<KeyValuePair<string, int>> GetCheckBoxListItems()
        {
            List<KeyValuePair<string, int>> selectedItems = new List<KeyValuePair<string, int>>();
            foreach (ListItem item in optionsCheckBoxList.Items)
            {
                if (item.Selected)
                {
                    selectedItems.Add(new KeyValuePair<string, int>(
                        item.Text,
                        Convert.ToInt32(item.Value)
                    ));
                }
            }

            return selectedItems;
        }
    }
}

