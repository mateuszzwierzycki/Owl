Public Class LoadText
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Load TensorSet", "FromText", "Loads the TensorSet from a text file.", "I/O")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("ae41466f-7350-4297-8b9d-6968e1c01bb6")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorLoad(Owl.Core.IO.OwlFileFormat.TensorText))
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim dir As GH_String = Nothing
        If Not DA.GetData(0, dir) Then Return

        Dim ts As TensorSet = Owl.Core.IO.TensorSerialization.LoadTensorsText(dir.Value, "*")
        DA.SetData(0, ts)
    End Sub

End Class
