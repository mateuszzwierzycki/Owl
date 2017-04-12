Imports GH_IO.Serialization
Imports Grasshopper.Kernel.Types

Public Class GH_OwlFilePath
    Inherits Grasshopper.Kernel.Types.GH_Goo(Of String)

    Sub New()

    End Sub

    Sub New(S As String)
        Me.m_value = S
    End Sub

    Sub New(Other As GH_OwlFilePath)
        Me.m_value = Other.m_value
    End Sub

    Public Overrides ReadOnly Property IsValid As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TypeDescription As String
        Get
            Return "File Path"
        End Get
    End Property

    Public Overrides ReadOnly Property TypeName As String
        Get
            Return "File Path"
        End Get
    End Property

    Public Overrides Function Duplicate() As IGH_Goo
        Return New GH_OwlFilePath(Me)
    End Function

    Public Overrides Function ToString() As String
        If Me.Value = "" Then Return "No path specified"
        Return Me.m_value
    End Function

    Public Overrides Function CastFrom(source As Object) As Boolean
        If TypeOf source Is String Then
            Dim obj As String = DirectCast(source, String)
            Me.Value = DirectCast(source, String)
            Return True
        ElseIf TypeOf source Is GH_String Then
            Dim obj As GH_String = DirectCast(source, GH_String)
            Me.Value = obj.Value
            Return True
        End If
        Return False
    End Function

    Public Overrides Function CastTo(Of Q)(ByRef target As Q) As Boolean
        If GetType(Q).IsAssignableFrom(GetType(String)) Then
            Dim obj As Object = Me.Value
            target = DirectCast(obj, Q)
            Return True
        Else
            Return False
        End If
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        writer.SetString("val", Me.m_value)
        Return True
    End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        Me.m_value = reader.GetString("val")
        Return True
    End Function
End Class