Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Accord.Neuro
Imports Owl.Core.Structures

Namespace Visualization

	Public Module NetworkDrawing

		Public Function NetworkImage(Net As ActivationNetwork, LineThickness As Range, PointRadius As Integer, BitmapSize As Size) As Bitmap

			Dim minv As Double = LineThickness.Minimum
			Dim maxv As Double = LineThickness.Maximum

			LineThickness.From = Math.Max(1, minv)
			LineThickness.To = Math.Max(1, maxv)

			Dim lays As Integer = Net.Layers.Count

			Dim itsx As New Range(0.08 * BitmapSize.Width, BitmapSize.Width - (0.08 * BitmapSize.Width))
			Dim itsy As New Range(0.08 * BitmapSize.Height, BitmapSize.Height - (0.08 * BitmapSize.Height))

			Dim pts As New List(Of List(Of Drawing.PointF))

			Dim wmin As Double = Double.MaxValue
			Dim wmax As Double = Double.MinValue

			Dim px As Single
			Dim py As Single

			pts.Add(New List(Of PointF))

			For i As Integer = 0 To Net.InputsCount - 1 Step 1
				pts(0).Add(New PointF(itsx.ValueAtParameter(0), itsy.ValueAtParameter(i / (Net.InputsCount - 1))))
			Next

			For i As Integer = 0 To Net.Layers.Count - 1 Step 1
				px = itsx.ValueAtParameter((i + 1) / (Net.Layers.Count))
				pts.Add(New List(Of PointF))

				For j As Integer = 0 To Net.Layers(i).Neurons.Count - 1 Step 1
					If (Net.Layers(i).Neurons.Count) = 1 Then
						py = itsy.ValueAtParameter(0.5)
					Else
						py = itsy.ValueAtParameter(j / (Net.Layers(i).Neurons.Count - 1))
					End If

					For k As Integer = 0 To Net.Layers(i).Neurons(j).Weights.Count - 1 Step 1
						wmin = Math.Min(wmin, Net.Layers(i).Neurons(j).Weights(k))
						wmax = Math.Max(wmax, Net.Layers(i).Neurons(j).Weights(k))

					Next

					pts(i + 1).Add(New PointF(px, py))

				Next
			Next

			Dim bmp As New Bitmap(BitmapSize.Width, BitmapSize.Height)
			Dim wit As New Range(wmin, wmax)
			Dim tar As New Range(0.5, 1)
			Dim lit As New Range(1, 0.5)

			Dim nnsums As New List(Of List(Of Double))
			Dim nitvs As New List(Of Range)

			Using g As Graphics = Graphics.FromImage(bmp)
				g.SmoothingMode = SmoothingMode.AntiAlias
				Using thickpen As Pen = New Pen(Color.Black, 3)

					For i As Integer = 1 To pts.Count - 1 Step 1

						Dim nsums As New List(Of Double)
						Dim nmin As Double = Double.MaxValue
						Dim nmax As Double = Double.MinValue

						Dim thislay As ActivationLayer = Net.Layers(i - 1)
						For j As Integer = 0 To pts(i).Count - 1 Step 1
							Dim thisn As ActivationNeuron = Net.Layers(i - 1).Neurons(j)
							Dim thisp As PointF = pts(i)(j)
							Dim thissum As Double = 0

							For k As Integer = 0 To thisn.Weights.Count - 1 Step 1
								Dim thatp As PointF = pts(i - 1)(k)

								thissum += thisn.Weights(k)
								Dim thisparam As Double = wit.ParameterAtValue(thisn.Weights(k))

								Dim hsl As New Owl.Core.Structures.ColorHSL(1.0, tar.ValueAtParameter(thisparam), thisparam, lit.ValueAtParameter(thisparam))

								Using thispen As Pen = New Pen(Color.FromArgb(thisparam * 200, Core.Visualization.HSLToRGB(hsl)), LineThickness.ValueAtParameter(thisparam))
									thispen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round)
									g.DrawLine(thispen, thisp, thatp)
								End Using
							Next

							nsums.Add(thissum)
							nmin = Math.Min(nmin, thissum)
							nmax = Math.Max(nmax, thissum)

						Next

						nnsums.Add(nsums)
						nitvs.Add(New Range(nmin, nmax))

					Next

					If PointRadius >= 1 Then


						Dim ritv As New Range(1, PointRadius)

						For i As Integer = 0 To pts.Count - 1 Step 1

							Dim nitv As Range = Range.Empty

							If i > 0 Then
								nitv = nitvs(i - 1)
							End If

							For j As Integer = 0 To pts(i).Count - 1 Step 1

								If i = 0 Then
									Dim thisr As Double = ritv.ValueAtParameter(0.3)
									thisr = thisr * (BitmapSize.Height / 300)
									g.FillEllipse(Brushes.White, GetCircle(pts(i)(j), thisr))
									g.DrawEllipse(thickpen, GetCircle(pts(i)(j), thisr))
								Else
									Dim thisr As Double = nnsums(i - 1)(j)
									thisr = nitv.ParameterAtValue(thisr)
									thisr = Math.Min(thisr, 1.5)
									thisr = ritv.ValueAtParameter(thisr) * (BitmapSize.Height / 300)
									thisr = Math.Max(thisr, 2)

									g.FillEllipse(Brushes.White, GetCircle(pts(i)(j), thisr))
									g.DrawEllipse(thickpen, GetCircle(pts(i)(j), thisr))


								End If
							Next
						Next
					End If
				End Using
			End Using

			Return bmp
		End Function

		Public Function NetworkImage(Net As ActivationNetwork, LineThickness As Range, PointRadius As Integer, BitmapSize As Integer) As Bitmap

			Dim minv As Double = LineThickness.Minimum
			Dim maxv As Double = LineThickness.Maximum

			LineThickness.From = Math.Max(1, minv)
			LineThickness.To = Math.Max(1, maxv)

			Dim lays As Integer = Net.Layers.Count

			Dim its As New Range(0.08 * BitmapSize, BitmapSize - (0.08 * BitmapSize))

			Dim pts As New List(Of List(Of Drawing.PointF))

			Dim wmin As Double = Double.MaxValue
			Dim wmax As Double = Double.MinValue

			Dim px As Single
			Dim py As Single

			pts.Add(New List(Of PointF))

			For i As Integer = 0 To Net.InputsCount - 1 Step 1
				pts(0).Add(New PointF(its.ValueAtParameter(0), its.ValueAtParameter(i / (Net.InputsCount - 1))))
			Next

			For i As Integer = 0 To Net.Layers.Count - 1 Step 1
				px = its.ValueAtParameter((i + 1) / (Net.Layers.Count))
				pts.Add(New List(Of PointF))

				For j As Integer = 0 To Net.Layers(i).Neurons.Count - 1 Step 1
					If (Net.Layers(i).Neurons.Count) = 1 Then
						py = its.ValueAtParameter(0.5)
					Else
						py = its.ValueAtParameter(j / (Net.Layers(i).Neurons.Count - 1))
					End If

					For k As Integer = 0 To Net.Layers(i).Neurons(j).Weights.Count - 1 Step 1
						wmin = Math.Min(wmin, Net.Layers(i).Neurons(j).Weights(k))
						wmax = Math.Max(wmax, Net.Layers(i).Neurons(j).Weights(k))

					Next

					pts(i + 1).Add(New PointF(px, py))

				Next
			Next

			Dim bmp As New Bitmap(BitmapSize, BitmapSize)
			Dim wit As New Range(wmin, wmax)
			Dim tar As New Range(0.5, 1)
			Dim lit As New Range(1, 0.5)

			Dim nnsums As New List(Of List(Of Double))
			Dim nitvs As New List(Of Range)

			Using g As Graphics = Graphics.FromImage(bmp)
				g.SmoothingMode = SmoothingMode.AntiAlias
				Using thickpen As Pen = New Pen(Color.Black, 3)

					For i As Integer = 1 To pts.Count - 1 Step 1

						Dim nsums As New List(Of Double)
						Dim nmin As Double = Double.MaxValue
						Dim nmax As Double = Double.MinValue

						Dim thislay As ActivationLayer = Net.Layers(i - 1)
						For j As Integer = 0 To pts(i).Count - 1 Step 1
							Dim thisn As ActivationNeuron = Net.Layers(i - 1).Neurons(j)
							Dim thisp As PointF = pts(i)(j)
							Dim thissum As Double = 0

							For k As Integer = 0 To thisn.Weights.Count - 1 Step 1
								Dim thatp As PointF = pts(i - 1)(k)

								thissum += thisn.Weights(k)
								Dim thisparam As Double = wit.ParameterAtValue(thisn.Weights(k))

								Dim hsl As New Owl.Core.Structures.ColorHSL(1.0, tar.ValueAtParameter(thisparam), thisparam, lit.ValueAtParameter(thisparam))

								Using thispen As Pen = New Pen(Color.FromArgb(thisparam * 200, Core.Visualization.HSLToRGB(hsl)), LineThickness.ValueAtParameter(thisparam))
									thispen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round)
									g.DrawLine(thispen, thisp, thatp)
								End Using
							Next

							nsums.Add(thissum)
							nmin = Math.Min(nmin, thissum)
							nmax = Math.Max(nmax, thissum)

						Next

						nnsums.Add(nsums)
						nitvs.Add(New Range(nmin, nmax))

					Next

					If PointRadius >= 1 Then


						Dim ritv As New Range(1, PointRadius)

						For i As Integer = 0 To pts.Count - 1 Step 1

							Dim nitv As Range = Range.Empty

							If i > 0 Then
								nitv = nitvs(i - 1)
							End If

							For j As Integer = 0 To pts(i).Count - 1 Step 1

								If i = 0 Then
									Dim thisr As Double = ritv.ValueAtParameter(0.3)
									thisr = thisr * (BitmapSize / 300)
									g.FillEllipse(Brushes.White, GetCircle(pts(i)(j), thisr))
									g.DrawEllipse(thickpen, GetCircle(pts(i)(j), thisr))
								Else
									Dim thisr As Double = nnsums(i - 1)(j)
									thisr = nitv.ParameterAtValue(thisr)
									thisr = Math.Min(thisr, 1.5)
									thisr = ritv.ValueAtParameter(thisr) * (BitmapSize / 300)
									thisr = Math.Max(thisr, 2)

									g.FillEllipse(Brushes.White, GetCircle(pts(i)(j), thisr))
									g.DrawEllipse(thickpen, GetCircle(pts(i)(j), thisr))


								End If
							Next



						Next

					End If

				End Using

			End Using

			Return bmp
		End Function

		Private Function GetCircle(P As PointF, R As Double) As RectangleF
			Return New RectangleF(P.X - R / 2, P.Y - R / 2, R, R)
		End Function

	End Module
End Namespace
