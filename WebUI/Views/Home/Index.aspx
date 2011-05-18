<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">   
    Home Page   
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    $(function () {
        SetDefaultTextInputs();
    });

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

    function ShowLoadingWindow(MessageTitle, MessageText) {
        SetLoadingWindowImageAndText(MessageTitle, MessageText);
        centerPopup();
        loadPopup();
    }

    function SetLoadingWindowImageAndText(MessageTitle, MessageText){
        var ImageNumber = Math.floor(Math.random()*6);
        $("img#LoadingImage").attr("src", "<%= Url.Content("~/Content/LoadingImages/") %>" + ImageNumber + ".png");
        $("div#LoadingTitle").text(MessageTitle);
        $("div#LoadingText").text(MessageText);
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
    
    
    <div id="popupContact">  
        <div id="LoadingTitle">
            Loading
        </div>
        <p />
        <img id="LoadingImage" src="<%= Url.Content("~/Content/LoadingImages/") %>1.png" alt="Loading Image"/>
        <br />
        <div id="LoadingText">
            Loading!
        </div>
    </div> 
    <div id="backgroundPopup"></div> 

    <% using (Ajax.BeginForm("GetSvnLog", new AjaxOptions { UpdateTargetId = "SvnLogResults", OnBegin = "function(){ShowLoadingWindow('Checking for changes', 'Wait there while I go and look for some changes.')}", OnSuccess = "CloseLoadingWindow" }))
       { %>

        <p><%= Html.TextBox("TrunckPath", ViewData["TrunckPath"], new { @id="TitleBar", title="Trunk Path"})%></p>
        <p><%= Html.TextBox("BranchPath", ViewData["BranchPath"], new { @id="TitleBar", title="Branch Path" })%></p>
        <input type="submit" value="Go" />

    <% } %>
     
    <div id="SvnLogResults" class="ChangesLoading">
        <% Html.RenderPartial("ChangeResults"); %>
    </div>

</asp:Content>
