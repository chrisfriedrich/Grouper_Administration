using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GroupBuilder 
{
    public class Course
    {
        public int CourseID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FullName { get
            {
                return Code + " - " + Name;
            }
            set
            {
                
            }
        }
        public bool CoreCourseFlag { get; set; }
        public double? Grade { get; set; }
    }

    public class Instructor
    {
        public int InstructorID { get; set; }
        public string DuckID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentityUserID { get; set; }
        public List<InstructorCourse> Courses { get; set; }

        public Instructor()
        {
            Courses = new List<InstructorCourse>();
        }
    }

    public class InstructorCourse
    {
        public int InstructorCourseID { get; set; }
        public int InstructorID { get; set; }
        public int CourseID { get; set; }
        public Course Course { get; set; }
        public int TermNumber { get; set; }
        public string TermName { get; set; }
        public int Year { get; set; }
        public bool? CurrentCourseSectionFlag { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<Student> Students { get; set; }
        public InstructorCourse()
        {
            Students = new List<Student>();
        }
    }

    public class Skill
    {
        public int SkillID { get; set; }
        public string Name { get; set; }
        public int? ProficiencyLevel { get; set; }
    }

    [Serializable]
    public class ProgrammingLanguage
    {
        public int LanguageID { get; set; }
        public string Name { get; set; }
        public int? ProficiencyLevel { get; set; }
    }

    public class Student
    {
        public int StudentID { get; set; }
        public int InstructorCourseID { get; set; }
        public string DuckID { get; set; }
        public int UOID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PreferredName { get; set; }
        public int? OutgoingLevel { get; set;}
        public string DevelopmentExperience { get; set; }
        public string LearningExpectations { get; set; }
        public string ContributingRole { get; set; }
        public string OtherProgrammingLanguage { get; set; }
        public int? OtherProgrammingLanguageProficiency { get; set; }
        public DateTime? InitialNotificationSentDate { get; set; }
        public List<Skill> Skills { get; set; }
        public List<ProgrammingLanguage> Languages { get; set; }
        public List<Course> PriorCourses { get; set; }

        public Student()
        {
            Skills = new List<Skill>();
            Languages = new List<ProgrammingLanguage>();
            PriorCourses = new List<Course>();
        }
    }

    public class Group
    {
        public int GroupID { get; set; }
        public int InstructorCourseID { get; set; }
        public int GroupNumber { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public List<Student> Members { get; set; }
    }


    public class GrouperMethods
    {
        private static System.Configuration.ConnectionStringSettings GrouperConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"];

        #region Helper Methods

        public static string GetSafeString(SqlDataReader reader, string key)
        {
            string returnValue = "";

            returnValue = reader[key].ToString();

            return returnValue;
        }

        public static int GetSafeInteger(SqlDataReader reader, string key)
        {
            int returnValue = 0;
            string rawValue = GetSafeString(reader, key);

            Int32.TryParse(rawValue, out returnValue);
            return returnValue;
        }

        public static bool GetSafeBoolean(SqlDataReader reader, string key)
        {
            bool returnValue = false;
            string rawValue = GetSafeString(reader, key);

            Boolean.TryParse(rawValue, out returnValue);
            return returnValue;
        }

        public static DateTime? GetSafeDateTime(SqlDataReader reader, string key)
        {
            DateTime returnValue;
            string rawValue = GetSafeString(reader, key);

            if (DateTime.TryParse(rawValue, out returnValue))
            {
                return returnValue;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Instructors

        public static int InsertInstructor(Instructor instructor)
        {
            int instructorID = 0;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT *  
                FROM Instructors    
                WHERE DuckID = @DuckID;", con);
            cmd.Parameters.AddWithValue("@DuckID", instructor.DuckID);

            instructorID = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            if (instructorID == 0)
            {
                cmd = new SqlCommand(
                @"INSERT INTO Instructors    
                    (DuckID, LastName, FirstName, IdentityUserID) 
                    VALUES 
                    (@DuckID, @LastName, @FirstName, @IdentityUserID);
                    SELECT SCOPE_IDENTITY();", con);
                cmd.Parameters.AddWithValue("@DuckID", instructor.DuckID);
                cmd.Parameters.AddWithValue("@LastName", instructor.LastName);
                cmd.Parameters.AddWithValue("@FirstName", instructor.FirstName);
                cmd.Parameters.AddWithValue("@IdentityUserID", instructor.IdentityUserID);
                con.Open();
                instructorID = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
            }
            return instructorID;
        }

        public static List<Instructor> GetInstructors()
        {
            List<Instructor> instructors = new List<Instructor>();
            List<int> instructorIDs = new List<int>();

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Instructors ORDER BY LastName, FirstName;", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Instructor instructor = new Instructor();

                int instructorID = GetSafeInteger(reader, "InstructorID");

                instructorIDs.Add(instructorID);
            }
            con.Close();

            foreach(int id in instructorIDs)
            {
                Instructor instructor = GetInstructor(id);
                if (instructor != null)
                {
                    instructors.Add(instructor);
                }
            }

            return instructors;
        }

        public static Instructor GetInstructor(int instructorID)
        {
            Instructor instructor = null;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Instructors WHERE InstructorID = @InstructorID;", con);
            cmd.Parameters.AddWithValue("@InstructorID", instructorID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                instructor = new Instructor();

                instructor.InstructorID = GetSafeInteger(reader, "InstructorID");
                instructor.FirstName = GetSafeString(reader, "FirstName");
                instructor.LastName = GetSafeString(reader, "LastName");
                instructor.DuckID = GetSafeString(reader, "DuckID");
                instructor.IdentityUserID = GetSafeString(reader, "IdentityUserID");
            }
            con.Close();

            if (instructor != null)
            {
                instructor.Courses = GetInstructorCourses(instructor.InstructorID);
            }
            return instructor;
        }

        public static Instructor GetInstructor(string userName)
        {
            Instructor instructor = null;
            int instructorID = 0;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Instructors WHERE IdentityUserID = @IdentityUserID;", con);
            cmd.Parameters.AddWithValue("@IdentityUserID", userName);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                instructorID = GetSafeInteger(reader, "InstructorID");
            }
            con.Close();

            if (instructorID > 0)
            {
                instructor = GetInstructor(instructorID);
            }
            return instructor;
        }


        public static void UpdateInstructor(Instructor instructor)
        {

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(
                @"UPDATE Instructors 
                    SET DuckID = @DuckID,
                        FirstName = @FirstName,
                        LastName = @LastName, 
                        IdentityUserID = @IdentityUserID 
                    WHERE InstructorID = @InstructorID;", con);
            cmd.Parameters.AddWithValue("@InstructorID", instructor.InstructorID);
            cmd.Parameters.AddWithValue("@DuckID", instructor.DuckID);
            cmd.Parameters.AddWithValue("@FirstName", instructor.FirstName);
            cmd.Parameters.AddWithValue("@LastName", instructor.LastName);
            cmd.Parameters.AddWithValue("@IdentityUserID", instructor.IdentityUserID);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void DeleteInstructor(int instructorID)
        {

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(
                @"DELETE FROM Instructors WHERE InstructorID = @InstructorID;", con);
            cmd.Parameters.AddWithValue("@InstructorID", instructorID);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }


        #endregion

        #region Programming Languages 

        public static List<ProgrammingLanguage> GetLanguages()
        {
            List<ProgrammingLanguage> languages = new List<ProgrammingLanguage>();
            List<int> languageIDs = new List<int>();

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM ProgrammingLanguages ORDER BY Name;", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ProgrammingLanguage language = new ProgrammingLanguage();

                int languageID = GetSafeInteger(reader, "LanguageID");

                languageIDs.Add(languageID);
            }
            con.Close();

            foreach (int id in languageIDs)
            {
                ProgrammingLanguage language = GetLanguage(id);
                if (languages != null)
                {
                    languages.Add(language);
                }
            }

            return languages;
        }

        public static ProgrammingLanguage GetLanguage(int languageID)
        {
            ProgrammingLanguage language = null;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM ProgrammingLanguages WHERE LanguageID = @LanguageID;", con);
            cmd.Parameters.AddWithValue("@LanguageID", languageID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                language = new ProgrammingLanguage();

                language.LanguageID = GetSafeInteger(reader, "LanguageID");
                language.Name = GetSafeString(reader, "Name");

            }
            con.Close();

            return language;
        }
        #endregion

        #region Courses

        public static int InsertCourse(Course course)
        {
            int courseID = 0;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT *  
                FROM Courses    
                Code = @Code 
                AND Name = @Name;", con);
            cmd.Parameters.AddWithValue("@Code", course.Code);
            cmd.Parameters.AddWithValue("@Name", course.Name);

            courseID = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            if (courseID == 0)
            {
                cmd = new SqlCommand(
                @"INSERT INTO Courses    
                    (Code, Name, CoreCourseFlag) 
                    VALUES 
                    (@Code, @Name, @CoreCourseFlag);
                    SELECT SCOPE_IDENTITY();", con);
                cmd.Parameters.AddWithValue("@Code", course.Code);
                cmd.Parameters.AddWithValue("@Name", course.Name);
                cmd.Parameters.AddWithValue("@CoreCourseFlag", course.CoreCourseFlag);
                con.Open();
                courseID = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
            }
            return courseID;
        }

        public static List<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();
            List<int> courseIDs = new List<int>();

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Courses ORDER BY Code, Name;", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Course course = new Course();

                int courseID = GetSafeInteger(reader, "CourseID");

                courseIDs.Add(courseID);
            }
            con.Close();

            foreach (int id in courseIDs)
            {
                Course course = GetCourse(id);
                if (course != null)
                {
                    courses.Add(course);
                }
            }

            return courses;
        }

        public static Course GetCourse(int courseID)
        {
            Course course = null;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Courses WHERE CourseID = @CourseID;", con);
            cmd.Parameters.AddWithValue("@CourseID", courseID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                course = new Course();

                course.CourseID = GetSafeInteger(reader, "CourseID");
                course.Code = GetSafeString(reader, "Code");
                course.Name = GetSafeString(reader, "Name");
                course.CoreCourseFlag = GetSafeBoolean(reader, "CoreCourseFlag");

            }
            con.Close();

            return course;
        }

        public static void UpdateCourse(Course course)
        {

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(
                @"UPDATE Courses 
                    SET Code = @Code,
                        Name = @Name,
                        CoreCourseFlag = @CoreCourseFlag 
                    WHERE CourseID = @CourseID;", con);
            cmd.Parameters.AddWithValue("@CourseID", course.CourseID);
            cmd.Parameters.AddWithValue("@Code", course.Code);
            cmd.Parameters.AddWithValue("@Name", course.Name);
            cmd.Parameters.AddWithValue("@CoreCourseFlag", course.CoreCourseFlag);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void DeleteCourse(int courseID)
        {

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(
                @"DELETE FROM Courses WHERE CourseID = @CourseID;", con);
            cmd.Parameters.AddWithValue("@CourseID", courseID);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }


        #endregion

        #region Students

        public static int InsertStudent(Student student)
        {
            int studentID = 0;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT *  
                FROM Students     
                WHERE DuckID = @DuckID 
                AND InstructorCourseID = @InstructorCourseID;", con);
            cmd.Parameters.AddWithValue("@DuckID", student.DuckID);
            cmd.Parameters.AddWithValue("@InstructorCourseID", student.InstructorCourseID);
            studentID = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            if (studentID == 0)
            {
                cmd = new SqlCommand(
                @"INSERT INTO Students    
                    (DuckID, InstructorCourseID, LastName, FirstName, PreferredName, UOID, OutgoingLevel, DevelopmentExperience, LearningExpectations) 
                    VALUES 
                    (@DuckID, @InstructorCourseID, @LastName, @FirstName, @PreferredName, @UOID, @OutgoingLevel, @DevelopmentExperience, @LearningExpectations);
                    SELECT SCOPE_IDENTITY();", con);
                cmd.Parameters.AddWithValue("@DuckID", student.DuckID ?? (Object)DBNull.Value);
                cmd.Parameters.AddWithValue("@InstructorCourseID", student.InstructorCourseID);
                cmd.Parameters.AddWithValue("@LastName", student.LastName ?? (Object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FirstName", student.FirstName ?? (Object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PreferredName", student.PreferredName ?? (Object)DBNull.Value);
                if (student.UOID > 0)
                {
                    cmd.Parameters.AddWithValue("@UOID", student.UOID);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@UOID", (Object)DBNull.Value);
                }
                cmd.Parameters.AddWithValue("@OutgoingLevel", student.OutgoingLevel ?? (Object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DevelopmentExperience", student.DevelopmentExperience ?? (Object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LearningExpectations", student.LearningExpectations ?? (Object)DBNull.Value);
                con.Open();
                studentID = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
            }
            return studentID;
        }

        public static void InsertStudentLanguage(int studentID, int languageID, int proficiencyLevel)
        {

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT *  FROM Students_ProgrammingLanguages  
                WHERE StudentID = @StudentID 
                AND LanguageID = @LanguageID;", con);
            cmd.Parameters.AddWithValue("@StudentID", studentID);
            cmd.Parameters.AddWithValue("@LanguageID", languageID);

            var exists = cmd.ExecuteScalar();
            con.Close();
            if (exists == null)
            {
                cmd = new SqlCommand(
                @"INSERT INTO Students_ProgrammingLanguages 
                    (StudentID, LanguageID, ProficiencyRank) 
                    VALUES 
                    (@StudentID, @LanguageID, @ProficiencyRank);
                    SELECT SCOPE_IDENTITY();", con);
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                cmd.Parameters.AddWithValue("@LanguageID", languageID);
                cmd.Parameters.AddWithValue("@ProficiencyRank", proficiencyLevel);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public static Student GetStudent(int studentID)
        {
            Student student = null;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Students WHERE StudentID = @StudentID;", con);
            cmd.Parameters.AddWithValue("@StudentID", studentID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                student = new Student();

                student.StudentID = studentID;
                student.InstructorCourseID = GetSafeInteger(reader, "InstructorCourseID");
                student.DuckID = GetSafeString(reader, "DuckID");
                student.FirstName = GetSafeString(reader, "FirstName");
                student.LastName = GetSafeString(reader, "LastName");
                student.PreferredName = GetSafeString(reader, "PreferredName");
                student.UOID = GetSafeInteger(reader, "UOID");
                student.DevelopmentExperience = GetSafeString(reader, "DevelopmentExperience");
                student.LearningExpectations = GetSafeString(reader, "LearningExpectations");
                student.InitialNotificationSentDate = GetSafeDateTime(reader, "InitialNotificationSentDate");

            }
            con.Close();

            if(student != null)
            {
                student.Languages = GetStudentLanguages(student.StudentID);
            }
            return student;
        }

        public static List<ProgrammingLanguage> GetStudentLanguages(int studentID)
        {
            List<ProgrammingLanguage> languages = new List<ProgrammingLanguage>();

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT pl.*, spl.ProficiencyRank FROM Students_ProgrammingLanguages spl JOIN ProgrammingLanguages pl ON spl.LanguageID = pl.LanguageID WHERE spl.StudentID = @StudentID ORDER BY pl.Name;", con);
            cmd.Parameters.AddWithValue("@StudentID", studentID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ProgrammingLanguage language = new ProgrammingLanguage();

                language.LanguageID = GetSafeInteger(reader, "LanguageID");
                language.Name = GetSafeString(reader, "Name");
                language.ProficiencyLevel = GetSafeInteger(reader, "ProficiencyRank");

                languages.Add(language);

            }
            con.Close();

            return languages;
        }

        public static void DeleteStudentLanguages(int studentID)
        {
            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"DELETE FROM Students_ProgrammingLanguages WHERE StudentID = @StudentID;", con);
            cmd.Parameters.AddWithValue("@StudentID", studentID);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static List<Student> GetStudents()
        {
            List<Student> students = new List<Student>();
            List<int> studentIDs = new List<int>();

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Students ORDER BY LastName, FirstName;", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int studentID = GetSafeInteger(reader, "StudentID");
                studentIDs.Add(studentID);
            }
            con.Close();

            foreach (int studentID in studentIDs)
            {
                Student student = GrouperMethods.GetStudent(studentID);
                if(student != null)
                {
                    students.Add(student);
                }
            }
            return students;
        }

        public static List<Student> GetStudents(int instructorCourseID)
        {
            List<Student> students = new List<Student>();
            List<int> studentIDs = new List<int>();

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Students WHERE InstructorCourseID = @InstructorCourseID ORDER BY LastName, FirstName;", con);
            cmd.Parameters.AddWithValue("@InstructorCourseID", instructorCourseID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int studentID = GetSafeInteger(reader, "StudentID");
                studentIDs.Add(studentID);
            }
            con.Close();

            foreach (int studentID in studentIDs)
            {
                Student student = GrouperMethods.GetStudent(studentID);
                if (student != null)
                {
                    students.Add(student);
                }
            }
            return students;
        }

        public static void UpdateStudent(Student student)
        {

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(
                @"UPDATE Students  
                    SET DuckID = @DuckID,
                        FirstName = @FirstName,
                        LastName = @LastName, 
                        InitialNotificationSentDate = @InitialNotificationSentDate 
                    WHERE StudentID = @StudentID;", con);
            cmd.Parameters.AddWithValue("@StudentID", student.StudentID);
            cmd.Parameters.AddWithValue("@DuckID", student.DuckID);
            cmd.Parameters.AddWithValue("@FirstName", student.FirstName);
            cmd.Parameters.AddWithValue("@LastName", student.LastName);
            cmd.Parameters.AddWithValue("@InitialNotificationSentDate", student.InitialNotificationSentDate ?? (Object)DBNull.Value);


            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            DeleteStudentLanguages(student.StudentID);

            foreach(ProgrammingLanguage language in student.Languages)
            {
                InsertStudentLanguage(student.StudentID, language.LanguageID, (int)language.ProficiencyLevel);
            }
        }

        public static void DeleteStudent(int studentID)
        {

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(
                @"DELETE FROM Students WHERE StudentID = @StudentID;", con);
            cmd.Parameters.AddWithValue("@StudentID", studentID);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        #endregion

        #region Instructor Courses

        public static int InsertInstructorCourse(InstructorCourse course)
        {
            int instructorCourseID = 0;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT *  FROM Instructors_CourseSections 
                WHERE InstructorID = @InstructorID 
                AND CourseID = @CourseID 
                AND TermNumber = @TermNumber 
                AND Year = @Year;", con);
            cmd.Parameters.AddWithValue("@InstructorID", course.InstructorID);
            cmd.Parameters.AddWithValue("@CourseID", course.CourseID);
            cmd.Parameters.AddWithValue("@TermNumber", course.TermNumber);
            cmd.Parameters.AddWithValue("@Year", course.Year);

            instructorCourseID = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            if (instructorCourseID == 0)
            {
                cmd = new SqlCommand(
                @"INSERT INTO Instructors_CourseSections     
                    (InstructorID, CourseID, TermNumber, TermName, Year) 
                    VALUES 
                    (@InstructorID, @CourseID, @TermNumber, @TermName, @Year);
                    SELECT SCOPE_IDENTITY();", con);
                cmd.Parameters.AddWithValue("@InstructorID", course.InstructorID);
                cmd.Parameters.AddWithValue("@CourseID", course.CourseID);
                cmd.Parameters.AddWithValue("@TermNumber", course.TermNumber);
                cmd.Parameters.AddWithValue("@TermName", course.TermName);
                cmd.Parameters.AddWithValue("@Year", course.Year);

                con.Open();
                instructorCourseID = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
            }
            return instructorCourseID;
        }

        public static List<InstructorCourse> GetInstructorCourses()
        {
            List<InstructorCourse> instructorCourses = new List<InstructorCourse>();
            List<int> instructorCourseIDs = new List<int>();

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Instructors_CourseSections ORDER BY Year, TermNumber;", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int instructorCourseID = GetSafeInteger(reader, "InstructorCourseID");

                instructorCourseIDs.Add(instructorCourseID);
            }
            con.Close();

            foreach (int id in instructorCourseIDs)
            {
                InstructorCourse course = GetInstructorCourse(id);
                instructorCourses.Add(course);
            }

            return instructorCourses;
        }

        public static List<InstructorCourse> GetInstructorCourses(int instructorID)
        {
            List<InstructorCourse> instructorCourses = new List<InstructorCourse>();
            List<int> instructorCourseIDs = new List<int>();

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Instructors_CourseSections WHERE InstructorID = @InstructorID ORDER BY Year, TermNumber;", con);
            cmd.Parameters.AddWithValue("@InstructorID", instructorID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int instructorCourseID = GetSafeInteger(reader, "InstructorCourseID");
               
                instructorCourseIDs.Add(instructorCourseID);
            }
            con.Close();

            foreach(int id in instructorCourseIDs)
            {
                InstructorCourse course = GetInstructorCourse(id);
                instructorCourses.Add(course);
            }

            return instructorCourses;
        }

        public static InstructorCourse GetInstructorCourse(int instructorCourseID)
        {
            InstructorCourse course = null;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Instructors_CourseSections WHERE InstructorCourseID = @InstructorCourseID;", con);
            cmd.Parameters.AddWithValue("@InstructorCourseID", instructorCourseID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                course = new InstructorCourse();

                course.InstructorCourseID = GetSafeInteger(reader, "InstructorCourseID");
                course.InstructorID = GetSafeInteger(reader, "InstructorID");
                course.CourseID = GetSafeInteger(reader, "CourseID");
                course.TermNumber = GetSafeInteger(reader, "TermNumber");
                course.TermName = GetSafeString(reader, "TermName");
                course.Year = GetSafeInteger(reader, "Year");
                course.CurrentCourseSectionFlag = GetSafeBoolean(reader, "CurrentCourseSectionFlag");
            }
            con.Close();

            if(course.CourseID > 0)
            {
                course.Course = GetCourse(course.CourseID);
                course.Students = GetStudents(course.InstructorCourseID);
            }

            return course;
        }

        public static void DeleteInstructorCourse(int instructorCourseID)
        {
            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(
                @"DELETE FROM Instructors_CourseSections WHERE InstructorCourseID = @InstructorCourseID;", con);
            cmd.Parameters.AddWithValue("@InstructorCourseID", instructorCourseID);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        #endregion

        #region Groups

        public static int InsertGroup(Group group)
        {
            int groupID = 0;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT GroupID FROM Groups WHERE
                InstructorCourseID = @InstructorCourseID 
                AND GroupNumber = @GroupNumber;", con);
            cmd.Parameters.AddWithValue("@InstructorCourseID", group.InstructorCourseID);
            cmd.Parameters.AddWithValue("@GroupNumber", group.GroupNumber);

            groupID = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            if (groupID == 0)
            {
                cmd = new SqlCommand(
                @"INSERT INTO Groups     
                    (InstructorCourseID, GroupNumber, Name, Notes) 
                    VALUES 
                    (@InstructorCourseID, @GroupNumber, @Name, @Notes);
                    SELECT SCOPE_IDENTITY();", con);
                cmd.Parameters.AddWithValue("@InstructorCourseID", group.InstructorCourseID);
                cmd.Parameters.AddWithValue("@GroupNumber", group.GroupNumber);
                cmd.Parameters.AddWithValue("@Name", group.Name);
                cmd.Parameters.AddWithValue("@Notes", group.Notes);

                con.Open();
                groupID = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
            }
            return groupID;
        }

        public static Group GetGroup(int groupID)
        {
            Group group = null;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Groups WHERE GroupID = @GroupID;", con);
            cmd.Parameters.AddWithValue("@GroupID", groupID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                group = new Group();

                group.GroupID = groupID;
                group.GroupNumber = GetSafeInteger(reader, "GroupNumber");
                group.Name = GetSafeString(reader, "Name");
                group.Notes = GetSafeString(reader, "Notes");

            }
            con.Close();

            if(group != null)
            {
                group.Members = GetGroupMembers(group.GroupID);
            }

            return group;
        }

        public static List<Group> GetInstructorCourseGroups(int instructorCourseID)
        {
            List<Group> groups = new List<Group>();
            List<int> groupIDs = new List<int>();

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Groups WHERE InstructorCourseID = @InstructorCourseID ORDER BY GroupNumber;", con);
            cmd.Parameters.AddWithValue("@InstructorCourseID", instructorCourseID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int groupID = GetSafeInteger(reader, "GroupID");

                groupIDs.Add(groupID);
            }
            con.Close();

            foreach(int id in groupIDs)
            {
                Group group = GetGroup(id);
                
                if(group != null)
                {
                    groups.Add(group);
                }
            }

            return groups;
        }

        public InstructorCourse GetCurrentInstructorCourse(int instructorID)
        {
            InstructorCourse course = null;
            int courseSectionID = 0;

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Instructors_CourseSections WHERE InstructorID = @InstructorID 
                                                AND CurrentInstructorCourseFlag = 'True';", con);
            cmd.Parameters.AddWithValue("@InstructorID", instructorID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                courseSectionID = GetSafeInteger(reader, "CourseSectionID");
            }
            con.Close();

            if(courseSectionID > 0)
            {
                course = GrouperMethods.GetInstructorCourse(courseSectionID);
            }

            return course;
        }

        public static int InsertGroupMember(int groupID, int studentID)
        {
            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT * FROM GroupStudents 
                WHERE GroupID = @GroupID 
                AND StudentID = @StudentID;", con);
            cmd.Parameters.AddWithValue("@GroupID", groupID);
            cmd.Parameters.AddWithValue("@StudentID", studentID);
            con.Open();
            var exists = cmd.ExecuteScalar();
            con.Close();
            if (exists == null)
            {
                cmd = new SqlCommand(
                @"INSERT INTO GroupStudents      
                    (GroupID, StudentID) 
                    VALUES 
                    (@GroupID, @StudentID);", con);
                cmd.Parameters.AddWithValue("@GroupID", groupID);
                cmd.Parameters.AddWithValue("@StudentID", studentID);

                con.Open();
                groupID = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
            }
            return groupID;
        }

        public static void DeleteGroupMember(int groupID, int studentID)
        {
            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(
                @"DELETE FROM GroupStudents 
                WHERE GroupID = @GroupID 
                AND StudentID = @StudentID;", con);
            cmd.Parameters.AddWithValue("@GroupID", groupID);
            cmd.Parameters.AddWithValue("@StudentID", studentID);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
           
        }

        public static List<Student> GetGroupMembers(int groupID)
        {
            List<Student> students = new List<Student>();
            List<int> studentIDs = new List<int>();

            SqlConnection con = new SqlConnection(GrouperConnectionString.ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT StudentID FROM GroupStudents WHERE GroupID = @GroupID;", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int studentID = GetSafeInteger(reader, "StudentID");
                studentIDs.Add(studentID);
            }
            con.Close();

            foreach (int studentID in studentIDs)
            {
                Student student = GrouperMethods.GetStudent(studentID);
                if (student != null)
                {
                    students.Add(student);
                }
            }
            return students;
        }
        #endregion

    }
}