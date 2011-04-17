''' <summary>
''' A class to query svn.
''' </summary>
Public Class Svn

    Private mSvnPath As String
    Private mTrunckPath As String
    Private mBranchPath As String

    ''' <summary>
    ''' A class to store the individual changes return by the svn log command.
    ''' </summary>
    Public Class LogEntry
        Public Property Revision As Integer
        Public Property Author As String
        Public Property ChangeDate As Date
        Public Property Paths As String
        Public Property AllPaths As New List(Of String)
        Public Property Message As String
        Public Property Faults As String
        Public Property Merge As Boolean
    End Class

    ''' <summary>
    ''' Creates a class that can be used to query svn.
    ''' </summary>
    ''' <param name="svnPath">Path to the SVN executable.</param>
    ''' <param name="trunckPath">Path to the trunck of  the repro.</param>
    ''' <param name="branchPath">Path to the branch in the repro.</param>
    Public Sub New(ByVal svnPath As String, ByVal trunckPath As String, ByVal branchPath As String)
        mSvnPath = svnPath
        mTrunckPath = trunckPath
        mBranchPath = branchPath
    End Sub

    ''' <summary>
    ''' Given a list of all possible revisions to select, and the revisions that have been selected creates a set of revisions
    ''' ranges that can be passed to a merge command.
    ''' </summary>
    ''' <param name="allRevisions">All the revisions that can be selected.</param>
    ''' <param name="selectedRevisions">All the revisions that the user has selected.</param>
    ''' <returns>Returns an array of revisions ranges that represents the given selected revisions.</returns>
    Public Function GetRevisionRange(ByVal allRevisions As IEnumerable(Of String), ByVal selectedRevisions As IEnumerable(Of String)) As String()
        Dim Revisions As New List(Of String)

        Dim RangeStart As String = String.Empty
        Dim RangeEnd As String = String.Empty
        Dim Jumps = 0
        For position = 0 To selectedRevisions.Count - 1
            If allRevisions(position + Jumps) = selectedRevisions(position) Then
                If RangeStart = String.Empty Then
                    RangeStart = selectedRevisions(position)
                Else
                    RangeEnd = selectedRevisions(position)
                End If
            End If

            If allRevisions(position + Jumps) <> selectedRevisions(position) Then
                AddRange(RangeStart, RangeEnd, Revisions)

                RangeStart = selectedRevisions(position)
                RangeEnd = String.Empty
                Jumps += 1
            End If

            If position = selectedRevisions.Count - 1 Then
                AddRange(RangeStart, RangeEnd, Revisions)
            End If
        Next

        Return Revisions.ToArray
    End Function

    ''' <summary>
    ''' A given revisions, or a revision range, to a list of revisions.
    ''' </summary>
    ''' <param name="rangeStart">The revisions / start of revision range.</param>
    ''' <param name="rangeEnd">The end of the revisions range.</param>
    ''' <param name="revisions">The list of revisions this revisions / range will be added too.</param>
    Private Sub AddRange(ByVal rangeStart As String, ByVal rangeEnd As String, ByVal revisions As List(Of String))
        If RangeEnd = String.Empty Then
            Revisions.Add(RangeStart)
        Else
            Revisions.Add(RangeStart + ":" + RangeEnd)
        End If
    End Sub

    ''' <summary>
    ''' Gets all the changes between the 2 branchs.
    ''' </summary>
    Public Function GetChanges() As List(Of LogEntry)
        Dim LogOutPut = GetSvnLogOutput()

        Dim Logs As New List(Of LogEntry)
        For Each log In LogOutPut.Elements
            Dim entry As New LogEntry
            entry.Merge = True
            entry.Revision = log.Attribute("revision").Value
            entry.Author = log.Element("author").Value
            entry.ChangeDate = DateTime.Parse(log.Element("date").Value)
            'TODO:Need to parse fault here.
            entry.Message = log.Element("msg").Value

            For Each path In log.Element("paths").Elements
                'TODO:Could record added paths here.
                entry.AllPaths.Add(path.Value)
            Next
            entry.Paths = entry.AllPaths.Aggregate(Function(All, Current) All + vbNewLine + Current)
            Logs.Add(entry)
        Next

        Return Logs
    End Function

    ''' <summary>
    ''' Gets the location on the harddrive of a repository.
    ''' </summary>
    Public Function GetBranchLocation() As String
        Dim Location = String.Empty

        Using p = New SvnProcess("info " + mBranchPath + " --xml", mSvnPath)
            Dim BranchXml = XElement.Parse(p.ExecuteCommand)
            Dim Reprository = BranchXml.Elements.First.Elements.Where(Function(node) node.Name = "repository").FirstOrDefault
            Location = Reprository.Elements.Where(Function(node) node.Name = "root").FirstOrDefault.Value
        End Using

        Return Location
    End Function


    Public Sub MergeChanges(ByVal revisionRange As IEnumerable(Of String), ByVal branchPath As String, ByVal trunckPath As String)
        'TODO: Do I need branch location?
        Dim CheckOutFolder = IO.Path.Combine(Environment.CurrentDirectory, IO.Path.GetRandomFileName)
        Dim Revisions = String.Join(" ", revisionRange.Select(Function(x) "-r " + x).ToArray)

        CheckOut(branchPath, CheckOutFolder)
        Merge(trunckPath, CheckOutFolder, Revisions)
        Commit()

        IO.Directory.Delete(CheckOutFolder)
    End Sub


    Private Sub Merge(ByVal trunckPath As String, ByVal workingDirectory As String, ByVal revisions As String)

        Using p = New SvnProcess("merge " + trunckPath + " " + revisions + " " + workingDirectory, mSvnPath)
            p.ExecuteCommand()
        End Using

    End Sub


    Private Sub Commit()
        Using p = New SvnProcess("commit -m""Merge""", mSvnPath)
            p.ExecuteCommand()
        End Using
    End Sub

    Private Sub CheckOut(ByVal branchPath As String, ByVal checkOutFolder As String)
        'TODO: This will need testing, is this giving the correct hosting path?
        'Also, how to log errors if its not?
        Using p = New SvnProcess("checkout " + branchPath + " " + checkOutFolder, mSvnPath)
            p.ExecuteCommand()
        End Using
    End Sub


    ''' <summary>
    ''' Runs the log command against the branch and trunck assigned when this class was created.
    ''' </summary>
    ''' <returns>Returns an XElement that contains all the changes between the revisions of the two branches.</returns>
    Private Function GetSvnLogOutput() As XElement
        Dim Output As XElement
        Dim RevisionInfo As String

        Using p = New SvnProcess("mergeinfo " + mTrunckPath + " " + mBranchPath + " --show-revs eligible", mSvnPath)
            Dim MergableRevisions = p.ExecuteCommand.Replace(Environment.NewLine, " ").Split(" ")
            RevisionInfo = (From rev In MergableRevisions Select "-" + rev
                                Take MergableRevisions.Count - 1).Aggregate(Function(revs, rev) revs + " " + rev)
        End Using

        Using p = New SvnProcess("log " + mTrunckPath + " -v --xml " + RevisionInfo, mSvnPath)
            Output = XElement.Parse(p.ExecuteCommand)
        End Using

        Return Output
    End Function

    ''' <summary>
    ''' A class that creates a process to query svn.
    ''' </summary>
    Public Class SvnProcess
        Implements IDisposable

        Private mProcess As Process

        ''' <summary>
        ''' An exception assoiated to any svn errors.
        ''' </summary>
        Public Class SvnException
            Inherits Exception

            Public Property SvnError As String
        End Class

        ''' <summary>
        ''' A creates a process that can be used to query svn.
        ''' </summary>
        ''' <param name="arguments">Query command.</param>
        ''' <param name="svnPath">Path to the svn execeutable.</param>
        Public Sub New(ByVal arguments As String, ByVal svnPath As String)
            mProcess = New Process
            With mProcess
                .StartInfo.Arguments = arguments
                .StartInfo.FileName = svnPath

                .StartInfo.UseShellExecute = False
                .StartInfo.CreateNoWindow = True
                .StartInfo.RedirectStandardInput = True
                .StartInfo.RedirectStandardError = True
                .StartInfo.RedirectStandardOutput = True
            End With
        End Sub

        ''' <summary>
        ''' Executes the command.
        ''' </summary>
        ''' <returns>A string of the output generated by the svn command.</returns>
        ''' <remarks>Raise an SvnExcetion if somthing bad happens.</remarks>
        Public Function ExecuteCommand() As String
            mProcess.Start()

            Dim Output = mProcess.StandardOutput.ReadToEnd
            Dim Errors = mProcess.StandardError.ReadToEnd

            If Errors <> String.Empty Then
                Throw New SvnException() With {.SvnError = Errors}
            End If

            Return Output
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                mProcess.Dispose()
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Class
