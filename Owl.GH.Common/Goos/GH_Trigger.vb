Imports GH_IO.Serialization
Imports Grasshopper.Kernel.Types

Public Class GH_Trigger
    Inherits GH_Goo(Of Trigger)

    Sub New()
        MyBase.New
    End Sub

    Sub New(T As Trigger)
        MyBase.New(T)
    End Sub

    Public Overrides ReadOnly Property IsValid As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TypeDescription As String
        Get
            Return "Trigger"
        End Get
    End Property

    Public Overrides ReadOnly Property TypeName As String
        Get
            Return "Trigger"
        End Get
    End Property

    Public Overrides Function Duplicate() As IGH_Goo
        Return New GH_Trigger(Me.Value.Duplicate)
    End Function

    Public Overrides Function ToString() As String
        Return Me.Value.ToString
    End Function

    Public Overrides Function CastTo(Of Q)(ByRef target As Q) As Boolean

        Select Case GetType(Q)
            Case GetType(Trigger)
                Dim obj As Object = Me.Value.Duplicate
                target = obj
                Return True
        End Select

        Return False

    End Function

    Public Overrides Function CastFrom(source As Object) As Boolean
        Select Case source.GetType
            Case GetType(Trigger)
                Me.Value = DirectCast(source, Trigger)
                Return True
        End Select

        Return False
    End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean

        Dim d As Date = reader.GetDate("TrigDate")
        Dim g As Guid = reader.GetGuid("TrigGuid")
        Dim n As String = reader.GetString("TrigNick")

        Me.Value = New Trigger(d, g, n)

        Return True
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean

        writer.SetDate("TrigDate", Me.Value.Date)
        writer.SetGuid("TrigGuid", Me.Value.Source)
        writer.SetString("TrigNick", Me.Value.Nickname)

        Return True
    End Function

End Class
