﻿using System;
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
            MessageBoxOkayLinkButton.Text = "<span class='fa fa-ban'>&nbsp;&nbsp;Cancel</span>";
            MessageBoxOkayLinkButton.Visible = true;
            MessageBoxCreateLinkButton.CssClass = "btn btn-danger";
            MessageBoxCreateLinkButton.Text = "<span class='fa fa-remove'>&nbsp;&nbsp;Delete Student</span>";
            MessageBoxCreateLinkButton.Visible = true;

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "messageBox", "$('#messageBox').modal();", true);
            upModal.Update();
        }

        private void ConfirmDeleteAllStudentsMessageBox()
        {
            SelectedStudentIDHiddenField.Value = 0.ToString();
            MessageBoxTitleLabel.Text = "Delete All Students?";
            MessageBoxMessageLabel.Text = "Are you sure you want to delete all students associated with this course section?";
            MessageBoxOkayLinkButton.Text = "<span class='fa fa-ban'>&nbsp;&nbsp;Cancel</span>";
            MessageBoxOkayLinkButton.Visible = true;
            MessageBoxCreateLinkButton.CssClass = "btn btn-danger";
            MessageBoxCreateLinkButton.Text = "<span class='fa fa-remove'>&nbsp;&nbsp;Delete Students</span>";
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
                //ScriptManager.RegisterStartupScript(StudentsGridView, StudentsGridView.GetType(), "messageBox", "$('#messageBox').modal();", true);


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

            // Specify the path to save the uploaded file to.
            //string savePath = "c:\\temp\\uploads\\";

            // Get the name of the file to upload.
            //string fileName = file.FileName;
            string fileName = "Import_" + DateTime.Now.Day.ToString();
            string extension = ".csv";
            string fullFileName = fileName + extension;


            // Create the path and file name to check for duplicates.
            //string pathToCheck = Server.MapPath(FILE_PATH + fullFileName);

            string pathToCheck = "";

            if (LOCAL_FLAG == false)
            {
                pathToCheck = FILE_PATH + fullFileName;
            }
            else
            {
                pathToCheck = Server.MapPath(LOCAL_PATH + fullFileName);
            }

            // Create a temporary file name to use for checking duplicates.
            string tempfileName = "";

            // Check to see if a file already exists with the
            // same name as the file to upload.   

            if (System.IO.File.Exists(pathToCheck))
            {
                int counter = 2;
                while (System.IO.File.Exists(pathToCheck))
                {
                    // if a file with this name already exists,
                    // prefix the filename with a number.
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

                // Notify the user that the file name was changed.
                //UploadStatusLabel.Text = "A file with the same name already exists." +
                //    "<br />Your file was saved as " + fileName;
            }
            else
            {
                // Notify the user that the file was saved successfully.
                //UploadStatusLabel.Text = "Your file was uploaded successfully.";
                fileName = fullFileName;
            }

            // Append the name of the file to upload to the path.
            string savePath = "";

            if (LOCAL_FLAG == false)
            {
                savePath = FILE_PATH + fileName;
            }
            else
            {
                savePath = Server.MapPath(LOCAL_PATH + fileName);
            }

            // Call the SaveAs method to save the uploaded
            // file to the specified directory.

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
                //MessageBox("Student Deleted", "The student record has been deleted.", "Okay");

                //GrouperMethods.DeleteStudent(studentID);

                //StudentsGridView_BindGridView();
            }
            if(e.CommandName == "edit_student")
            {
                int studentID = int.Parse(e.CommandArgument.ToString());

                SelectedStudentIDHiddenField.Value = studentID.ToString();

                Student student = GrouperMethods.GetStudent(studentID);

                StudentListPanel.Visible = false;
                AddStudentPanel.Visible = true;

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

                GrouperMethods.UpdateStudent(student);
            }

            StudentsGridView_BindGridView();

            SelectedStudentIDHiddenField.Value = null;
            ViewState["Languages"] = null;

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

            // Embed header image
            string path;

            //if (HttpContext.Current != null)
            //{
            //    path = HttpContext.Current.Server.MapPath("~/Images/graduate-programs-email-header.jpg");
            //}
            //else
            //{
            //    path = HostingEnvironment.MapPath("~/Images/graduate-programs-email-header.jpg");
            //}

            //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(messageBody, null, MediaTypeNames.Text.Html);
            //LinkedResource headerImage = new LinkedResource(path, "image/jpeg");
            //headerImage.ContentId = "headerImage";
            //htmlView.LinkedResources.Add(headerImage);

            //mailMessage.AlternateViews.Add(htmlView);
            //Attachment att = new Attachment(path);
            //att.ContentDisposition.Inline = true;

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
            }
        }
    }
}