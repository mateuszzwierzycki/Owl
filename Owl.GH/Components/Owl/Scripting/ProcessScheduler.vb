Imports System.IO

Public Class ProcessScheduler

    Private _application As String = ""
    Private _vars As ArgsCollection = Nothing
    Private _comms As CmdCollection = Nothing
    Private _windowed As Boolean = False

    Dim output As New System.Text.StringBuilder

    Public Sub New(ApplicationPath As String, Optional Windowed As Boolean = False, Optional Arguments As ArgsCollection = Nothing, Optional Commands As CmdCollection = Nothing)
        Me.ApplicationPath = ApplicationPath
        Me.Arguments = Arguments
        Me.Commands = Commands
        Me.Windowed = Windowed
    End Sub

    Public Property ApplicationPath As String
        Get
            Return _application
        End Get
        Set(value As String)
            _application = value
        End Set
    End Property

    Public Property Arguments As ArgsCollection
        Get
            Return _vars
        End Get
        Set(value As ArgsCollection)
            _vars = value
        End Set
    End Property

    Public Property Commands As CmdCollection
        Get
            Return _comms
        End Get
        Set(value As CmdCollection)
            _comms = value
        End Set
    End Property

    Public Property Windowed As Boolean
        Get
            Return _windowed
        End Get
        Set(value As Boolean)
            _windowed = value
        End Set
    End Property

    Private _proc As Process = Nothing

    Public Function Run()
        Dim pif As ProcessStartInfo = New ProcessStartInfo(Me.ApplicationPath)

        Dim args As New String("")

        If Arguments IsNot Nothing Then
            For Each p As ScriptArgument In Me.Arguments
                args &= " " & p.Value
            Next
            pif.Arguments = args
        End If

        pif.CreateNoWindow = If(Windowed, False, True)
        pif.UseShellExecute = False
        pif.RedirectStandardInput = True
        pif.RedirectStandardError = True
        pif.RedirectStandardOutput = True

        Dim proc As Process = New Process() With {.StartInfo = pif}
        _proc = proc

        proc.EnableRaisingEvents = True
        AddHandler proc.Exited, AddressOf ExitHandler
        AddHandler proc.OutputDataReceived, AddressOf OutputHandler
        AddHandler proc.ErrorDataReceived, AddressOf ErrorHandler

        proc.Start()
        If Me.Commands IsNot Nothing Then Me.Commands.WriteAll(proc, False)

        proc.StandardInput.Close()

        RaiseEvent MessageReceived(Me, New MessageReceivedEventArgs("Process " & proc.ProcessName & " started."))

        proc.BeginErrorReadLine()
        proc.BeginOutputReadLine()

        Return Nothing
    End Function

    Public Sub AbortProcess()
        If _proc Is Nothing Then Return
        RemoveHandler _proc.OutputDataReceived, AddressOf OutputHandler
        RemoveHandler _proc.ErrorDataReceived, AddressOf ErrorHandler
        RemoveHandler _proc.Exited, AddressOf ExitHandler
        _proc.Kill()
        RaiseEvent Aborted(Me, EventArgs.Empty)
    End Sub

    Public Sub ExitHandler(sender As Object, e As EventArgs)
        RemoveHandler _proc.OutputDataReceived, AddressOf OutputHandler
        RemoveHandler _proc.ErrorDataReceived, AddressOf ErrorHandler
        RemoveHandler _proc.Exited, AddressOf ExitHandler
        RaiseEvent Exited(Me, EventArgs.Empty)
    End Sub

    Public Sub ErrorHandler(sender As Object, e As DataReceivedEventArgs)
        If Not String.IsNullOrEmpty(e.Data) Then
            RaiseEvent ErrorReceived(Me, New MessageReceivedEventArgs(e.Data))
        End If
    End Sub

    Public Sub OutputHandler(sender As Object, e As DataReceivedEventArgs)
        If Not String.IsNullOrEmpty(e.Data) Then
            RaiseEvent MessageReceived(Me, New MessageReceivedEventArgs(e.Data))
        End If
    End Sub

    Public Event MessageReceived(sender As Object, e As MessageReceivedEventArgs)
    Public Event ErrorReceived(sender As Object, e As MessageReceivedEventArgs)
    Public Event Exited(sender As Object, e As EventArgs)
    Public Event Aborted(sender As Object, e As EventArgs)
End Class

Public Class MessageReceivedEventArgs

    Inherits EventArgs

    Private _message As String

    Sub New(Message As String)
        Me.Message = Message
    End Sub

    Public Property Message As String
        Get
            Return _message
        End Get
        Set(value As String)
            _message = value
        End Set
    End Property
End Class

Public Class ScriptArgument

    Public Shared Function Create(Name As String, Value As String) As ScriptArgument
        Return New ScriptArgument(Name, Value)
    End Function

    Public Sub New(VariableName As String, VariableValue As String)
        Value = VariableValue
        Name = VariableName
    End Sub

    Public Property Value As String
    Public Property Name As String

    Public Overrides Function ToString() As String
        Return "ScriptArgument " & Name & " " & Value
    End Function

End Class

Public Class ScriptCommand

    Public Shared Function Create(Command As String) As ScriptCommand
        Return New ScriptCommand(Command)
    End Function

    Public Sub New(Command As String)
        Me.Command = Command
    End Sub

    Public Property Command As String

    Public Overrides Function ToString() As String
        Return "ScriptCommand " & Command
    End Function

End Class

Public Class CmdCollection
    Implements IEnumerable(Of ScriptCommand)

    Public Property Values As New List(Of ScriptCommand)

    Public Sub WriteAll(Proc As Process, Optional Flush As Boolean = True)
        For i As Integer = 0 To Me.Count - 1 Step 1
            Proc.StandardInput.WriteLine(Me(i).Command)
        Next
        If Flush Then Proc.StandardInput.Flush()
    End Sub

    Public Shared Function FromStringEnumerable(Enumerable As IEnumerable(Of String)) As CmdCollection
        If Enumerable.Count = 0 Then Return Nothing
        Dim comm As New CmdCollection
        For Each str As String In Enumerable
            comm.Values.Add(ScriptCommand.Create(str))
        Next
        Return comm
    End Function

    Public Function GetEnumerator() As IEnumerator(Of ScriptCommand) Implements IEnumerable(Of ScriptCommand).GetEnumerator
        Return Values.GetEnumerator
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Values.GetEnumerator
    End Function
End Class

Public Class ArgsCollection
    Implements IEnumerable(Of ScriptArgument)

    Public Property Values As New List(Of ScriptArgument)

    Sub New()

    End Sub

    Public Sub Add(Variable As ScriptArgument)
        Me.Values.Add(Variable)
    End Sub

    Public Sub AddRange(Values As IEnumerable(Of ScriptArgument))
        For Each v In Values
            Me.Values.Add(v)
        Next
    End Sub

    ''' <summary>
    ''' Are you proud of yourself ? 
    ''' </summary>
    ''' <param name="Values"></param>
    ''' <returns></returns>
    Public Shared Function IDontCareAboutNames(Values As IEnumerable(Of String)) As ArgsCollection
        If Values.Count = 0 Then Return Nothing
        Dim nn As New ArgsCollection()
        For Each v As String In Values
            nn.Add(ScriptArgument.Create("Variable_" & nn.Count, v))
        Next
        Return nn
    End Function

    Public Sub RemoveAt(Index As Integer)
        Me.Values.RemoveAt(Index)
    End Sub

    Default Public Property Item(index As Integer) As ScriptArgument
        Get
            Return Values(index)
        End Get
        Set(value As ScriptArgument)
            Values(index) = value
        End Set
    End Property

    Public Sub Clear()
        Values.Clear()
    End Sub

    Public Function GetEnumerator() As IEnumerator(Of ScriptArgument) Implements IEnumerable(Of ScriptArgument).GetEnumerator
        Return Values.GetEnumerator
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Values.GetEnumerator
    End Function
End Class