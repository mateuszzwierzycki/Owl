Public Class DeconstructTensor
    Inherits OwlComponentBase

    Sub New()
		MyBase.New("Deconstruct Tensor",
				   "DeTensor",
				   "Deconstruct Owl Tensor",
				   SubCategoryPrimitive)
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{AFB830DB-5E50-4464-B82F-4B062F8BE7BF}")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensor)
    End Sub

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_05
        End Get
    End Property

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddNumberParameter("Values", "V", "Values", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim tens As New Tensor
        If Not DA.GetData(0, tens) Then Return
        DA.SetDataList(0, tens.ToArray)
    End Sub
End Class
