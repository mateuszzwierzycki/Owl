Imports Grasshopper.Kernel.Parameters

Public Class LoadTBIN
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Load TBIN TensorSet", "FromTBIN", "Loads the TensorSet from a binary TBIN file.", "I/O")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("338729fc-682f-406f-8947-1a8675462034")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorLoad(Owl.Core.IO.OwlFileFormat.TensorBinary))
        pManager.AddIntegerParameter("Position", "P", "Tensor from which to start loading", GH_ParamAccess.item, 0)
        pManager.AddIntegerParameter("Count", "C", "Number of Tensors to load, -1 to load all", GH_ParamAccess.item, -1)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim dir As GH_String = Nothing
        If Not DA.GetData(0, dir) Then Return

        Dim pos As Integer
        Dim len As Integer = -1

        If Not DA.GetData(1, pos) Then Return
        If Not DA.GetData(2, len) Then Return

        Dim ts As TensorSet = Nothing

        If len = -1 Then
            ts = Owl.Core.IO.TensorSerialization.LoadTensorsBinary(dir.Value)
        Else
            ts = Owl.Core.IO.TensorSerialization.LoadTensorsBinary(dir.Value, pos, len)
        End If


        DA.SetData(0, ts)
    End Sub

End Class
