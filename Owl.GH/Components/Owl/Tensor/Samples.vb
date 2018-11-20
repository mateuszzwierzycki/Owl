Imports Rhino.Geometry

Public Class Samples
	Inherits GH_Component

	Sub New()
		MyBase.New("Samples", "Samples", "Create sampling frames", "Owl", "Image")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{A356EA1A-7487-4FC3-91B0-B8B4D04BE924}")
		End Get
	End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_04
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddParameter(New Param_OwlTensor)
		pManager.AddIntegerParameter("Size", "S", "Size", GH_ParamAccess.item)
		pManager.AddIntegerParameter("Step", "D", "Step", GH_ParamAccess.item)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddRectangleParameter("Frames", "F", "Sampling frames", GH_ParamAccess.list)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim tens As GH_OwlTensor = Nothing
		If Not DA.GetData(0, tens) Then Return

		Dim sz As Integer = -1
		If Not DA.GetData(1, sz) Then Return
		If sz < 1 Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Size < 1") : Return

		Dim st As Integer = -1
		If Not DA.GetData(2, st) Then Return
		If st < 1 Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Step < 1") : Return

		Dim img As Tensor = tens.Value
		Dim w As Integer = img.Width
		Dim h As Integer = img.Height

		Dim cntw As Integer = Math.Floor((w - (sz - st)) / st)
		Dim ww As Integer = Math.Floor((w - ((sz - st) + (cntw * st))) / 2)

		Dim cnth As Integer = Math.Floor((h - (sz - st)) / st)
		Dim hh As Integer = Math.Floor((h - ((sz - st) + (cnth * st))) / 2)

		Dim rects As New GH_Structure(Of GH_Rectangle)

		Dim thisp As GH_Path = DA.ParameterTargetPath(0)
		thisp.AppendElement(DA.ParameterTargetIndex(0))

		For i As Integer = 0 To cnth - 1 Step 1
			For j As Integer = 0 To cntw - 1 Step 1
				Dim tp As New Plane((New Point3d((st * j) + ww, (st * i) + hh, 0)), Vector3d.ZAxis)
				rects.Append(New GH_Rectangle(New Rectangle3d(tp, sz, sz)), thisp.AppendElement(i))
			Next
		Next

		DA.SetDataTree(0, rects)

	End Sub

End Class
