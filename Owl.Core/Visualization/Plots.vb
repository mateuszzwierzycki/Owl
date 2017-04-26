Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Owl.Core.Structures
Imports Owl.Core.Tensors

Namespace Visualization

    Public Module PlotFactory

        ''' <summary>
        ''' Shape = {Rows, Columns, Channel}
        ''' </summary>
        ''' <param name="Tensor3D"></param>
        ''' <returns></returns>
        Public Function Tensor3DImage(Tensor3D As Tensor, R As Range) As Bitmap
            'there is not much code here, but leaving it as it is for --questionable-- clarity.

            Tensor3D = Tensor3D.Duplicate
                Tensor3D.TrimFloor(R.Minimum)
                Tensor3D.TrimCeiling(R.Maximum)
                Tensor3D.Remap(R, New Range(0, 255))

                Return Owl.Core.Images.ToBitmap(Tensor3D)
        End Function

        Public Function Tensor3DImage(Tensor3D) As Bitmap
            Return Owl.Core.Images.ToBitmap(Tensor3D)
        End Function

        Public Function Tensor2DImage(Tensor2D) As Bitmap
            Return Owl.Core.Images.ImageConverters.ToGrayscale(Tensor2D)
        End Function

        Public Function Tensor2DImage(Tensor2D As Tensor, R As Range) As Bitmap
            Return Owl.Core.Images.ImageConverters.ToGrayscale(Tensor2D, R)
        End Function

        Public Function TensorSetPlot(TSet As TensorSet, YAxisRange As Range, PlotSize As Size, PenThickness As Single, HighlightTensors As List(Of Integer)) As Bitmap

            Dim ts As TensorSet = TSet.Duplicate()
            Dim vsr As Range = ts.GetRange

            ts.Remap(YAxisRange, New Range(PlotSize.Height, 0))

            Dim Image = New Bitmap(PlotSize.Width, PlotSize.Height)

            Using g As Graphics = Graphics.FromImage(Image)
                g.SmoothingMode = SmoothingMode.HighQuality

                For i As Integer = 0 To ts.Count - 1 Step 1

                    Dim tens As Tensor = ts(i)

                    If tens.Length = 0 Then

                    ElseIf tens.Length = 1 Then
                        Dim thisgp As New GraphicsPath

                        Dim pts(1) As Drawing.Point

                        For j As Integer = 0 To 1 Step 1
                            pts(j) = New Drawing.Point(j * PlotSize.Width, tens(0))
                        Next

                        thisgp.AddLines(pts)

                        Dim rndhsl As New ColorHSL(120, ((i) / (ts.Count - 1)) * 360, 1, 0.5)

                        Using p As Pen = New Pen(Color.FromArgb(100, ColorConversion.HSLToRGB(rndhsl)), PenThickness)
                            p.DashPattern = {4, 4}
                            p.LineJoin = LineJoin.Round
                            p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round)
                            g.DrawPath(p, thisgp)
                        End Using

                        thisgp.Dispose()
                    Else
                        Dim thisgp As New GraphicsPath

                        Dim pts(tens.Length - 1) As Drawing.Point

                        For j As Integer = 0 To tens.Length - 1 Step 1
                            pts(j) = New Drawing.Point(CInt((j / (tens.Length - 1)) * PlotSize.Width), CInt(tens(j)))
                        Next

                        thisgp.AddLines(pts)

                        Dim rndhsl As New ColorHSL(120, ((i) / (ts.Count - 1)) * 360, 1, 0.5)

                        Using p As Pen = New Pen(Color.FromArgb(100, ColorConversion.HSLToRGB(rndhsl)), PenThickness)
                            p.LineJoin = LineJoin.Round
                            p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round)
                            g.DrawPath(p, thisgp)
                        End Using

                        thisgp.Dispose()
                    End If

                Next

                If HighlightTensors IsNot Nothing Then

                    For i As Integer = 0 To HighlightTensors.Count - 1 Step 1
                        Dim v As Tensor = ts(HighlightTensors(i))
                        Dim thisi As Integer = HighlightTensors(i)

                        Dim thisgp As New GraphicsPath

                        Dim pts(v.Count - 1) As Drawing.Point

                        For j As Integer = 0 To v.Count - 1 Step 1
                            pts(j) = New Drawing.Point((j / (v.Count - 1)) * PlotSize.Width, v(j))
                        Next

                        thisgp.AddLines(pts)

                        Using p As Pen = New Pen(Color.FromArgb(120, Color.Black), PenThickness * 5)
                            p.LineJoin = LineJoin.Round
                            p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round)
                            g.DrawPath(p, thisgp)
                        End Using

                        Dim rndhsl As New ColorHSL(120, (thisi) / (ts.Count - 1), 1, 0.5)

                        Using p As Pen = New Pen(Color.FromArgb(100, ColorConversion.HSLToRGB(rndhsl)), PenThickness)
                            g.DrawPath(p, thisgp)
                        End Using

                        thisgp.Dispose()
                    Next

                End If

            End Using

            Return Image
        End Function

        Public Function DrawGridBackground(ImageW As Integer, ImageH As Integer, Optional GridW As Integer = 10, Optional GridH As Integer = 10) As Bitmap
            Dim filler As New Bitmap(GridW, GridH)
            Dim background As New Bitmap(ImageW, ImageH)

            Using g As Graphics = Graphics.FromImage(filler)
                g.SmoothingMode = SmoothingMode.None
                g.InterpolationMode = InterpolationMode.NearestNeighbor
                g.PixelOffsetMode = PixelOffsetMode.None
                g.Clear(Color.White)
                g.DrawRectangle(Pens.LightGray, 0, 0, GridW, GridH)
            End Using

            Using g As Graphics = Graphics.FromImage(background)
                g.SmoothingMode = SmoothingMode.None
                g.InterpolationMode = InterpolationMode.NearestNeighbor
                g.PixelOffsetMode = PixelOffsetMode.None
                Using tx As Brush = New TextureBrush(filler)
                    g.FillRectangle(tx, 0, 0, ImageW, ImageH)
                End Using
            End Using

            filler.Dispose()
            Return background
        End Function

    End Module
End Namespace

