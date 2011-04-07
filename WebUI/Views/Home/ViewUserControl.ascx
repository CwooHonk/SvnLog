<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<SvnDomainModel.Svn+LogEntry>>" %>

<table id="SvnResults">
 <thead>
  <tr>
   <th>Build? </br><input id="SelectAll" type="checkbox" onclick="CheckAll()" /></th>       
   <th>Revision</th>
   <th>Author</th>
   <th>Paths</th>
   <th>Message</th>
  </tr>
 </thead>
    <%
    foreach (var log in Model)
        {%>
        <tbody>
          <tr>  
            <td id="Build"><input type="checkbox" id = "<%= log.Revision %>" /></td>
            <td><%= log.Revision %></td>
            <td><%= log.Author %></td>
            <td class="AutoSize"><%= log.Paths %></td>
            <td class="AutoSize"><%= log.Message %></td>
          </tr>
        </tbody>
     <% } %>
</table>

         <% using (Ajax.BeginForm("MergeSvnFiles", new AjaxOptions { UpdateTargetId = "MergeForm" }))
        {
        %>
            <input id="MergeSubmit" type="submit" value="Merge" onclick="SetSelectedRevisions()" />
            <%= Html.Hidden("SelectedRevisions")%>

    <% } %>