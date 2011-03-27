<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<SvnDomainModel.Svn+LogEntry>>" %>
       

<table id="SvnResults">
 <thead>
  <tr>
   <th>Build?</th>       
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
          <tr id = "<%= log.Revision %>">  
            <td id="Build"><input type="checkbox" onclick="AddClick(<%= log.Revision %>)" /></td>
            <td><%= log.Revision %></td>
            <td><%= log.Author %></td>
            <td class="AutoSize"><%= log.Paths %></td>
            <td class="AutoSize"><%= log.Message %></td>
          </tr>
        </tbody>
     <% } %>
</table>