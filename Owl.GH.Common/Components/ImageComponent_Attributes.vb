Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports Grasshopper.GUI
Imports Grasshopper.GUI.Canvas
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Attributes

Public Class ImageComponent_Attributes
    Inherits Attributes.GH_ComponentAttributes

    Private _resizing As SnapSide = SnapSide.None
    Private _resizestart As RectangleF

    Private _snapgap As Integer = 10
    Private _snappoint As PointF

    <Flags>
    Private Enum SnapSide As Byte
        None = 0
        Left = 1
        Bottom = 2
        Right = 4
        Top = 8
    End Enum

    Sub New(Owner As ImageComponentBase)
        MyBase.New(Owner)
        _imgowner = Owner
    End Sub

    Public Sub OnResize(NewFrameWidth As Integer, NewFrameHeight As Integer)
        TypedOwner.FrameWidth = NewFrameWidth
        TypedOwner.FrameHeight = NewFrameHeight
        TypedOwner.UpdateImages()
        Owner.OnDisplayExpired(True)
    End Sub

    Private _imgowner As ImageComponentBase = Nothing

    Private ReadOnly Property TypedOwner As ImageComponentBase
        Get
            Return _imgowner
        End Get
    End Property

    Protected Overrides Sub Layout()
        Dim width As Integer = TypedOwner.FrameWidth
        Dim height As Integer = TypedOwner.FrameHeight

        m_innerBounds = New System.Drawing.RectangleF(Pivot.X, Pivot.Y, width, height)
        m_innerBounds = GH_Convert.ToRectangle(m_innerBounds)


        If (Not TypedOwner.LockedSize) And TypedOwner.Resizeable And _resizing <> SnapSide.None Then
            Dim nsp As New PointF(CInt(_snappoint.X), CInt(_snappoint.Y))

            If _resizing And SnapSide.Right Then
                Dim oldw As Single = Me.m_innerBounds.Width
                Me.m_innerBounds.Width = Math.Max(TypedOwner.MinWidth, Me.m_innerBounds.Width + (nsp.X - Me.m_innerBounds.X - Me.m_innerBounds.Width))
                Me.m_innerBounds.Width = Math.Min(800.0F, m_innerBounds.Width)
            End If

            If _resizing And SnapSide.Bottom Then
                Dim oldw As Single = Me.m_innerBounds.Height
                Me.m_innerBounds.Height = Math.Max(TypedOwner.MinHeight, Me.m_innerBounds.Height + (nsp.Y - Me.m_innerBounds.Y - Me.m_innerBounds.Height))
                Me.m_innerBounds.Height = Math.Min(800.0F, m_innerBounds.Height)
            End If

            If _resizing And SnapSide.Left Then
                Dim oldx As Single = m_innerBounds.X
                Dim oldw As Single = m_innerBounds.Width
                m_innerBounds.Width -= nsp.X - oldx
                m_innerBounds.Width = Math.Max(TypedOwner.MinWidth, m_innerBounds.Width)
                m_innerBounds.X = Math.Min(nsp.X, oldw + oldx - TypedOwner.MinWidth)
                Pivot = New PointF(m_innerBounds.X, Pivot.Y)
            End If

            If _resizing And SnapSide.Top Then
                Dim oldy As Single = m_innerBounds.Y
                Dim oldh As Single = m_innerBounds.Height
                m_innerBounds.Height -= nsp.Y - oldy
                m_innerBounds.Height = Math.Max(TypedOwner.MinHeight, m_innerBounds.Height)
                m_innerBounds.Y = Math.Min(nsp.Y, oldh + oldy - TypedOwner.MinHeight)
                Pivot = New PointF(Pivot.X, m_innerBounds.Y)
            End If

            m_innerBounds.Width = Math.Max(m_innerBounds.Width, TypedOwner.MinWidth)
            m_innerBounds.Height = Math.Max(m_innerBounds.Height, TypedOwner.MinHeight)
            TypedOwner.FrameWidth = m_innerBounds.Width
            TypedOwner.FrameHeight = m_innerBounds.Height
        End If

        GH_ComponentAttributes.LayoutInputParams(Me.Owner, Me.m_innerBounds)
        GH_ComponentAttributes.LayoutOutputParams(Me.Owner, Me.m_innerBounds)
        Me.Bounds = GH_ComponentAttributes.LayoutBounds(Me.Owner, Me.m_innerBounds)
    End Sub

    Private Function ConvertRectangle(rectf As RectangleF) As Rectangle
        Return New Rectangle(rectf.X, rectf.Y, rectf.Width, rectf.Height)
    End Function

    Private Function ConvertRectangle(rect As Rectangle) As RectangleF
        Return New RectangleF(rect.X, rect.Y, rect.Width, rect.Height)
    End Function

    Protected Overrides Sub Render(canvas As Grasshopper.GUI.Canvas.GH_Canvas, graphics As Drawing.Graphics, channel As Grasshopper.GUI.Canvas.GH_CanvasChannel)

        If channel = GH_CanvasChannel.Wires Then
            MyBase.Render(canvas, graphics, channel)
        ElseIf channel = GH_CanvasChannel.Objects Then
            'capsule and stuff
            Dim thisviewport As GH_Viewport = canvas.Viewport
            If Not thisviewport.IsVisible(Me.Bounds, 10.0F) Then Return

            Dim capsule As GH_Capsule = GH_Capsule.CreateCapsule(Me.Bounds, GH_Palette.Normal, 3, 12)

            For Each p As IGH_Param In Me.Owner.Params.Input
                capsule.AddInputGrip(p.Attributes.Bounds.Y + p.Attributes.Bounds.Height / 2)
            Next

            For Each p As IGH_Param In Me.Owner.Params.Output
                capsule.AddOutputGrip(p.Attributes.Bounds.Y + p.Attributes.Bounds.Height / 2)
            Next

            Dim capsPalette As GH_Palette = GH_Palette.Hidden

            If Owner.RuntimeMessageLevel = GH_RuntimeMessageLevel.Warning Then capsPalette = GH_Palette.Warning
            If Owner.RuntimeMessageLevel = GH_RuntimeMessageLevel.[Error] Then capsPalette = GH_Palette.[Error]
            If Owner.Locked Then capsPalette = GH_Palette.Locked

            Dim style As GH_PaletteStyle = GH_CapsuleRenderEngine.GetImpliedStyle(capsPalette, Me.Selected, Me.Owner.Locked, True)

            capsule.Render(graphics, style)
            RenderComponentParameters(canvas, graphics, Me.Owner, style)
            capsule.Dispose()

            Dim imgrect As RectangleF = Me.m_innerBounds

            Dim pixoff As PixelOffsetMode = graphics.PixelOffsetMode
            Dim intpol As InterpolationMode = graphics.InterpolationMode

            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality

            If canvas.Viewport.Zoom >= 1 Then
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor
                If TypedOwner.Background IsNot Nothing Then
                    If TypedOwner.TileBackground Then
                        Using tb As New TextureBrush(TypedOwner.Background)
                            graphics.FillRectangle(tb, imgrect)
                        End Using
                    Else
                        graphics.DrawImage(TypedOwner.Background, imgrect)
                    End If
                End If
                If TypedOwner.Foreground IsNot Nothing Then graphics.DrawImage(TypedOwner.Foreground, imgrect)
            ElseIf canvas.Viewport.Zoom >= 0.25 Then
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor
                If TypedOwner.Background IsNot Nothing Then
                    If TypedOwner.TileBackground Then
                        Using tb As New TextureBrush(TypedOwner.Background)
                            graphics.FillRectangle(tb, imgrect)
                        End Using
                    Else
                        graphics.DrawImage(TypedOwner.Background, imgrect)
                    End If
                End If
                If TypedOwner.Foreground IsNot Nothing Then graphics.DrawImage(TypedOwner.Foreground, imgrect)
            Else
                graphics.FillRectangle(Brushes.LightGray, imgrect)
            End If

            graphics.InterpolationMode = intpol
            graphics.PixelOffsetMode = pixoff

            Using tp As New Pen(style.Edge)
                graphics.DrawRectangle(tp, ConvertRectangle(imgrect))
            End Using


        End If

    End Sub

    Private Function DetermineCursor(cursnap As SnapSide) As Cursor
        If Not TypedOwner.Resizeable Then Return Cursors.Default
        If TypedOwner.LockedSize Then Return Cursors.No

        If cursnap = SnapSide.Bottom Or cursnap = SnapSide.Top Then Return Cursors.SizeNS
        If cursnap = SnapSide.Right Or cursnap = SnapSide.Left Then Return Cursors.SizeWE

        If cursnap = (SnapSide.Bottom + SnapSide.Left) Then Return Cursors.SizeNESW
        If cursnap = (SnapSide.Top + SnapSide.Left) Then Return Cursors.SizeNWSE

        If cursnap = (SnapSide.Bottom + SnapSide.Right) Then Return Cursors.SizeNWSE
        If cursnap = (SnapSide.Top + SnapSide.Right) Then Return Cursors.SizeNESW

        Return Cursors.Default
    End Function

    Private Function IsSnapping(MouseLocation As PointF) As SnapSide
        If (Not TypedOwner.Resizeable) Then Return SnapSide.None

        Dim ret As SnapSide = SnapSide.None

        If Me.m_innerBounds.Contains(MouseLocation) Then
            If MouseLocation.X < Me.m_innerBounds.Location.X + _snapgap Then ret += SnapSide.Left
            If MouseLocation.X > Me.m_innerBounds.Location.X + Me.m_innerBounds.Width - _snapgap Then ret += SnapSide.Right
            If MouseLocation.Y < Me.m_innerBounds.Location.Y + _snapgap Then ret += SnapSide.Top
            If MouseLocation.Y > Me.m_innerBounds.Location.Y + Me.m_innerBounds.Height - _snapgap Then ret += SnapSide.Bottom
            _snappoint = MouseLocation
        End If

        Return ret
    End Function

    Public Overrides Function RespondToMouseDown(sender As GH_Canvas, e As GH_CanvasMouseEvent) As GH_ObjectResponse

        If TypedOwner.Resizeable Then
            If e.Button = MouseButtons.Left Then
                _resizing = IsSnapping(e.CanvasLocation) 'if is snapping then start resizing

                If _resizing <> SnapSide.None Then  'if resizing then
                    _resizestart = Me.m_innerBounds
                    Return GH_ObjectResponse.Capture
                End If
            End If
        End If

        Return MyBase.RespondToMouseDown(sender, e)
    End Function

    Public Overrides Function RespondToMouseMove(sender As GH_Canvas, e As GH_CanvasMouseEvent) As GH_ObjectResponse

        If _resizing <> SnapSide.None Then 'currently resizing
            _snappoint = e.CanvasLocation
            Me.ExpireLayout()
            sender.Refresh()
            Return GH_ObjectResponse.Capture
        Else                               'not resizing
            If IsSnapping(e.CanvasLocation) <> SnapSide.None Then 'check if change cursor
                sender.Cursor = DetermineCursor(IsSnapping(e.CanvasLocation))
                Return GH_ObjectResponse.Handled
            End If
        End If

        Return MyBase.RespondToMouseMove(sender, e)
    End Function

    Public Overrides Function RespondToMouseUp(sender As GH_Canvas, e As GH_CanvasMouseEvent) As GH_ObjectResponse

        If _resizing <> SnapSide.None Then 'is resizing, now to finish 
            _resizing = SnapSide.None

            If _resizestart.Width <> Me.m_innerBounds.Width Or _resizestart.Height <> Me.m_innerBounds.Height Then
                OnResize(Me.m_innerBounds.Width, Me.m_innerBounds.Height)
            End If

            Return GH_ObjectResponse.Release
        End If

        Return MyBase.RespondToMouseUp(sender, e)
    End Function

    Public Overrides Function RespondToMouseDoubleClick(sender As GH_Canvas, e As GH_CanvasMouseEvent) As GH_ObjectResponse
        If (Bounds.Contains(e.CanvasLocation)) Then
            TypedOwner.SaveForeground()
            Return Grasshopper.GUI.Canvas.GH_ObjectResponse.Handled
        End If

        Return Grasshopper.GUI.Canvas.GH_ObjectResponse.Ignore
    End Function

End Class

Public Class ImageRequestArgs
    Inherits EventArgs

    Private _w As Integer
    Private _h As Integer

    Public Property W As Integer
        Get
            Return _w
        End Get
        Set(value As Integer)
            _w = value
        End Set
    End Property

    Public Property H As Integer
        Get
            Return _h
        End Get
        Set(value As Integer)
            _h = value
        End Set
    End Property

    Sub New(W As Integer, H As Integer)
        Me.W = W
        Me.H = H
    End Sub

End Class