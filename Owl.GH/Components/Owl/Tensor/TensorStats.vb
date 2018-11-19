Public Class TensorStats
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Tensor Stats", "Stats", "Get various information about the Tensor.", SubCategoryTensor)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("21802003-26c2-4839-a4c7-a591cba6ec4a")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_33
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.quarternary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensor, "Tensor", "T", "Tensor", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddIntegerParameter("Count", "C", "Tensor count", GH_ParamAccess.item)
        pManager.AddIntegerParameter("Shape", "S", "Tensor shape", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New GH_OwlTensor
        If Not DA.GetData(0, ts) Then Return

        DA.SetData(0, ts.Value.Length)
        DA.SetDataList(1, ts.Value.GetShape)
    End Sub
End Class
