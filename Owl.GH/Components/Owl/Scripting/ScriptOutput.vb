Imports System
Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Forms

''' <summary>
''' As for now a read only console... a debug window.
''' </summary>
Public Class ScriptOutput

    Private Sub ConsoleBox_TextChanged(sender As Object, e As System.EventArgs) Handles ConsoleBox.TextChanged
        ConsoleBox.SelectionStart = ConsoleBox.Text.Length
        ConsoleBox.ScrollToCaret()
    End Sub

    Private _mess As New List(Of String)

    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Property Messages As List(Of String)
        Get
            Return _mess
        End Get
        Set(value As List(Of String))
            _mess = value
        End Set
    End Property

    Public CurrentText As New System.Text.StringBuilder

    Friend xthread As New List(Of String)

    Public Sub WriteMessage()
        SyncLock xthread
            For i As Integer = 0 To xthread.Count - 1 Step 1
                Me.Messages.Add("[" & DateTime.Now.ToLongTimeString & "] " & xthread(i))
            Next
            xthread.Clear()
        End SyncLock

        UpdateWindow()
    End Sub

    Public Sub WriteMessage(Message As String)
        Me.Messages.Add("[" & DateTime.Now.ToLongTimeString & "] " & Message)
        UpdateWindow()
    End Sub

    Private Function HowManyLines() As Integer
        Dim s As SizeF = TextRenderer.MeasureText("A", ConsoleBox.Font, ConsoleBox.Size, TextFormatFlags.WordBreak)
        Dim letterHeight As Integer = CInt(s.Height)
        Dim displayableLines As Integer = ConsoleBox.Height / letterHeight
        Return displayableLines
    End Function

    Private Sub UpdateWindow()
        CurrentText.Clear()

        If ClearBox.Checked Then
            Dim leng As Integer = Messages.Count
            Dim printingstart As Integer = Math.Max(0, (leng - HowManyLines()))

            For i As Integer = printingstart To Messages.Count - 1 Step 1
                CurrentText.AppendLine(Messages(i))
            Next
        Else
            For Each str As String In Messages
                CurrentText.AppendLine(str)
            Next
        End If

        ConsoleBox.Text = CurrentText.ToString
        ConsoleBox.Refresh()
    End Sub

    Private Sub Clearbut_Click(sender As Object, e As EventArgs) Handles Clearbut.Click
        Messages.Clear()
        UpdateWindow()
    End Sub

    Private Sub CopyBut_Click(sender As Object, e As EventArgs) Handles CopyBut.Click
        UpdateWindow()
        Clipboard.SetText(If(CurrentText.ToString = "", "Nothing was there", CurrentText.ToString))
    End Sub

    Private Sub Savebut_Click(sender As Object, e As EventArgs) Handles Savebut.Click
        UpdateWindow()
        Dim nn As New SaveFileDialog()
        nn.AddExtension = True
        nn.DefaultExt = ".txt"
        nn.Filter = "Text file *|*.txt"

        If nn.ShowDialog() = DialogResult.OK Then
            Using str As StreamWriter = New StreamWriter(nn.FileName)
                str.Write(CurrentText.ToString)
                Me.WriteMessage("Saved file as " & nn.FileName)
            End Using
        End If

        UpdateWindow()
    End Sub

    Private Sub PythonConsole_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WriteMessage("Hi, this is the Process Output !")
        UpdateWindow()
    End Sub

    Private Sub EscapeHandle(sender As Object, e As PreviewKeyDownEventArgs) Handles Me.PreviewKeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Hide()
        End If
    End Sub

End Class