Imports System
Imports System.Web.UI
Imports DevExpress.Web
Imports System.Data.SqlClient
Imports WebApplication1

Public Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Protected ReadOnly connectionString As String = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim username As String = Request.Form("txtUsername")
        Dim password As String = PasswordHelper.HashPassword(Request.Form("txtPassword"))

        Using connection As New SqlConnection(connectionString)
            connection.Open()

            ' First query: Get RoleId and Id based on username and password
            Dim query As String = "SELECT RoleId, Id FROM Users WHERE Username = @Username AND Password = @Password"
            Dim command As New SqlCommand(query, connection)
            command.Parameters.AddWithValue("@Username", username)
            command.Parameters.AddWithValue("@Password", password)

            Using reader As SqlDataReader = command.ExecuteReader()
                If reader.Read() Then
                    Dim roleId As Integer = Convert.ToInt32(reader("RoleId"))
                    Dim userId As Integer = Convert.ToInt32(reader("Id"))

                    Session("UserId") = userId

                    reader.Close()

                    Dim roleCommand As New SqlCommand("SELECT Name FROM Roles WHERE Id = @RoleId", connection)
                    roleCommand.Parameters.AddWithValue("@RoleId", roleId)

                    Dim roleName As Object = roleCommand.ExecuteScalar()

                    If roleName IsNot Nothing Then
                        Dim role As String = roleName.ToString().ToLower()

                        ' Store the RoleId and RoleName in session for later use
                        Session("RoleId") = roleId
                        Session("RoleName") = role

                        ' Set authentication cookie
                        Web.Security.FormsAuthentication.SetAuthCookie(username, False)

                        ' Redirect based on the role
                        If role = "admin" Then
                            Response.Redirect("~/Admin/Dashboard.aspx")
                        ElseIf role = "employee" Then
                            Response.Redirect("~/Employee/Dashboard.aspx")
                        ElseIf role = "manager" Then
                            Response.Redirect("~/Manager/Dashboard.aspx")
                        Else
                            ' Handle other roles or invalid role
                            Response.Redirect("~/Unauthorized.aspx") ' Create this page to handle invalid roles
                        End If
                    Else
                        ' Role not found

                        Response.Write("Invalid role for this user.")
                    End If
                Else
                    ' Invalid username or password
                    Response.Write("Invalid username or password.")
                End If
            End Using
        End Using
    End Sub


End Class