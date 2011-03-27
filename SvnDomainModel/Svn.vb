Imports System.ComponentModel

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
        Private mRevision As Integer
        Private mAuthor As String
        Private mDate As Date
        Private mPaths As String
        Private mAllPaths As New List(Of String)
        Private mMessage As String
        Private mFaults As String
        Private mMerge As Boolean

#Region "LogEntry Public Properties"
        Public Property Revision() As Integer
            Get
                Return mRevision
            End Get
            Set(ByVal value As Integer)
                mRevision = value
            End Set
        End Property
        Public Property Merge() As Boolean
            Get
                Return mMerge
            End Get
            Set(ByVal value As Boolean)
                mMerge = value
            End Set
        End Property
        Public Property Faults() As String
            Get
                Return mFaults
            End Get
            Set(ByVal value As String)
                mFaults = value
            End Set
        End Property
        Public Property Author() As String
            Get
                Return mAuthor
            End Get
            Set(ByVal value As String)
                mAuthor = value
            End Set
        End Property
        Public Property ChangeDate() As String
            Get
                Return mDate
            End Get
            Set(ByVal value As String)
                mDate = value
            End Set
        End Property
        Public Property Paths() As String
            Get
                Return mPaths
            End Get
            Set(ByVal value As String)
                mPaths = value
            End Set
        End Property
        Public Property AllPaths() As List(Of String)
            Get
                Return mAllPaths
            End Get
            Set(ByVal value As List(Of String))
                mAllPaths = value
            End Set
        End Property
        Public Property Message() As String
            Get
                Return mMessage
            End Get
            Set(ByVal value As String)
                mMessage = value
            End Set
        End Property
#End Region
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
    ''' Gets all the changes between the 2 branchs.
    ''' </summary>
    Public Function GetChanges() As BindingList(Of LogEntry)
        Dim LogOutPut = GetSvnLogOutput()

        Dim Logs As New BindingList(Of LogEntry)
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
    ''' Runs the log command against the branch and trunck assigned when this class was created.
    ''' </summary>
    ''' <returns>Returns an XElement that contains all the changes between the revisions of the two branches.</returns>
    Private Function GetSvnLogOutput() As XElement
        Dim p = CreateProcess("mergeinfo " + mTrunckPath + " " + mBranchPath + " --show-revs eligible")
        p.Start()

        Dim MergableRevisions = p.StandardOutput.ReadToEnd.Replace(Environment.NewLine, " ").Split(" ")
        Dim RevisionInfo = (From rev In MergableRevisions Select "-" + rev
                            Take MergableRevisions.Count - 1).Aggregate(Function(revs, rev) revs + " " + rev)

        p = CreateProcess("log " + mTrunckPath + " -v --xml " + RevisionInfo)
        p.Start()
        Return XElement.Parse(p.StandardOutput.ReadToEnd)
    End Function

    ''' <summary>
    ''' Creates a process to run an svn command on.
    ''' </summary>
    ''' <param name="arguments">Arguments to the command you wish to run.</param>
    Private Function CreateProcess(ByVal arguments As String) As Process
        Dim p As New Process
        p.StartInfo.Arguments = arguments
        p.StartInfo.FileName = mSvnPath

        p.StartInfo.UseShellExecute = False
        p.StartInfo.CreateNoWindow = True
        p.StartInfo.RedirectStandardInput = True
        p.StartInfo.RedirectStandardError = True
        p.StartInfo.RedirectStandardOutput = True

        Return p
    End Function

End Class
