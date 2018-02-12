using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GroupBuilder;
using Microsoft.AspNet.Identity;

namespace GroupBuilderAdmin
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                BindGridView();
                BindDropDownLists();
            }
        }

        protected void BindGridView()
        {

            string userName = Context.User.Identity.GetUserName();

            List<InstructorCourse> courses = new List<InstructorCourse>();

            if (!string.IsNullOrEmpty(userName))
            {
                Instructor instructor = GrouperMethods.GetInstructor(userName);

                if(instructor != null)
                {
                    InstructorNameLabel.Text = instructor.FirstName + " " + instructor.LastName;
                }

                courses = GrouperMethods.GetInstructorCourses(instructor.InstructorID);

                if(courses.Count > 0)
                {
                    CoursesPanel.Visible = true;
                    NoCoursesPanel.Visible = false;
                }
                else
                {
                    NoCoursesPanel.Visible = true;
                    CoursesPanel.Visible = false;
                }
            }
            //else
            //{
            //    courses = GrouperMethods.GetInstructorCourses();
            //}
            CoursesGridView.DataSource = courses;
            CoursesGridView.DataBind();
        }

        protected void BindDropDownLists()
        {
            List<Course> courses = GrouperMethods.GetCourses();

            CoursesDropDownList.DataSource = courses;
            CoursesDropDownList.DataBind();

            List<ListItem> years = new List<ListItem>();

            for(int i=DateTime.Now.Year; i < DateTime.Now.Year+10; i++)
            {
                ListItem year = new ListItem { Text = i.ToString(), Value = i.ToString() };
                years.Add(year);
            }

            YearsDropDownList.DataSource = years;
            YearsDropDownList.DataBind();

            List<ListItem> times = new List<ListItem>();

            for(int i=8; i < 19; i++)
            {
                for(int j=0; j < 2; j++)
                {
                    int hour = i;
                    if(i > 12)
                    {
                        hour = i - 12;
                    }

                    string minutes = (j * 30).ToString();

                    if (minutes.Length == 1)
                    {
                        minutes += "0";
                    }

                    string time = hour.ToString() + ":" + minutes;

                    ListItem item = new ListItem { Text = time, Value = time };
                    times.Add(item);

                }
            }

            TimesDropDownList.DataSource = times;
            TimesDropDownList.DataBind();
        }

        protected void CancelAddCourseLinkButton_Click(object sender, EventArgs e)
        {

        }

        protected void SaveCourseLinkButton_Click(object sender, EventArgs e)
        {
            string userName = Context.User.Identity.GetUserName();
            Instructor instructor = GrouperMethods.GetInstructor(userName);

            if (instructor != null)
            {
                InstructorCourse course = new InstructorCourse();
                course.InstructorID = instructor.InstructorID;
                course.CourseID = int.Parse(CoursesDropDownList.SelectedValue);
                course.TermNumber = int.Parse(TermsDropDownList.SelectedValue);
                course.Year = int.Parse(YearsDropDownList.SelectedValue);

                switch(course.TermNumber)
                {
                    case 1:
                        course.TermName = "Fall " + course.Year.ToString();
                        break;
                    case 2:
                        course.TermName = "Winter " + course.Year.ToString();
                        break;
                    case 3:
                        course.TermName = "Spring " + course.Year.ToString();
                        break;
                    case 4:
                        course.TermName = "Summer " + course.Year.ToString();
                        break;
                }

                int instructorCourseID = GrouperMethods.InsertInstructorCourse(course);

                BindGridView();
            }
        }

        protected void CoursesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int instructorCourseID = int.Parse(e.CommandArgument.ToString());

            if(e.CommandName == "delete_instructor_course")
            {
                GrouperMethods.DeleteInstructorCourse(instructorCourseID);

                BindGridView();
            }
            if(e.CommandName == "edit_instructor_course")
            {
                Response.Redirect("students.aspx?ID=" + instructorCourseID);
            }
        }
    }
}