<%@ Page Title="Groups" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Groups.aspx.cs" Inherits="GroupBuilderAdmin.Groups" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="panel panel-default">
        
    </div>
    <h4>
        Current Group List
    </h4>
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
