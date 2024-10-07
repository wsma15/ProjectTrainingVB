Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports DevExpress.Web
Imports System.Data.SqlClient
Imports System.IO

Public Class Dashboard
    Inherits System.Web.UI.Page
    Protected ReadOnly connectionString As String = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"
    Public Function UserIsInRole(roleName As String) As Boolean
        If Session("RoleName") IsNot Nothing Then
            Dim currentRole As String = Session("RoleName").ToString().ToLower()
            Return currentRole.Equals(roleName.ToLower())
        End If
        Return False
    End Function
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindUsersGridView()
            BindRolesGridView()
            BindTasksGridView()
        End If
        If Not UserIsInRole("Admin") Then
            Response.Redirect("~/Log/Login.aspx?ReturnUrl=" & Server.UrlEncode(Request.RawUrl))
        End If
    End Sub

    Private Sub BindUsersGridView()
        'Dim rptdoc As New ReportDocument()
        ' rptdoc.Load(Server.MapPath("~/UsersReport.rpt"))
        Dim query As String = "SELECT Id,Name,[Username],[Password],[RoleId],[ManagerId] FROM [TMSDB].[dbo].[Users]"
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(query, conn)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            conn.Open()
            da.Fill(dt)
            conn.Close()
            For Each row As DataRow In dt.Rows
                Dim encryptedPassword As String = row("Password").ToString()
                Dim decryptedPassword As String = PasswordHelper.Decrypt(encryptedPassword)
                row("Password") = decryptedPassword
            Next
            Session("UsersDs") = dt
            UsersGridView.DataBind()
            UsersGridView.EnableViewState = False
        End Using
    End Sub

    Private Function IsValidUsername(username As String)
        Dim regex = New Regex("^[a-zA-Z0-9]{6,}$")
        Return regex.IsMatch(username)
    End Function
    Private Function IsValidName(name As String)
        Dim regex = New Regex("^[a-zA-Z]+( [a-zA-Z]+)*$")
        Return regex.IsMatch(name)
    End Function

    Private Function IsValidPassword(password As String) As Boolean
        Dim regex As New Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$")
        Return regex.IsMatch(password)
    End Function


    Protected Function IsRoleAssignedToUsers(RoleId As Integer) As Boolean
        Dim isAssigned As Boolean = False
        Dim query As String = "SELECT COUNT(*) FROM [dbo].[Users] WHERE RoleId = @RoleId"
        Using conn As New SqlConnection(connectionString)
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@RoleId", RoleId)
                conn.Open()
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                isAssigned = count > 0
            End Using
        End Using
        Return isAssigned
    End Function
    Private Sub BindTasksGridView()
        Dim query As String = "SELECT [Id],[CreateDate],[Title],[Priority],[StatusId],[Description],[Deadline], [AssignedTaskId] FROM [TMSDB].[dbo].[Tasks]"
        Using conn As New SqlConnection(connectionString)
            Using cmd As New SqlCommand(query, conn)
                Using da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    Session("TasksDs") = dt
                End Using
            End Using
        End Using
        Debug.Write("call TaskDatabind \n")
        TasksGridView.DataBind()
    End Sub

    Private Sub BindRolesGridView()
        Dim query As String = "SELECT Id, Name FROM Roles"
        Using conn As New SqlConnection(connectionString)
            Using cmd As New SqlCommand(query, conn)
                Using da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    Session("RolesDs") = dt
                End Using
            End Using
        End Using
        Debug.Write("call \n")
        RolesGridView.DataBind()
    End Sub


    Protected Function GetUsersDataFromProcedure() As DataTable
        Dim dt As New DataTable()
        Dim storedProcedureName As String = "RetriveAllData"
        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(storedProcedureName, conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error: " & ex.Message)
        End Try
        Return dt
    End Function

    Protected Function GetRolesDataFromProcedure() As DataTable
        Dim dt As New DataTable()
        Dim storedProcedureName As String = "RetriveRoles"
        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(storedProcedureName, conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error: " & ex.Message)
        End Try
        Return dt
    End Function

    Protected Sub UsersGridView_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs)
        If e.NewValues("Username") IsNot Nothing AndAlso e.NewValues("Password") IsNot Nothing Then
            Dim username As String = e.NewValues("Username").ToString()
            Dim name As String = e.NewValues("Name")?.ToString() ' Using ?. for safety
            Dim managerId As Object = e.NewValues("ManagerId") ' Keep it as Object to check for DBNull
            Dim password As String = e.NewValues("Password").ToString()
            Dim hashedPassword As String = PasswordHelper.HashPassword(password)
            Dim roleId As Object = e.NewValues("RoleId")

            '' Check if RoleId is missing
            'If roleId Is Nothing Then
            '    e.Cancel = True
            '    UsersGridView.JSProperties("cpMessage") = "Role ID is missing."
            '    Return
            'End If

            '' Validate username and password
            'If Not IsValidUsername(username) Then
            '    e.Cancel = True
            '    UsersGridView.JSProperties("cpMessage") = "Username must be at least 6 characters long and contain no spaces or special characters."
            '    Return
            'End If

            'If Not IsValidPassword(password) Then
            '    e.Cancel = True
            '    UsersGridView.JSProperties("cpMessage") = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character."
            '    Return
            'End If

            ' Check if username exists
            Dim checkQuery As String = "SELECT COUNT(1) FROM [dbo].[Users] WHERE Username = @Username"
            Using conn As New SqlConnection(connectionString)
                Dim checkCmd As New SqlCommand(checkQuery, conn)
                checkCmd.Parameters.AddWithValue("@Username", username)
                conn.Open()
                Dim usernameExists As Boolean = Convert.ToBoolean(checkCmd.ExecuteScalar())
                conn.Close()

                If usernameExists Then
                    e.Cancel = True
                    UsersGridView.JSProperties("cpMessage") = "Username already exists. Please choose a different username."
                    Return
                End If
            End Using

            ' Insert the new user
            Dim insertQuery As String = "INSERT INTO [dbo].[Users] (Username, Password, RoleId, Name, ManagerId) VALUES (@Username, @Password, @RoleId, @Name, @ManagerId)"
            Using conn As New SqlConnection(connectionString)
                Dim insertCmd As New SqlCommand(insertQuery, conn)
                insertCmd.Parameters.AddWithValue("@Username", username)
                insertCmd.Parameters.AddWithValue("@Password", hashedPassword)
                insertCmd.Parameters.AddWithValue("@Name", name)
                insertCmd.Parameters.AddWithValue("@RoleId", If(roleId, DBNull.Value))

                ' Handle ManagerId safely
                If managerId Is DBNull.Value OrElse managerId Is Nothing Then
                    insertCmd.Parameters.AddWithValue("@ManagerId", DBNull.Value) ' Use NULL if there's no manager
                Else
                    insertCmd.Parameters.AddWithValue("@ManagerId", Convert.ToInt32(managerId)) ' Ensure it's an integer
                End If

                conn.Open()
                insertCmd.ExecuteNonQuery()
                conn.Close()
            End Using

            e.Cancel = True
            UsersGridView.CancelEdit()
        End If

        ' Generate the report
        '      Dim rptdoc As New ReportDocument()
        '     rptdoc.Load(Server.MapPath("~/UsersReport.rpt"))
        Dim query As String = "SELECT [Id],[Name],[Username],[Password],[RoleId],[ManagerId] FROM [TMSDB].[dbo].[Users]"
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(query, conn)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            conn.Open()
            da.Fill(dt)
            conn.Close()

            ' Decrypt passwords
            For Each row As DataRow In dt.Rows
                Dim encryptedPassword As String = row("Password").ToString()
                Dim decryptedPassword As String = PasswordHelper.Decrypt(encryptedPassword)
                row("Password") = decryptedPassword
            Next

            Session("UsersDs") = dt
        End Using
    End Sub

    Protected Sub UsersGridView_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs)
        Dim username As String = If(e.Keys("Id") IsNot Nothing, e.Keys("Id").ToString(), String.Empty)

        If String.IsNullOrEmpty(username) Then
            Throw New Exception("Username key is missing.")
        End If
        Dim name As String = e.NewValues("Name").ToString()
        Dim password As String = e.NewValues("Password").ToString()
        Dim roleId As Integer = Convert.ToInt32(e.NewValues("RoleId"))
        Dim managerId As Integer? = If(e.NewValues("ManagerId") IsNot Nothing, Convert.ToInt32(e.NewValues("ManagerId")), CType(Nothing, Integer?))

        ' Create the SQL update query
        Dim query As String = "UPDATE [dbo].[Users] SET Name = @Name, Password = @Password, RoleId = @RoleId, ManagerId = @ManagerId WHERE Id = @Username"

        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Username", username)
            cmd.Parameters.AddWithValue("@Name", name)
            cmd.Parameters.AddWithValue("@Password", PasswordHelper.HashPassword(password))
            cmd.Parameters.AddWithValue("@RoleId", roleId)
            cmd.Parameters.AddWithValue("@ManagerId", If(managerId.HasValue, CType(managerId, Object), DBNull.Value)) ' Handle nullable ManagerId

            conn.Open()
            cmd.ExecuteNonQuery()
            conn.Close()
        End Using

        e.Cancel = True
        UsersGridView.CancelEdit()

        ' Reload the updated user list
        'Dim rptdoc As New ReportDocument()
        'rptdoc.Load(Server.MapPath("~/UsersReport.rpt"))

        Dim selectQuery As String = "SELECT [Id],[Name],[Username],[Password],[RoleId],[ManagerId] FROM [TMSDB].[dbo].[Users]"
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(selectQuery, conn)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()

            conn.Open()
            da.Fill(dt)
            conn.Close()

            For Each row As DataRow In dt.Rows
                Dim encryptedPassword As String = row("Password").ToString()
                Dim decryptedPassword As String = PasswordHelper.Decrypt(encryptedPassword)
                row("Password") = decryptedPassword
            Next

            Session("UsersDs") = dt
        End Using
    End Sub

    Protected Sub UsersGridView_RowValidating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataValidationEventArgs)
        e.NewValues("Id") = 0 ' Reset or set default value for 'Id'

        ' Loop through columns for validation
        For Each row As GridViewColumn In UsersGridView.Columns
            Dim dataRow As GridViewDataColumn = TryCast(row, GridViewDataColumn)

            If dataRow IsNot Nothing AndAlso
           String.Compare(dataRow.FieldName, "Id", StringComparison.Ordinal) <> 0 Then
                If String.Compare(dataRow.FieldName, "ManagerId", StringComparison.Ordinal) = 0 Then
                    If e.NewValues IsNot Nothing AndAlso e.NewValues.Contains("RoleId") Then
                        Dim userRole As String = e.NewValues("RoleId")?.ToString()
                        If userRole = "Employee" AndAlso e.NewValues(dataRow.FieldName) Is Nothing Then
                            e.Errors(dataRow) = "ManagerId cannot be null for Employee role."
                        End If
                    Else
                        e.Errors(dataRow) = "RoleId is required."
                    End If
                ElseIf e.NewValues IsNot Nothing AndAlso e.NewValues(dataRow.FieldName) Is Nothing Then
                    e.Errors(dataRow) = "Value cannot be null."
                End If
            End If
        Next
        If e.NewValues("Username") IsNot Nothing AndAlso Not IsValidUsername(e.NewValues("Username").ToString()) Then
            e.Errors(CType(UsersGridView.Columns("Username"), GridViewDataColumn)) = "The input must be at least 6 characters long, only contain letters and numbers, and have no special characters."
        End If

        ' Additional validation for 'Password'
        If e.NewValues("Password") IsNot Nothing AndAlso Not IsValidPassword(e.NewValues("Password").ToString()) Then
            e.Errors(CType(UsersGridView.Columns("Password"), GridViewDataColumn)) = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character."
        End If

        ' Check if any validation errors occurred
        If e.Errors.Count > 0 Then
            e.RowError = $"{e.Errors.Last}Please fill in all fields."
        End If
    End Sub
    Protected Sub UsersGridView_RowDeleting(sender As Object, e As DevExpress.Web.Data.ASPxDataDeletingEventArgs)
        ' Retrieve the ID of the user to be deleted
        Dim userId As Integer = Convert.ToInt32(e.Keys("Id"))

        ' Define the query for deleting the user from the database
        Dim deleteQuery As String = "DELETE FROM Users WHERE Id = @Id"

        ' Perform the delete operation using ADO.NET
        Using conn As New SqlConnection(connectionString)
            Using cmd As New SqlCommand(deleteQuery, conn)
                cmd.Parameters.AddWithValue("@Id", userId)

                Try
                    conn.Open()
                    cmd.ExecuteNonQuery()  ' Execute the delete command
                Catch ex As Exception
                    ' Handle any exceptions, such as foreign key constraint violations
                    UsersGridView.JSProperties("cpErrorMessage") = ex.Message
                Finally
                    conn.Close()
                End Try
            End Using
        End Using

        ' Prevent further processing by the grid, as the deletion has already been handled
        e.Cancel = True
        UsersGridView.CancelEdit()
        Dim username As String = If(e.Keys("Id") IsNot Nothing, e.Keys("Id").ToString(), String.Empty)


        Dim query = "SELECT [Id],[Name],[Username],[Password],[RoleId],[ManagerId] FROM [TMSDB].[dbo].[Users]"
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(query, conn)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            conn.Open()
            da.Fill(dt)
            conn.Close()
            For Each row As DataRow In dt.Rows
                Dim encryptedPassword As String = row("Password").ToString()
                Dim decryptedPassword As String = PasswordHelper.Decrypt(encryptedPassword)
                row("Password") = decryptedPassword
            Next
            Session("UsersDs") = dt
        End Using
    End Sub

    Protected Sub RolesGridView_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs)
        If e.NewValues("Name") IsNot Nothing Then
            Dim roleName As String = e.NewValues("Name").ToString()
            Dim query As String = "INSERT INTO [dbo].[Roles] (Name) VALUES (@Name)"
            Using conn As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Name", roleName)
                conn.Open()
                cmd.ExecuteNonQuery()
                conn.Close()
            End Using
            e.Cancel = True
            RolesGridView.CancelEdit()
        End If
        Dim selectQuery As String = "SELECT Id, Name FROM Roles"
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(selectQuery, conn)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)
            Session("RolesDs") = dt
        End Using
    End Sub

    Protected Sub RolesGridView_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs)
        Dim RoleId As String = e.Keys("Id").ToString()
        Dim roleName As String = e.NewValues("Name").ToString()
        Dim query As String = "UPDATE [dbo].[Roles] SET Name = @Name WHERE Id = @id"
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Name", roleName)
            cmd.Parameters.AddWithValue("@id", RoleId)
            conn.Open()
            cmd.ExecuteNonQuery()
            conn.Close()
        End Using
        e.Cancel = True
        RolesGridView.CancelEdit()
        Dim selectQuery As String = "SELECT Id, Name FROM Roles"
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(selectQuery, conn)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)
            Session("RolesDs") = dt
        End Using
    End Sub

    Protected Sub RolesGridView_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs)
        Dim RoleId As Integer = Convert.ToInt32(e.Keys("Id"))
        If IsRoleAssignedToUsers(RoleId) Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "script", "alert('Cannot delete the role as it is assigned to one or more users.')", True)
            RolesGridView.JSProperties("cpMessage") = "Cannot delete the role as it is assigned to one or more users."
            e.Cancel = True
        Else
            Dim query As String = "DELETE FROM [dbo].[Roles] WHERE Id = @id"
            Using conn As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@id", RoleId)
                conn.Open()
                cmd.ExecuteNonQuery()
                conn.Close()
            End Using
            e.Cancel = True
        End If
        Dim selectQuery As String = "SELECT Id, Name FROM Roles"
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(selectQuery, conn)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)
            Session("RolesDs") = dt
        End Using
    End Sub

    Protected Sub RolesGridView_RowValidating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataValidationEventArgs)
        If e.NewValues("Name") Is Nothing Then
            e.Errors(RolesGridView.Columns("Name")) = "Value cannot be null."
        Else
            If Not IsValidName(e.NewValues("Name").ToString()) Then
                e.Errors(RolesGridView.Columns("Name")) = "The name must contain only letters and have at least two words."
            End If
        End If
        If e.Errors.Count > 0 Then
            e.RowError = "Please fill in all fields correctly."
        End If
    End Sub

    Protected Sub UsersGridView_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
        Debug.Write("Here Data Binding" & vbLf)
        UsersGridView.DataSource = Session("UsersDs")
    End Sub

    Protected Sub RolesGridView_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
        RolesGridView.DataSource = Session("RolesDs")
    End Sub

    Protected Sub TasksGridView_DataBinding(sender As Object, e As EventArgs)
        TasksGridView.DataSource = Session("TasksDs")
    End Sub

    Protected Sub TasksGridView_RowInserting(sender As Object, e As Data.ASPxDataInsertingEventArgs)
        ' Set default values for CreateDate and StatusId
        Dim CreateDate As DateTime = DateTime.Now ' Set to today's date
        Dim statusId As Integer = GetStatusId("Not Started") ' Assume GetStatusId function returns the ID for the 'Not Started' status

        If e.NewValues("Title") IsNot Nothing Then
            Dim title As String = e.NewValues("Title").ToString()
            Dim priority As String = e.NewValues("Priority")?.ToString() ' Using ?. for safety
            Dim description As String = e.NewValues("Description")?.ToString()
            Dim deadline As Object = e.NewValues("Deadline") ' Keep as Object for potential NULL
            Dim assignedTaskId As Object = e.NewValues("AssignedTaskId") ' Same for AssignedTaskId

            ' Insert the new task
            Dim insertQuery As String = "INSERT INTO [dbo].[Tasks] (CreateDate, Title, Priority, StatusId, Description, Deadline, AssignedTaskId) VALUES (@CreateDate, @Title, @Priority, @StatusId, @Description, @Deadline, @AssignedTaskId)"
            Using conn As New SqlConnection(connectionString)
                Dim insertCmd As New SqlCommand(insertQuery, conn)
                insertCmd.Parameters.AddWithValue("@CreateDate", CreateDate) ' Use today's date
                insertCmd.Parameters.AddWithValue("@Title", title)
                insertCmd.Parameters.AddWithValue("@Priority", priority)
                insertCmd.Parameters.AddWithValue("@StatusId", statusId) ' Default to "Not Started"

                ' Handle Description safely
                If String.IsNullOrEmpty(description) Then
                    insertCmd.Parameters.AddWithValue("@Description", DBNull.Value)
                Else
                    insertCmd.Parameters.AddWithValue("@Description", description)
                End If

                ' Handle Deadline safely
                If deadline Is DBNull.Value OrElse deadline Is Nothing Then
                    insertCmd.Parameters.AddWithValue("@Deadline", DBNull.Value) ' Use NULL if no deadline
                Else
                    insertCmd.Parameters.AddWithValue("@Deadline", Convert.ToDateTime(deadline)) ' Ensure it's a DateTime
                End If

                ' Handle AssignedTaskId safely
                If assignedTaskId Is DBNull.Value OrElse assignedTaskId Is Nothing Then
                    insertCmd.Parameters.AddWithValue("@AssignedTaskId", DBNull.Value) ' Use NULL if there's no assigned task
                Else
                    insertCmd.Parameters.AddWithValue("@AssignedTaskId", Convert.ToInt32(assignedTaskId)) ' Ensure it's an integer
                End If

                ' Execute the query
                conn.Open()
                insertCmd.ExecuteNonQuery()
                conn.Close()
            End Using

            e.Cancel = True
            TasksGridView.CancelEdit()
        End If

        ' Generate the report
        'Dim rptdoc As New ReportDocument()
        'rptdoc.Load(Server.MapPath("~/TasksReport.rpt"))
        Dim query As String = "SELECT [Id], [CreateDate], [Title], [Priority], [StatusId], [Description], [Deadline], [AssignedTaskId] FROM [TMSDB].[dbo].[Tasks]"
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(query, conn)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            conn.Open()
            da.Fill(dt)
            conn.Close()

            ' Store the dataset in session (if necessary)
            Session("TasksDs") = dt
        End Using
    End Sub

    ' Helper function to get the status ID based on the status name
    Private Function GetStatusId(statusName As String) As Integer
        Dim statusId As Integer = 0
        Dim query As String = "SELECT StatusId FROM [dbo].[Status] WHERE StatusName = @StatusName"
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@StatusName", statusName)
            conn.Open()
            Dim result As Object = cmd.ExecuteScalar()
            If result IsNot Nothing Then
                statusId = Convert.ToInt32(result)
            End If
            conn.Close()
        End Using
        Return statusId
    End Function


    Protected Sub TasksGridView_RowDeleting(sender As Object, e As Data.ASPxDataDeletingEventArgs)
        ' Ensure the Task ID is provided
        If e.Values("Id") IsNot Nothing Then
            Dim taskId As Integer = Convert.ToInt32(e.Values("Id"))

            ' Delete query to remove the task by its ID
            Dim deleteQuery As String = "DELETE FROM [dbo].[Tasks] WHERE Id = @Id"

            Using conn As New SqlConnection(connectionString)
                Dim deleteCmd As New SqlCommand(deleteQuery, conn)
                deleteCmd.Parameters.AddWithValue("@Id", taskId)

                conn.Open()
                deleteCmd.ExecuteNonQuery()
                conn.Close()
            End Using

            ' Cancel the default delete operation (since we manually handle it)
            e.Cancel = True
        Else
            ' If no Task ID is provided, cancel the deletion and display an error message
            e.Cancel = True
            TasksGridView.JSProperties("cpMessage") = "Task ID is missing."
        End If
        Dim query As String = "SELECT [Id], [CreateDate], [Title], [Priority], [StatusId], [Description], [Deadline], [AssignedTaskId] FROM [TMSDB].[dbo].[Tasks]"
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(query, conn)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            conn.Open()
            da.Fill(dt)
            conn.Close()

            ' Store the dataset in session (if necessary)
            Session("TasksDs") = dt
        End Using
    End Sub

    Protected Sub TasksGridView_RowValidating(sender As Object, e As Data.ASPxDataValidationEventArgs)
        ' Validate CreateDate (should be a valid date)
        e.NewValues("CreateDate") = Date.Now

        If e.NewValues("CreateDate") Is Nothing Then
            e.Errors(TasksGridView.Columns("CreateDate")) = "Create date is required and must be a valid date."
        End If

        ' Validate Title (required, should not be empty)
        If e.NewValues("Title") Is Nothing OrElse String.IsNullOrWhiteSpace(e.NewValues("Title").ToString()) Then
            e.Errors(TasksGridView.Columns("Title")) = "Title is required."
        End If

        ' Validate Priority (required, should be Low, Medium, or High)
        If e.NewValues("Priority") Is Nothing Then
            e.Errors(TasksGridView.Columns("Priority")) = "Priority must be Low, Medium, or High."
        End If

        ' Validate Deadline (if provided, it must be a valid date)
        If e.NewValues("Deadline") Is Nothing Then
            e.Errors(TasksGridView.Columns("Deadline")) = "Deadline must be a valid date."
        End If

        ' If there are any errors, cancel the operation and display error messages
        If e.Errors.Count > 0 Then
            e.RowError = $"{e.Errors.Last}Please correct all errors and try again."
        End If
    End Sub

    Protected Sub TasksGridView_RowUpdating(sender As Object, e As Data.ASPxDataUpdatingEventArgs)
        ' Check if required fields are not null
        Dim taskId As Integer = Convert.ToInt32(e.Keys("Id")) ' Get the primary key (Task Id)
        Dim title As String = e.NewValues("Title").ToString()
        Dim priority As String = e.NewValues("Priority")?.ToString() ' Use ?. for safety
        Dim statusId As Object = e.NewValues("StatusId") ' Foreign key for status
        Dim description As String = e.NewValues("Description")?.ToString()
        Dim deadline As Object = e.NewValues("Deadline") ' Can be NULL
        Dim assignedTaskId As Object = e.NewValues("AssignedTaskId") ' Can be NULL

        ' Check if StatusId is missing
        If statusId Is Nothing Then
            e.Cancel = True
            TasksGridView.JSProperties("cpMessage") = "Status ID is required."
            Return
        End If
        Debug.Write("updwwating...")

        ' Update query
        Dim updateQuery As String = "UPDATE [dbo].[Tasks] 
                                     SET  Title = @Title, Priority = @Priority, 
                                         StatusId = @StatusId, Description = @Description, 
                                         Deadline = @Deadline, AssignedTaskId = @AssignedTaskId 
                                     WHERE Id = @Id"

        ' Execute the update
        Using conn As New SqlConnection(connectionString)
            Dim updateCmd As New SqlCommand(updateQuery, conn)
            updateCmd.Parameters.AddWithValue("@Title", title)
            updateCmd.Parameters.AddWithValue("@Priority", priority)
            updateCmd.Parameters.AddWithValue("@StatusId", statusId)

            ' Handle Description safely
            If String.IsNullOrEmpty(description) Then
                updateCmd.Parameters.AddWithValue("@Description", DBNull.Value)
            Else
                updateCmd.Parameters.AddWithValue("@Description", description)
            End If

            ' Handle Deadline safely
            If deadline Is DBNull.Value OrElse deadline Is Nothing Then
                updateCmd.Parameters.AddWithValue("@Deadline", DBNull.Value) ' Use NULL if no deadline
            Else
                updateCmd.Parameters.AddWithValue("@Deadline", Convert.ToDateTime(deadline)) ' Ensure it's DateTime
            End If

            ' Handle AssignedTaskId safely
            If assignedTaskId Is DBNull.Value OrElse assignedTaskId Is Nothing Then

                updateCmd.Parameters.AddWithValue("@AssignedTaskId", DBNull.Value) ' Use NULL if no assigned task
            Else
                updateCmd.Parameters.AddWithValue("@AssignedTaskId", Convert.ToInt32(assignedTaskId)) ' Ensure it's integer
            End If

            updateCmd.Parameters.AddWithValue("@Id", taskId) ' Use TaskId for the WHERE clause

            ' Open the connection and execute the command
            conn.Open()
            updateCmd.ExecuteNonQuery()
            conn.Close()
        End Using
        Dim query As String = "SELECT [Id], [CreateDate], [Title], [Priority], [StatusId], [Description], [Deadline], [AssignedTaskId] FROM [TMSDB].[dbo].[Tasks]"
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(query, conn)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            conn.Open()
            da.Fill(dt)
            conn.Close()

            ' Store the dataset in session (if necessary)
            Session("TasksDs") = dt
        End Using
        e.Cancel = True
        TasksGridView.CancelEdit()
    End Sub
End Class
