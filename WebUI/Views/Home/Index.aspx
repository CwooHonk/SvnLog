<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">   
    Home Page   
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    function AddClick(CheckBoxId) {
        var selectorString = "#SvnResults tbody tr#" + CheckBoxId;
        var Row = $(selectorString);
        var RowId = Row.attr("id");
        var IsChecked = Row.find("#Build input").attr("checked");

        if (IsChecked) {
            //Check all the boxes below this one.
            $("#SvnResults tbody tr").each(function() {
                if ($(this).attr("id") < RowId) {
                    $(this).find("#Build input").attr("checked", true);
                }
            });
            $("#ToRevision").val(RowId);
            $("#FromRevision").val($("#SvnResults tbody tr:last").attr("id"));
        }
        else {
            //Uncheck all the boxes above this one.
            $("#SvnResults tbody tr").each(function() {
                if ($(this).attr("id") > RowId) {
                    $(this).find("#Build input").attr("checked", false);
                }
            });
        }
        $("#TrunckPath1").val($("#TrunckPath").val());
        $("#BranchPath1").val($("#BranchPath").val());
    };
</script>

    <% using (Ajax.BeginForm("GetSvnLog", new AjaxOptions { UpdateTargetId = "SvnLogResults" }))
       { %>
        <p>Trunck Path: <%= Html.TextBox("TrunckPath", ViewData["TrunckPath"], new { style="width:92%" })%></p>
        <p>Branch Path: <%= Html.TextBox("BranchPath", ViewData["BranchPath"], new { style = "width:92%" })%></p>
        <input type="submit" value="Go" />
    <% }
     
         using (Ajax.BeginForm("MergeSvnFiles", new AjaxOptions { UpdateTargetId = "MergeForm" }))
        {
        %>
            <input id="MergeSubmit" type="submit" value="Merge" />
            <!-- Must be a better way of doing this.. :-( -->
            <%= Html.Hidden("TrunckPath1") %>
            <%= Html.Hidden("BranchPath1") %>
            <%= Html.Hidden("FromRevision") %>
            <%= Html.Hidden("ToRevision") %>

    <% } %>

        <div id="SvnLogResults" style="border: 2px dotted red; padding:.5em">
            Results will appear here
        </div>

</asp:Content>
