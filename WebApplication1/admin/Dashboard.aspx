<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="Dashboard.aspx.vb" Inherits="WebApplication1.Dashboard" %>

<%@ Register Assembly="DevExpress.Web.v19.1, Version=19.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard</title>

</head>
<a href="Dashboard.aspx">Dashboard.aspx</a>
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
                                    OnRowInserting="RolesGridView_RowInserting"
                                    OnRowValidating="RolesGridView_RowValidating"
                                    OnRowUpdating="RolesGridView_RowUpdating"
                                    EnableCallBacks="true"
                                    Theme="Default"
                                    OnDataBinding="RolesGridView_DataBinding"
                                    SettingsEditing-Mode="Batch">
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
                                    KeyFieldName="Id"
                                    OnDataBinding="UsersGridView_DataBinding"
                                    OnRowInserting="UsersGridView_RowInserting"
                                    OnRowUpdating="UsersGridView_RowUpdating"
                                    OnRowDeleting="UsersGridView_RowDeleting"
                                    OnRowValidating="UsersGridView_RowValidating"
                                    EnableRowsCache="false"
                                    SettingsEditing-Mode="Batch"
                                    EnableCallBacks="true"
                                    Theme="Default">
                                    <Settings ShowGroupPanel="True" ShowFilterRow="True"></Settings>
                                    <SettingsSearchPanel Visible="True"></SettingsSearchPanel>
                                    <SettingsDataSecurity AllowEdit="true" AllowInsert="true" AllowDelete="true" />
                                    <Columns>
                                        <dx:GridViewCommandColumn VisibleIndex="0" ShowEditButton="True" ShowDeleteButton="True" ShowNewButton="True" ShowNewButtonInHeader="True" ShowClearFilterButton="True" SelectAllCheckboxMode="Page" ShowSelectCheckbox="True" />
                                        <dx:GridViewDataTextColumn FieldName="Id" Caption="ID" VisibleIndex="1" Name="Id">
                                            <EditFormSettings Visible="False" />
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="Name" Caption="Name" VisibleIndex="2" ReadOnly="false" Name="Name" />

                                        <dx:GridViewDataTextColumn FieldName="Username" Caption="Username" VisibleIndex="3" ReadOnly="false" />
                                        <dx:GridViewDataTextColumn FieldName="Password" Caption="Password" VisibleIndex="4" ReadOnly="false" />
                                        <dx:GridViewDataComboBoxColumn FieldName="RoleId" Caption="Role" VisibleIndex="5" ReadOnly="false"
                                            PropertiesComboBox-DataSourceID="RoleDataSource"
                                            PropertiesComboBox-TextField="Name"
                                            PropertiesComboBox-ValueField="Id">
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataComboBoxColumn FieldName="ManagerId" Caption="Manager" VisibleIndex="6" ReadOnly="false"
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
                                <dx:ASPxGridView ID="TasksGridView" runat="server" KeyFieldName="Id" AutoGenerateColumns="False"
                                    SettingsDataSecurity-AllowEdit="true"
                                    OnDataBinding="TasksGridView_DataBinding"
                                    OnRowInserting="TasksGridView_RowInserting"
                                    SettingsEditing-Mode="Batch"
                                    OnRowUpdating="TasksGridView_RowUpdating"
                                    OnRowDeleting="TasksGridView_RowDeleting"
                                    OnRowValidating="TasksGridView_RowValidating"
                                    EnableRowsCache="false"
                                    EnableCallBacks="true"
                                    Theme="Default">


                                    <Columns>
                                        <dx:GridViewCommandColumn ShowEditButton="True" VisibleIndex="0" ShowNewButtonInHeader="True" ShowDeleteButton="True"></dx:GridViewCommandColumn>
                                        <dx:GridViewDataTextColumn FieldName="Id" Visible="False" />
                                        <dx:GridViewDataTextColumn FieldName="Title" Caption="Task Title" VisibleIndex="1" />
                                        <dx:GridViewDataComboBoxColumn FieldName="Priority" Caption="Priority" Visible="true"
                                            PropertiesComboBox-DataSourceID="PriorityDataSource"
                                            PropertiesComboBox-TextField="Name"
                                            PropertiesComboBox-ValueField="id" VisibleIndex="2" />
                                        <dx:GridViewDataTextColumn FieldName="Description" Caption="Description" VisibleIndex="3" />
                                        <dx:GridViewDataDateColumn FieldName="Deadline" Caption="Deadline" VisibleIndex="4" />
                                        <dx:GridViewDataDateColumn FieldName="CreateDate" Caption="Create" VisibleIndex="5" EditFormSettings-Visible="False"></dx:GridViewDataDateColumn>
                                        <dx:GridViewDataComboBoxColumn FieldName="StatusId" Caption="Status"
                                            PropertiesComboBox-DataSourceID="StatusDataSource"
                                            PropertiesComboBox-TextField="StatusName"
                                            PropertiesComboBox-ValueField="StatusId"
                                            EditFormSettings-Visible="False"
                                            VisibleIndex="6" />
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
                SelectCommand="SELECT [id],[Name]  FROM [TMSDB].[dbo].[users] " />

            <asp:SqlDataSource ID="RoleDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT Id, Name FROM [dbo].[Roles]" />

            <asp:SqlDataSource ID="UserDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT [Id],[Name],[Username],[Password],[RoleId],[ManagerId] FROM [dbo].[Users]" />

            <asp:SqlDataSource ID="StatusDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT [StatusId],[StatusName] FROM [TMSDB].[dbo].[Status]" />
        </div>
        <div>
        </div>
    </form>
</body>
</html>
