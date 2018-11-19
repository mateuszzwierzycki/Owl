Imports System.Windows.Forms
Imports GH_IO.Serialization

Public Class Param_OwlLoad
    Inherits GH_PersistentParam(Of GH_OwlFilePath)

    Private _ext As New List(Of String)

    Public Property Extensions As List(Of String)
        Get
            Return _ext
        End Get
        Set(value As List(Of String))
            _ext = value
        End Set
    End Property

    Sub New()
        MyBase.New("Load File", "F", "File Path", "Owl", OwlComponentBase.SubCategoryParam)
        Dim allval() As OwlFileFormat = GetType(OwlFileFormat).GetEnumValues()

        For i As Integer = 0 To allval.Length - 1 Step 1
            If allval(i) >= 0 And allval(i) < Integer.MaxValue Then Extensions.Add(GetExtension(allval(i)))
        Next
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("97ca660d-1ea7-44b8-b15f-63ec881453ed")
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

    'Word Documents|*.doc|Excel Worksheets|*.xls|PowerPoint Presentations|*.ppt|Office Files|*.doc;*.xls;*.ppt|All Files|*.*
    Protected Overrides Function Prompt_Singular(ByRef value As GH_OwlFilePath) As GH_GetterResult
        Dim sf As New OpenFileDialog()
        Dim fil As String = "TensorSetFiles|"

        For i = 0 To Extensions.Count - 1
            fil &= "*" & Extensions(i) & If(i < Extensions.Count - 1, ";", "")
        Next

        fil &= "|All Files|*.*"

        sf.Filter = fil
        Dim res As DialogResult = sf.ShowDialog(Instances.ActiveCanvas)
        If res = DialogResult.OK Then value = New GH_OwlFilePath(sf.FileName) : Return GH_GetterResult.success
        Return GH_GetterResult.cancel
    End Function

End Class

Public Class Param_OwlSave
    Inherits GH_PersistentParam(Of GH_OwlFilePath)

    Private _ext As New List(Of String)

    Public Property Extensions As List(Of String)
        Get
            Return _ext
        End Get
        Set(value As List(Of String))
            _ext = value
        End Set
    End Property

    Sub New()
        MyBase.New("Load File", "F", "File Path", "Owl", "Params")
        Dim allval() As OwlFileFormat = GetType(OwlFileFormat).GetEnumValues()

        For i As Integer = 0 To allval.Length - 1 Step 1
            If allval(i) >= 0 And allval(i) < Integer.MaxValue Then Extensions.Add(GetExtension(allval(i)))
        Next
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("7c6fefd5-bf90-4d7f-9998-3ee7412904d8")
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

    'Word Documents|*.doc|Excel Worksheets|*.xls|PowerPoint Presentations|*.ppt|Office Files|*.doc;*.xls;*.ppt|All Files|*.*
    Protected Overrides Function Prompt_Singular(ByRef value As GH_OwlFilePath) As GH_GetterResult
        Dim sf As New SaveFileDialog()
        Dim fil As String = "TensorSetFiles|"

        For i = 0 To Extensions.Count - 1
            fil &= "*" & Extensions(i) & If(i < Extensions.Count - 1, ";", "")
        Next

        fil &= "|All Files|*.*"

        sf.Filter = fil
        Dim res As DialogResult = sf.ShowDialog(Instances.ActiveCanvas)
        If res = DialogResult.OK Then value = New GH_OwlFilePath(sf.FileName) : Return GH_GetterResult.success
        Return GH_GetterResult.cancel
    End Function

End Class