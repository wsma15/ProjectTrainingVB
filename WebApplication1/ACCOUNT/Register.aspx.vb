Imports System.Data.SqlClient

Partial Class Register
    Inherits System.Web.UI.Page
    Protected ReadOnly connectionString As String = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TMSDB;Integrated Security=True"

    Protected Sub btnRegister_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Initialize variables
        Dim username As String = If(txtUsername IsNot Nothing, txtUsername.Text.Trim(), String.Empty)
        Dim password As String = If(txtPassword IsNot Nothing, txtPassword.Text.Trim(), String.Empty)
        Dim confirmPassword As String = If(txtConfirmPassword IsNot Nothing, txtConfirmPassword.Text.Trim(), String.Empty)
        Dim Name As String = If(txtName IsNot Nothing, txtName.Text.Trim(), String.Empty)

        ' Check if the dropdown has a selected item
        Dim roleId As Integer
        If ddlManager Is Nothing OrElse ddlManager.SelectedItem Is Nothing Then
            lblErrorMessage.Text = "Role must be selected."
            lblErrorMessage.Visible = True
            lblSuccessMessage.Visible = False
            Return
        Else
            roleId = Convert.ToInt32(ddlManager.SelectedItem.Value)
        End If

        ' Check for empty fields
        If String.IsNullOrWhiteSpace(username) Then
            lblErrorMessage.Text = "Username is required."
            lblErrorMessage.Visible = True
            lblSuccessMessage.Visible = False
            Return
        End If

        If String.IsNullOrWhiteSpace(password) Then
            lblErrorMessage.Text = "Password is required."
            lblErrorMessage.Visible = True
            lblSuccessMessage.Visible = False
            Return
        End If

        If String.IsNullOrWhiteSpace(confirmPassword) Then
            lblErrorMessage.Text = "Confirm Password is required."
            lblErrorMessage.Visible = True
            lblSuccessMessage.Visible = False
            Return
        End If

        ' Validate Name
        If String.IsNullOrWhiteSpace(Name) Then
            lblErrorMessage.Text = "Name is required."
            lblErrorMessage.Visible = True
            lblSuccessMessage.Visible = False
            Return
        End If

        ' Check if the name has at least two words
        Dim nameParts As String() = Name.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
        If nameParts.Length < 2 Then
            lblErrorMessage.Text = "Name must contain at least two words."
            lblErrorMessage.Visible = True
            lblSuccessMessage.Visible = False
            Return
        End If

        ' Validate if the name contains only letters and spaces
        If Not IsValidName(Name) Then
            lblErrorMessage.Text = "Name can only contain letters and spaces."
            lblErrorMessage.Visible = True
            lblSuccessMessage.Visible = False
            Return
        End If

        ' Validate username length
        If username.Length < 6 Then
            lblErrorMessage.Text = "Username must be at least 6 characters long."
            lblErrorMessage.Visible = True
            lblSuccessMessage.Visible = False
            Return
        End If

        ' Validate password length
        If password.Length < 6 Then
            lblErrorMessage.Text = "Password must be at least 6 characters long."
            lblErrorMessage.Visible = True
            lblSuccessMessage.Visible = False
            Return
        End If

        ' Validate password criteria
        If Not IsValidPassword(password) Then
            lblErrorMessage.Text = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character."
            lblErrorMessage.Visible = True
            lblSuccessMessage.Visible = False
            Return
        End If

        ' Validate password match
        If password <> confirmPassword Then
            lblErrorMessage.Text = "Passwords do not match."
            lblErrorMessage.Visible = True
            lblSuccessMessage.Visible = False
            Return
        End If

        ' Hash the password before storing
        password = PasswordHelper.HashPassword(password)

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
            Dim insertCmd As New SqlCommand("INSERT INTO Users (Name, Username, Password, RoleId) VALUES (@Name, @Username, @Password, @RoleId)", connection)
            insertCmd.Parameters.AddWithValue("@Name", Name)
            insertCmd.Parameters.AddWithValue("@Username", username)
            insertCmd.Parameters.AddWithValue("@Password", password) ' Hashed password
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

    Private Function IsValidPassword(password As String) As Boolean
        Dim hasUpperCase As Boolean = password.Any(Function(c) Char.IsUpper(c))
        Dim hasLowerCase As Boolean = password.Any(Function(c) Char.IsLower(c))
        Dim hasDigit As Boolean = password.Any(Function(c) Char.IsDigit(c))
        Dim hasSpecialChar As Boolean = password.Any(Function(c) Not Char.IsLetterOrDigit(c))

        Return hasUpperCase AndAlso hasLowerCase AndAlso hasDigit AndAlso hasSpecialChar
    End Function

    Private Function IsValidName(name As String) As Boolean
        ' Check if the name contains only letters and spaces
        Return name.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c))
    End Function
End Class
