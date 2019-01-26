Imports System.Windows.Forms
Imports Grasshopper.Kernel.Data
Imports Owl.Core.Tensors
Imports Rhino

Public MustInherit Class OwlMultiThreadedBase
    Inherits OwlComponentBase

    Sub New(Name As String, Nickname As String, Description As String, SubCategory As String)
        MyBase.New(Name, Nickname, Description, SubCategory)
    End Sub

    Sub New(Name As String, Nickname As String, Description As String, Category As String, SubCategory As String)
        MyBase.New(Name, Nickname, Description, Category, SubCategory)
    End Sub

    Dim ttask As New List(Of TensorSetTask)
    Dim thr As System.Threading.Thread = Nothing
    Public AbortFlag As Boolean = False
    Public AllSetUp As Boolean = False


    ''' <summary>
    ''' True if its on, false if not
    ''' </summary>
    ''' <returns></returns>
    Public Function ThreadIsAlive() As Boolean
        If thr IsNot Nothing Then
            If thr.ThreadState = Threading.ThreadState.Stopped Then
                Return False
            Else
                Return True
            End If
        End If
        Return False
    End Function

    Protected Overrides Sub BeforeSolveInstance()
        MyBase.BeforeSolveInstance()
        If ThreadIsAlive() Then Exit Sub
        If ThreadComplete Then
            If AbortFlag Then MyTasks.Clear()
            Me.Message = ""
            Exit Sub
        End If

        MyTasks.Clear()
    End Sub

    Protected Overrides Sub AfterSolveInstance()
        MyBase.AfterSolveInstance()
        If Not AllSetUp Then Exit Sub
        AllSetUp = False
        If ThreadComplete Then
            ThreadComplete = False
            Exit Sub
        End If
        If Not ThreadIsAlive() Then
            thr = New System.Threading.Thread(AddressOf ProcessTasks)
            thr.Start()
        Else
            If thr.ThreadState = Threading.ThreadState.Stopped Then
                thr = New Threading.Thread(AddressOf ProcessTasks)
                thr.Start()
            End If
        End If
    End Sub

    Protected Overrides Sub AppendAdditionalComponentMenuItems(menu As ToolStripDropDown)
        Menu_AppendItem(menu, "Abort", AddressOf AbortFlagSwitch)
        MyBase.AppendAdditionalComponentMenuItems(menu)
    End Sub

    Sub AbortFlagSwitch()
        AbortFlag = True
        Me.Message = "Aborting"
        Me.OnDisplayExpired(True)
    End Sub

    Private Sub ProcessTasks()
        Dim done As Integer = 0
        Dim st As New System.Diagnostics.Stopwatch
        st.Start()
        InvokePercent(-1)

        Try
            Parallel.For(0, MyTasks.Count, Sub(index)
                                               If AbortFlag Then Exit Sub

                                               If st.ElapsedMilliseconds > 100 Then
                                                   '    SyncLock SecretMessage
                                                   InvokePercent(done)
                                                   '   End SyncLock
                                                   st.Restart()
                                               End If

                                               MyTasks(index).Run()
                                               Threading.Interlocked.Add(done, 1)
                                           End Sub)
        Catch ex As Exception

        End Try


        Me.ThreadComplete = True
        InvokeExpire()
    End Sub

    Public Property ThreadComplete As Boolean = False
    Private mes As String = ""

    Private Property SecretMessage As String
        Get
            Return mes
        End Get
        Set(value As String)
            mes = value
        End Set
    End Property

    Public Property MyTasks As List(Of TensorSetTask)
        Get
            Return ttask
        End Get
        Set(value As List(Of TensorSetTask))
            ttask = value
        End Set
    End Property

    Private Sub InvokeExpire()
        Dim action As Action = AddressOf Me.ExpireMe
        If RhinoApp.MainApplicationWindow().InvokeRequired() Then
            RhinoApp.MainApplicationWindow().Invoke(action)
        Else
            Me.ExpireSolution(True)
        End If
    End Sub

    Private Sub ExpireMe()
        Me.ExpireSolution(True)
    End Sub

	Public Sub InvokePercent(done As Integer)
        If done = -1 Then SecretMessage = "Running"
        If done > 0 Then SecretMessage = "Processed " & done

        Me.Message = SecretMessage
        Dim action As Action = AddressOf Me.ExpireDisplay
        If RhinoApp.MainApplicationWindow().InvokeRequired() Then
            RhinoApp.MainApplicationWindow().Invoke(action)
        End If
    End Sub

    Private Sub ExpireDisplay()
        Me.OnDisplayExpired(False)
    End Sub

    MustOverride Function ComponentTensorAction(TS As TensorSet, Params As List(Of Object)) As List(Of Object)

End Class

Public Delegate Function TensorSetAction(TS As TensorSet, Params As List(Of Object)) As List(Of Object)

Public Class TensorSetTask
    Dim _thisset As TensorSet = Nothing
    Dim _thisact As TensorSetAction = Nothing
    Dim _thispar As New List(Of Object)
    Dim _thispath As GH_Path = Nothing

    Sub New(TS As TensorSet, Params As List(Of Object), Action As TensorSetAction, Path As GH_Path)
        Data = TS
        Me.Action = Action
        Me.ActionParams = Params
        Me.DataPath = Path
    End Sub

    Public Property Data As TensorSet
        Get
            Return _thisset
        End Get
        Set(value As TensorSet)
            _thisset = value
        End Set
    End Property

    Public Property ActionParams As List(Of Object)
        Get
            Return _thispar
        End Get
        Set(value As List(Of Object))
            _thispar = value
        End Set
    End Property

    Public Property Action As TensorSetAction
        Get
            Return _thisact
        End Get
        Set(value As TensorSetAction)
            _thisact = value
        End Set
    End Property

    Public Property Result As List(Of Object)

    Public Property DataPath As GH_Path
        Get
            Return _thispath
        End Get
        Set(value As GH_Path)
            _thispath = value
        End Set
    End Property

    Public Sub Run()
        Result = Action.Invoke(Data, ActionParams)
    End Sub

End Class