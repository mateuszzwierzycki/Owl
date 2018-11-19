Public Class ConstructFromDataTree
    Inherits OwlComponentBase

    Sub New()
		MyBase.New("DataTree to TensorSet",
				   "DT->TS",
				   "Convert a DataTree of Numbers into a TensorSet",
				   SubCategoryConvert)
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("53d01ca3-8de7-4698-8fe6-b0e83ecd6a87")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddNumberParameter("DataTree", "D", "Numbers", GH_ParamAccess.tree)
    End Sub

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_09
        End Get
    End Property

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim dt As New GH_Structure(Of GH_Number)

        If Not DA.GetDataTree(0, dt) Then Return

        Dim ts As New TensorSet()
        For i As Integer = 0 To dt.Branches.Count - 1 Step 1
            Dim thist As New Tensor(dt.Branches(i).Count)
            For j As Integer = 0 To dt.Branches(i).Count - 1 Step 1
                thist(j) = dt.Branches(i)(j).Value
            Next
            ts.Add(thist)
        Next

        DA.SetData(0, ts)
    End Sub
End Class
