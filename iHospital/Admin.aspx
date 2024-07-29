<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="iHospital.Admin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="adminScreenLabel" runat="server" Text="Admin Panel" ></asp:Label>
            <div>
            <asp:TextBox ID="searchTextBox" runat="server"></asp:TextBox>
            <asp:Button ID="searchButton" runat="server" Text="Search"  />
             </div>

            <%-- Filter --%>
            <asp:DropDownList ID="DropDownList1" runat="server"  ></asp:DropDownList>


            <asp:GridView ID="respondantGridView" runat="server"></asp:GridView>
        </div>
    </form>
</body>
</html>
