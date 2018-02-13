<%@ Page Title="Students" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Students.aspx.cs" Inherits="GroupBuilderAdmin.Students" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="modal fade" id="messageBox">
        <div class="modal-dialog">
            <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title">
                                <asp:Label ID="MessageBoxTitleLabel" runat="server"></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="MessageBoxMessageLabel" runat="server" />
                        </div>
                        <div class="modal-footer">
                            <p>
                            <asp:LinkButton ID="MessageBoxOkayLinkButton" runat="server" data-dismiss="modal" CssClass="btn btn-default" Style="margin-bottom: 0px;">
                            </asp:LinkButton>&nbsp;&nbsp;
                            <asp:LinkButton ID="MessageBoxCreateLinkButton" runat="server" OnClick="MessageBoxCreateLinkButton_Click" CssClass="btn btn-default">
                            </asp:LinkButton>
                            </p>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

<asp:Panel ID="ImportStudentsPanel" runat="server" CssClass="panel panel-default" Visible="false">
    <h3>
        Import Students
    </h3>

    <div class="row">
        <div class="col-md-6">
                <asp:FileUpload ID="StudentsFileUpload" runat="server" />

        </div>
        <div class="col-md-3">
                <asp:LinkButton ID="CancelImportStudentsLinkButton" runat="server" CssClass="btn btn-default btn-sm" OnClick="CancelImportStudentsLinkButton_Click"><span class="fa fa-ban"></span>&nbsp;&nbsp;Cancel</asp:LinkButton>
                <asp:LinkButton ID="ProcessStudentsFileLinkButton" runat="server" CssClass="btn btn-default btn-sm" OnClick="ProcessStudentsFileLinkButton_Click"><span class="fa fa-upload"></span>&nbsp;&nbsp;Process File</asp:LinkButton>

        </div>
        <div class="col-md-3"></div>
    </div>
    </asp:Panel>

    <asp:UpdatePanel runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="ImportStudentsLinkButton" />
        </Triggers>
        <ContentTemplate>
    <br />
    <asp:HiddenField ID="SelectedStudentIDHiddenField" runat="server" />
    <asp:Panel ID="StudentListPanel" runat="server">
    <h3>
        Current Student List for <asp:Label ID="CourseNameLabel" runat="server"></asp:Label>
    </h3>
    <div class="btn-group">
        <asp:HyperLink ID="ReturnToCoursesHyperLink" runat="server" CssClass="btn btn-default btn-sm" NavigateUrl="~/Default.aspx"><span class="fa fa-arrow-left"></span>&nbsp;&nbsp;Return to Courses</asp:HyperLink>
        <asp:LinkButton ID="ImportStudentsLinkButton" runat="server" CssClass="btn btn-default btn-sm" OnClick="ImportStudentsLinkButton_Click"><span class="fa fa-upload"></span>&nbsp;&nbsp;Import Students From CSV</asp:LinkButton>
        <asp:LinkButton ID="AddStudentLinkButton" runat="server" CssClass="btn btn-default btn-sm float-right" OnClick="AddStudentLinkButton_Click"><span class="fa fa-plus"></span>&nbsp;&nbsp;Add Student</asp:LinkButton>
        <asp:LinkButton ID="BeginGroupingLinkButton" runat="server" CssClass="btn btn-default btn-sm" OnClick="BeginGroupingLinkButton_Click">Begin Grouping&nbsp;&nbsp;<span class="fa fa-arrow-right"></span></asp:LinkButton>
    </div>

    <div>
        <asp:LinkButton ID="DeleteAllStudentsLinkButton" runat="server" CssClass="btn btn-danger btn-sm float-right" OnClick="DeleteAllStudentsLinkButton_Click"><span class="fa fa-remove"></span>&nbsp;&nbsp;Delete All Students</asp:LinkButton>
        <asp:LinkButton ID="SendWelcomeToAllStudentsLinkButton" runat="server" CssClass="btn btn-default btn-sm float-right" OnClick="SendWelcomeToAllStudentsLinkButton_Click"><span class="fa fa-send"></span>&nbsp;&nbsp;Send Welcome to All</asp:LinkButton>
    </div>

    <asp:GridView ID="StudentsGridView" runat="server" CssClass="table table-bordered danger table-condensed" DataKeyNames="StudentID" AutoGenerateColumns="false" OnRowCommand="StudentsGridView_RowCommand" OnRowDataBound="StudentsGridView_RowDataBound">
        <EmptyDataTemplate>
            <h4>No current student records for this course section.</h4>
        </EmptyDataTemplate>
        <Columns>
            <asp:BoundField HeaderText="DuckID" DataField="DuckID" />
            <asp:BoundField HeaderText="First Name" DataField="FirstName" />
            <asp:BoundField HeaderText="Last Name" DataField="LastName" />
            <asp:TemplateField HeaderText="Interested Roles">
                <ItemTemplate>
                    <asp:Label ID="RolesLabel" runat="server"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Languages">
                <ItemTemplate>
                    <asp:Label ID="LanguagesLabel" runat="server"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Skills">
                <ItemTemplate>
                    <asp:Label ID="SkillsLabel" runat="server"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Welcome Sent" DataField="InitialNotificationSentDate"/>
            <asp:TemplateField HeaderStyle-Width="1%">
                <ItemTemplate>
                    <asp:LinkButton ID="EditLinkButton" runat="server" CssClass="btn btn-default btn-xs" CommandName="edit_student" CommandArgument='<%# Eval("StudentID") %>'><span class="fa fa-pencil"></span>&nbsp;&nbsp;Edit</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderStyle-Width="1%">
                <ItemTemplate>
                    <asp:LinkButton ID="RemoveLinkButton" runat="server" CssClass="btn btn-danger btn-xs" CommandName="delete_student" CommandArgument='<%# Eval("StudentID") %>'><span class="fa fa-remove"></span>&nbsp;&nbsp;Delete</asp:LinkButton> 
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderStyle-Width="1%">
                <ItemTemplate>
                    <asp:LinkButton ID="SendWelcomeEmail" runat="server" CssClass="btn btn-default btn-xs" CommandName="send_welcome" CommandArgument='<%# Eval("StudentID") %>'><span class="fa fa-send"></span>&nbsp;&nbsp;Send Welcome</asp:LinkButton>
                 </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    </asp:Panel>
                <asp:Panel ID="AddStudentPanel" runat="server" CssClass="panel panel-default" Visible="false">
    <div class="row">
        <div class="col-md-4">
            <div class="form-group">
                <asp:Label ID="DuckIDLabel" CssClass="control-label" runat="server">DuckID: </asp:Label>
                <asp:TextBox ID="DuckIDTextBox" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="col-md-4">
            <div class="form-group">
                <asp:Label ID="FirstNameLabel" CssClass="control-label" runat="server">First Name: </asp:Label>
                <asp:TextBox ID="FirstNameTextBox" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="col-md-4">
            <div class="form-group">
                <asp:Label ID="LastNameLabel" CssClass="control-label" runat="server">Last Name: </asp:Label>
                <asp:TextBox ID="LastNameTextBox" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        </div>
        <div class="row">
            <div class="col-md-4">
                <div class="form-group">
                    <asp:Label ID="RolesLabel" runat="server" CssClass="control-label">Interested Roles: </asp:Label>
                    <asp:DropDownList ID="RolesDropDownList" runat="server" CssClass="form-control" DataTextField="Name" DataValueField="RoleID">
                </asp:DropDownList>
                </div>
            </div>
            <div class="col-md-2">
                <div class="form-group">
                    <asp:Label ID="RoleInterestLabel" runat="server" CssClass="control-label">Interest Level: </asp:Label>
                    <asp:DropDownList ID="RoleInterestDropDownList" runat="server" CssClass="form-control">
<%--                        <asp:ListItem Text="None" Value="0"></asp:ListItem>--%>
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                        <asp:ListItem Text="4" Value="4"></asp:ListItem>
                        <asp:ListItem Text="5" Value="5"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="col-md-2" style="vertical-align: bottom;">
                <br />
                <asp:LinkButton ID="AddRoleLinkButton" runat="server" CssClass="btn btn-default btn-sm" OnClick="AddRoleLinkButton_Click"><span class="fa fa-plus"></span>&nbsp;&nbsp;Add Role</asp:LinkButton>
            </div>
            <div class="col-md-4">
                <asp:GridView ID="RolesGridView" runat="server" CssClass="table table-bordered table-condensed" AutoGenerateColumns="false" OnRowCommand="RolesGridView_RowCommand">
                    <Columns>
                        <asp:BoundField HeaderText="Role" DataField="Name"/>
                        <asp:BoundField HeaderText="Level of Interest" DataField="InterestLevel" />
                        <asp:TemplateField HeaderStyle-Width="1%">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CssClass="btn btn-danger btn-xs" CommandName="delete_role" CommandArgument='<%# Eval("RoleID") %>'><span class="fa fa-remove"></span></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div class="row">
            <div class="col-md-4">
                <div class="form-group">
                    <asp:Label ID="ProgrammingLanguagesLabel" runat="server" CssClass="control-label">Programming Languages: </asp:Label>
                    <asp:DropDownList ID="ProgrammingLanguagesDropDownList" runat="server" CssClass="form-control" DataTextField="Name" DataValueField="LanguageID">
                </asp:DropDownList>
                </div>
            </div>
            <div class="col-md-2">
                <div class="form-group">
                    <asp:Label ID="LevelOfAbilityLabel" runat="server" CssClass="control-label">Ability Level: </asp:Label>
                    <asp:DropDownList ID="ProgrammingAbilityLevelDropDownList" runat="server" CssClass="form-control">
                        <asp:ListItem Text="None" Value="0"></asp:ListItem>
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                        <asp:ListItem Text="4" Value="4"></asp:ListItem>
                        <asp:ListItem Text="5" Value="5"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="col-md-2" style="vertical-align: bottom;">
                <br />
                <asp:LinkButton ID="AddProgrammingLanguageLinkButton" runat="server" CssClass="btn btn-default btn-sm" OnClick="AddProgrammingLanguageLinkButton_Click"><span class="fa fa-plus"></span>&nbsp;&nbsp;Add Language</asp:LinkButton>
            </div>
            <div class="col-md-4">
                                <asp:GridView ID="ProgrammingLanguagesGridView" runat="server" CssClass="table table-bordered table-condensed" AutoGenerateColumns="false" OnRowCommand="ProgrammingLanguagesGridView_RowCommand">
                    <Columns>
                        <asp:BoundField HeaderText="Language" DataField="Name"/>
                        <asp:BoundField HeaderText="Level of Ability" DataField="ProficiencyLevel" />
                        <asp:TemplateField HeaderStyle-Width="1%">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CssClass="btn btn-danger btn-xs" CommandName="delete_language" CommandArgument='<%# Eval("LanguageID") %>'><span class="fa fa-remove"></span></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div class="row">
            <div class="col-md-4">
                <div class="form-group">
                    <asp:Label ID="SkillsLabel" runat="server" CssClass="control-label">Skills: </asp:Label>
                    <asp:DropDownList ID="SkillsDropDownList" runat="server" CssClass="form-control" DataTextField="Name" DataValueField="SkillID">
                </asp:DropDownList>
                </div>
            </div>
            <div class="col-md-2">
                <div class="form-group">
                    <asp:Label ID="SkillsLevel" runat="server" CssClass="control-label">Ability Level: </asp:Label>
                    <asp:DropDownList ID="SkillsLevelDropDownList" runat="server" CssClass="form-control">
<%--                        <asp:ListItem Text="None" Value="0"></asp:ListItem>--%>
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                        <asp:ListItem Text="4" Value="4"></asp:ListItem>
                        <asp:ListItem Text="5" Value="5"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="col-md-2" style="vertical-align: bottom;">
                <br />
                <asp:LinkButton ID="AddSkillLinkButton" runat="server" CssClass="btn btn-default btn-sm" OnClick="AddSkillLinkButton_Click"><span class="fa fa-plus"></span>&nbsp;&nbsp;Add Skill</asp:LinkButton>
            </div>
            <div class="col-md-4">
                <asp:GridView ID="SkillsGridView" runat="server" CssClass="table table-bordered table-condensed" AutoGenerateColumns="false" OnRowCommand="SkillsGridView_RowCommand">
                    <Columns>
                        <asp:BoundField HeaderText="Skill" DataField="Name"/>
                        <asp:BoundField HeaderText="Level of Ability" DataField="ProficiencyLevel" />
                        <asp:TemplateField HeaderStyle-Width="1%">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CssClass="btn btn-danger btn-xs" CommandName="delete_skill" CommandArgument='<%# Eval("SkillID") %>'><span class="fa fa-remove"></span></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div class="row">
        <div class="col-md-12">
            <asp:LinkButton ID="CancelAddStudentLinkButton" runat="server" CssClass="btn btn-default btn-sm float-right" OnClick="CancelAddStudentLinkButton_Click"><span class="fa fa-ban"></span>&nbsp;&nbsp;Cancel</asp:LinkButton>
            <asp:LinkButton ID="SaveAddStudentLinkButton" runat="server" CssClass="btn btn-default btn-sm float-right" OnClick="SaveAddStudentLinkButton_Click"><span class="fa fa-save"></span>&nbsp;&nbsp;Save Changes</asp:LinkButton>
        </div>
    </div>
    </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
