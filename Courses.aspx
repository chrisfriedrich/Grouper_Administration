<%@ Page Title="Courses" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Courses.aspx.cs" Inherits="GroupBuilderAdmin.Courses" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="CoursesGridView" runat="server" CssClass="table table-bordered table-condensed">   
    </asp:GridView>
</asp:Content>