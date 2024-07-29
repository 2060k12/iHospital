<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="iHospital.Homepage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div>
                 <asp:Label ID="companyNameLabel" runat="server" Text="iHospital"></asp:Label>
                <asp:Button ID="loginButton" runat="server" Text="Login" OnClick="loginButton_Click" />
            </div>
            <asp:Button ID="registerButton" runat="server" Text="Start Survey" OnClick="registerButton_Click" />
            <asp:Button ID="startSurveyButton" runat="server" Text="Start Survey" OnClick="startSurveyButton_Click" />

        </div>
    </form>
</body>
</html>
