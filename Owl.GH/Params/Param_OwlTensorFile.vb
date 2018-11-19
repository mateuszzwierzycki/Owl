Imports System.Windows.Forms
Imports GH_IO.Serialization

Public Class Param_OwlTensorLoad
    Inherits GH_PersistentParam(Of GH_OwlFilePath)

    Private _ext As String = GetExtension(OwlFileFormat.TensorBinary)

    Public Property Extension As String
        Get
            Return _ext
        End Get
        Set(value As String)
            _ext = value
        End Set
    End Property

    Sub New()
        MyBase.New("Load File", "F", "File Path", "Owl", "Params")
    End Sub

    Public Sub New(Format As Owl.Core.IO.OwlFileFormat)
        MyBase.New("Load File", "F", "File Path", "Owl", "Params")
        _ext = Owl.Core.IO.GetExtension(Format)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("8f863eab-1f5c-4d76-9670-cec287e73e38")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.hidden
        End Get
    End Property

    Protected Overrides Function Prompt_Plural(ByRef values As List(Of GH_OwlFilePath)) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        _ext = reader.GetString("Extension")
        Return MyBase.Read(reader)
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        writer.SetString("Extension", _ext)
        Return MyBase.Write(writer)
    End Function

    Protected Overrides Function Prompt_Singular(ByRef value As GH_OwlFilePath) As GH_GetterResult
        'Word Documents|*.doc|Excel Worksheets|*.xls|PowerPoint Presentations|*.ppt|Office Files|*.doc;*.xls;*.ppt|All Files|*.*

        Dim sf As New OpenFileDialog()
        sf.Filter = "TensorSet Files|" & Extension & "|All files|*.*"
        Dim res As DialogResult = sf.ShowDialog(Grasshopper.Instances.ActiveCanvas)
        If res = DialogResult.OK Then
            value = New GH_OwlFilePath(sf.FileName)
            Return GH_GetterResult.success
        End If

        Return GH_GetterResult.cancel
    End Function

End Class

Public Class Param_OwlTensorSave
    Inherits GH_PersistentParam(Of GH_OwlFilePath)

    Private _ext As String = Owl.Core.IO.GetExtension(Owl.Core.IO.OwlFileFormat.TensorBinary)

    Public Property Extension As String
        Get
            Return _ext
        End Get
        Set(value As String)
            _ext = value
        End Set
    End Property

    Sub New()
        MyBase.New("Save File", "F", "File Path", "Owl", "Params")
    End Sub

    Public Sub New(Format As Owl.Core.IO.OwlFileFormat)
        MyBase.New("Save File", "F", "File Path", "Owl", "Params")
        _ext = Owl.Core.IO.GetExtension(Format)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("c2056c9b-fa1a-437d-855c-0e1175a85d39")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.hidden
        End Get
    End Property

    Protected Overrides Function Prompt_Plural(ByRef values As List(Of GH_OwlFilePath)) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        _ext = reader.GetString("Extension")
        Return MyBase.Read(reader)
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        writer.SetString("Extension", _ext)
        Return MyBase.Write(writer)
    End Function

    Protected Overrides Function Prompt_Singular(ByRef value As GH_OwlFilePath) As GH_GetterResult
        Dim sf As New SaveFileDialog
        sf.Filter = "TensorSet Files|*" & Extension
        Dim res As DialogResult = sf.ShowDialog(Grasshopper.Instances.ActiveCanvas)
        If res = DialogResult.OK Then
            value = New GH_OwlFilePath(sf.FileName)
            Return GH_GetterResult.success
        End If
        Return GH_GetterResult.cancel
    End Function
End Class