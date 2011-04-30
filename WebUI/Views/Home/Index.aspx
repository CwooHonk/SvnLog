<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">   
    Home Page   
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    function CheckAll() {
        var Checked = $("#SelectAll")[0].checked;
        $("#SvnResults tbody tr td#Merge input").each(function (index) {
            $(this).attr("checked", Checked);
        });
    }

    function SetSelectedRevisions() {
        var Revisions = [];
        $("#SvnResults tbody tr td#Merge input:checked").each(function (index) {
            Revisions.push($(this).attr("id"));
        });
        $("#SelectedRevisions").attr("value", Revisions.join(","));
    }

    function ShowLoadingWindow() {
        centerPopup();
        loadPopup();
    }

    function CloseLoadingWindow() {
        disablePopup();

        if ($("div.validation-summary-errors").length == 0) {
            RemoveMergedRevisions();
            $("div#SvnLogResults").removeClass("MergeError").addClass("ChangesMerged");
        }
        else {
            $("div#SvnLogResults").removeClass("ChangesMerged").addClass("MergeError");
        }
    }

    function RemoveMergedRevisions() {
        $("#SvnResults tbody tr").filter(function () {
            return $(this).find("td#Merge input").attr('checked') == true;
        }).remove()
    }
</script> 


    <% using (Ajax.BeginForm("GetSvnLog", new AjaxOptions { UpdateTargetId = "SvnLogResults" }))
       { %>

        <p>Trunck Path: <%= Html.TextBox("TrunckPath", ViewData["TrunckPath"], new { @class="TitleBar"})%></p>
        <p>Branch Path: <%= Html.TextBox("BranchPath", ViewData["BranchPath"], new { @class="TitleBar" })%></p>
        <input type="submit" value="Go" />

    <% } %>
     
    <div id="SvnLogResults" class="ChangesLoading">
        Results will appear here
    </div>

</asp:Content>
