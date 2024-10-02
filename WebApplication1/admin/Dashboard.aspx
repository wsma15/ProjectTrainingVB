<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Dashboard.aspx.vb" Inherits="WebApplication1.Dashboard" %>

<%@ Register Assembly="DevExpress.Web.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard</title>

</head>
<body>
    <script type="text/javascript">
        function OnGridEndCallback(s, e) {
            var errorMessage = s.cpErrorMessage;
            if (errorMessage) {
                alert(errorMessage);
                s.cpErrorMessage = null; 
            }
        }
    </script>

    <form id="form1" runat="server">
        <div>
            <dx:ASPxPageControl ID="ASPxPageControl1" runat="server" ActiveTabIndex="0">
                <TabPages>
                    <dx:TabPage Name="RolePage" Text="Role">
                        <ContentCollection>
                            <dx:ContentControl runat="server">
                                <dx:ASPxGridView ID="RolesGridView" runat="server" KeyFieldName="Id" AutoGenerateColumns="true"
                                    OnRowDeleting="RolesGridView_RowDeleting"
                                    OnRowInserting="RolesGridView_RowInserting" OnRowValidating="RolesGridView_RowValidating"
                                    OnRowUpdating="RolesGridView_RowUpdating" EnableCallBacks="true" Theme="Default"
                                    OnDataBinding="RolesGridView_DataBinding">
                                    <Settings ShowGroupPanel="True" ShowFilterRow="True"></Settings>
                                    <SettingsSearchPanel Visible="True"></SettingsSearchPanel>
                                    <SettingsDataSecurity AllowEdit="true" AllowInsert="true" AllowDelete="true" />
                                    <Columns>
                                        <dx:GridViewCommandColumn ShowEditButton="True" VisibleIndex="0" ShowNewButtonInHeader="True" ShowDeleteButton="True" ShowClearFilterButton="True" SelectAllCheckboxMode="Page" ShowSelectCheckbox="True" />
                                        <dx:GridViewDataTextColumn FieldName="Id" Caption="ID" VisibleIndex="1">
                                            <EditFormSettings Visible="False" />
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="Name" Caption="Name" VisibleIndex="2" />
                                    </Columns>
                                </dx:ASPxGridView>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                    <dx:TabPage Name="UsersPage" Text="Users">
                        <ContentCollection>
                            <dx:ContentControl runat="server">
                                <dx:ASPxGridView ID="UsersGridView" runat="server"
                                    AutoGenerateColumns="false"
                                    KeyFieldName="Username"
                                    OnDataBinding="UsersGridView_DataBinding"
                                    OnRowInserting="UsersGridView_RowInserting"
                                    OnRowUpdating="UsersGridView_RowUpdating"
                                    OnRowDeleting="UsersGridView_RowDeleting"
                                    OnRowValidating="UsersGridView_RowValidating"
                                    EnableRowsCache="false"
                                    EnableCallBacks="true"
                                    Theme="Default">
                                    <Settings ShowGroupPanel="True" ShowFilterRow="True"></Settings>
                                    <SettingsSearchPanel Visible="True"></SettingsSearchPanel>
                                    <SettingsDataSecurity AllowEdit="true" AllowInsert="true" AllowDelete="true" />
                                    <Columns>
                                        <dx:GridViewCommandColumn VisibleIndex="0" ShowEditButton="True" ShowDeleteButton="True" ShowNewButton="True" ShowNewButtonInHeader="True" ShowClearFilterButton="True" SelectAllCheckboxMode="Page" ShowSelectCheckbox="True" />
                                        <dx:GridViewDataTextColumn FieldName="Id" Caption="ID" VisibleIndex="1">
                                            <EditFormSettings Visible="False" />
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="Name" Caption="Name" VisibleIndex="2" ReadOnly="false" LoadReadOnlyValueFromDataModel="false" />

                                        <dx:GridViewDataTextColumn FieldName="Username" Caption="Username" VisibleIndex="2" ReadOnly="false" LoadReadOnlyValueFromDataModel="false" />
                                        <dx:GridViewDataTextColumn FieldName="Password" Caption="Password" VisibleIndex="3" ReadOnly="false" LoadReadOnlyValueFromDataModel="false" />
                                        <dx:GridViewDataComboBoxColumn FieldName="RoleId" Caption="Role" VisibleIndex="4" ReadOnly="false" LoadReadOnlyValueFromDataModel="false"
                                            PropertiesComboBox-DataSourceID="RoleDataSource"
                                            PropertiesComboBox-TextField="Name"
                                            PropertiesComboBox-ValueField="Id"
                                            PropertiesComboBox-ClientSideEvents-SelectedIndexChanged="">
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataComboBoxColumn FieldName="ManagerId" Caption="Manager" VisibleIndex="5" ReadOnly="false" LoadReadOnlyValueFromDataModel="false"
                                            PropertiesComboBox-DataSourceID="ManagerDataSource"
                                            PropertiesComboBox-TextField="name"
                                            PropertiesComboBox-ValueField="Id"
                                            Visible="true">
                                        </dx:GridViewDataComboBoxColumn>
                                    </Columns>
                                </dx:ASPxGridView>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                    <dx:TabPage Name="TasksPage" Text="Tasks">
                        <ContentCollection>
                            <dx:ContentControl runat="server">
                                <dx:ASPxGridView ID="TasksGridView" runat="server" AutoGenerateColumns="false" KeyFieldName="Id" 
                                    EnableRowsCache="false" 
                                    EnableCallBacks="true"
                                    Theme="Default" 
                                    OnDataBinding="TasksGridView_DataBinding"
                                    OnRowInserting="TasksGridView_RowInserting"
                                    OnRowUpdating="TasksGridView_RowUpdating"
                                    OnRowDeleting="TasksGridView_RowDeleting"
                                    OnRowValidating="TasksGridView_RowValidating">
                                    <Settings ShowFilterRow="True" ShowGroupPanel="True"></Settings>

                                    <SettingsSearchPanel Visible="True"></SettingsSearchPanel>
                                    <SettingsDataSecurity AllowEdit="true" AllowInsert="true" AllowDelete="true" />
                                    <Columns>
                                        <dx:GridViewCommandColumn VisibleIndex="0" ShowNewButton="True" ShowNewButtonInHeader="True" ShowDeleteButton="True" SelectAllCheckboxMode="Page" ShowSelectCheckbox="True" ShowClearFilterButton="True" ShowEditButton="True" />
                                        <dx:GridViewDataTextColumn FieldName="Id" Caption="ID" VisibleIndex="1">
                                            <EditFormSettings Visible="False" />
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="Title" Caption="Title" VisibleIndex="2" ReadOnly="false" LoadReadOnlyValueFromDataModel="false" />
                                        <dx:GridViewDataTextColumn FieldName="Description" Caption="Description" VisibleIndex="3" ReadOnly="false" LoadReadOnlyValueFromDataModel="false" />
                                        <dx:GridViewDataComboBoxColumn FieldName="AssignedTaskId" Caption="Assigned To" VisibleIndex="4" ReadOnly="false"
    PropertiesComboBox-DataSourceID="UserDataSource"
    PropertiesComboBox-TextField="name"
    PropertiesComboBox-ValueField="Id"
    LoadReadOnlyValueFromDataModel="false"
    EditFormSettings-Visible="True" />
                                        <dx:GridViewDataComboBoxColumn FieldName="StatusId" Caption="status" VisibleIndex="4" ReadOnly="false"
    PropertiesComboBox-DataSourceID="StatusDataSource"
    PropertiesComboBox-TextField="StatusName"
    PropertiesComboBox-ValueField="StatusId"
    LoadReadOnlyValueFromDataModel="false"
    EditFormSettings-Visible="True" />
                                        <dx:GridViewDataTextColumn FieldName="CreateDate" Caption="Created" VisibleIndex="5" ReadOnly="false" LoadReadOnlyValueFromDataModel="false" EditFormSettings-Visible="False" />
                                        <dx:GridViewDataComboBoxColumn FieldName="Priority" Caption="Priority" VisibleIndex="6" ReadOnly="false"
                                            PropertiesComboBox-DataSourceID="PriorityDataSource"
                                            PropertiesComboBox-TextField="name"
                                            PropertiesComboBox-ValueField="Id"
                                            LoadReadOnlyValueFromDataModel="false" />
                                        <dx:GridViewDataDateColumn FieldName="Deadline" Caption="Deadline" VisibleIndex="7" ReadOnly="false" LoadReadOnlyValueFromDataModel="false" />
                                    </Columns>
                                </dx:ASPxGridView>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                </TabPages>
            </dx:ASPxPageControl>

            <asp:SqlDataSource ID="TaskDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT  [Id],[CreateDate],[Title],[Priority],[StatusId],[Description],[Deadline], [AssignedTaskId] FROM [TMSDB].[dbo].[Tasks]" />
            
            <asp:SqlDataSource ID="PriorityDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT [id],[Name]  FROM [TMSDB].[dbo].[Priority]" />
            
            <asp:SqlDataSource ID="ManagerDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT [id],[Name]  FROM [TMSDB].[dbo].[users] where Roleid = 5003" />
            
            <asp:SqlDataSource ID="RoleDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT Id, Name FROM [dbo].[Roles]" />
            
            <asp:SqlDataSource ID="UserDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT [Id],[Name],[Username],[Password],[RoleId],[ManagerId]   FROM [dbo].[Users]" />
            <asp:SqlDataSource ID="StatusDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT [StatusId],[StatusName] FROM [TMSDB].[dbo].[Status]"
 />
        </div>
        <div>
            <asp:Button ID="UsersbtnExportPDF" runat="server" Text="Export to PDF" OnClick="UsersbtnExportPDF_Click" />
        </div>
    </form>
</body>
</html>
