Imports Rhino.Geometry

Public Class ClusterDirections
	Inherits GH_Component

	Sub New()
		MyBase.New("Cluster Lines",
				   "ClusterL",
				   "Cluster lines",
				   "Owl.Learning",
				   "Unsupervised")
	End Sub

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_20
		End Get
	End Property

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{5D9D9D60-F38A-4663-A6B4-4EEDD9769499}")
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddLineParameter("Lines", "L", "Lines to cluster", GH_ParamAccess.list)
		pManager.AddIntegerParameter("Clusters", "C", "Clusters", GH_ParamAccess.item, 3)
		pManager.AddIntegerParameter("Iterations", "I", "Iterations", GH_ParamAccess.item, 1)
		pManager.AddIntegerParameter("Seed", "S", "Seed", GH_ParamAccess.item, 123)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddIntegerParameter("Clusters", "C", "Clusters", GH_ParamAccess.tree)
		pManager.AddParameter(New Param_OwlTensor, "Seeds", "S", "Seeds", GH_ParamAccess.list)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim lines As New List(Of Line)
		Dim clusters As Integer
		Dim iterations As Integer
		Dim seed As Integer

		If Not DA.GetDataList(0, lines) Then Return
		If Not DA.GetData(1, clusters) Then Return
		If Not DA.GetData(2, iterations) Then Return
		If Not DA.GetData(3, seed) Then Return

		If clusters > lines.Count Then
			Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "nope")
			Return
		End If

		Dim tens As New TensorSet

		For i As Integer = 0 To lines.Count - 1 Step 1
			tens.Add(New Tensor({lines(i).From.X, lines(i).From.Y, lines(i).From.Z, lines(i).To.X, lines(i).To.Y, lines(i).To.Z}))
		Next

		Dim km As New Owl.Learning.Clustering.KMeansEngineMetric(tens, AddressOf MeasureDistance, AddressOf AverageTensors)

		Dim rnd As New Random(seed)
		Dim pick As New HashSet(Of Integer)
		Dim safe As Integer = 0

		While pick.Count < clusters
			If safe > 100000 Then Exit Sub
			pick.Add(rnd.Next(0, tens.Count))
			safe += 1
		End While

		For Each p As Integer In pick
			km.AddSeed(tens(p).Duplicate)
		Next

		For i As Integer = 0 To iterations - 1 Step 1
			km.RunOnce()
		Next

		Dim sorted() As List(Of Integer) = km.SplitAsIndices

		Dim dt As New GH_Structure(Of GH_Integer)

		For i As Integer = 0 To sorted.Count - 1 Step 1
			For j As Integer = 0 To sorted(i).Count - 1 Step 1
				dt.Append(New GH_Integer(sorted(i)(j)), New GH_Path(i))
			Next
		Next

		DA.SetDataTree(0, dt)

		Dim outs As New List(Of GH_OwlTensor)

		For Each ts As Tensor In km.GetSeeds
			outs.Add(New GH_OwlTensor(ts))
		Next

		DA.SetDataList(1, outs)

	End Sub

	Public Function AverageTensors(TS As IEnumerable(Of Tensor)) As Tensor
		Dim pts As New List(Of Point3d)
		Dim maxdist As Double = 0

		For i As Integer = 0 To TS.Count - 1 Step 1
			Dim tens As Tensor = TS(i)
			Dim pt As Point3d = Point3d.Origin + (New Vector3d(tens(0), tens(1), tens(2)) - New Vector3d(tens(3), tens(4), tens(5)))
			maxdist = Math.Max(maxdist, Point3d.Origin.DistanceTo(pt))
			pts.Add(-pt)
			pts.Add(pt)
		Next

		For i As Integer = 0 To pts.Count - 1 Step 1
			pts(i) = pts(i) * maxdist * 10
		Next

		Dim nl As New Line
		Line.TryFitLineToPoints(pts, nl)

		Dim nlv As Vector3d = nl.To - Point3d.Origin
		nlv.Unitize()
		nlv *= maxdist
		nl = New Line(New Point3d(-nlv), New Point3d(nlv))
		Return New Tensor({nl.From.X, nl.From.Y, nl.From.Z, nl.To.X, nl.To.Y, nl.To.Z})

	End Function

	Public Function MeasureDistance(TA As Tensor, TB As Tensor) As Double
		Dim a1 As New Point3d(TA(0), TA(1), TA(2))
		Dim a2 As New Point3d(TA(3), TA(4), TA(5))

		Dim b1 As New Point3d(TB(0), TB(1), TB(2))
		Dim b2 As New Point3d(TB(3), TB(4), TB(5))

		Dim va As Vector3d = a2 - a1
		Dim vb1 As Vector3d = b2 - b1
		Dim vb2 As Vector3d = b1 - b2

		Return Math.Min(Vector3d.VectorAngle(va, vb1), Vector3d.VectorAngle(va, vb2))
	End Function

End Class
