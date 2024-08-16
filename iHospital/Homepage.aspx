<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="iHospital.Homepage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Homepage</title>
    <link href="CSS/styles.css" rel="stylesheet" type="text/css" />
</head>
<body class ="homepage">
    <form id="form1" runat="server" >
        <div class="homepage-container">
            <div class="header">
                <asp:Label ID="companyNameLabel" runat="server" CssClass="company-name" Text="iHospital"></asp:Label>
            </div>
            <div class="button-group">
                <asp:Button ID="loginButton" runat="server" CssClass="btn btn-primary" Text="Login" OnClick="loginButton_Click" />
                <asp:Button ID="startSurveyButton" runat="server" CssClass="btn btn-secondary" Text="Start Survey" OnClick="startSurveyButton_Click" />
            </div>
        </div>
    </form>
</body>
</html>
