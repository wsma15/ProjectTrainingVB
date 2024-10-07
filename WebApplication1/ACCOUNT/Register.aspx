<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Register.aspx.vb" Inherits="WebApplication1.Register" %>
<%@ Register Assembly="DevExpress.Web.v19.1, Version=19.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxTextBox ID="txtUsername" runat="server" Placeholder="Username" Required="True" Caption="Username"></dx:ASPxTextBox>
        <br />
        <dx:ASPxTextBox ID="txtName" runat="server" Placeholder="Name" Required="True" Caption="Name"></dx:ASPxTextBox>
        <br />
        <dx:ASPxTextBox ID="txtPassword" runat="server" Placeholder="Password" TextMode="Password" Required="True" Caption="Password"></dx:ASPxTextBox>
        <br />

        <dx:ASPxTextBox ID="txtConfirmPassword" runat="server" Placeholder="Confirm Password" TextMode="Password" Required="True" Caption="Confirm Password"></dx:ASPxTextBox>
        <br />

        <dx:ASPxComboBox ID="ddlManager" runat="server" 
            DataSourceID="RolesDataSource" 
            ValueField="Id" 
            TextField="Name" 
            Caption="Type">
        </dx:ASPxComboBox>
        <br />

        <dx:ASPxButton ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click"></dx:ASPxButton>
        <br />
        <dx:ASPxLabel ID="lblErrorMessage" runat="server" Visible="False" ForeColor="Red"></dx:ASPxLabel>
        <br />
        <dx:ASPxLabel ID="lblSuccessMessage" runat="server" Visible="False" ForeColor="Green"></dx:ASPxLabel>

        <br />
        <asp:SqlDataSource ID="RolesDataSource" runat="server" 
            ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
            SelectCommand="SELECT Id, Name FROM [dbo].[Roles] WHERE Name <> 'Admin'" />
    </form>
</body>
</html>
