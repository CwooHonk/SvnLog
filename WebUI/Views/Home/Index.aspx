<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">   
    Home Page   
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    function CheckAll() {
        var Checked = $("#SelectAll")[0].checked;
        $("#SvnResults tbody tr td#Build input").each(function (index) {
            $(this).attr("checked", Checked);
        });
    }

    function SetSelectedRevisions() {
        var Revisions = [];
        $("#SvnResults tbody tr td#Build input:checked").each(function (index) {
            Revisions.push($(this).attr("id"));
        });
        $("#SelectedRevisions").attr("value", Revisions.join(", "));
    }
</script> 

    <% using (Ajax.BeginForm("GetSvnLog", new AjaxOptions { UpdateTargetId = "SvnLogResults" }))
       { %>
        <p>Trunck Path: <%= Html.TextBox("TrunckPath", ViewData["TrunckPath"], new { @class="TitleBar"})%></p>
        <p>Branch Path: <%= Html.TextBox("BranchPath", ViewData["BranchPath"], new { @class="TitleBar" })%></p>
        <input type="submit" value="Go" />
    <% } %>
     
    <div id="SvnLogResults" style="border: 2px dotted red; padding:.5em">
        Results will appear here
    </div>

</asp:Content>
