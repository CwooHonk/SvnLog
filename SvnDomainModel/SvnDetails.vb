
Public Class SvnDetails
    Public Property Changes As List(Of Svn.LogEntry)
    Public Property TrunckPath As String
    Public Property BranchPath As String
    Public Property SvnUser As String
    Public Property SvnPassword As String

    Public Const SvnCookieName = "SvnInfo"
    Public Const SvnCookieUserName = "SvnUser"
    Public Const SvnCookieUserPassword = "SvnPassword"

    Public Sub New()
        Changes = New List(Of Svn.LogEntry)
    End Sub
End Class
