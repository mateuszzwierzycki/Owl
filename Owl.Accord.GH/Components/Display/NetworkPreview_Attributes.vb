Imports Grasshopper.GUI.Canvas
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Grasshopper.GUI
Imports Rhino.Geometry
Imports System.Drawing.Imaging
Imports Grasshopper.Kernel


Public Class NetworkPreview_Attributes
    Inherits Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    Implements IDisposable

    Public Sub New(ByVal owner As NetworkPreview_OBSOLETE)
        MyBase.New(owner)
        DrawBackgroundBitmap()
    End Sub

    Private m_image As New Bitmap(100, 100)
    Private m_background As New Bitmap(300, 300)

    Sub New(Owner As GH_Component)
        MyBase.New(Owner)
    End Sub

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


    Friend Function CreateImage(LineThickness As Interval, PointRadius As Integer, Q As Integer) As Bitmap

        LineThickness.T0 = Math.Max(1, LineThickness.T0)
        LineThickness.T1 = Math.Max(1, LineThickness.T1)

        Dim own As NetworkPreview_OBSOLETE = Me.Owner
        Dim nn As ActivationNetwork = own.m_net

        Dim lays As Integer = nn.Layers.Count

        Dim its As New Interval(0.08 * Q, Q - (0.08 * Q))

        Dim pts As New List(Of List(Of Drawing.PointF))

        Dim wmin As Double = Double.MaxValue
        Dim wmax As Double = Double.MinValue

        Dim px As Single
        Dim py As Single

        pts.Add(New List(Of PointF))

        For i As Integer = 0 To nn.InputsCount - 1 Step 1
            pts(0).Add(New PointF(its.ParameterAt(0), its.ParameterAt(i / (nn.InputsCount - 1))))
        Next

        For i As Integer = 0 To nn.Layers.Count - 1 Step 1
            px = its.ParameterAt((i + 1) / (nn.Layers.Count))
            pts.Add(New List(Of PointF))

            For j As Integer = 0 To nn.Layers(i).Neurons.Count - 1 Step 1
                If (nn.Layers(i).Neurons.Count) = 1 Then
                    py = its.ParameterAt(0.5)
                Else
                    py = its.ParameterAt(j / (nn.Layers(i).Neurons.Count - 1))
                End If



                For k As Integer = 0 To nn.Layers(i).Neurons(j).Weights.Count - 1 Step 1
                    wmin = Math.Min(wmin, nn.Layers(i).Neurons(j).Weights(k))
                    wmax = Math.Max(wmax, nn.Layers(i).Neurons(j).Weights(k))

                Next

                pts(i + 1).Add(New PointF(px, py))

            Next
        Next


        Dim bmp As New Bitmap(Q, Q)
        Dim wit As New Interval(wmin, wmax)
        Dim tar As New Interval(0.5, 1)
        Dim lit As New Interval(1, 0.5)

        Dim nnsums As New List(Of List(Of Double))
        Dim nitvs As New List(Of Interval)

        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            Using thickpen As Pen = New Pen(Color.Black, 3)

                For i As Integer = 1 To pts.Count - 1 Step 1

                    Dim nsums As New List(Of Double)
                    Dim nmin As Double = Double.MaxValue
                    Dim nmax As Double = Double.MinValue

                    Dim thislay As ActivationLayer = nn.Layers(i - 1)
                    For j As Integer = 0 To pts(i).Count - 1 Step 1
                        Dim thisn As ActivationNeuron = nn.Layers(i - 1).Neurons(j)
                        Dim thisp As PointF = pts(i)(j)
                        Dim thissum As Double = 0

                        For k As Integer = 0 To thisn.Weights.Count - 1 Step 1
                            Dim thatp As PointF = pts(i - 1)(k)

                            thissum += thisn.Weights(k)
                            Dim thisparam As Double = wit.NormalizedParameterAt(thisn.Weights(k))

                            Dim hsl As New Rhino.Display.ColorHSL(tar.ParameterAt(thisparam), thisparam, lit.ParameterAt(thisparam))

                            Using thispen As Pen = New Pen(Color.FromArgb(thisparam * 200, hsl.ToArgbColor), LineThickness.ParameterAt(thisparam))
                                thispen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round)
                                g.DrawLine(thispen, thisp, thatp)
                            End Using
                        Next

                        nsums.Add(thissum)
                        nmin = Math.Min(nmin, thissum)
                        nmax = Math.Max(nmax, thissum)

                    Next

                    nnsums.Add(nsums)
                    nitvs.Add(New Interval(nmin, nmax))

                Next

                If PointRadius >= 1 Then


                    Dim ritv As New Interval(1, PointRadius)

                    For i As Integer = 0 To pts.Count - 1 Step 1

                        Dim nitv As Interval = Interval.Unset

                        If i > 0 Then
                            nitv = nitvs(i - 1)
                        End If

                        For j As Integer = 0 To pts(i).Count - 1 Step 1

                            If i = 0 Then
                                Dim thisr As Double = ritv.ParameterAt(0.3)
                                thisr = thisr * (Q / 300)
                                g.FillEllipse(Brushes.White, GetCircle(pts(i)(j), thisr))
                                g.DrawEllipse(thickpen, GetCircle(pts(i)(j), thisr))
                            Else
                                Dim thisr As Double = nnsums(i - 1)(j)
                                thisr = nitv.NormalizedParameterAt(thisr)
                                thisr = Math.Min(thisr, 1.5)
                                thisr = ritv.ParameterAt(thisr) * (Q / 300)
                                thisr = Math.Max(thisr, 2)

                                g.FillEllipse(Brushes.White, GetCircle(pts(i)(j), thisr))
                                g.DrawEllipse(thickpen, GetCircle(pts(i)(j), thisr))


                            End If
                        Next



                    Next

                End If

            End Using

        End Using

        Image.Dispose()
        Image = bmp
        Return Image

    End Function

    Private Function GetCircle(P As PointF, R As Double) As RectangleF
        Return New RectangleF(P.X - R / 2, P.Y - R / 2, R, R)
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

        Dim cqp As NetworkPreview_OBSOLETE = Me.Owner

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
                Image.Save(save.FileName, ImageFormat.Png)
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
