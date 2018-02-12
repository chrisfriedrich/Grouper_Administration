using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GroupBuilder;

namespace GroupBuilderAdmin
{
    public partial class Instructors : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InstructorsGridView_BindGridView();
        }

        protected void InstructorsGridView_BindGridView()
        {
            List<Instructor> instructors = GrouperMethods.GetInstructors();
            InstructorsGridView.DataSource = instructors;
            InstructorsGridView.DataBind();
        }
    }
}