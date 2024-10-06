Imports System.Data.SqlClient
Imports System.IO
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

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

    Protected Sub btnDownloadReport_Click(sender As Object, e As EventArgs)
        Dim report As New ReportDocument()
        report.Load(Server.MapPath("~/TasksReport.rpt"))

        ' Set the database connection info (same as your ASPxGridView)
        Dim connectionInfo As New ConnectionInfo()
        connectionInfo.ServerName = "(localdb)\MSSQLLocalDB"
        connectionInfo.DatabaseName = "TMSDB"
        connectionInfo.IntegratedSecurity = True

        ' Apply connection info to all tables in the report
        For Each table As Table In report.Database.Tables
            Dim logOnInfo As TableLogOnInfo = table.LogOnInfo
            logOnInfo.ConnectionInfo = connectionInfo
            table.ApplyLogOnInfo(logOnInfo)
        Next

        ' Ensure that the report is using the stored procedure
        report.SetParameterValue("@UserId", Convert.ToInt32(Session("UserId")))

        ' Export the report to a generic Stream
        Using stream As Stream = report.ExportToStream(ExportFormatType.PortableDocFormat)
            ' Convert the Stream to a byte array
            Using memoryStream As New MemoryStream()
                stream.CopyTo(memoryStream)
                Dim byteArray() As Byte = memoryStream.ToArray()

                ' Send the PDF to the client for download
                Response.Clear()
                Response.Buffer = True
                Response.ContentType = "application/pdf"
                Response.AddHeader("content-disposition", "attachment;filename=TaskReport.pdf")
                Response.BinaryWrite(byteArray)
                Response.Flush()
                Response.End()
            End Using
        End Using
    End Sub
End Class