Imports System.Windows.Forms
Imports GH_IO.Serialization

Public Class SaveIDX
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Save IDX TensorSet", "ToIDX", "Saves the TensorSet in an IDX file.", SubCategoryIO)
        Me.Message = DataType.ToString().Split(".")(1)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("3340c28c-e921-47e8-8beb-4fe65865eeab")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_27
        End Get
    End Property

    Private _split As Type = GetType(Double)

    Public Property DataType As Type
        Get
            Return _split
        End Get
        Set(value As Type)
            _split = value
        End Set
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(menu As ToolStripDropDown)
        Menu_AppendItem(menu, "Byte", AddressOf ChangeAndExpireByte, True, Me.Message = GetType(Byte).ToString().Split(".")(1))
        Menu_AppendItem(menu, "SByte", AddressOf ChangeAndExpireSByte, True, Me.Message = GetType(SByte).ToString.Split(".")(1))
        Menu_AppendItem(menu, "Int16", AddressOf ChangeAndExpireShort, True, Me.Message = GetType(Short).ToString.Split(".")(1))
        Menu_AppendItem(menu, "Int32", AddressOf ChangeAndExpireInt32, True, Me.Message = GetType(Int32).ToString.Split(".")(1))
        Menu_AppendItem(menu, "Single", AddressOf ChangeAndExpireSingle, True, Me.Message = GetType(Single).ToString.Split(".")(1))
        Menu_AppendItem(menu, "Double", AddressOf ChangeAndExpireDouble, True, Me.Message = GetType(Double).ToString.Split(".")(1))

        MyBase.AppendAdditionalComponentMenuItems(menu)
    End Sub

    Sub ChangeAndExpire(newtype As Type)
        DataType = newtype
        Me.Message = DataType.ToString().Split(".")(1)
        Me.ExpireSolution(True)
    End Sub

    Private Sub ChangeAndExpireByte()
        ChangeAndExpire(GetType(Byte))
    End Sub

    Private Sub ChangeAndExpireSByte()
        ChangeAndExpire(GetType(SByte))
    End Sub

    Private Sub ChangeAndExpireSingle()
        ChangeAndExpire(GetType(Single))
    End Sub

    Private Sub ChangeAndExpireShort()
        ChangeAndExpire(GetType(Short))
    End Sub

    Private Sub ChangeAndExpireInt32()
        ChangeAndExpire(GetType(Int32))
    End Sub

    Private Sub ChangeAndExpireDouble()
        ChangeAndExpire(GetType(Double))
    End Sub

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        Select Case reader.GetString("DataType")
            Case GetType(Byte).ToString
                DataType = GetType(Byte)
            Case GetType(SByte).ToString
                DataType = GetType(SByte)
            Case GetType(Short).ToString
                DataType = GetType(Short)
            Case GetType(Int32).ToString
                DataType = GetType(Int32)
            Case GetType(Single).ToString
                DataType = GetType(Single)
            Case GetType(Double).ToString
                DataType = GetType(Double)
        End Select

        Me.Message = DataType.ToString().Split(".")(1)
        Return MyBase.Read(reader)
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        writer.SetString("DataType", DataType.ToString)
        Return MyBase.Write(writer)
    End Function

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
        pManager.AddParameter(New Param_OwlTensorSave(Owl.Core.IO.OwlFileFormat.IDX))
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddIntegerParameter("Size", "S", "File size in bytes", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As GH_OwlTensorSet = Nothing
        If Not DA.GetData(0, ts) Then Return
        Dim dir As String = Nothing
        If Not DA.GetData(1, dir) Then Return

        SaveTensorsIDX(ts.Value, System.IO.Path.GetDirectoryName(dir), System.IO.Path.GetFileNameWithoutExtension(dir), DataType)

        Dim nn As New System.IO.FileInfo(dir)
        DA.SetData(0, nn.Length)
    End Sub

End Class
