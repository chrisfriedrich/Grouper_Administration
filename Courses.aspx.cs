using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GroupBuilder;

namespace GroupBuilderAdmin
{
    public partial class Courses : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CoursesGridView_BindGridView();
        }

        protected void CoursesGridView_BindGridView()
        {
            List<Course> courses = GrouperMethods.GetCourses();
            CoursesGridView.DataSource = courses;
            CoursesGridView.DataBind();
        }
    }
}