Imports System.Configuration
Imports System.IO

Class SvnLog

    Private mSvnPath As String

    Public Sub New()
        InitializeComponent()

        mSvnPath = ConfigurationManager.AppSettings("SvnPath")
        If Not File.Exists(mSvnPath) Then
            MsgBox("SVN.exe could not be found. Please check the path set in the app.config.", MsgBoxStyle.Critical)
            Application.Current.Shutdown()
        End If

    End Sub

    Private Sub ButtonGo_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ButtonGo.Click
        Dim TrunckPath = TextBoxTrunck.Text
        Dim BranchPath = TextBoxBranch.Text

        Dim Log As New SvnDomainModel.Svn(mSvnPath, TrunckPath, BranchPath)

        DataGridLog.ItemsSource = Log.GetChanges

        TabItemSvnOutput.IsEnabled = True
        TabItemSvnOutput.IsSelected = True
    End Sub

End Class
