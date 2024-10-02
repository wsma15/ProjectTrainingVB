<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="dashboard.aspx.vb" Inherits="WebApplication1.dashboard1" %>

<%@ Register Assembly="DevExpress.Web.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxPageControl ID="ManagerPageControl" runat="server" ActiveTabIndex="0">
                <TabPages>
                    <dx:TabPage Name="EmployeePage" Text="Employee">
                        <ContentCollection>
                            <dx:ContentControl runat="server">
                                <dx:ASPxGridView ID="ASPxGridView1" runat="server" DataSourceID="UserDataSource" AutoGenerateColumns="False" KeyFieldName="Id">
                                    <Settings ShowFilterRow="True" ShowGroupPanel="True"></Settings>

                                    <SettingsSearchPanel Visible="True"></SettingsSearchPanel>
                                    <Columns>
                                        <dx:GridViewDataTextColumn FieldName="Id" ReadOnly="True" VisibleIndex="0">
                                            <EditFormSettings Visible="False"></EditFormSettings>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="Name" VisibleIndex="1"></dx:GridViewDataTextColumn>
                                        <dx:GridViewDataComboBoxColumn
                                            FieldName="RoleId" Caption="Role"
                                            VisibleIndex="2" ReadOnly="false"
                                            LoadReadOnlyValueFromDataModel="false"
                                            PropertiesComboBox-DataSourceID="RoleDataSource"
                                            PropertiesComboBox-TextField="Name"
                                            PropertiesComboBox-ValueField="Id">
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataTextColumn FieldName="TaskCount" Caption="Count Tasks (3 Maximum)" VisibleIndex="3"></dx:GridViewDataTextColumn>
                                    </Columns>
                                </dx:ASPxGridView>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>

                    <dx:TabPage Name="TasksPage" Text="Tasks" ToolTip="Assign or Review Tasks">
                        <ContentCollection>
                            <dx:ContentControl runat="server">
                                <dx:ASPxGridView ID="ASPxGridView2" runat="server" DataSourceID="TaskDataSource" AutoGenerateColumns="False" KeyFieldName="Id">
                                    <Settings ShowFilterRow="True" ShowGroupPanel="True"></Settings>
                                    <SettingsEditing Mode="Batch" />
                                    <Columns>
                                        <dx:GridViewDataTextColumn FieldName="Title" Caption="Task Title" VisibleIndex="0"></dx:GridViewDataTextColumn>
                                        <dx:GridViewDataComboBoxColumn
                                            FieldName="Priority" Caption="Priority"
                                            VisibleIndex="1"
                                            PropertiesComboBox-DataSourceID="PriorityDataSource"
                                            PropertiesComboBox-TextField="Name"
                                            PropertiesComboBox-ValueField="Id">
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataComboBoxColumn
                                            FieldName="StatusId" Caption="Status"
                                            VisibleIndex="2"
                                            PropertiesComboBox-DataSourceID="StatusDataSource"
                                            PropertiesComboBox-TextField="StatusName"
                                            PropertiesComboBox-ValueField="StatusId">
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataDateColumn FieldName="Deadline" Caption="Deadline" VisibleIndex="3"></dx:GridViewDataDateColumn>
                                        <dx:GridViewDataComboBoxColumn
                                            FieldName="AssignedTaskId" Caption="Assign to Employee"
                                            VisibleIndex="4"
                                            PropertiesComboBox-DataSourceID="UserDataSource"
                                            PropertiesComboBox-TextField="Name"
                                            PropertiesComboBox-ValueField="Id">
                                        </dx:GridViewDataComboBoxColumn>
                                    </Columns>
                                </dx:ASPxGridView>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                </TabPages>
            </dx:ASPxPageControl>

<asp:SqlDataSource ID="TaskDataSource" runat="server"
    ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
    SelectCommand="SELECT [Id],[CreateDate],[Title],[Priority],[StatusId],[Description],[Deadline], [AssignedTaskId] FROM [TMSDB].[dbo].[Tasks]"
    UpdateCommand="UPDATE [TMSDB].[dbo].[Tasks] SET [Title] = @Title, [Priority] = @Priority, [StatusId] = @StatusId, [Description] = @Description, [Deadline] = @Deadline, [AssignedTaskId] = @AssignedTaskId WHERE [Id] = @Id">
    
    <UpdateParameters>
        <asp:Parameter Name="Id" Type="Int32" />
        <asp:Parameter Name="Title" Type="String" />
        <asp:Parameter Name="Priority" Type="Int32" />
        <asp:Parameter Name="StatusId" Type="Int32" />
        <asp:Parameter Name="Description" Type="String" />
        <asp:Parameter Name="Deadline" Type="DateTime" />
        <asp:Parameter Name="AssignedTaskId" Type="Int32" />
    </UpdateParameters>
</asp:SqlDataSource>

            <asp:SqlDataSource ID="PriorityDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT [id],[Name]  FROM [TMSDB].[dbo].[Priority]" />

            <asp:SqlDataSource ID="ManagerDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT [id],[Name]  FROM [TMSDB].[dbo].[users] where Roleid = 5003" />

            <asp:SqlDataSource ID="UserDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT U.Id, U.Name, U.RoleId, U.ManagerId, COUNT(T.Id) AS TaskCount FROM [dbo].[Users] U LEFT JOIN [dbo].[Tasks] T ON U.Id = T.AssignedTaskId WHERE U.ManagerId = @ManagerId GROUP BY U.Id, U.Name, U.RoleId, U.ManagerId">
                <SelectParameters>
                    <asp:Parameter Name="ManagerId" Type="Int32" DefaultValue='6' />
                </SelectParameters>
            </asp:SqlDataSource>

            <asp:SqlDataSource ID="StatusDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT [StatusId],[StatusName] FROM [TMSDB].[dbo].[Status]" />

            <asp:SqlDataSource ID="RoleDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
                SelectCommand="SELECT Id, Name FROM [dbo].[Roles]" />
        </div>
    </form>
</body>
</html>
