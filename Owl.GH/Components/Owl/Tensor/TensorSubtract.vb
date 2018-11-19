Public Class TensorSubtract
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Tensor Subtraction", "Subtract", "Tensor Subtraction.", SubCategoryTensor)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("235928e5-f6b9-4747-81d3-4815b6bb5426")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_32
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensor, "Tensor", "A", "Tensor", GH_ParamAccess.item)
        pManager.AddParameter(New Param_OwlTensor, "Tensor", "B", "Tensor", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor, "Tensor", "C", "Tensor", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim va As New GH_OwlTensor
        If Not DA.GetData(0, va) Then Return

        Dim vb As New GH_OwlTensor
        If Not DA.GetData(1, vb) Then Return

        DA.SetData(0, va.Value - vb.Value)
    End Sub
End Class