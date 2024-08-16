<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Survey.aspx.cs" Inherits="iHospital.WebForm1" %>
<%--<%@ Register Src ="~/UserControl/CheckListUserControl.ascx" TagPrefix="uc" TagName="CheckList" %>
<%@ Register Src ="~/UserControl/ChooseUserControl.ascx" TagPrefix="uc" TagName="ChooseList" %>
<%@ Register Src ="~/UserControl/DropDownUserControl.ascx" TagPrefix="uc" TagName="DropDownList" %>
<%@ Register Src ="~/UserControl/InputUserControl.ascx" TagPrefix="uc" TagName="InputList" %>--%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <div>

          
                <div>
            <asp:Label ID="welcomeScreenLabel" runat="server" Text="Label">
   Welcome to Survey Site
</asp:Label>
                </div>
           

                <div>
                    <asp:Label ID="infoLabel" runat="server" Text="Feel free to skip any question You are unconfortable with"></asp:Label>
                </div>

                  </div>
            <asp:PlaceHolder ID="surveyPlaceHolder" runat="server"></asp:PlaceHolder>
           
          
            <asp:Button ID="previousButton" runat="server" Text="Previous" OnClick="previousButton_Click" CausesValidation="false" UseSubmitBehavior="false"/>
            <asp:Button ID="nextButton" runat="server" Text="Next" OnClick="nextButton_Click" CausesValidation="false" UseSubmitBehavior="False" />
            
        </div>
    </form>
</body>
</html>
