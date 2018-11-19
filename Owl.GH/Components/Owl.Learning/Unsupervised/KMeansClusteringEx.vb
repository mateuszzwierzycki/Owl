Public Class KMeansClusteringEx
	Inherits OwlComponentBase

	Sub New()
		MyBase.New("KMeans Clustering Ex",
				   "KMeansEx",
				   "A KMeans clustering component",
				   "Owl.Learning",
				   SubCategoryUnsupervised)
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("da0f7c91-5f1b-460c-b34b-7f6dfe26ad96")
		End Get
	End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.icon_49
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.primary
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddParameter(New Param_OwlTensorSet, "Data", "D", "Data to cluster", GH_ParamAccess.item)
		pManager.AddParameter(New Param_OwlTensorSet, "Seeds", "S", "Seeds to start from", GH_ParamAccess.item)
		pManager.AddIntegerParameter("Iterations", "I", "Iterations", GH_ParamAccess.item, 1)
		pManager.AddNumberParameter("Energy", "E", "Optional target energy", GH_ParamAccess.item, Double.Epsilon)
		pManager.AddNumberParameter("Radius", "R", "Optional radius for each seed", GH_ParamAccess.list)
		pManager.AddNumberParameter("Weigths", "W", "Optional dimension weights", GH_ParamAccess.list)

		Me.Params.Input(3).Optional = True
		Me.Params.Input(4).Optional = True
		Me.Params.Input(5).Optional = True
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddParameter(New Param_OwlTensorSet, "Clusters", "C", "Data split into clusters", GH_ParamAccess.list)
		pManager.AddIntegerParameter("Indices", "I", "Indices", GH_ParamAccess.tree)
		pManager.AddIntegerParameter("Iterations", "I", "Iterations executed", GH_ParamAccess.item)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim data As New TensorSet
		Dim seeds As New TensorSet
		Dim iter As Integer

		Dim energy As Double
		Dim radius As New List(Of Double)
		Dim weights As New List(Of Double)

		If Not DA.GetData(0, data) Then Return
		If Not data.IsHomogeneous Then
			Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "TensorSet is not homogeneous")
			Exit Sub
		End If

		If Not DA.GetData(1, seeds) Then Return
		If Not seeds.IsHomogeneous Then
			Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Seeds are notS homogeneous")
			Exit Sub
		End If

		If seeds(0).Length <> data(0).Length Then
			Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Data dimensionality <> Seed dimensionality")
			Exit Sub
		End If

		If Not DA.GetData(2, iter) Then Return

		DA.GetData(3, energy)
		DA.GetDataList(4, radius)

		If radius.Count <> seeds.Count And radius.Count > 0 Then
			Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Radius count <> Seed count")
			Exit Sub
		End If

		DA.GetDataList(5, weights)

		If weights.Count <> data(0).Length And weights.Count > 0 Then
			Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Weight count <> Dimension count")
			Exit Sub
		End If

		Dim nkm As New KMeansEngine(data)

		For i As Integer = 0 To seeds.Count - 1 Step 1
			nkm.AddSeed(seeds(i))
		Next

		For i As Integer = 0 To weights.Count - 1 Step 1
			nkm.DimensionWeights(i) = weights(i)
		Next

		For i As Integer = 0 To radius.Count - 1 Step 1
			nkm.SeedWeights(i) = radius(i)
		Next

		Dim cnt As Integer
		For i As Integer = 0 To iter - 1 Step 1
			cnt += 1
			If nkm.RunOnce() < energy Then Exit For
		Next


		DA.SetDataList(0, nkm.SplitIntoSets)

		Dim dto As New DataTree(Of Integer)
		Dim thisp As GH_Path = DA.ParameterTargetPath(1)
		thisp = thisp.AppendElement(DA.ParameterTargetIndex(0))

		Dim lll() As List(Of Integer) = nkm.SplitAsIndices
		For i As Integer = 0 To lll.Length - 1 Step 1
			dto.AddRange(lll(i), thisp.AppendElement(i))
		Next

		DA.SetDataTree(1, dto)
		DA.SetData(2, cnt)
	End Sub
End Class
