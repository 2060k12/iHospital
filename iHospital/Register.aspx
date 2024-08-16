<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="iHospital.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div>
            <asp:Label ID="registerLabel" runat="server" Text="Register?"></asp:Label>

            </div>
            <div>
            <asp:PlaceHolder ID="registerPlaceHolder" runat="server"></asp:PlaceHolder>
            </div>

            <div>
            <asp:Button ID="anonymousRegisterButton" runat="server" Text="Submit Anonymously (Working)" OnClick="anonymousRegisterButton_Click" />
              <asp:Button ID="submitNowButton" runat="server" Text="Register And Submit"  />

            </div>
            <asp:Button ID="cancelButton" runat="server" Text="Cancel" OnClick="cancelButton_Click" />


        </div>
    </form>
</body>
</html>
