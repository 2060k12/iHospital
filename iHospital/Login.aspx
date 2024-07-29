<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="iHospital.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>



            <asp:Label ID="userNameLbl" runat="server" Text="Username"></asp:Label>
            <asp:TextBox ID="userNameTextBox" runat="server"></asp:TextBox>

            <asp:Label ID="passwordLbl" runat="server" Text="Password"></asp:Label>
            <asp:TextBox ID="passwordTextBox" runat="server"></asp:TextBox>

            <asp:Button ID="loginButton" runat="server" Text="Login" OnClick="loginButton_Click" />
            
        </div>
    </form>
</body>
</html>
