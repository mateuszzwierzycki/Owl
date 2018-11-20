Public Class Matrix2QMatrix
	Inherits GH_Component

	Sub New()
		MyBase.New("Matrix2QMatrix", "M2Q", "Convert a spare adjacency matrix to QMatrix.", "Owl.Learning", "Reinforced")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{CCB6854D-A62A-4401-B1DD-721E8E73FDEC}")
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddIntegerParameter("Matrix", "M", "Sparse adjacency matrix", GH_ParamAccess.tree)
		pManager.AddIntegerParameter("Connection", "C", "Connection value", GH_ParamAccess.item, 0)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddParameter(New Param_OwlTensor, "QMatrix", "Q", "QMatrix", GH_ParamAccess.item)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim dt As New GH_Structure(Of GH_Integer)
		Dim connectionValue As Integer = 0

		If Not DA.GetDataTree(0, dt) Then Return
		If Not DA.GetData(1, connectionValue) Then Return

		'Dim tensout As New Tensor(dt.Branch(0).Count, pat.Count)

		'actions,states
		Dim tensout As New Tensor(dt.Branches.Count, dt.Branches.Count)

		For i As Integer = 0 To tensout.Height - 1
			For j As Integer = 0 To tensout.Width - 1
				tensout.ValueAt(i, j) = -1
			Next
		Next

		For i As Integer = 0 To dt.Branches.Count - 1
			Dim thislist As IList(Of GH_Integer) = dt.Branches(i)

			For j As Integer = 0 To thislist.Count - 1
				tensout.ValueAt(i, thislist(j).Value) = connectionValue
			Next
		Next

		DA.SetData(0, tensout)
	End Sub
End Class
