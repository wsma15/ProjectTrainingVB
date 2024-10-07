<%@ Page Language="vb" AutoEventWireup="true" CodeFile="Login.aspx.vb" Inherits="Login" %>
<%@ Register Assembly="DevExpress.Web.v19.1, Version=19.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxTextBox ID="txtUsername" runat="server" Placeholder="Username" Caption="Username : ">
                <ValidationSettings RequiredField-IsRequired="true" ErrorText="Username is required." />
            </dx:ASPxTextBox>

            <dx:ASPxTextBox ID="txtPassword" runat="server" Password="true" Placeholder="Password" Caption="Password : ">
                <ValidationSettings RequiredField-IsRequired="true" ErrorText="Password is required." />
            </dx:ASPxTextBox>

            <dx:ASPxButton ID="btnLogin" runat="server" Text="Login" AutoPostBack="true" OnClick="btnLogin_Click" />

            <dx:ASPxLabel ID="lblErrorMessage" runat="server" ForeColor="Red" Visible="false" />
        </div>
    </form>
</body>
</html>
