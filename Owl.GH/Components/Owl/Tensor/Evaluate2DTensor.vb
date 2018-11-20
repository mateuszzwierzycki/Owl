Imports Rhino.Geometry

Public Class Evaluate2DTensor
	Inherits GH_Component

	Sub New()
		MyBase.New("Evaluate 2D", "Eval2D", "Evaluate 2D Tensor", "Owl", "Image")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{2F677085-A7FC-425B-B797-4A51BE9E54F5}")
		End Get
	End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_01
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddParameter(New Param_OwlTensor)
		pManager.AddPointParameter("Points", "Q", "Evaluate coordinates", GH_ParamAccess.list)
		pManager.AddBooleanParameter("Reparametrize", "R", "Reparametrize the Tensor size", GH_ParamAccess.item, True)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddNumberParameter("Values", "V", "Values at parameters", GH_ParamAccess.list)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim tens As Tensor = Nothing
		If Not DA.GetData(0, tens) Then Return

		Dim pts As New List(Of Point3d)
		If Not DA.GetDataList(1, pts) Then Return

		Dim rep As Boolean = True
		If Not DA.GetData(2, rep) Then Return

		Dim iw As Interval = New Interval(0, tens.Width - 1)
		Dim ih As Interval = New Interval(0, tens.Height - 1)

		Dim vals As New List(Of GH_Number)

		For Each p As Point3d In pts
			Dim thisw As Integer = -1
			Dim thish As Integer = -1

			If rep Then
				thisw = iw.ParameterAt(p.X)
				thish = ih.ParameterAt(p.Y)
			Else
				thisw = p.X
				thish = p.Y
			End If

			vals.Add(New GH_Number(tens.ValueAt(thish, thisw)))
		Next

		DA.SetDataList(0, vals)
	End Sub

End Class
