<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="dashboard.aspx.vb" Inherits="WebApplication1.dashboard2" %>

<%@ Register Assembly="DevExpress.Web.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Employee Tasks</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxGridView ID="EmployeeTasksGridView" runat="server" DataSourceID="EmployeeTaskDataSource" KeyFieldName="Id" AutoGenerateColumns="False">
                <Settings ShowFilterRow="True" ShowGroupPanel="True"></Settings>
                <SettingsEditing Mode="Batch"></SettingsEditing>
                <Columns>
                    <dx:GridViewCommandColumn ShowSelectCheckbox="True" SelectAllCheckboxMode="Page" VisibleIndex="0" />
                    <dx:GridViewDataTextColumn FieldName="Id" Visible="False" />
                    <dx:GridViewDataTextColumn FieldName="Title" Caption="Task Title" VisibleIndex="1" />
                    <dx:GridViewDataTextColumn FieldName="Priority" Caption="Priority" VisibleIndex="2" />
                    <dx:GridViewDataTextColumn FieldName="Description" Caption="Task Description" VisibleIndex="3" />
                    <dx:GridViewDataTextColumn FieldName="Deadline" Caption="Deadline" VisibleIndex="4" />
                    <dx:GridViewDataComboBoxColumn FieldName="StatusId" Caption="Status" VisibleIndex="5" 
                        PropertiesComboBox-DataSourceID="StatusDataSource" 
                        PropertiesComboBox-TextField="StatusName" 
                        PropertiesComboBox-ValueField="StatusId" />
                </Columns>
            </dx:ASPxGridView>

<asp:SqlDataSource ID="EmployeeTaskDataSource" runat="server"
    ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
    SelectCommand="SELECT [Id], [Title], [Priority], [Description], [Deadline], [StatusId] FROM [TMSDB].[dbo].[Tasks] WHERE [AssignedTaskId] = @EmployeeId"
    UpdateCommand="UPDATE [Tasks] SET [StatusId] = @StatusId WHERE [Id] = @Id">
    <SelectParameters>
        <asp:SessionParameter Name="EmployeeId" DefaultValue="15" Type="Int32" />
    </SelectParameters>
    <UpdateParameters>
       
        <asp:Parameter Name="StatusId" Type="Int32" />
        <asp:Parameter Name="Id" Type="Int32" />
    </UpdateParameters>
</asp:SqlDataSource>

            <asp:SqlDataSource ID="StatusDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT [StatusId], [StatusName] FROM [TMSDB].[dbo].[Status]" />
        </div>
    </form>
</body>
</html>
