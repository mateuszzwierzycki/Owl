Imports System.Windows.Forms
Imports GH_IO.Serialization

Public Class RunProcess
    Inherits OwlComponentBase
    Implements IDisposable

    Dim typedatts As RunProcess_Attributes = Nothing

    Sub New()
        MyBase.New("Run Process", "RunProcess", "Run any application.", "Owl", "Scripting")
        cons = New ScriptOutput
    End Sub

    'Protected Overrides ReadOnly Property Icon As Bitmap
    '    Get
    '        Return My.Resources.icon_04
    '    End Get
    'End Property

    Public Overrides Sub RemovedFromDocument(document As GH_Document)
        If Console IsNot Nothing Then Console.Hide()
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{96E27D02-24E6-42FA-8300-3CD67B5D57EF}")
        End Get
    End Property

    Public Overrides Sub CreateAttributes()
        m_attributes = New RunProcess_Attributes(Me)
        typedatts = m_attributes
    End Sub

    Private Sub SwitchOut()
        getalltext = Not getalltext
    End Sub

    Protected Overrides Sub AppendAdditionalComponentMenuItems(menu As ToolStripDropDown)
        Menu_AppendItem(menu, "Get Output", AddressOf SwitchOut, True, getalltext)
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "Abort Process", AddressOf StopProcess)
        MyBase.AppendAdditionalComponentMenuItems(menu)
    End Sub

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        getalltext = reader.GetBoolean("OutputText")
        Return MyBase.Read(reader)
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        writer.SetBoolean("OutputText", getalltext)
        Return MyBase.Write(writer)
    End Function

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddTextParameter("Application", "P", "Application to run", GH_ParamAccess.item)
        pManager.AddTextParameter("Arguments", "A", "Optional arguments for the application", GH_ParamAccess.list)
        pManager.AddTextParameter("Commands", "C", "Commands write", GH_ParamAccess.list)
        pManager.AddBooleanParameter("Windowed", "W", "Show the window, if there is any", GH_ParamAccess.item, False)
        pManager.AddBooleanParameter("Run", "R", "Run", GH_ParamAccess.item, False)

        Me.Params.Input(1).Optional = True
        Me.Params.Input(2).Optional = True
        Me.Params.Input(3).Optional = True
        Me.Params.Input(4).Optional = True
    End Sub

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Public Overrides Sub AddedToDocument(document As GH_Document)
        MyBase.AddedToDocument(document)
        Me.Message = "Double Click"
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddBooleanParameter("Done", "D", "True when the process is done, even if it fails.", GH_ParamAccess.item)
        pManager.AddTextParameter("Output", "O", "Total output when done, optional. Right click to change.", GH_ParamAccess.item)
    End Sub

    Dim getalltext As Boolean = False

	Dim nn As ProcessScheduler = Nothing
	Dim done As Boolean = False

    Dim cons As ScriptOutput = Nothing

    Friend Property Console As ScriptOutput
        Get
            Return cons
        End Get
        Set(value As ScriptOutput)
            cons = value
        End Set
    End Property

    Dim expcomp As New Action(AddressOf ExpireThis)

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim app As String = ""
        Dim vals As New List(Of String)
        Dim comms As New List(Of String)
        Dim win As Boolean = False
        Dim run As Boolean = False

        comms.Clear()
        vals.Clear()

        If Not DA.GetData(0, app) Then AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please specify the application to run") : Return
        DA.GetDataList(1, vals)
        DA.GetDataList(2, comms)
        DA.GetData(3, win)
        DA.GetData(4, run)

        DA.SetData(0, False)

        If done Then
            DA.SetData(0, True)
            If getalltext Then DA.SetData(1, Console.CurrentText)

            done = False
            Console.WriteMessage("Done")
            Message = ""
            OnDisplayExpired(True)
            nn = Nothing
        Else
            If Not run Then Return

            If nn IsNot Nothing Then
                Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The previous script is still running")
                Return
            End If

            If Not IO.File.Exists(app) Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Can't find the specified interpreter file") : Return

			nn = New ProcessScheduler(app, win, ArgsCollection.IDontCareAboutNames(vals), CmdCollection.FromStringEnumerable(comms))

			AddHandler nn.Exited, AddressOf ProcessDone
            AddHandler nn.MessageReceived, AddressOf HandleMessage
            AddHandler nn.ErrorReceived, AddressOf HandleError

            Dim tsk As New Task(AddressOf RunProcess)
            tsk.Start()
            Message = "Running"
            OnDisplayExpired(True)
        End If

    End Sub

    Public Sub ExpireThis()
        Me.ExpireSolution(True)
    End Sub

    Public Sub ProcessDone(sender As Object, e As EventArgs)
        done = True

        If nn Is Nothing Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Something went wrong.") : Return

        RemoveHandler nn.Exited, AddressOf ProcessDone
        RemoveHandler nn.MessageReceived, AddressOf HandleMessage
        RemoveHandler nn.ErrorReceived, AddressOf HandleError

        Rhino.RhinoApp.MainApplicationWindow.Invoke(expcomp)
    End Sub

    Private Sub StopProcess()
        If nn IsNot Nothing Then
            nn.AbortProcess()
            Me.Message = "Aborted"
            Me.Console.WriteMessage("Process aborted by user")
            nn = Nothing
        End If
    End Sub

    Public Sub RunProcess()
        If nn Is Nothing Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Something went wrong.") : Return
        nn.Run()
    End Sub

	Public Sub HandleError(sender As Object, e As MessageReceivedEventArgs)
		If e.Message = "" Then Return

		SyncLock Console.xthread
			Console.xthread.Add("[ERROR] " & e.Message)
			Dim act As New Action(AddressOf Console.WriteMessage)
			Rhino.RhinoApp.MainApplicationWindow.Invoke(act)
		End SyncLock
	End Sub

	Public Sub HandleMessage(sender As Object, e As MessageReceivedEventArgs)
		If e.Message = "" Then Return

		SyncLock Console.xthread
			Console.xthread.Add(e.Message)
			Dim act As New Action(AddressOf Console.WriteMessage)
			Rhino.RhinoApp.MainApplicationWindow.Invoke(act)
		End SyncLock
	End Sub

	Public Sub Dispose() Implements IDisposable.Dispose
        Console.Dispose()
    End Sub

End Class
