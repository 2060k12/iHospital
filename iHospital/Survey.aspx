<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Survey.aspx.cs" Inherits="iHospital.WebForm1" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Survey</title>
    <link href="CSS/SurveyScreen.css" rel="stylesheet" type="text/css" />
    <script src="JS/checkboxLimit.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="survey-body">
            <div class="survey-container">
                <div class="header">
                    <asp:Label ID="welcomeScreenLabel" runat="server" Text="Welcome to Survey Site" CssClass="question-label"></asp:Label>
                    <asp:Label ID="infoLabel" runat="server" Text="Feel free to skip any question you are uncomfortable with" CssClass="info-label"></asp:Label>
                </div>
                <asp:PlaceHolder ID="surveyPlaceHolder" runat="server"></asp:PlaceHolder>
               
                <div class="controls-container">
                    <asp:Button ID="previousButton" runat="server" Text="Previous" OnClick="previousButton_Click" CausesValidation="false" UseSubmitBehavior="false" CssClass="btn btn-secondary" />
                    <asp:Button ID="nextButton" runat="server" Text="Next" OnClick="nextButton_Click" CausesValidation="false" UseSubmitBehavior="false" CssClass="btn btn-primary" />

                </div>
            <asp:Label ID="errorLbl" runat="server" Text="Error!" Visible="false" CssClass="error-label"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
