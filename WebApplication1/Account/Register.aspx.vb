Imports System.Data.SqlClient

Partial Class Register
    Inherits System.Web.UI.Page
    Protected ReadOnly connectionString As String = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"

    Protected Sub btnRegister_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim username As String = txtUsername.Text
        Dim password As String = PasswordHelper.HashPassword(txtPassword.Text)
        Dim confirmPassword As String = PasswordHelper.HashPassword(txtConfirmPassword.Text)
        Dim Name As String = txtName.Text
        Dim roleId As Integer = Convert.ToInt32(ddlManager.SelectedItem.Value)

        ' Validate password match
        If password <> confirmPassword Then
            lblErrorMessage.Text = "Passwords do not match."
            lblErrorMessage.Visible = True
            lblSuccessMessage.Visible = False
            Return
        End If

        ' Connection string

        Using connection As New SqlConnection(connectionString)
            connection.Open()

            ' Check if the username already exists
            Dim checkUserCmd As New SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", connection)
            checkUserCmd.Parameters.AddWithValue("@Username", username)
            Dim userExists As Integer = Convert.ToInt32(checkUserCmd.ExecuteScalar())

            If userExists > 0 Then
                lblErrorMessage.Text = "Username already exists."
                lblErrorMessage.Visible = True
                lblSuccessMessage.Visible = False
                Return
            End If

            ' Insert new user into the Users table
            Dim insertCmd As New SqlCommand("INSERT INTO Users (Name,Username, Password, RoleId) VALUES (@Name, @Username, @Password, @RoleId)", connection)
            insertCmd.Parameters.AddWithValue("@Name", Name)
            insertCmd.Parameters.AddWithValue("@Username", username)
            insertCmd.Parameters.AddWithValue("@Password", password) ' Consider hashing the password
            insertCmd.Parameters.AddWithValue("@RoleId", roleId)

            Dim rowsAffected As Integer = insertCmd.ExecuteNonQuery()

            If rowsAffected > 0 Then
                lblSuccessMessage.Text = "Registration successful!"
                lblSuccessMessage.Visible = True
                lblErrorMessage.Visible = False
                ' Optionally, clear the fields
                txtUsername.Text = String.Empty
                txtPassword.Text = String.Empty
                txtConfirmPassword.Text = String.Empty
                ddlManager.SelectedIndex = -1
            Else
                lblErrorMessage.Text = "Registration failed. Please try again."
                lblErrorMessage.Visible = True
                lblSuccessMessage.Visible = False
            End If
        End Using
    End Sub
End Class
