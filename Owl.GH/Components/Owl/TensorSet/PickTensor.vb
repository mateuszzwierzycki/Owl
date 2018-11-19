Public Class PickTensor
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Pick Tensor", "Pick", "Pick single Tensors out of the TensorSet.", SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("3cfbc342-6ff8-4c78-8598-50dd1b1fec47")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_41
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
        pManager.AddIntegerParameter("Indices", "i", "Indices of the Tensors to pick", GH_ParamAccess.list, 0)
        pManager.AddBooleanParameter("Wrap", "W", "Wrap indices", GH_ParamAccess.item, True)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New GH_OwlTensorSet
        Dim indices As New List(Of Integer)
        Dim wr As Boolean

        If Not DA.GetData(0, ts) Then Return
        If Not DA.GetDataList(1, indices) Then Return
        If Not DA.GetData(2, wr) Then Return

        Dim outs As New List(Of Tensor)

        For Each index In indices
            If wr Then outs.Add((ts.Value((index + ts.Value.Count) Mod ts.Value.Count)).Duplicate) : Continue For
            If index >= ts.Value.Count Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Index out of bounds") : Continue For
            outs.Add(ts.Value(index).Duplicate)
        Next

        DA.SetDataList(0, outs)
    End Sub
End Class
