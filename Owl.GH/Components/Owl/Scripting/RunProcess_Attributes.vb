Imports Grasshopper.GUI.Canvas
Imports Grasshopper.GUI
Imports System.Drawing
Imports Grasshopper.Kernel

Public Class RunProcess_Attributes
    Inherits Grasshopper.Kernel.Attributes.GH_ComponentAttributes

    Dim _typedowner As RunProcess
    'Dim back As New Bitmap(15, 15)

    Sub New(Owner As RunProcess)
        MyBase.New(Owner)
        TypedOwner = Owner
        'DrawBack()
    End Sub

    'Private Sub DrawBack()
    '    Using g As Graphics = Graphics.FromImage(back)
    '        Using np As New Pen(Color.FromArgb(15, 0, 0, 0), 4)
    '            g.DrawLine(np, New Point(-15, 30), New Point(30, -15))
    '        End Using
    '    End Using
    'End Sub

    Private Property TypedOwner As RunProcess
        Get
            Return _typedowner
        End Get
        Set(value As RunProcess)
            _typedowner = value
        End Set
    End Property

    Public Overrides Function RespondToMouseDoubleClick(sender As GH_Canvas, e As GH_CanvasMouseEvent) As GH_ObjectResponse
        If Me.Bounds.Contains(e.CanvasLocation) Then
            If TypedOwner.Console.Visible Then TypedOwner.Console.Hide() : Return GH_ObjectResponse.Handled

            If Me.TypedOwner.Message = "Double Click" Then Me.TypedOwner.Message = "" : TypedOwner.OnDisplayExpired(True)

            TypedOwner.Console.Show(Grasshopper.Instances.DocumentEditor)
            TypedOwner.Console.Location = System.Windows.Forms.Cursor.Position + New Size(10, 10)

            Return GH_ObjectResponse.Handled
        End If

        Return GH_ObjectResponse.Ignore
    End Function

    'Protected Overrides Sub Render(canvas As GH_Canvas, graphics As Graphics, channel As GH_CanvasChannel)
    '    MyBase.Render(canvas, graphics, channel)

    '    If channel = GH_CanvasChannel.Objects Then

    '        MyBase.Render(canvas, graphics, channel)

    '        Dim capsPalette As GH_Palette = GH_Palette.Hidden

    '        If Owner.RuntimeMessageLevel = GH_RuntimeMessageLevel.Warning Then capsPalette = GH_Palette.Warning
    '        If Owner.RuntimeMessageLevel = GH_RuntimeMessageLevel.[Error] Then capsPalette = GH_Palette.[Error]
    '        If Owner.Locked Then capsPalette = GH_Palette.Locked

    '        Dim style As GH_PaletteStyle = GH_CapsuleRenderEngine.GetImpliedStyle(capsPalette, Me.Selected, Me.Owner.Locked, True)

    '        If TypedOwner.Console.Visible Then
    '            Using nb As New TextureBrush(back)
    '                Dim bnds As RectangleF = Me.Bounds
    '                bnds.Inflate(10, 10)
    '                graphics.FillRectangle(nb, bnds)
    '            End Using
    '        End If

    '    End If

    'End Sub


End Class
