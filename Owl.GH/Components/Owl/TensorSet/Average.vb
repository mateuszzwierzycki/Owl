Public Class Average
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Average TSet", "Average", "Get TensorSet average.", SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("91e80eb4-debb-4ac0-83e0-8be46bc04666")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_37
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
        pManager.AddParameter(New Param_OwlTensor)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New GH_OwlTensorSet
        If Not DA.GetData(0, ts) Then Return
        If ts.Value.Count = 0 Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "TensorSet has no Tensors.") : Return
        DA.SetData(0, TensorSet.Average(ts.Value))
    End Sub

End Class
