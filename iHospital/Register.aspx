<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="iHospital.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <link href="CSS/Register.css" rel="stylesheet" type="text/css" />

</head>
<body>
     <form id="form1" runat="server" class="survey-body">
        <div class="survey-container">
            <div>
                <asp:Label ID="registerLabel" runat="server" CssClass="question-label" Text="Register?"></asp:Label>
            </div>
            <div>
                <asp:PlaceHolder ID="registerPlaceHolder" runat="server"></asp:PlaceHolder>
            </div>
            <div>
                <asp:Button ID="anonymousRegisterButton" runat="server" CssClass="btn btn-secondary" Text="Submit Anonymously" OnClick="anonymousRegisterButton_Click" />
                <asp:Button ID="submitNowButton" runat="server" CssClass="btn btn-primary" Text="Register And Submit" OnClick="registerButton_Click" />
            </div>
            <div>
                <asp:Button ID="cancelButton" runat="server" CssClass="btn btn-cancel" Text="Back to Home Screen" OnClick="cancelButton_Click" />
            </div>


            <asp:Label ID="errorLabel" runat="server" Text="Error!" Visible="false" 
           style="color: red; font-weight: bold;"></asp:Label>

        </div>
    </form>
</body>
</html>
