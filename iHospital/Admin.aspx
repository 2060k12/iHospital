<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="iHospital.Admin" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:DropDownList ID="ddlQuestions" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlQuestions_SelectedIndexChanged">
                <asp:ListItem Text="Select Question" Value="" />
               
            </asp:DropDownList>
            <asp:DropDownList ID="ddlOptions" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlOptions_SelectedIndexChanged">
            </asp:DropDownList>
               
            </asp:DropDownList>
            <asp:TextBox ID="txtSearch" runat="server" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged" placeHolder ="Type and press Enter to search"></asp:TextBox>

            <asp:GridView ID="respondantGridView" runat="server" AutoGenerateColumns="true"></asp:GridView>
        </div>
    </form>
</body>
</html>
