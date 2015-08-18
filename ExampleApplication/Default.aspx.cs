using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ExampleApplication
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void testButton_Click(object sender, EventArgs e)
        {
            if(!Page.IsValid)
            {
                return;
            }

            testButton.CssClass = "btn btn-success";
            testButton.Text = "Successful Submission!";
        }
    }
}