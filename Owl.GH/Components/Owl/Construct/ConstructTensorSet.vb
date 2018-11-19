Public Class ConstructTensorSet
	Inherits OwlComponentBase

	Sub New()
		MyBase.New("Construct TensorSet",
				   "TensorSet",
				   "Construct Owl TensorSet",
				   SubCategoryPrimitive)
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{32B1C464-8D93-4917-9AA2-837BEA79B735}")
		End Get
	End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return icon_12
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.secondary
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddParameter(New Param_OwlTensor, "Tensors", "T", "Tensors", GH_ParamAccess.list)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddParameter(New Param_OwlTensorSet)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim l As New List(Of Tensor)
		If Not DA.GetDataList(0, l) Then Return
		DA.SetData(0, New TensorSet(l))
	End Sub
End Class
