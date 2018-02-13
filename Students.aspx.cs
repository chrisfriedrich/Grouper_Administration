using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GroupBuilder;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Net.Mail;
using System.Threading;
using System.Web.Hosting;

namespace GroupBuilderAdmin
{
    public partial class Students : System.Web.UI.Page
    {
        private static string FILE_PATH = ".files";
        private static string LOCAL_PATH = "./App_Data/";
        private static bool LOCAL_FLAG = true;

        private int _InstructorCourseID;
        public int InstructorCourseID
        {
            get
            {
                _InstructorCourseID = 0;

                if(Session["ID"] != null)
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

        private void MessageBox(string title, string message, string button)
        {
            MessageBoxTitleLabel.Text = title;
            MessageBoxMessageLabel.Text = message;
            MessageBoxOkayLinkButton.Text = button;
            MessageBoxOkayLinkButton.Visible = true;
            MessageBoxCreateLinkButton.Visible = false;

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "messageBox", "$('#messageBox').modal();", true);
            upModal.Update();
        }

        private void ConfirmDeleteMessageBox(Student student)
        {
            SelectedStudentIDHiddenField.Value = student.StudentID.ToString();
            MessageBoxTitleLabel.Text = "Delete Student?";
            MessageBoxMessageLabel.Text = "Are you sure you want to delete the student '" + student.FirstName + " " + student.LastName + "?";
            MessageBoxOkayLinkButton.Text = "<span class='fa fa-ban'></span>&nbsp;&nbsp;Cancel";
            MessageBoxOkayLinkButton.Visible = true;
            MessageBoxCreateLinkButton.CssClass = "btn btn-danger";
            MessageBoxCreateLinkButton.Text = "<span class='fa fa-remove'></span>&nbsp;&nbsp;Delete Student";
            MessageBoxCreateLinkButton.Visible = true;

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "messageBox", "$('#messageBox').modal();", true);
            upModal.Update();
        }

        private void ConfirmDeleteAllStudentsMessageBox()
        {
            SelectedStudentIDHiddenField.Value = 0.ToString();
            MessageBoxTitleLabel.Text = "Delete All Students?";
            MessageBoxMessageLabel.Text = "Are you sure you want to delete all students associated with this course section?";
            MessageBoxOkayLinkButton.Text = "<span class='fa fa-ban'></span>&nbsp;&nbsp;Cancel";
            MessageBoxOkayLinkButton.Visible = true;
            MessageBoxCreateLinkButton.CssClass = "btn btn-danger";
            MessageBoxCreateLinkButton.Text = "<span class='fa fa-remove'></span>&nbsp;&nbsp;Delete Students";
            MessageBoxCreateLinkButton.Visible = true;

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "messageBox", "$('#messageBox').modal();", true);
            upModal.Update();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != "" && Request.QueryString["ID"] != null)
            {
                int instructorCourseID = int.Parse(Request.QueryString["ID"]);

                InstructorCourseID = instructorCourseID;

                InstructorCourse course = GrouperMethods.GetInstructorCourse(instructorCourseID);

                CourseNameLabel.Text = course.Course.FullName;

                StudentsGridView_BindGridView();
            }
        }

        protected void StudentsGridView_BindGridView()
        {

            InstructorCourse course = GrouperMethods.GetInstructorCourse(InstructorCourseID);

            StudentsGridView.DataSource = course.Students;
            StudentsGridView.DataBind();
        }

        protected void ProgrammingLanguagesGridView_BindGridView()
        {
            InstructorCourse course = GrouperMethods.GetInstructorCourse(InstructorCourseID);
        }

        protected void ProcessStudentsFileLinkButton_Click(object sender, EventArgs e)
        {
            List<Student> students = new List<Student>();

            HttpPostedFile file = StudentsFileUpload.PostedFile;

            string fileName = "";

            if (file.ContentLength > 0)
            {
                fileName = SaveFile(file);
            }

            string path = Server.MapPath(LOCAL_PATH + fileName);

            if (File.Exists(path))
            {
                StreamReader reader = File.OpenText(path);

                students = ParseFileForStudents(reader);
                int importedCount = 0;
                foreach (Student student in students)
                {
                    int studentID = GrouperMethods.InsertStudent(student);
                    if(studentID > 0)
                    {
                        importedCount++;
                    }
                }
                if(importedCount > 0)
                {
                    StudentsGridView_BindGridView();

                    ImportStudentsLinkButton.Visible = true;
                    ImportStudentsPanel.Visible = false;
                    StudentListPanel.Visible = true;
                    AddStudentLinkButton.Visible = true;

                    MessageBox("Student Records Imported", importedCount.ToString() + " student records were found and imported.", "Okay");
                }
            }
        }

        protected List<Student> ParseFileForStudents(StreamReader reader)
        {
            List<Student> studentList = new List<Student>();

            TextFieldParser parser = new TextFieldParser(reader.BaseStream);

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            int index = 0;

            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                if (index > 0)
                {
                    if (fields[0].Length > 0)
                    {
                        Student student = new Student();
                        student.InstructorCourseID = InstructorCourseID;
                        student.DuckID = fields[0].Trim();
                        student.LastName = fields[1].Trim();
                        student.FirstName = fields[2].Trim();

                        studentList.Add(student);
                    }
                }
                index++;
            }

            return studentList;
        }


        private string SaveFile(HttpPostedFile file)
        {

            string fileName = "Import_" + DateTime.Now.Day.ToString();
            string extension = ".csv";
            string fullFileName = fileName + extension;

            string pathToCheck = "";

            if (LOCAL_FLAG == false)
            {
                pathToCheck = FILE_PATH + fullFileName;
            }
            else
            {
                pathToCheck = Server.MapPath(LOCAL_PATH + fullFileName);
            }

            string tempfileName = "";

            if (System.IO.File.Exists(pathToCheck))
            {
                int counter = 2;
                while (System.IO.File.Exists(pathToCheck))
                {
                    tempfileName = fileName + "_" + counter.ToString() + extension;
                    if (LOCAL_FLAG == false)
                    {
                        pathToCheck = FILE_PATH + tempfileName;
                    }
                    else
                    {
                        pathToCheck = Server.MapPath(LOCAL_PATH + tempfileName);
                    }
                    counter++;
                }

                fileName = tempfileName;
            }
            else
            {
                fileName = fullFileName;
            }

            string savePath = "";

            if (LOCAL_FLAG == false)
            {
                savePath = FILE_PATH + fileName;
            }
            else
            {
                savePath = Server.MapPath(LOCAL_PATH + fileName);
            }

            file.SaveAs(savePath);
            return fileName;
        }

        protected void StudentsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if(e.CommandName == "delete_student")
            {
                int studentID = int.Parse(e.CommandArgument.ToString());
                Student student = GrouperMethods.GetStudent(studentID);

                ConfirmDeleteMessageBox(student);
            }
            if(e.CommandName == "edit_student")
            {
                int studentID = int.Parse(e.CommandArgument.ToString());

                SelectedStudentIDHiddenField.Value = studentID.ToString();

                Student student = GrouperMethods.GetStudent(studentID);



                FirstNameTextBox.Text = student.FirstName;
                LastNameTextBox.Text = student.LastName;
                DuckIDTextBox.Text = student.DuckID;

                ProgrammingLanguagesDropDownList.DataSource = GrouperMethods.GetLanguages();
                ProgrammingLanguagesDropDownList.DataBind();

                ViewState["Languages"] = new List<ProgrammingLanguage>();

                foreach(ProgrammingLanguage language in student.Languages)
                {
                    ((List<ProgrammingLanguage>)ViewState["Languages"]).Add(language);
                    ProgrammingLanguagesDropDownList.Items.FindByValue(language.LanguageID.ToString()).Enabled = false;
                }

                ProgrammingLanguagesGridView.DataSource = (List<ProgrammingLanguage>)ViewState["Languages"];
                ProgrammingLanguagesGridView.DataBind();

                RolesDropDownList.DataSource = GrouperMethods.GetRoles();
                RolesDropDownList.DataBind();

                ViewState["Roles"] = new List<Role>();
                
                foreach(Role role in student.InterestedRoles)
                {
                    ((List<Role>)ViewState["Roles"]).Add(role);
                    RolesDropDownList.Items.FindByValue(role.RoleID.ToString()).Enabled = false;
                }

                RolesGridView.DataSource = (List<Role>)ViewState["Roles"];
                RolesGridView.DataBind();

                SkillsDropDownList.DataSource = GrouperMethods.GetSkills();
                SkillsDropDownList.DataBind();

                ViewState["Skills"] = new List<Skill>();

                foreach (Skill skill in student.Skills)
                {
                    ((List<Skill>)ViewState["Skills"]).Add(skill);
                    SkillsDropDownList.Items.FindByValue(skill.SkillID.ToString()).Enabled = false;
                }

                SkillsGridView.DataSource = (List<Skill>)ViewState["Skills"];
                SkillsGridView.DataBind();

                StudentListPanel.Visible = false;
                AddStudentPanel.Visible = true;
            }
            if(e.CommandName == "send_welcome")
            {
                int studentID = int.Parse(e.CommandArgument.ToString());

                Student student = GrouperMethods.GetStudent(studentID);

                SendSurveyLinkMessage(student);

                student.InitialNotificationSentDate = DateTime.Now;
                GrouperMethods.UpdateStudent(student);

                StudentsGridView_BindGridView();
            }
        }

        protected void AddStudentLinkButton_Click(object sender, EventArgs e)
        {
            StudentListPanel.Visible = false;
            AddStudentPanel.Visible = true;

            DuckIDTextBox.Text = "";
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            AddStudentLinkButton.Visible = false;
            SendWelcomeToAllStudentsLinkButton.Visible = false;
            DeleteAllStudentsLinkButton.Visible = false;

            ProgrammingLanguagesDropDownList.DataSource = GrouperMethods.GetLanguages();
            ProgrammingLanguagesDropDownList.DataBind();

            RolesDropDownList.DataSource = GrouperMethods.GetRoles();
            RolesDropDownList.DataBind();

            SkillsDropDownList.DataSource = GrouperMethods.GetSkills();
            SkillsDropDownList.DataBind();

            RolesGridView.DataSource = null;
            RolesGridView.DataBind();

            ProgrammingLanguagesGridView.DataSource = null;
            ProgrammingLanguagesGridView.DataBind();

            SkillsGridView.DataSource = null;
            SkillsGridView.DataBind();

            ProgrammingLanguagesDropDownList.DataSource = GrouperMethods.GetLanguages();
            ProgrammingLanguagesDropDownList.DataBind();
        }

        protected void SaveAddStudentLinkButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(SelectedStudentIDHiddenField.Value))
            {

                Student student = new Student();
                student.InstructorCourseID = InstructorCourseID;
                student.FirstName = FirstNameTextBox.Text.Trim();
                student.LastName = LastNameTextBox.Text.Trim();
                student.DuckID = DuckIDTextBox.Text.Trim();

                int studentID = GrouperMethods.InsertStudent(student);

                if (studentID > 0)
                {
                    if (ViewState["Languages"] != null)
                    {
                        if (((List<ProgrammingLanguage>)ViewState["Languages"]).Count() > 0)
                        {
                            foreach (ProgrammingLanguage language in ((List<ProgrammingLanguage>)ViewState["Languages"]))
                            {
                                GrouperMethods.InsertStudentLanguage(studentID, language.LanguageID, (int)language.ProficiencyLevel);
                            }
                        }
                    }
                    if (ViewState["Roles"] != null)
                    {
                        if (((List<Role>)ViewState["Roles"]).Count() > 0)
                        {
                            foreach (Role role in ((List<Role>)ViewState["Roles"]))
                            {
                                GrouperMethods.InsertStudentRoleInterest(studentID, role.RoleID, (int)role.InterestLevel);
                            }
                        }
                    }
                    if (ViewState["Skills"] != null)
                    {
                        if (((List<Skill>)ViewState["Skills"]).Count() > 0)
                        {
                            foreach (Skill skill in ((List<Skill>)ViewState["Skills"]))
                            {
                                GrouperMethods.InsertStudentSkill(studentID, skill.SkillID, (int)skill.ProficiencyLevel);
                            }
                        }
                    }
                }
            }
            else
            {
                int studentID = int.Parse(SelectedStudentIDHiddenField.Value);

                Student student = GrouperMethods.GetStudent(studentID);

                student.FirstName = FirstNameTextBox.Text.Trim();
                student.LastName = LastNameTextBox.Text.Trim();
                student.DuckID = DuckIDTextBox.Text.Trim();

                if (ViewState["Languages"] != null)
                {
                    student.Languages = (List<ProgrammingLanguage>)ViewState["Languages"];
                }

                if(ViewState["Roles"] != null)
                {
                    student.InterestedRoles = (List<Role>)ViewState["Roles"];
                }

                if (ViewState["Skills"] != null)
                {
                    student.Skills = (List<Skill>)ViewState["Skills"];
                }

                GrouperMethods.UpdateStudent(student);
            }

            StudentsGridView_BindGridView();

            SelectedStudentIDHiddenField.Value = null;

            RolesGridView.DataSource = null;
            RolesGridView.DataBind();
            ProgrammingLanguagesGridView.DataSource = null;
            ProgrammingLanguagesGridView.DataBind();
            SkillsGridView.DataSource = null;
            SkillsGridView.DataBind();

            AddStudentPanel.Visible = false;
            DuckIDTextBox.Text = "";
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            AddStudentLinkButton.Visible = true;
            SendWelcomeToAllStudentsLinkButton.Visible = true;
            DeleteAllStudentsLinkButton.Visible = true;

            StudentListPanel.Visible = true;

        }

        protected void CancelAddStudentLinkButton_Click(object sender, EventArgs e)
        {
            AddStudentPanel.Visible = false;
            DuckIDTextBox.Text = "";
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            AddStudentLinkButton.Visible = true;
            SendWelcomeToAllStudentsLinkButton.Visible = true;
            DeleteAllStudentsLinkButton.Visible = true;

            RolesGridView.DataSource = null;
            RolesGridView.DataBind();
            ProgrammingLanguagesGridView.DataSource = null;
            ProgrammingLanguagesGridView.DataBind();
            SkillsGridView.DataSource = null;
            SkillsGridView.DataBind();

            StudentListPanel.Visible = true;
        }

        protected void ImportStudentsLinkButton_Click(object sender, EventArgs e)
        {
            ImportStudentsPanel.Visible = true;
            ImportStudentsLinkButton.Visible = false;

            StudentListPanel.Visible = false;
        }

        protected void CancelImportStudentsLinkButton_Click(object sender, EventArgs e)
        {
            ImportStudentsPanel.Visible = false;
            ImportStudentsLinkButton.Visible = true;

            StudentListPanel.Visible = true;
        }

        protected void SendSurveyLinkMessage(Student student)
        {
            InstructorCourse course = GrouperMethods.GetInstructorCourse(student.InstructorCourseID);

            string header = @" <html>
                                <head>
                                    <style>
                                        body {
                                            font-size:15px;
	                                        font-family:'Calibri',sans-serif;
                                            }
                                        p, ul, li {
                                            font-size:15px;
	                                        font-family:'Calibri',sans-serif;
                                            }
                                    </style>
                                </head>
                                <body>";

            string footer = @"</body>
                        </html>";

            string messageBody = header;

            messageBody +=
                                @"<table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%' id='bodyTable'>
                        <tr>
                            <td>
                                <table border='0' cellpadding='20' cellspacing='0' width='600' id='emailContainer'>
                                    <tr>
                                        <td valign='top'>";
            messageBody += "<h3>" + course.Course.FullName + " - Welcome</h3>";

            string link = "";

            link = "https://groupbuilder.azurewebsites.net?id=" + (student.FirstName + student.LastName);

            messageBody += @"</td>
                    </tr>
                    <tr>
                                    <td>
                                        <p>Welcome to " + course.Course.FullName + @".  This course requires you to complete a short survey to best match you to a group.</p>
                                        <p>
                                            At your earliest convenience, please visit <a href='" + link + "'>" + link + @"</a> and complete the survey.
                                        </p>
                                    </td>
                                </tr>";
            messageBody += @"   <tr>
                                    <td>
                                        <p>Please contact <a href='mailto:cnf@uoregon.edu'>Chris Friedrich</a> with general questions.</p>
                                    </td>
                                </tr>
                            </table>
                        </td> 
                    </tr>
                </table> ";

            messageBody += footer;

            SmtpClient client = new SmtpClient();
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("donotreply@uoregon.edu", course.Course.FullName);

            mailMessage.To.Add(student.DuckID + "@uoregon.edu");
            mailMessage.Subject = "Welcome to " + course.Course.Code;
            mailMessage.Body = messageBody;
            mailMessage.IsBodyHtml = true;

            if (client.Host != null)
            {
                if (mailMessage.To.Count > 0)
                {
                    try
                    {
                        client.Send(mailMessage);
                    }
                    catch (SmtpFailedRecipientException ex)
                    {
                        SmtpStatusCode statusCode = ex.StatusCode;

                        if (statusCode == SmtpStatusCode.MailboxBusy || statusCode == SmtpStatusCode.MailboxUnavailable || statusCode == SmtpStatusCode.TransactionFailed)
                        {
                            Thread.Sleep(2000);

                            client.Send(mailMessage);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        mailMessage.Dispose();
                    }
                }
            }
        }

        protected void DeleteAllStudentsLinkButton_Click(object sender, EventArgs e)
        {
            ConfirmDeleteAllStudentsMessageBox();

        }

        protected void SendWelcomeToAllStudentsLinkButton_Click(object sender, EventArgs e)
        {
            InstructorCourse course = GrouperMethods.GetInstructorCourse(InstructorCourseID);

            if (course.Students.Where(x => x.InitialNotificationSentDate == null).Count() == 0)
            {
                MessageBox("No Students to Notify", "All students have already been sent a welcome email.  To resend, select <b>Send Welcome</b> for the individual student row.", "Okay");
            }
            else
            {
                foreach (Student student in course.Students)
                {
                    if (student.InitialNotificationSentDate == null)
                    {
                        SendSurveyLinkMessage(student);

                        student.InitialNotificationSentDate = DateTime.Now;
                        GrouperMethods.UpdateStudent(student);
                    }
                }

                StudentsGridView_BindGridView();

                MessageBox("Welcome Messages Sent", "Welcome messages have been sent to all previously unnotified students.", "Okay");
            }
        }

        protected void AddProgrammingLanguageLinkButton_Click(object sender, EventArgs e)
        {
            int programmingLanguageID = int.Parse(ProgrammingLanguagesDropDownList.SelectedValue);
            
            if(ViewState["Languages"] != null)
            {
                List<ProgrammingLanguage> languages = (List<ProgrammingLanguage>)ViewState["Languages"];

                ProgrammingLanguage language = new ProgrammingLanguage();
                language.Name = ProgrammingLanguagesDropDownList.SelectedItem.Text;
                language.LanguageID = int.Parse(ProgrammingLanguagesDropDownList.SelectedValue);
                language.ProficiencyLevel = int.Parse(ProgrammingAbilityLevelDropDownList.SelectedValue);

                ((List<ProgrammingLanguage>)ViewState["Languages"]).Add(language);

                ProgrammingLanguagesGridView.DataSource = (List<ProgrammingLanguage>)ViewState["Languages"];
                ProgrammingLanguagesGridView.DataBind();

                ProgrammingLanguagesDropDownList.Items.FindByValue(language.LanguageID.ToString()).Enabled = false;
            }
            else
            {
                ViewState["Languages"] = new List<ProgrammingLanguage>();
                ProgrammingLanguage language = new ProgrammingLanguage();
                language.Name = ProgrammingLanguagesDropDownList.SelectedItem.Text;
                language.LanguageID = int.Parse(ProgrammingLanguagesDropDownList.SelectedValue);
                language.ProficiencyLevel = int.Parse(ProgrammingAbilityLevelDropDownList.SelectedValue);

                ((List<ProgrammingLanguage>)ViewState["Languages"]).Add(language);

                ProgrammingLanguagesGridView.DataSource = (List<ProgrammingLanguage>)ViewState["Languages"];
                ProgrammingLanguagesGridView.DataBind();
                ProgrammingLanguagesDropDownList.Items.FindByValue(language.LanguageID.ToString()).Enabled = false;

            }
        }

        protected void MessageBoxCreateLinkButton_Click(object sender, EventArgs e)
        {
            int studentID = int.Parse(SelectedStudentIDHiddenField.Value);

            if(studentID == 0)
            {

                InstructorCourse course = GrouperMethods.GetInstructorCourse(InstructorCourseID);

                foreach (Student student in course.Students)
                {
                    GrouperMethods.DeleteStudent(student.StudentID);
                }

                StudentsGridView_BindGridView();
                MessageBox("Students Deleted", "All students have been deleted.", "Okay");
            }
            else
            {
                GrouperMethods.DeleteStudent(studentID);
                MessageBox("Student Deleted", "The student record has been deleted.", "Okay");

                StudentsGridView_BindGridView();

            }
        }

        protected void ProgrammingLanguagesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if(e.CommandName == "delete_language")
            {
                int languageID = int.Parse(e.CommandArgument.ToString());

                if(ViewState["Languages"] != null)
                {
                    ((List<ProgrammingLanguage>)ViewState["Languages"]).RemoveAll(x => x.LanguageID == languageID).ToString();
                    ProgrammingLanguagesGridView.DataSource = (List<ProgrammingLanguage>)ViewState["Languages"];
                    ProgrammingLanguagesGridView.DataBind();

                    ProgrammingLanguagesDropDownList.Items.FindByValue(languageID.ToString()).Enabled = true;

                }
            }
        }

        protected void StudentsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if(e.Row.RowType == DataControlRowType.DataRow)
            {
                int studentID = Convert.ToInt32(StudentsGridView.DataKeys[e.Row.RowIndex].Values[0]);

                Student student = GrouperMethods.GetStudent(studentID);

                string languages = "";

                if(student.Languages != null)
                {
                    if(student.Languages.Count > 0)
                    {
                        languages += "<ul>";
                        foreach(ProgrammingLanguage language in student.Languages)
                        {
                            languages += "<li>" + language.Name + " - " + language.ProficiencyLevel.ToString() + "</li>";
                        }
                        languages += "</ul>";
                    }
                }
                Label programmingLanguagesLabel = (Label)e.Row.FindControl("LanguagesLabel");
                programmingLanguagesLabel.Text = languages;

                string roles = "";

                if (student.InterestedRoles != null)
                {
                    if (student.InterestedRoles.Count > 0)
                    {
                        roles += "<ul>";
                        foreach (Role role in student.InterestedRoles)
                        {
                            roles += "<li>" + role.Name + " - " + role.InterestLevel.ToString() + "</li>";
                        }
                        roles += "</ul>";
                    }
                }
                Label rolesLabel = (Label)e.Row.FindControl("RolesLabel");
                rolesLabel.Text = roles;


                string skills = "";

                if (student.Skills != null)
                {
                    if (student.Skills.Count > 0)
                    {
                        skills += "<ul>";
                        foreach (Skill skill in student.Skills)
                        {
                            skills += "<li>" + skill.Name + " - " + skill.ProficiencyLevel.ToString() + "</li>";
                        }
                        skills += "</ul>";
                    }
                }
                Label skillsLabel = (Label)e.Row.FindControl("SkillsLabel");
                skillsLabel.Text = skills;
            }
        }

        protected void BeginGroupingLinkButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Groups.aspx?ID=" + InstructorCourseID);
        }

        protected void AddSkillLinkButton_Click(object sender, EventArgs e)
        {
            int skillID = int.Parse(SkillsDropDownList.SelectedValue);

            if (ViewState["Skills"] != null)
            {
                List<Skill> skills = (List<Skill>)ViewState["Skills"];

                Skill skill = new Skill();
                skill.Name = SkillsDropDownList.SelectedItem.Text;
                skill.SkillID = int.Parse(SkillsDropDownList.SelectedValue);
                skill.ProficiencyLevel = int.Parse(SkillsLevelDropDownList.SelectedValue);

                ((List<Skill>)ViewState["Skills"]).Add(skill);

                SkillsGridView.DataSource = (List<Skill>)ViewState["Skills"];
                SkillsGridView.DataBind();

                SkillsDropDownList.Items.FindByValue(skill.SkillID.ToString()).Enabled = false;
            }
            else
            {
                ViewState["Skills"] = new List<Skill>();
                Skill skill = new Skill();
                skill.Name = SkillsDropDownList.SelectedItem.Text;
                skill.SkillID = int.Parse(SkillsDropDownList.SelectedValue);
                skill.ProficiencyLevel = int.Parse(SkillsLevelDropDownList.SelectedValue);

                ((List<Skill>)ViewState["Skills"]).Add(skill);

                SkillsGridView.DataSource = (List<Skill>)ViewState["Skills"];
                SkillsGridView.DataBind();
                SkillsDropDownList.Items.FindByValue(skill.SkillID.ToString()).Enabled = false;

            }
        }

        protected void SkillsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "delete_skill")
            {
                int skillID = int.Parse(e.CommandArgument.ToString());

                if (ViewState["Skills"] != null)
                {
                    ((List<Skill>)ViewState["Skills"]).RemoveAll(x => x.SkillID == skillID).ToString();
                    SkillsGridView.DataSource = (List<Skill>)ViewState["Skills"];
                    SkillsGridView.DataBind();

                    SkillsDropDownList.Items.FindByValue(skillID.ToString()).Enabled = true;

                }
            }
        }

        protected void AddRoleLinkButton_Click(object sender, EventArgs e)
        {
            int roleID = int.Parse(RolesDropDownList.SelectedValue);

            if (ViewState["Roles"] != null)
            {
                List<Role> roles = (List<Role>)ViewState["Roles"];

                Role role = new Role();
                role.Name = RolesDropDownList.SelectedItem.Text;
                role.RoleID = int.Parse(RolesDropDownList.SelectedValue);
                role.InterestLevel = int.Parse(RoleInterestDropDownList.SelectedValue);

                ((List<Role>)ViewState["Roles"]).Add(role);

                RolesGridView.DataSource = (List<Role>)ViewState["Roles"];
                RolesGridView.DataBind();

                RolesDropDownList.Items.FindByValue(role.RoleID.ToString()).Enabled = false;
            }
            else
            {
                ViewState["Roles"] = new List<Role>();
                Role role = new Role();
                role.Name = RolesDropDownList.SelectedItem.Text;
                role.RoleID = int.Parse(RolesDropDownList.SelectedValue);
                role.InterestLevel = int.Parse(RoleInterestDropDownList.SelectedValue);

                ((List<Role>)ViewState["Roles"]).Add(role);

                RolesGridView.DataSource = (List<Role>)ViewState["Roles"];
                RolesGridView.DataBind();
                RolesDropDownList.Items.FindByValue(role.RoleID.ToString()).Enabled = false;

            }
        }

        protected void RolesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "delete_role")
            {
                int roleID = int.Parse(e.CommandArgument.ToString());

                if (ViewState["Roles"] != null)
                {
                    ((List<Role>)ViewState["Roles"]).RemoveAll(x => x.RoleID == roleID).ToString();
                    RolesGridView.DataSource = (List<Role>)ViewState["Roles"];
                    RolesGridView.DataBind();

                    RolesDropDownList.Items.FindByValue(roleID.ToString()).Enabled = true;

                }
            }
        }
    }
}