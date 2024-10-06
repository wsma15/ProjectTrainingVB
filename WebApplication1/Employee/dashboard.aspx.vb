Imports System.Data.SqlClient

Public Class dashboard2
    Inherits System.Web.UI.Page

    Public Function UserIsInRole(roleName As String) As Boolean
        If Session("RoleName") IsNot Nothing Then
            Dim currentRole As String = Session("RoleName").ToString().ToLower()
            Return currentRole.Equals(roleName.ToLower())
        End If
        Return False
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not UserIsInRole("Employee") Then
            Response.Redirect("~/Log/Login.aspx?ReturnUrl=" & Server.UrlEncode(Request.RawUrl))
        End If

    End Sub
End Class