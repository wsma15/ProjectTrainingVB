<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="login.aspx.vb" Inherits="WebApplication1.login" %>

<%@ Register Assembly="DevExpress.Web.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxLabel ID="Usernamelbl" runat="server" Text="username"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="Usernametxt" runat="server" Width="170px"></dx:ASPxTextBox>
            <br />
            <dx:ASPxLabel ID="Passwordlbl" runat="server" Text="password"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="Passwordtxt" runat="server" Width="170px" Password="True"></dx:ASPxTextBox>
            <br />
            <dx:ASPxButton ID="Loginbtn" runat="server" Text="Login" OnClick="Loginbtn_Click" AutoPostBack="False"></dx:ASPxButton>
        </div>
    </form>
</body>
</html>
