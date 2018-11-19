Imports System.Windows.Forms
Imports GH_IO.Serialization

Public Class LoadIDX
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Load IDX TensorSet", "FromIDX", "Loads the TensorSet from a binary IDX file.", "I/O")
        Me.Message = If(SplitTensor, "Split", "Set")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("2abb45ad-bb6e-4623-af83-b56af6641a14")
        End Get
    End Property

    Public Property SplitTensor As Boolean
        Get
            Return _split
        End Get
        Set(value As Boolean)
            _split = value
        End Set
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(menu As ToolStripDropDown)
        Menu_AppendItem(menu, "Split", AddressOf Switch, True, SplitTensor)
        MyBase.AppendAdditionalComponentMenuItems(menu)
    End Sub

    Private _split As Boolean = True

    Public Sub Switch()
        SplitTensor = Not SplitTensor
        Me.Message = If(SplitTensor, "Split", "Set")
        Me.ExpireSolution(True)
    End Sub

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        SplitTensor = reader.GetBoolean("Split")
        Me.Message = If(SplitTensor, "Split", "Set")
        Return MyBase.Read(reader)
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        writer.SetBoolean("Split", SplitTensor)
        Return MyBase.Write(writer)
    End Function

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorLoad(Owl.Core.IO.OwlFileFormat.IDX))
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim dir As GH_String = Nothing
        If Not DA.GetData(0, dir) Then Return

        Dim ts As TensorSet = Nothing

        If SplitTensor Then
            ts = New TensorSet(dir.Value)
        Else
            ts = New TensorSet
            ts.Add(LoadTensorIDX(dir.Value))
        End If

        DA.SetData(0, ts)
    End Sub

End Class
