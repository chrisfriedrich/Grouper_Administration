<%@ Page Title="Groups" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Groups.aspx.cs" Inherits="GroupBuilderAdmin.Groups" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function allowDrop(ev) {
            ev.preventDefault();
        }

        function drag(ev) {
            ev.dataTransfer.setData("innerhtml", ev.target.id);
        }

        function drop(ev) {
            ev.preventDefault();
            var data = ev.dataTransfer.getData("innerhtml");
            ev.target.appendChild(document.getElementById(data));
        }
    </script>
    <h4>
        Current Group List
    </h4>
    <asp:Panel ID="NoGroupsPanel" runat="server">
        <p class="lead">
            You have no groups created yet.  Click <b>Create Groups</b> to create some groups.
        </p>
        <asp:LinkButton ID="CreateGroupsLinkButton" runat="server" CssClass="btn btn-default btn-sm float-rigtht" OnClick="CreateGroupsLinkButton_Click"><span class="fa fa-plus"></span>&nbsp;&nbsp;Create Groups</asp:LinkButton>
    </asp:Panel>
    <asp:Panel ID="AddGroupsPanel" runat="server" CssClass="panel panel-default">
        <div class="row">
            <div class="col-md-2">
                <div class="form-group">
                    <asp:Label ID="NumberOfGroupsLabel" CssClass="control-label" runat="server">Number of Groups:</asp:Label>
                    <asp:DropDownList ID="NumberOfGroupsDropDownList" CssClass="form-control" runat="server">
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                        <asp:ListItem Text="4" Value="4"></asp:ListItem>
                        <asp:ListItem Text="5" Value="5"></asp:ListItem>
                        <asp:ListItem Text="6" Value="6"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="col-md-2">
                <br />
                <asp:LinkButton ID="BuildGroupsLinkButton" CssClass="btn btn-default btn-sm" runat="server" OnClick="BuildGroupsLinkButton_Click"><span class="fa fa-check"></span>&nbsp;&nbsp;Build Groups</asp:LinkButton>
            </div>
            <div class="col-md-4">
                <br />
                <div class="btn-group">
<%--                <asp:LinkButton ID="CancelBuildGroupsLinkButton" CssClass="btn btn-default btn-sm" runat="server" OnClick="CancelBuildGroupsLinkButton_Click"><span class="fa fa-ban"></span>&nbsp;&nbsp;Cancel</asp:LinkButton>--%>
                <asp:LinkButton ID="ResetGroupsLinkButton" CssClass="btn btn-default btn-sm" runat="server" OnClick="ResetGroupsLinkButton_Click"><span class="fa fa-refresh"></span>&nbsp;&nbsp;Reset Groups</asp:LinkButton>
                <asp:LinkButton ID="DeleteAllGroupsLinkButton" CssClass="btn btn-danger btn-sm" runat="server" OnClick="DeleteAllGroupsLinkButton_Click"><span class="fa fa-remove"></span>&nbsp;&nbsp;Delete All Groups</asp:LinkButton>
                </div>
            </div>
        </div>


    </asp:Panel>
    <asp:LinkButton ID="ReturnToStudentsLinkButton" runat="server" CssClass="btn btn-default btn-sm" OnClick="ReturnToStudentsLinkButton_Click"><span class="fa fa-arrow-left"></span>&nbsp;&nbsp;Return to Students</asp:LinkButton>
    <div class="container">
        <div class="row">
            <div class="col-md-3">
                <asp:GridView ID="StudentsGridView" runat="server" CssClass="table" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Students">
                            <ItemTemplate>
                                <div style='min-height: 40px;' ondrop="drop(event)" ondragover="allowDrop(event)">
                                <div class="panel panel-default panel-condensed" id="studentPanel" draggable="true" ondragstart="drag(event)">
                                    <div class="row">
                                        <div class="col-md-9">
                                            <%# Eval("FirstName") + " " + Eval("LastName") %>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:LinkButton ID="RemoveLinkButton" runat="server" CssClass="btn btn-danger btn-xs"><span class="fa fa-remove"></span></asp:LinkButton>
                                        </div>
                                    </div>
<%--                                    <asp:Label ID="StudentLabel" runat="server" Text=''></asp:Label>--%>
                                </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <div class="col-md-9">
                <div class="row">
                    <asp:Repeater ID="GroupsRepeater" runat="server">
                        <ItemTemplate>
                            <div class="col-md-4">
                                <asp:Label ID="GroupNameLabel" runat="server" CssClass="control-label" Text='<%# Eval("Name") %>'></asp:Label>
                                <div class="panel panel-default" ondrop="drop(event)" ondragover="allowDrop(event)" style="min-height: 300px;">

                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>

    <asp:GridView ID="GroupsGridView" runat="server" CssClass="table table-bordered table-condensed" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField HeaderText="Group Number" DataField="DuckID" />
            <asp:BoundField HeaderText="Name" DataField="FirstName" />
            <asp:TemplateField HeaderText="Group Members">
                <ItemTemplate>
                    <asp:GridView ID="GroupMembersGridView" runat="server" CssClass="table table-bordered table-condensed">
                        <Columns>
                            <asp:BoundField DataField="Name" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                        <asp:LinkButton ID="RemoveLinkButton" runat="server" CssClass="btn btn-danger btn-xs" CommandName="delete_student" CommandArgument='<%# Eval("StudentID") %>'><span class="fa fa-remove"></span></asp:LinkButton> 

                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Notes" DataField="LastName" />
            <asp:TemplateField HeaderStyle-Width="1%">
                <ItemTemplate>
                    <asp:LinkButton ID="EditLinkButton" runat="server" CssClass="btn btn-default btn-xs" CommandName="edit_student" CommandArgument='<%# Eval("StudentID") %>'><span class="fa fa-pencil"></span></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderStyle-Width="1%">
                <ItemTemplate>
                    <asp:LinkButton ID="RemoveLinkButton" runat="server" CssClass="btn btn-danger btn-xs" CommandName="delete_student" CommandArgument='<%# Eval("StudentID") %>'><span class="fa fa-remove"></span></asp:LinkButton> 
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
