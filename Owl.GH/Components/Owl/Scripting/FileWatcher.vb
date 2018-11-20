Imports System.IO
Imports Grasshopper.Kernel.Parameters

Public Class FileWatcher
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("File Watch", "Watcher", "Watch file for changes", "Owl", "Scripting")
        Me.Message = DateTime.Now.ToLongTimeString
    End Sub

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_06
		End Get
	End Property

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{E5BE32D4-6868-4A1C-A190-D46A5F046979}")
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_FilePath, "Directory", "D", "Directory to watch", GH_ParamAccess.tree)
        Me.Params.Input(0).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddTextParameter("Path", "P", "Path will be outputted here whenever a change happens", GH_ParamAccess.item)
    End Sub

    Public Overrides Sub RemovedFromDocument(document As GH_Document)
        MyBase.RemovedFromDocument(document)
        RemoveWatcher()
    End Sub

    Dim watcher As FileSystemWatcher = Nothing
    Dim filename As String = ""

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Me.Message = DateTime.Now.ToLongTimeString

        Dim FilePath As String = GetInput(DA)

        If FilePath = "" Then
            If watcher IsNot Nothing Then
                RemoveWatcher()
                filename = ""
            End If
        Else
            If FilePath <> filename Then
                RemoveWatcher()
                filename = FilePath
                AddWatcher(filename)
                DA.SetData(0, filename)
            Else
                If IsFileReady(filename) Then
                    DA.SetData(0, filename)
                Else
                    DA.AbortComponentSolution()
                End If
            End If
        End If
    End Sub

    Private Function GetInput(DA As IGH_DataAccess) As String
        Dim ghs As New GH_Structure(Of GH_String)
        If Not DA.GetDataTree(0, ghs) Then Return ""
        Dim str As GH_String = TryCast(ghs.AllData(True)(0), GH_String)
        If str Is Nothing Then Return ""
        Return str.Value
    End Function

    Private Sub AddWatcher(FilePath As String)
        Dim pth As String = Path.GetDirectoryName(FilePath)
        Dim fname As String = Path.GetFileName(FilePath)

        Dim watch As FileSystemWatcher = Nothing
        watch = New FileSystemWatcher(pth)

        AddHandler watch.Created, AddressOf FileChanged
        AddHandler watch.Changed, AddressOf FileChanged
        AddHandler watch.Deleted, AddressOf FileChanged
        AddHandler watch.Renamed, AddressOf FileChanged

        watch.EnableRaisingEvents = True

        watcher = watch
    End Sub

    Private Sub RemoveWatcher()
        If watcher Is Nothing Then Return

        RemoveHandler watcher.Created, AddressOf FileChanged
        RemoveHandler watcher.Changed, AddressOf FileChanged
        RemoveHandler watcher.Deleted, AddressOf FileChanged
        RemoveHandler watcher.Renamed, AddressOf FileChanged
        watcher.Dispose()
    End Sub

    Dim act As New Action(AddressOf Exp)

    Private Sub EvalComponent()
        Rhino.RhinoApp.MainApplicationWindow.Invoke(act)
    End Sub

    Private Sub Exp()
        ExpireSolution(True)
        Message = DateTime.Now.ToLongTimeString
    End Sub

    Private Sub FileChanged(sender As Object, e As FileSystemEventArgs)
        If Path.GetFileName(e.FullPath) <> Path.GetFileName(filename) Then Return

        Dim flags As WatcherChangeTypes = WatcherChangeTypes.Created + WatcherChangeTypes.Renamed + WatcherChangeTypes.Changed

        If e.ChangeType & flags Then
            If IsFileReady(filename) Then
                EvalComponent()
            End If

            If e.ChangeType = WatcherChangeTypes.Deleted Then
                RemoveWatcher()
                EvalComponent()
            End If
        End If

    End Sub

    Private Function IsFileReady(Path As String) As Boolean
        Try
            Using str = File.Open(Path, FileMode.Open, FileAccess.ReadWrite)
                Return True
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

End Class
