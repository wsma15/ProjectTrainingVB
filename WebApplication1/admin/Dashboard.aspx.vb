Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports DevExpress.Web
Imports System.Data.SqlClient
Imports System.IO

Public Class Dashboard
    Inherits System.Web.UI.Page
    Protected ReadOnly connectionString As String = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TrainingApp;Integrated Security=True"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindUsersGridView()
            BindRolesGridView()
            BindTasksGridView()
        End If
    End Sub

    Protected Sub UsersbtnExportPDF_Click(sender As Object, e As EventArgs)
        Dim dataTable As DataTable = GetUsersDataFromProcedure()
        If dataTable Is Nothing OrElse dataTable.Rows.Count = 0 Then
            System.Diagnostics.Debug.WriteLine("The DataTable is empty. No data returned from the procedure.")
            Return ' Exit if there's no data
        End If
        For Each row As DataRow In dataTable.Rows
            System.Diagnostics.Debug.WriteLine("Row: " & row("username").ToString() & ", " & row("roleid").ToString())
        Next
        Dim rptDoc As New ReportDocument()
        Dim reportPath As String = Server.MapPath("~/UsersReport.rpt")
        rptDoc.Load(reportPath)
        rptDoc.SetDataSource(dataTable)
        Dim directoryPath As String = Server.MapPath("~/Reports")
        Dim filePath As String = Path.Combine(directoryPath, "UsersReport.pdf")
        If Not Directory.Exists(directoryPath) Then
            Directory.CreateDirectory(directoryPath)
        End If
        Dim exportOptions As New ExportOptions()
        Dim diskOptions As New DiskFileDestinationOptions()
        Dim pdfOptions As New PdfRtfWordFormatOptions()
        diskOptions.DiskFileName = filePath
        exportOptions.ExportDestinationType = ExportDestinationType.DiskFile
        exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat
        exportOptions.DestinationOptions = diskOptions
        exportOptions.FormatOptions = pdfOptions
        Try
            rptDoc.Export(exportOptions)
        Catch ex As Exception
            Throw New Exception("An error occurred while exporting the report.", ex)
        End Try
        Dim file As New FileInfo(filePath)
        If file.Exists Then
            Response.Clear()
            Response.Buffer = True
            Response.AddHeader("content-disposition", "attachment;filename=" & file.Name)
            Response.AddHeader("content-length", file.Length.ToString())
            Response.ContentType = "application/pdf"
            Response.WriteFile(file.FullName)
            Response.End()
        End If
    End Sub

    Protected Sub RolesbtnExportPDF_Click(sender As Object, e As EventArgs)
        Dim dataTable As DataTable = GetRolesDataFromProcedure()
        Dim rptDoc As New ReportDocument()
        Dim reportPath As String = Server.MapPath("~/RolesReport.rpt")
        rptDoc.Load(reportPath)
        rptDoc.SetDataSource(dataTable)
        Dim directoryPath As String = Server.MapPath("~/Reports")
        Dim filePath As String = Path.Combine(directoryPath, "RolesReport.pdf")
        If Not Directory.Exists(directoryPath) Then
            Directory.CreateDirectory(directoryPath)
        End If
        Dim exportOptions As New ExportOptions()
        Dim diskOptions As New DiskFileDestinationOptions()
        Dim pdfOptions As New PdfRtfWordFormatOptions()
        diskOptions.DiskFileName = filePath
        exportOptions.ExportDestinationType = ExportDestinationType.DiskFile
        exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat
        exportOptions.DestinationOptions = diskOptions
        exportOptions.FormatOptions = pdfOptions
        Try
            rptDoc.Export(exportOptions)
        Catch ex As Exception
            Throw New Exception("An error occurred while exporting the report.", ex)
        End Try
        Dim file As New FileInfo(filePath)
        If file.Exists Then
            Response.Clear()
            Response.Buffer = True
            Response.AddHeader("content-disposition", "attachment;filename=" & file.Name)
            Response.AddHeader("content-length", file.Length.ToString())
            Response.ContentType = "application/pdf"
            Response.WriteFile(file.FullName)
            Response.Flush()
            Response.SuppressContent = True
        End If
    End Sub

    Private Sub BindUsersGridView()
        Dim rptdoc As New ReportDocument()
        rptdoc.Load(Server.MapPath("~/UsersReport.rpt"))
        Dim query As String = "SELECT Id, Name, Username, Password, RoleId, ManagerId FROM [dbo].[Users]"
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
        Dim query As String = "SELECT [Id],[CreateId],[Title],[Priority],[Status],[Description],[Deadline]FROM [TrainingApp].[dbo].[Tasks]"
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
            System.Diagnostics.Debug.WriteLine("Error: " & ex.Message)
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

            ' Check if RoleId is missing
            If roleId Is Nothing Then
                e.Cancel = True
                UsersGridView.JSProperties("cpMessage") = "Role ID is missing."
                Return
            End If

            ' Validate username and password
            If Not IsValidUsername(username) Then
                e.Cancel = True
                UsersGridView.JSProperties("cpMessage") = "Username must be at least 6 characters long and contain no spaces or special characters."
                Return
            End If

            If Not IsValidPassword(password) Then
                e.Cancel = True
                UsersGridView.JSProperties("cpMessage") = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character."
                Return
            End If

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
        Dim rptdoc As New ReportDocument()
        rptdoc.Load(Server.MapPath("~/UsersReport.rpt"))
        Dim query As String = "SELECT [Id],[Name],[Username],[Password],[RoleId],[ManagerId] FROM [TrainingApp].[dbo].[Users]"
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
        Dim username As String = e.Keys("Username").ToString()
        Dim name As String = e.NewValues("Name").ToString()
        Dim password As String = e.NewValues("Password").ToString()
        Dim roleId As Integer = Convert.ToInt32(e.NewValues("RoleId"))
        Dim managerId As Integer? = If(e.NewValues("ManagerId") IsNot Nothing, Convert.ToInt32(e.NewValues("ManagerId")), CType(Nothing, Integer?))

        ' Create the SQL update query
        Dim query As String = "UPDATE [dbo].[Users] SET Name = @Name, Password = @Password, RoleId = @RoleId, ManagerId = @ManagerId WHERE Username = @Username"

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
        Dim rptdoc As New ReportDocument()
        rptdoc.Load(Server.MapPath("~/UsersReport.rpt"))

        Dim selectQuery As String = "SELECT [Id],[Name],[Username],[Password],[RoleId],[ManagerId] FROM [TrainingApp].[dbo].[Users]"
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

    Protected Sub UsersGridView_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs)
        Dim username As String = e.Keys("Username").ToString()

        ' Check if the user is a manager of any other users
        Dim checkManagerQuery As String = "SELECT COUNT(*) FROM [dbo].[Users] WHERE ManagerId = (SELECT Id FROM [dbo].[Users] WHERE Username = @Username)"
        Dim isManager As Boolean = False
        Using conn As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(checkManagerQuery, conn)
            cmd.Parameters.AddWithValue("@Username", username)
            conn.Open()
            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            isManager = (count > 0) ' If count is greater than 0, the user is a manager
            conn.Close()
        End Using

        If isManager Then
            ' Notify the user that the deletion cannot be performed
            e.Cancel = True
            ' You can also set a message to display to the user, for example:
            UsersGridView.JSProperties("cpErrorMessage") = "Cannot delete this user because they are a manager of another user."
        Else
            ' Proceed with the deletion
            Dim query As String = "DELETE FROM [dbo].[Users] WHERE Username = @Username"
            Using conn As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Username", username)
                conn.Open()
                cmd.ExecuteNonQuery()
                conn.Close()
            End Using

            e.Cancel = True
            Dim rptdoc As New ReportDocument()
            rptdoc.Load(Server.MapPath("~/UsersReport.rpt"))
            query = "SELECT [Id],[Name],[Username],[Password],[RoleId],[ManagerId] FROM [TrainingApp].[dbo].[Users]"
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
        End If
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

    End Sub

    Protected Sub TasksGridView_RowUpdating(sender As Object, e As Data.ASPxDataUpdatingEventArgs)

    End Sub

    Protected Sub TasksGridView_RowDeleting(sender As Object, e As Data.ASPxDataDeletingEventArgs)

    End Sub

    Protected Sub TasksGridView_RowValidating(sender As Object, e As Data.ASPxDataValidationEventArgs)

    End Sub
End Class