Public Class TensorAdd
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Tensor Addition", "Add", "Tensor Addition.", SubCategoryTensor)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("9b4e26fd-4dff-4d0e-8044-d83568802a60")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_31
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

        DA.SetData(0, va.Value + vb.Value)
    End Sub
End Class
