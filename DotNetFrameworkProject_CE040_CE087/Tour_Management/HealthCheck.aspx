<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HealthCheck.aspx.cs" Inherits="Tour_Management.HealthCheck" %>
<!DOCTYPE html>
<html>
<head>
    <title>Health Check</title>
</head>
<body>
    <% Response.ContentType = "application/json"; %>
    <% Response.Write("{\"status\": \"Healthy\"}"); %>
</body>
</html>
