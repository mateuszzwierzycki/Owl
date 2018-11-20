Public Class MarkovChain
	Inherits OwlComponentBase

	Sub New()
		MyBase.New("Markov Chain",
				   "MChain",
				   "Markov Chain series generator",
				   "Owl.Learning",
				   SubCategoryUnsupervised)
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("574b21e5-3e08-4fe7-800d-e03b89fdb072")
		End Get
	End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.icon_36
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.tertiary
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddGenericParameter("Source", "L", "Source for probability calculations. Inputs a list of sortable data here.", GH_ParamAccess.list)
		pManager.AddGenericParameter("Start", "S", "Item to start the sequence from", GH_ParamAccess.item)
		pManager.AddIntegerParameter("Count", "C", "Number of elements in the resulting sequence", GH_ParamAccess.item, 10)
		pManager.AddIntegerParameter("Seed", "R", "Randomness seed", GH_ParamAccess.item, 123)
		pManager.AddBooleanParameter("Dead-end", "D", "Prevent dead-ends", GH_ParamAccess.item, True)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddGenericParameter("Sequence", "S", "Generated sequence", GH_ParamAccess.list)
		pManager.AddIntegerParameter("Matrix", "M", "Matrix", GH_ParamAccess.tree)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim src As New List(Of Object)
		Dim it As Object = Nothing
		Dim cnt As Integer
		Dim see As Integer
		Dim dea As Boolean

		If Not DA.GetDataList(0, src) Then Return
		If Not DA.GetData(1, it) Then Return
		If Not DA.GetData(2, cnt) Then Return
		If Not DA.GetData(3, see) Then Return
		If Not DA.GetData(4, dea) Then Return


		Dim conv As New List(Of IComparable)

		For i As Integer = 0 To src.Count - 1 Step 1
			conv.Add(src(i).Value)
		Next

		Dim asicom As IComparable = Nothing

		If it Is Nothing Then
			asicom = conv(0)
		Else
			asicom = it.Value
		End If

		If GetType(IComparable).IsAssignableFrom(conv(0).GetType) Then
			Dim thistype As Type = conv(0).GetType
			Dim mc As New MarkovChain(Of IComparable)(conv, see)
			mc.AddChain(conv)
			DA.SetDataList(0, mc.GetSeries(asicom, cnt, dea))

			'For i As Integer = 1 To Chain.Count - 1 Step 1
			'	Dim thisk As T = Chain(i - 1)
			'	Dim thisv As T = Chain(i)
			'	m_prob(m_dict(thisk), m_dict(thisv)) += 1

			Dim dt As New GH_Structure(Of GH_Integer)
			Dim pth As GH_Path = DA.ParameterTargetPath(1)
			Dim values(,) As Integer = mc.Probabilities

			For i As Integer = 0 To values.GetUpperBound(0)
				For j As Integer = 0 To values.GetUpperBound(1)
					dt.Append(New GH_Integer(values(i, j)), pth.AppendElement(i))
				Next
			Next

			DA.SetDataTree(1, dt)
		Else
			Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "This type of data is not comparable.")
		End If
	End Sub


End Class
