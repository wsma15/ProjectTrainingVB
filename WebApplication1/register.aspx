<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="register.aspx.vb" Inherits="WebApplication1.register" %>

<%@ Register Assembly="DevExpress.Web.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Registration</title>
    <script type="text/javascript">
        function showPopupMenu(evt, popupMenu) {
            evt.preventDefault();
            popupMenu.ShowAtPos(evt.pageX, evt.pageY);
            return false;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="display: flex; flex-direction: column; align-items: center; gap: 20px;">

<dx:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" ShowCollapseButton="true" Width="350px" HeaderText="User Registration" 
    BackColor="#f5f5f5">
    <PanelCollection>
        <dx:PanelContent>
            <dx:ASPxTextBox ID="ASPxTextBox1" runat="server" Caption="Full Name" 
                AccessibilityLabelText="Full Name :" 
                />
            <br />
            <dx:ASPxTextBox ID="ASPxEmailTextBox" runat="server" Caption="Email" 
                NullText="Enter your email..." />
            <br />
            <dx:ASPxTextBox ID="ASPxPasswordTextBox" runat="server" Width="300px" Caption="Password" 
                Password="true"
                NullText="Enter a strong password" />
            <dx:ASPxButtonEdit ID="ASPxButtonEdit1" runat="server">
                <Buttons>
                    <dx:EditButton Text="Hey" >
                    </dx:EditButton>
                </Buttons>
            </dx:ASPxButtonEdit>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxRoundPanel>
            <div style="display: flex; flex-direction: column; align-items: flex-start; width: 350px;">
                <label for="ASPxComboBox1" style="margin-bottom: 5px; font-weight: bold;">Role</label>
                <dx:ASPxComboBox ID="ASPxComboBox1" runat="server" DataSourceID="RoleDataSource" 
                    ValueField="Id" TextField="Name" Width="300px"
                    ValueType="System.String" DropDownButton-Text="Select Role" 
                    Theme="Office2010Blue">
                </dx:ASPxComboBox>
                <asp:SqlDataSource ID="RoleDataSource" runat="server"
                    ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TrainingApp;Integrated Security=True"
                    SelectCommand="SELECT Name, Id FROM [dbo].[Roles]" />
            </div>
            <div style="display: flex; gap: 20px;">
                <dx:ASPxRadioButton ID="ASPxRadioButton1" runat="server" Text="Male" GroupName="Gender" Width="150px" Theme="Office2010Blue" />
                <dx:ASPxRadioButton ID="ASPxRadioButton2" runat="server" Text="Female" GroupName="Gender" Width="150px" Theme="Office2010Blue" />
            </div>
            <dx:ASPxButton ID="ASPxButton1" runat="server" Text="Register" Width="150px" AutoPostBack="false" />
        </div>
    </form>
</body>
</html>
