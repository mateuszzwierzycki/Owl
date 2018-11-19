Public Class DeconstructTensorSet
    Inherits OwlComponentBase

    Sub New()
		MyBase.New("Deconstruct TensorSet",
				   "DeTSet",
				   "Deconstruct Owl TensorSet",
				   SubCategoryPrimitive)
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{3926ADFF-A0C2-4D1F-9045-1BEFFB141CB7}")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor, "Tensors", "T", "Tensors", GH_ParamAccess.list)
    End Sub

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_11
        End Get
    End Property

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New TensorSet
        If Not DA.GetData(0, ts) Then Return
        Dim nl As New List(Of Tensor)
        For i As Integer = 0 To ts.Count - 1 Step 1
            nl.Add(ts(i))
        Next

        DA.SetDataList(0, nl)
    End Sub
End Class
