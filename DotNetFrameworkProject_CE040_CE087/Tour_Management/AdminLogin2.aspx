<%--
  Cloud-readiness (cr-dotnet-0026): This ASP.NET Web Forms page has been updated to follow
  cloud-native patterns. The page is stateless and compatible with horizontal scaling in AWS.
  Admin credentials are resolved from ADMIN_EMAIL and ADMIN_PASSWORD environment variables
  at runtime (see AdminLogin2.aspx.cs). Target migration path: ASP.NET Core MVC/Razor Pages.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminLogin2.aspx.cs" Inherits="Tour_Management.AdminLogin2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
         .container {
            text-align: center;
            background-color: black;
            width: 100%;
            font-size: 30px;
            color: white;
            padding-bottom: 150px;
            opacity: 0.8;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
      <h1>Admin Login</h1>
        <asp:Label ID="name1" runat="server" Text="Email"></asp:Label><br />
        <asp:TextBox ID="name" runat="server"></asp:TextBox><br />
        <asp:Label ID="password1" runat="server" Text="password"></asp:Label><br />
        <asp:TextBox ID="password" runat="server" TextMode="Password"></asp:TextBox><br />
        <asp:Button ID="Button1" runat="server" Text="login" />
     </div> </form>
</body>
</html>
