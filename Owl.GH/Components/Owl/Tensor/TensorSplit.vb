Public Class TensorSplit
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Tensor Split", "SplitT", "Splits Tensor by it's leftmost dimension.", SubCategoryTensor)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("a0564663-1cfd-4661-8b9d-de784b07555c")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_35
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.tertiary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensor)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim va As New GH_OwlTensor
        If Not DA.GetData(0, va) Then Return
        DA.SetData(0, va.Value.SplitIntoSet)
    End Sub
End Class
