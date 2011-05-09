<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    Before you can merge changes you need to provide valid <del>bank account details</del> SVN credentials:
    <% using (Html.BeginForm())
       { %>
       <p>Svn User: </p> <%= Html.TextBox("SvnUser", Request.Cookies[SvnDomainModel.SvnDetails.SvnCookieName][SvnDomainModel.SvnDetails.SvnCookieUserName]) %>
       <p>Svn Password: </p><%= Html.Password("SvnPassword", Request.Cookies[SvnDomainModel.SvnDetails.SvnCookieName][SvnDomainModel.SvnDetails.SvnCookieUserPassword])%> 
       <p><input type="submit" value="Assign Credentials" /></p>
    <%} %>
    <br />

</asp:Content>
