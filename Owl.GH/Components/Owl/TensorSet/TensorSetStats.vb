Public Class TensorSetStats
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("TensorSet Stats", "Stats", "Get various information about the TensorSet.", SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{635ADE82-A419-48AD-AE28-8F2C86952A20}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_45
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddIntegerParameter("Count", "C", "Tensor count", GH_ParamAccess.item)
        pManager.AddBooleanParameter("Homogeneity", "H", "TensorSet homogeneity", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New GH_OwlTensorSet

        If Not DA.GetData(0, ts) Then Return

        DA.SetData(0, ts.Value.Count)
        DA.SetData(1, ts.Value.IsHomogeneous)
    End Sub
End Class
