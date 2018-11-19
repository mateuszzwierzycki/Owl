Imports System.Windows.Forms
Imports Grasshopper.Kernel

Public Class LifeSaverControl
    Private m_owner As LifeSaver = Nothing
    Friend m_string As String = ""
    Friend m_instring As String = ""
    Dim dlgexp As Action = AddressOf Expire
    Dim dlglock As Action = AddressOf LockSolution

    Sub New(Owner As LifeSaver)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        m_owner = Owner
    End Sub

    Private Sub PushChanges_Click(sender As Object, e As EventArgs) Handles PushChanges.Click
        SyncLock m_string
            m_string = Me.InputBox.Text
        End SyncLock

        Rhino.RhinoApp.MainApplicationWindow.Invoke(dlgexp)
    End Sub

    Private Sub SolverEnabled_CheckedChanged(sender As Object, e As EventArgs) Handles SolverEnabled.CheckedChanged
        Rhino.RhinoApp.MainApplicationWindow.Invoke(dlglock)
    End Sub

    Private Sub LockSolution()
        GH_Document.EnableSolutions = Me.SolverEnabled.Checked
    End Sub

    Private Sub Expire()
        m_owner.ExpireSolution(True)
    End Sub

    Private Sub LifeSaverControl_Closing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Hide()
    End Sub
End Class