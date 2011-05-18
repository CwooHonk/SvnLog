<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SvnDomainModel.SvnDetails>" %>

<div id="ValidationSummary">
    <% Html.RenderPartial("ValidationSummary"); %>
</div>

<table id="SvnResults">
 <thead>
  <tr>
   <th>Merge? <br /><input id="SelectAll" type="checkbox" onclick="CheckAll()" /></th>       
   <th>Revision</th>
   <th>Author</th>
   <th>Paths</th>
   <th>Message</th>
  </tr>
 </thead>
    <%
    foreach (var log in Model.Changes.OrderByDescending(a=>a.Revision))
        {%>
        <tbody>
          <tr>  
            <td id="Merge"><input type="checkbox" id = "<%= log.Revision %>" /></td>
            <td><%= log.Revision %></td>
            <td><%= log.Author %></td>
            <td class="AutoSize"><%= string.Join("<br />", log.AllPaths.OrderByDescending(a => a)) %></td>
            <td class="AutoSize"><%= log.Message %></td>
          </tr>
        </tbody>
     <% } %>
</table>

<% using (Ajax.BeginForm("MergeSvnFiles", new AjaxOptions { UpdateTargetId = "ValidationSummary", OnBegin = "function(){ShowLoadingWindow('Merging Changes', 'The whole repro has to be checked out before the merge can be performed. This might take a little while!')}", OnSuccess = "CloseLoadingWindow" }))
   { %>

    <input id="MergeSubmit" type="submit" value="Merge" onclick="SetSelectedRevisions()" />
    <%= Html.Hidden("SelectedRevisions")%>

<% } %>
