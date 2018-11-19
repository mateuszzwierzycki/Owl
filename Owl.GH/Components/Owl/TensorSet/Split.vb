Public Class Split
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Split TSet", "SplitTS", "Split TensorSet.", SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{BBCB3AB6-003B-45BE-A1B7-3B498312A36F}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_44
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
        pManager.AddNumberParameter("Proporions", "P", "Proportions", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "TensorSets", Param_OwlTensorSet.Nick, "TensorSets", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New GH_OwlTensorSet
        Dim pro As New List(Of Double)

        If Not DA.GetData(0, ts) Then Return
        If Not DA.GetDataList(1, pro) Then Return

        DA.SetDataList(0, ts.Value.Duplicate.Split(pro))
    End Sub
End Class
