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

        protected void StudentsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if(e.Row.RowType == DataControlRowType.DataRow)
            {
                int studentID = Convert.ToInt32(StudentsGridView.DataKeys[e.Row.RowIndex].Values[0]);

                Student student = GrouperMethods.GetStudent(studentID);

                
                Label languagesLabel = (Label)e.Row.FindControl("LanguagesLabel");

                string languages = "";

                foreach(ProgrammingLanguage language in student.Languages.OrderBy(x => x.ProficiencyLevel))
                {
                    switch (language.Name)
                    {
                        case "Java":
                            languages += "<span class='fa fa-coffee'></span>";
                            break;
                        case "Python":
                            languages += "<span class='fa fa-python'></span>";
                            break;
                        case "Android":
                            languages += "<span class='fa fa-android'></span>";
                            break;
                        case "Web Programming (PHP)":
                            languages += "<span class='fa fa-php'></span>";
                            break;
                        case "Web Design (HTML/XML)":
                            languages += "<span class='fa fa-html5'></span>";
                            break;
                        case "C++":
                            languages += "<span style='font-size: small; font-weight: bold; margin-left: 3px; margin-right: 3px;'>C++</span>";
                            //languages += @"<span class='fa-layers fa-fw'>
                            //                <i class='fas fa-circle-o'></i>
                            //                <span class='fa-layers-text fa-inverse' data-fa-transform='shrink-8 down-3' style='font-weight:900'>C++</span>
                            //                      </span>";
                            break;
                        case "C":
                            languages += "<span style='font-size: small; font-weight: bold; margin-left: 3px; margin-right: 3px;'>C</span>";
                            break;
                    }
                }
                languagesLabel.Text = languages;
            }
        }
    }
}