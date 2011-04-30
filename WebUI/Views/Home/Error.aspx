<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Oh Noes!
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <img src="../../Content/ErrorCroc.gif" alt="Scum bag ErrorCroc!"/>

    <% if (!ViewData.ContainsKey("ErrorMessage"))
       { %>
           <p>Error!</p>
       <%}
       else
       {%>
            <p><%= ViewData["ErrorMessage"]%></p>
      <% } %>

</asp:Content>
