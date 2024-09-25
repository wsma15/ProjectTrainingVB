Imports System.Data.SqlClient

Public Class login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Protected Sub Loginbtn_Click(sender As Object, e As EventArgs)
        Try
            Dim username As String = Usernametxt.Text.ToUpper()
            Dim password As String = Passwordtxt.Text
            Dim connectionString As String = "Server=(localdb)\MSSQLLocalDB;Database=TrainingApp;Trusted_Connection=True;"

            Dim query As String = "SELECT password, roleid FROM Users WHERE username = @Username"

            Using connection As New SqlConnection(connectionString)
                Dim command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Username", username)
                connection.Open()

                Dim reader As SqlDataReader = command.ExecuteReader()


                If reader.Read() Then
                    Dim storedHash As String = reader("password").ToString()
                    Dim roleId As Integer = Convert.ToInt32(reader("roleid"))

                    If storedHash = PasswordHelper.HashPassword(password) Then
                        Select Case roleId
                            Case 1
                                Response.Redirect("Dashboard.aspx") ' Admin
                                Exit Select
                            Case 2
                                Response.Redirect("UserDashboard.aspx") ' User
                                Exit Select
                            Case Else
                                Response.Write("Access denied.") ' Handle other roles if necessary
                                Exit Select
                        End Select
                    Else
                        Response.Write("Incorrect Username or Password.")
                    End If
                Else
                    Response.Write("Incorrect Username or Password.")
                End If
            End Using
        Catch ex As Exception
            Response.Write("An error occurred. Please try again later." + ex.ToString)
        End Try
    End Sub

End Class