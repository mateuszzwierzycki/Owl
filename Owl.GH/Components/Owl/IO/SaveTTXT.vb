Public Class SaveTTXT
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Save TensorSet", "ToTTXT", "Saves the TensorSet to a text file.", SubCategoryIO)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("bf11179b-33cb-49f0-8e56-6c6ceb2e2670")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_26
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
        pManager.AddParameter(New Param_OwlTensorSave(Owl.Core.IO.OwlFileFormat.TensorText))
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddIntegerParameter("Size", "S", "File size in bytes", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As GH_OwlTensorSet = Nothing
        If Not DA.GetData(0, ts) Then Return
        Dim dir As GH_String = Nothing
        If Not DA.GetData(1, dir) Then Return

        SaveTensorsText(ts.Value, System.IO.Path.GetDirectoryName(dir.Value), System.IO.Path.GetFileNameWithoutExtension(dir.Value))

        Dim nn As New System.IO.FileInfo(dir.Value)
        DA.SetData(0, nn.Length)
    End Sub

End Class
