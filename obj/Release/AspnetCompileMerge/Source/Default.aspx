<%@ Page Title="Students" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GroupBuilderAdmin.Default" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel runat="server">
        <ContentTemplate>

    <h3>
        Welcome <asp:Label ID="InstructorNameLabel" runat="server"></asp:Label>!
    </h3>
    <asp:Panel ID="NoCoursesPanel" runat="server" Visible="false">

        <p class="lead">
            You have not yet created any class sections.&nbsp;&nbsp;Click <b>Create Section</b> to create a new class section.
        </p>
    </asp:Panel>
    <asp:Panel ID="AddCourseSectionPanel" runat="server" Visible="false" CssClass="panel panel-default">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <asp:Label ID="CourseDropDownLabel" runat="server" Text="Course"></asp:Label>
                                <asp:DropDownList ID="CoursesDropDownList" runat="server" CssClass="form-control" DataTextField="FullName" DataValueField="CourseID"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <asp:Label ID="TermLabel" runat="server" Text="Term"></asp:Label>
                                <asp:DropDownList ID="TermsDropDownList" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="Fall" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Winter" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Spring" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Summer" Value="4"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <asp:Label ID="YearLabel" runat="server" Text="Year"></asp:Label>
                                <asp:DropDownList ID="YearsDropDownList" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <asp:Label ID="TimeLabel" runat="server" Text="Time"></asp:Label>
                                <asp:DropDownList ID="TimesDropDownList" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12" style="vertical-align: bottom;">
                            <p class="text-align: right">
                                <asp:LinkButton ID="CancelAddCourseLinkButton" runat="server" CssClass="btn btn-default float-right" OnClick="CancelAddCourseLinkButton_Click"><span class="fa fa-ban"></span>&nbsp;&nbsp;Cancel</asp:LinkButton>
                                <asp:LinkButton ID="SaveCourseLinkButton" runat="server" CssClass="btn btn-default float-right" OnClick="SaveCourseLinkButton_Click"><span class="fa fa-save"></span>&nbsp;&nbsp;Save Course Section</asp:LinkButton>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>     
    </asp:Panel>
    <div class="row">
        <div class="col-md-12">
            <asp:LinkButton ID="CreateInstructorCourseLinkButton" runat="server" CssClass="btn btn-default btn-sm float-right" OnClick="CreateInstructorCourseLinkButton_Click"><span class="fa fa-plus"></span>&nbsp;&nbsp;Create Course Section</asp:LinkButton>
        </div>
    </div>
    <asp:Panel ID="CoursesPanel" runat="server" Visible="false">
        <p class="lead">
            Below is a list of your current course sections.  Click <b>Select Course</b> to view your students and groups.
        </p>
    <div class="container">
        <div class="row">
            <div class="col-md-12">
                        <asp:GridView ID="CoursesGridView" runat="server" CssClass="table table-bodered table-condensed" AutoGenerateColumns="false" OnRowCommand="CoursesGridView_RowCommand">
            <Columns>

                <asp:TemplateField HeaderText="Course">
                    <ItemTemplate>
                        <asp:Label ID="CourseLabel" Text='<%# Eval("Course.FullName") %>' runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Term">
                    <ItemTemplate>
                        <asp:Label ID="TermLabel" runat="server" Text='<%# Eval("TermName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-Width="1%">
                    <ItemTemplate>
                        <asp:LinkButton ID="EditInstructorCourseLinkButton" runat="server" CssClass="btn btn-default btn-xs" CommandName="edit_instructor_course" CommandArgument='<%# Eval("InstructorCourseID") %>'><span class="fa fa-check"></span>&nbsp;&nbsp;Select Course</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-Width="1%">
                    <ItemTemplate>
                        <asp:LinkButton ID="RemoveInstructorCourse" runat="server" CssClass="btn btn-danger btn-xs" CommandName="delete_instructor_course" CommandArgument='<%# Eval("InstructorCourseID") %>'><span class="fa fa-remove"></span>&nbsp;&nbsp;Delete</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
            </div>
        </div>

    </div>
    </asp:Panel>
    
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
