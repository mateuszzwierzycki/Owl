Public Class ConstructTensor
	Inherits OwlComponentBase

	Sub New()
		MyBase.New("Construct Tensor",
				   "Tensor",
				   "Construct Owl Tensor",
				   SubCategoryPrimitive)
	End Sub

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.icon_04
		End Get
	End Property

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{37D6ADA4-8E54-4853-A06E-1C0B43E7205A}")
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddNumberParameter("Values", "V", "Values", GH_ParamAccess.list)
	End Sub

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.primary
		End Get
	End Property

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddParameter(New Param_OwlTensor)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim l As New List(Of Double)
		If Not DA.GetDataList(0, l) Then Return
		DA.SetData(0, New Tensor(l))
	End Sub
End Class
