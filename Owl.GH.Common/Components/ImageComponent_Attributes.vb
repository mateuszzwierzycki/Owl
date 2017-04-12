Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports GH_IO.Serialization
Imports Grasshopper.GUI
Imports Grasshopper.GUI.Canvas
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Attributes

Public Class ImageComponent_Attributes
    Inherits Attributes.GH_ComponentAttributes

    Private _imgowner As ImageComponentBase = Nothing

    Private _framew As Single = 100
    Private _frameh As Single = 100
    Private _resolution As Single = 1

    Private _backimage As New Bitmap(ImageWidth, ImageHeight)
    Private _foreimage As New Bitmap(ImageWidth, ImageHeight)

    Private _minw As Single = 50
    Private _minh As Single = 50

    Private _maxw As Single = 800
    Private _maxh As Single = 800

    Private _resizeable As Boolean = True
    Private _lockedsize As Boolean = False

    Private _tilebackground As Boolean = False

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

    Public Sub UpdateImages()
        TypedOwner.DrawBackground(Background)
        TypedOwner.DrawForeground(Foreground)
    End Sub

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        writer.SetSingle("W", FrameWidth)
        writer.SetSingle("H", FrameHeight)
        writer.SetSingle("R", Resolution)
        writer.SetSingle("MaxW", MaxWidth)
        writer.SetSingle("MaxH", MaxHeight)
        writer.SetSingle("MinW", MinWidth)
        writer.SetSingle("MinH", MinHeight)

        writer.SetBoolean("Lock", LockedSize)
        Return MyBase.Write(writer)
    End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        FrameWidth = reader.GetSingle("W")
        FrameHeight = reader.GetSingle("H")
        Resolution = reader.GetSingle("R")
        LockedSize = reader.GetBoolean("Lock")

        MinWidth = reader.GetSingle("MinW")
        MinHeight = reader.GetSingle("MinH")

        MaxWidth = reader.GetSingle("MaxW")
        MaxHeight = reader.GetSingle("MaxH")

        Resize(FrameWidth, FrameHeight, Resolution, True)
        Return MyBase.Read(reader)
    End Function

    Public Sub Resize(NewFrameWidth As Integer, NewFrameHeight As Integer, NewResolution As Single, Optional Immediate As Boolean = True)
        FrameWidth = NewFrameWidth
        FrameHeight = NewFrameHeight
        Resolution = NewResolution

        If Foreground IsNot Nothing Then Foreground.Dispose()
        If Background IsNot Nothing Then Background.Dispose()
        Foreground = New Bitmap(ImageWidth, ImageHeight)
        Background = New Bitmap(ImageWidth, ImageHeight)

        UpdateImages()

        If Immediate Then Me.ExpireLayout()
        If Immediate Then Owner.OnDisplayExpired(True)

    End Sub

    Public Property Background As Bitmap
        Get
            Return _backimage
        End Get
        Set(value As Bitmap)
            _backimage = value
        End Set
    End Property

    Public Property Foreground As Bitmap
        Get
            Return _foreimage
        End Get
        Set(value As Bitmap)
            _foreimage = value
        End Set
    End Property

    Public ReadOnly Property ImageWidth As Integer
        Get
            Return _framew * Resolution
        End Get
    End Property

    Public ReadOnly Property ImageHeight As Integer
        Get
            Return _frameh * Resolution
        End Get
    End Property

    ''' <summary>
    ''' Frame width in the canvas units
    ''' </summary>
    ''' <returns></returns>
    Public Property FrameWidth As Single
        Get
            Return _framew
        End Get
        Set(value As Single)
            _framew = Math.Max(MinWidth, Math.Min(MaxWidth, value))
        End Set
    End Property

    ''' <summary>
    ''' Frame height in the canvas units
    ''' </summary>
    ''' <returns></returns>
    Public Property FrameHeight As Single
        Get
            Return _frameh
        End Get
        Set(value As Single)
            _frameh = Math.Max(MinHeight, Math.Min(MaxHeight, value))
        End Set
    End Property

    Public Property Resolution As Single
        Get
            Return _resolution
        End Get
        Set(value As Single)
            _resolution = value
        End Set
    End Property

    Public Property MinWidth As Single
        Get
            Return _minw
        End Get
        Set(value As Single)
            _minw = value
        End Set
    End Property

    ''' <summary>
    ''' Typically 20 * Parameters.Count
    ''' </summary>
    ''' <returns></returns>
    Public Property MinHeight As Single
        Get
            Return _minh
        End Get
        Set(value As Single)
            _minh = value
        End Set
    End Property

    ''' <summary>
    ''' Generally resizeable.
    ''' </summary>
    ''' <returns></returns>
    Public Property Resizeable As Boolean
        Get
            Return _resizeable
        End Get
        Set(value As Boolean)
            _resizeable = value
        End Set
    End Property

    ''' <summary>
    ''' Locked by the user.
    ''' </summary>
    ''' <returns></returns>
    Public Property LockedSize As Boolean
        Get
            Return _lockedsize
        End Get
        Set(value As Boolean)
            _lockedsize = value
        End Set
    End Property

    Public Property MaxWidth As Single
        Get
            Return _maxw
        End Get
        Set(value As Single)
            _maxw = value
        End Set
    End Property

    Public Property MaxHeight As Single
        Get
            Return _maxh
        End Get
        Set(value As Single)
            _maxh = value
        End Set
    End Property

    Public Property TileBackground As Boolean
        Get
            Return _tilebackground
        End Get
        Set(value As Boolean)
            _tilebackground = value
        End Set
    End Property

    Private ReadOnly Property TypedOwner As ImageComponentBase
        Get
            Return _imgowner
        End Get
    End Property

    Protected Overrides Sub Layout()
        Dim width As Integer = FrameWidth
        Dim height As Integer = FrameHeight
        m_innerBounds = New System.Drawing.RectangleF(Pivot.X, Pivot.Y, width, height)
        m_innerBounds = GH_Convert.ToRectangle(m_innerBounds)

        If (Not LockedSize) And Resizeable And _resizing <> SnapSide.None Then
            Dim nsp As New PointF(CInt(_snappoint.X), CInt(_snappoint.Y))

            If _resizing And SnapSide.Right Then
                Dim oldw As Single = Me.m_innerBounds.Width
                Me.m_innerBounds.Width = Math.Max(MinWidth, Me.m_innerBounds.Width + (nsp.X - Me.m_innerBounds.X - Me.m_innerBounds.Width))
                Me.m_innerBounds.Width = Math.Min(800.0F, m_innerBounds.Width)
            End If

            If _resizing And SnapSide.Bottom Then
                Dim oldw As Single = Me.m_innerBounds.Height
                Me.m_innerBounds.Height = Math.Max(MinHeight, Me.m_innerBounds.Height + (nsp.Y - Me.m_innerBounds.Y - Me.m_innerBounds.Height))
                Me.m_innerBounds.Height = Math.Min(800.0F, m_innerBounds.Height)
            End If

            If _resizing And SnapSide.Left Then
                Dim oldx As Single = m_innerBounds.X
                Dim oldw As Single = m_innerBounds.Width
                m_innerBounds.Width -= nsp.X - oldx
                m_innerBounds.Width = Math.Max(MinWidth, m_innerBounds.Width)
                m_innerBounds.X = Math.Min(nsp.X, oldw + oldx - MinWidth)
                Pivot = New PointF(m_innerBounds.X, Pivot.Y)
            End If

            If _resizing And SnapSide.Top Then
                Dim oldy As Single = m_innerBounds.Y
                Dim oldh As Single = m_innerBounds.Height
                m_innerBounds.Height -= nsp.Y - oldy
                m_innerBounds.Height = Math.Max(MinHeight, m_innerBounds.Height)
                m_innerBounds.Y = Math.Min(nsp.Y, oldh + oldy - MinHeight)
                Pivot = New PointF(Pivot.X, m_innerBounds.Y)
            End If

            m_innerBounds.Width = Math.Max(m_innerBounds.Width, MinWidth)
            m_innerBounds.Height = Math.Max(m_innerBounds.Height, MinHeight)
            FrameWidth = m_innerBounds.Width
            FrameHeight = m_innerBounds.Height
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
            Dim smo As SmoothingMode = graphics.SmoothingMode

            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor
            graphics.SmoothingMode = SmoothingMode.None

            If canvas.Viewport.Zoom >= 1 Then
                If Background IsNot Nothing Then
                    If TileBackground Then
                        Using tb As New TextureBrush(Background)
                            tb.TranslateTransform(Pivot.X, Pivot.Y)
                            graphics.FillRectangle(tb, ConvertRectangle(imgrect))
                        End Using
                    Else
                        graphics.DrawImage(Background, ConvertRectangle(imgrect))
                    End If
                End If
                If Foreground IsNot Nothing Then graphics.DrawImage(Foreground, imgrect.X, imgrect.Y, imgrect.Width, imgrect.Height)
            ElseIf canvas.Viewport.Zoom >= 0.25 Then
                If Background IsNot Nothing Then
                    If TileBackground Then
                        Using tb As New TextureBrush(Background)
                            graphics.FillRectangle(tb, imgrect)
                        End Using
                    Else
                        graphics.DrawImage(Background, imgrect)
                    End If
                End If
                If Foreground IsNot Nothing Then graphics.DrawImage(Foreground, imgrect)
            Else
                graphics.FillRectangle(Brushes.LightGray, imgrect)
            End If

            graphics.InterpolationMode = intpol
            graphics.PixelOffsetMode = pixoff
            graphics.SmoothingMode = smo

            Using tp As New Pen(style.Edge)
                graphics.DrawRectangle(tp, ConvertRectangle(imgrect))
            End Using

        End If

    End Sub

    Private Function DetermineCursor(cursnap As SnapSide) As Cursor
        If Not Resizeable Then Return Cursors.Default
        If LockedSize Then Return Cursors.No

        If cursnap = SnapSide.Bottom Or cursnap = SnapSide.Top Then Return Cursors.SizeNS
        If cursnap = SnapSide.Right Or cursnap = SnapSide.Left Then Return Cursors.SizeWE

        If cursnap = (SnapSide.Bottom + SnapSide.Left) Then Return Cursors.SizeNESW
        If cursnap = (SnapSide.Top + SnapSide.Left) Then Return Cursors.SizeNWSE

        If cursnap = (SnapSide.Bottom + SnapSide.Right) Then Return Cursors.SizeNWSE
        If cursnap = (SnapSide.Top + SnapSide.Right) Then Return Cursors.SizeNESW

        Return Cursors.Default
    End Function

    Private Function IsSnapping(MouseLocation As PointF) As SnapSide
        If (Not Resizeable) Then Return SnapSide.None

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

        If Resizeable Then
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
                Resize(Me.m_innerBounds.Width, Me.m_innerBounds.Height, Resolution)
            End If

            Return GH_ObjectResponse.Release
        End If

        Return MyBase.RespondToMouseUp(sender, e)
    End Function

    Public Overrides Function RespondToMouseDoubleClick(sender As GH_Canvas, e As GH_CanvasMouseEvent) As GH_ObjectResponse
        If (Bounds.Contains(e.CanvasLocation)) Then
            SaveForeground()
            Return Grasshopper.GUI.Canvas.GH_ObjectResponse.Handled
        End If

        Return Grasshopper.GUI.Canvas.GH_ObjectResponse.Ignore
    End Function

    Public Sub SaveForeground()
        Dim save As New System.Windows.Forms.SaveFileDialog()

        save.Filter = "PNG Files (*.png)|*.png"

        If save.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            Foreground.Save(save.FileName)
        End If
    End Sub

    Public Sub SaveForeground(Filepath As String)
        Foreground.Save(Filepath)
    End Sub

End Class
