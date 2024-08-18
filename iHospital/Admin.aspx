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
            <div>
            <div class="select-wrapper">
                <asp:DropDownList ID="ddlQuestions" runat="server" CssClass="select" AutoPostBack="true" OnSelectedIndexChanged="ddlQuestions_SelectedIndexChanged">
                    <asp:ListItem Text="Select Question" Value="" />
                </asp:DropDownList>
            </div>
                
            </div>
            <div class="select-wrapper">
                <asp:DropDownList ID="ddlOptions" runat="server" CssClass="select" AutoPostBack="True" OnSelectedIndexChanged="ddlOptions_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
             <asp:Button ID="btnSignOut" runat="server" Text="Sign Out" OnClick="btnSignOut_Click" CssClass="btn-primary" />

            
            </div>
            <asp:Label ID="Label1" runat="server" Text="If an input type question is selected Search:"></asp:Label>
            <asp:TextBox ID="txtSearch" runat="server" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged" placeholder="Search Input" CssClass="input"></asp:TextBox>
             <asp:Button ID="btnFilter" runat="server" Text="Filter" OnClick="btnFilter_Click" CssClass="btn-secondary" />


            <div >
                <asp:TextBox ID="txtRespondantId" runat="server" CssClass="input" placeholder="Enter Respondant ID"></asp:TextBox>
                <asp:TextBox ID="txtSessionDate" runat="server" CssClass="input" placeholder="Enter Session Date (yyyy-MM-dd)"></asp:TextBox>
                <asp:Button ID="btnApplyFilters" runat="server" Text="Apply Filters" OnClick="btnApplyFilters_Click" CssClass="btn-secondary" />

                </div>

             
               
        <asp:GridView ID="respondantGridView" runat="server" AutoGenerateColumns="true"  ></asp:GridView>
    </form>
</body>
</html>
