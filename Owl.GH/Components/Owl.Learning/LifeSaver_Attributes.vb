Imports Grasshopper.GUI
Imports Grasshopper.GUI.Canvas
Imports Grasshopper.Kernel

Public Class LifeSaver_Attributes
    Inherits Grasshopper.Kernel.Attributes.GH_ComponentAttributes

    Dim myowner As LifeSaver = Nothing

    Sub New(Owner As LifeSaver)
        MyBase.New(Owner)
        myowner = Owner
    End Sub

    Public Overrides Function RespondToMouseDoubleClick(sender As GH_Canvas, e As GH_CanvasMouseEvent) As GH_ObjectResponse
        If Me.ContentBox.Contains(e.CanvasLocation) Then
            Dim th As New Threading.Thread(AddressOf RunForm)
            th.Start()
            Return GH_ObjectResponse.Handled
        End If

        Return GH_ObjectResponse.Ignore
    End Function

    Sub RunForm()
        SyncLock myowner
            If myowner.lsc Is Nothing Then myowner.lsc = New LifeSaverControl(myowner)
            myowner.lsc.Show()
        End SyncLock
    End Sub

End Class
