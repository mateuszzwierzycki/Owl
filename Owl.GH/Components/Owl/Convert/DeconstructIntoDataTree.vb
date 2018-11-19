Public Class DeconstructIntoDataTree
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("TensorSet to DataTree", "TS->DT", "Deconstruct Owl TensorSet into a DataTree", OwlComponentBase.SubCategoryConvert)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("df08953d-2713-4f6b-b046-e8cdfe8e905a")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddNumberParameter("DataTree", "D", "DataTree", GH_ParamAccess.tree)
    End Sub

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_08
        End Get
    End Property

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New GH_OwlTensorSet
        If Not DA.GetData(0, ts) Then Return

        Dim dt As New DataTree(Of Double)
        Dim thisp As GH_Path = DA.ParameterTargetPath(0)
        thisp.AppendElement(DA.ParameterTargetIndex(0))

        For i As Integer = 0 To ts.Value.Count - 1 Step 1
            dt.AddRange(ts.Value(i), thisp.AppendElement(i))
        Next

        DA.SetDataTree(0, dt)
    End Sub
End Class
