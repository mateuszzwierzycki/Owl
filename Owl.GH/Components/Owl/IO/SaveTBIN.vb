Public Class SaveTBIN

    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Save TBIN TensorSet", "ToTBIN", "Saves the TensorSet to a binary TBIN file.", SubCategoryIO)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("881c687f-bd0d-48c5-b7c3-ac2469e907cc")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_28
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
        pManager.AddParameter(New Param_OwlTensorSave(Owl.Core.IO.OwlFileFormat.TensorBinary))
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddIntegerParameter("Size", "S", "File size in bytes", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As GH_OwlTensorSet = Nothing
        If Not DA.GetData(0, ts) Then Return
        Dim dir As String = Nothing
        If Not DA.GetData(1, dir) Then Return

        Owl.Core.IO.TensorSerialization.SaveTensorsBinary(ts.Value, System.IO.Path.GetDirectoryName(dir), System.IO.Path.GetFileNameWithoutExtension(dir))

        Dim nn As New System.IO.FileInfo(dir)
        DA.SetData(0, nn.Length)
    End Sub

End Class
