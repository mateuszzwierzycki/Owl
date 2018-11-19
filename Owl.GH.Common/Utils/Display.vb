Imports Owl.Core.Tensors
Imports Rhino.Geometry

Public Module Display

	''' <summary>
	''' Converts a 3D Tensor into a mesh. 
	''' </summary>
	''' <param name="Tens"></param>
	''' <returns></returns>
	Public Function ConvertTensorToMesh(Tens As Tensor) As Mesh
		Dim nm As New Mesh

		Dim pts As New List(Of Point3d)

		For i As Integer = 0 To Tens.Height Step 1
			For j As Integer = 0 To Tens.Width Step 1
				pts.Add(New Point3d(j, i, 0))
			Next
		Next

		nm.Vertices.AddVertices(pts)

		Dim cols(nm.Vertices.Count - 1) As List(Of Double)

		For i As Integer = 0 To cols.Length - 1 Step 1
			cols(i) = New List(Of Double)
		Next

		For i As Integer = 0 To Tens.Height - 1 Step 1
			For j As Integer = 0 To Tens.Width - 1 Step 1

				Dim ta As Integer = j + (i * (Tens.Width + 1))
				Dim tb As Integer = ta + 1
				Dim tc As Integer = ta + Tens.Width + 1
				Dim td As Integer = tc + 1

				Dim val As Double = Tens.ValueAt(i, j)

				cols(ta).Add(val)
				cols(tb).Add(val)
				cols(tc).Add(val)
				cols(td).Add(val)

				nm.Faces.AddFace(ta, tb, td, tc)
			Next
		Next

		For i As Integer = 0 To cols.Length - 1 Step 1
			Dim thisl As List(Of Double) = cols(i)
			Dim av As Double = 0

			For j As Integer = 0 To thisl.Count - 1 Step 1
				av += thisl(j)
			Next

			av /= thisl.Count

			nm.VertexColors.Add(av, av, av)
		Next

		Return nm
	End Function


End Module
