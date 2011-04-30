<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%= Html.ValidationSummary("Somthing went wrong!!!1!") %>

<div id="popupContact">  
    <h1>Merging Changes</h1>  
    <p id="contactArea">  
        <img src="<%= Url.Content((string)ViewData["LoadingImage"]) %>" alt="Loading Image"/>
        <br />
        <br />
        Before the changes can be merged the repro has to be checked out. This might take a little time!
    </p>  
</div> 
<div id="backgroundPopup"></div> 