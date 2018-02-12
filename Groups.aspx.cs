using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GroupBuilder;

namespace GroupBuilderAdmin
{
    public partial class Groups : System.Web.UI.Page
    {
        private int _InstructorCourseID;
        public int InstructorCourseID
        {
            get
            {
                _InstructorCourseID = 0;

                if (Session["ID"] != null)
                {
                    _InstructorCourseID = int.Parse(Session["ID"].ToString());

                }

                return _InstructorCourseID;
            }
            set
            {
                _InstructorCourseID = value;
                Session["ID"] = _InstructorCourseID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != "" && Request.QueryString["ID"] != null)
            {
                int instructorCourseID = int.Parse(Request.QueryString["ID"]);

                InstructorCourseID = instructorCourseID;

                InstructorCourse course = GrouperMethods.GetInstructorCourse(instructorCourseID);

                GroupsRepeater_BindRepeater();
            }

            if(!IsPostBack)
            {
                StudentsGridView_BindGridView();
            }
        }

        protected void GroupsRepeater_BindRepeater()
        {
            InstructorCourse course = GrouperMethods.GetInstructorCourse(InstructorCourseID);

            if (course.Groups.Count > 0)
            {


                NoGroupsPanel.Visible = false;
            }
            else
            {
                NoGroupsPanel.Visible = true;
            }
            GroupsRepeater.DataSource = course.Groups;
            GroupsRepeater.DataBind();
        }

        protected void StudentsGridView_BindGridView()
        {
            InstructorCourse course = GrouperMethods.GetInstructorCourse(InstructorCourseID);

            if(course.Groups.Count > 0)
            {
                NumberOfGroupsDropDownList.SelectedValue = course.Groups.Count.ToString();
            }

            StudentsGridView.DataSource = course.Students;
            StudentsGridView.DataBind();
        }

        protected void BuildGroupsLinkButton_Click(object sender, EventArgs e)
        {
            int numberOfGroups = int.Parse(NumberOfGroupsDropDownList.SelectedValue);

            for (int i = 0; i < numberOfGroups; i++)
            {
                Group group = new GroupBuilder.Group();
                group.InstructorCourseID = InstructorCourseID;
                group.GroupNumber = i + 1;
                group.Name = "Group " + (i + 1).ToString() + " (untitled)";
                GrouperMethods.InsertGroup(group);
            }

            GroupsRepeater_BindRepeater();
        }

        protected void CreateGroupsLinkButton_Click(object sender, EventArgs e)
        {

        }

        protected void CancelBuildGroupsLinkButton_Click(object sender, EventArgs e)
        {

        }

        protected void ReturnToStudentsLinkButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Students.aspx?ID=" + InstructorCourseID);
        }

        protected void ResetGroupsLinkButton_Click(object sender, EventArgs e)
        {
            StudentsGridView_BindGridView();
            GroupsRepeater_BindRepeater();
        }

        protected void DeleteAllGroupsLinkButton_Click(object sender, EventArgs e)
        {
            InstructorCourse course = GrouperMethods.GetInstructorCourse(InstructorCourseID);

            foreach(Group group in course.Groups)
            {
                GrouperMethods.DeleteGroup(group.GroupID);
            }
            StudentsGridView_BindGridView();
            GroupsRepeater_BindRepeater();
        }

    }
}