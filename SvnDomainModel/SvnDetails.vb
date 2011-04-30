
Public Class SvnDetails
    Public Property Changes As List(Of Svn.LogEntry)
    Public Property TrunckPath As String
    Public Property BranchPath As String

    Public Sub New()
        Changes = New List(Of Svn.LogEntry)
    End Sub
End Class
