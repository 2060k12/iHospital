<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="iHospital.Admin" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Page</title>
    <link href="CSS/styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="controls-container">
            <div class="select-wrapper">
                <asp:DropDownList ID="ddlQuestions" runat="server" CssClass="select" AutoPostBack="true" OnSelectedIndexChanged="ddlQuestions_SelectedIndexChanged">
                    <asp:ListItem Text="Select Question" Value="" />
                </asp:DropDownList>
            </div>
            <div class="select-wrapper">
                <asp:DropDownList ID="ddlOptions" runat="server" CssClass="select" AutoPostBack="True" OnSelectedIndexChanged="ddlOptions_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            <asp:TextBox ID="txtSearch" runat="server" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged" placeholder="Type and press Enter to search" CssClass="input"></asp:TextBox>
       <asp:Button ID="btnSignOut" runat="server" Text="Sign Out" OnClick="btnSignOut_Click" CssClass="btn-primary" />

               </div>
        <asp:GridView ID="respondantGridView" runat="server" AutoGenerateColumns="true"  ></asp:GridView>
    </form>
</body>
</html>
