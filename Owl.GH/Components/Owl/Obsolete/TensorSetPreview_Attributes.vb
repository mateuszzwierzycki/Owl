Imports Grasshopper.GUI.Canvas
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Owl.Core.Tensors
Imports Owl.Core.Structures
Imports Grasshopper.GUI
Imports Grasshopper.Kernel

Public Class TensorSetPreview_Attributes
    Inherits Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    Implements IDisposable

    Public Sub New(ByVal owner As TensorSetPreview_OBSOLETE)
        MyBase.New(owner)
        DrawBackgroundBitmap()
    End Sub

    Private m_image As New Bitmap(100, 100)
    Private m_background As New Bitmap(300, 300)

    Public Property Image As Bitmap
        Get
            Return m_image
        End Get
        Set(value As Bitmap)
            m_image = value
        End Set
    End Property

    Public Property Background As Bitmap
        Get
            Return m_background
        End Get
        Set(value As Bitmap)
            m_background = value
        End Set
    End Property

    Private Sub DrawBackgroundBitmap()
        Dim filler As New Bitmap(10, 10)

        Using g As Graphics = Graphics.FromImage(filler)
            g.SmoothingMode = SmoothingMode.None
            g.InterpolationMode = InterpolationMode.NearestNeighbor
            g.Clear(Color.White)
            g.DrawRectangle(Pens.LightGray, 0, 0, CInt(10), CInt(10))
        End Using

        Using g As Graphics = Graphics.FromImage(Background)
            Using tx As Brush = New TextureBrush(filler)
                g.FillRectangle(tx, 0, 0, 300, 300)
            End Using
        End Using

        filler.Dispose()
    End Sub

    Friend Function CreateImage(R As Range, Highlight As List(Of Integer), Q As Integer) As Bitmap

        Dim own As TensorSetPreview_OBSOLETE = Me.Owner
        Dim ts As TensorSet = own.VecSet.Duplicate

        Dim vsr As Range = ts.GetRange
        ts.Remap(R, New Range(Q, 0))

        Me.Owner.Message = "Data range: " & vsr.From.ToString("0.000") & " to " & vsr.To.ToString("0.000")

        Image.Dispose()
        Image = New Bitmap(Q, Q)

        Using g As Graphics = Graphics.FromImage(Image)
            g.SmoothingMode = SmoothingMode.HighQuality

            '  g.Clear(Color.White)

            For i As Integer = 0 To ts.Count - 1 Step 1

                Dim v As Tensor = ts(i)

                If v.Count = 0 Then

                ElseIf v.Count = 1 Then
                    Dim thisgp As New GraphicsPath

                    Dim pts(1) As Drawing.Point

                    For j As Integer = 0 To 1 Step 1
                        pts(j) = New Drawing.Point(j * Q, v(0))
                    Next

                    thisgp.AddLines(pts)

                    Dim rndhsl As New Rhino.Display.ColorHSL(120, (i) / (ts.Count - 1), 1, 0.5)

                    Using p As Pen = New Pen(Color.FromArgb(100, rndhsl.ToArgbColor), 3)
                        p.DashPattern = {4, 4}
                        p.LineJoin = LineJoin.Round
                        p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round)
                        g.DrawPath(p, thisgp)
                    End Using

                    thisgp.Dispose()
                Else
                    Dim thisgp As New GraphicsPath

                    Dim pts(v.Count - 1) As Drawing.Point

                    For j As Integer = 0 To v.Count - 1 Step 1
                        pts(j) = New Drawing.Point((j / (v.Count - 1)) * Q, v(j))
                    Next

                    thisgp.AddLines(pts)

                    Dim rndhsl As New Rhino.Display.ColorHSL(120, (i) / (ts.Count - 1), 1, 0.5)

                    Using p As Pen = New Pen(Color.FromArgb(100, rndhsl.ToArgbColor), 2)
                        p.LineJoin = LineJoin.Round
                        p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round)
                        g.DrawPath(p, thisgp)
                    End Using

                    thisgp.Dispose()
                End If

            Next

            For i As Integer = 0 To Highlight.Count - 1 Step 1
                Dim v As Tensor = ts(Highlight(i))
                Dim thisi As Integer = Highlight(i)

                Dim thisgp As New GraphicsPath

                Dim pts(v.Count - 1) As Drawing.Point

                For j As Integer = 0 To v.Count - 1 Step 1
                    pts(j) = New Drawing.Point((j / (v.Count - 1)) * Q, v(j))
                Next

                thisgp.AddLines(pts)

                Using p As Pen = New Pen(Color.FromArgb(120, Color.Black), Math.Ceiling(5 * (Q / 300)))
                    p.LineJoin = LineJoin.Round
                    p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round)
                    g.DrawPath(p, thisgp)
                End Using

                Dim rndhsl As New Rhino.Display.ColorHSL(120, (thisi) / (ts.Count - 1), 1, 0.5)

                Using p As Pen = New Pen(Color.FromArgb(100, rndhsl.ToArgbColor), 2)
                    g.DrawPath(p, thisgp)
                End Using

                thisgp.Dispose()
            Next

        End Using

        Return Image
    End Function

    Protected Overrides Sub Layout()

        Me.Pivot = GH_Convert.ToPoint(Me.Pivot)

        Dim height As Integer = Math.Max(305, Math.Max(Owner.Params.Input.Count, Owner.Params.Output.Count) * 20)
        height = System.Math.Max(height, 24)
        Dim width As Integer = 24

        If Not GH_Attributes(Of IGH_Component).IsIconMode(Owner.IconDisplayMode) Then
            height = System.Math.Max(height, GH_Convert.ToSize(GH_FontServer.MeasureString(Owner.NickName, GH_FontServer.Large)).Width + 6)
        End If

        Dim bounds As System.Drawing.RectangleF = New System.Drawing.RectangleF(Owner.Attributes.Pivot.X - 0.5F * CSng(width), Owner.Attributes.Pivot.Y - 0.5F * CSng(height), CSng(width + 276), CSng(height))

        Me.m_innerBounds = bounds

        LayoutInputParams(Me.Owner, Me.m_innerBounds)

        Me.Bounds = LayoutBounds(Me.Owner, Me.m_innerBounds)

    End Sub

    Protected Overrides Sub Render(canvas As Grasshopper.GUI.Canvas.GH_Canvas, graphics As Drawing.Graphics, channel As Grasshopper.GUI.Canvas.GH_CanvasChannel)

        Dim cqp As TensorSetPreview_OBSOLETE = Me.Owner

        Select Case channel
            Case Grasshopper.GUI.Canvas.GH_CanvasChannel.Objects


                Me.RenderComponentCapsule(canvas, graphics, True, False, True, True, True, False)

                Dim pixoff As PixelOffsetMode = graphics.PixelOffsetMode
                Dim intpol As InterpolationMode = graphics.InterpolationMode

                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality

                If canvas.Viewport.Zoom >= 1 Then
                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor
                    graphics.DrawImage(Background, Me.Bounds.Location.X + Owner.Params.InputWidth + 5, Me.Bounds.Location.Y + 5, 300, 300)
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
                    graphics.DrawImage(Image, Me.Bounds.Location.X + Owner.Params.InputWidth + 5, Me.Bounds.Location.Y + 5, 300, 300)
                ElseIf canvas.Viewport.Zoom >= 0.25 Then
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
                    graphics.DrawImage(Background, Me.Bounds.Location.X + Owner.Params.InputWidth + 5, Me.Bounds.Location.Y + 5, 300, 300)
                    graphics.DrawImage(Image, Me.Bounds.Location.X + Owner.Params.InputWidth + 5, Me.Bounds.Location.Y + 5, 300, 300)
                Else
                    graphics.FillRectangle(Brushes.LightGray, Me.Bounds.Location.X + 5 + Owner.Params.InputWidth, Me.Bounds.Location.Y + 5, 300, 300)
                End If

                graphics.InterpolationMode = intpol
                graphics.DrawRectangle(Pens.Black, Me.Bounds.Location.X + Owner.Params.InputWidth + 5, Me.Bounds.Location.Y + 5, 300, 300)
                graphics.PixelOffsetMode = pixoff

            Case Else
                MyBase.Render(canvas, graphics, channel)
        End Select



    End Sub

    Public Overrides Function RespondToMouseDoubleClick(ByVal sender As GH_Canvas, ByVal e As GH_CanvasMouseEvent) As GH_ObjectResponse
        If (Bounds.Contains(e.CanvasLocation)) Then
            Dim save As New System.Windows.Forms.SaveFileDialog()

            save.Filter = "PNG Files | *.png"

            If save.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Image.Save(save.FileName)
            End If

            Return Grasshopper.GUI.Canvas.GH_ObjectResponse.Handled
        End If

        Return Grasshopper.GUI.Canvas.GH_ObjectResponse.Ignore
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                m_image.Dispose()
                m_background.Dispose()
            End If
        End If
        disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub
#End Region


End Class
