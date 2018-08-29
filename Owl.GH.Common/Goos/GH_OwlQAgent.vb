Imports GH_IO.Serialization
Imports Grasshopper.Kernel.Types
Imports Owl.Core.Tensors
Imports Owl.Learning.Probability

Public Class GH_OwlQAgent
    Inherits GH_Goo(Of QAgent)

    Sub New()
        MyBase.New()
    End Sub

    Sub New(QL As QAgent)
        MyBase.New(QL)
    End Sub

    Public Overrides ReadOnly Property IsValid As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TypeName As String
        Get
            Return "QAgent"
        End Get
    End Property

    Public Overrides ReadOnly Property TypeDescription As String
        Get
            Return "QAgent"
        End Get
    End Property

    Public Overrides Function Duplicate() As IGH_Goo
        Return New GH_OwlQAgent(Me.Value.Duplicate)
    End Function

    Public Overrides Function ToString() As String
        Return "QAgent"
    End Function

    Public Overrides Function CastTo(Of Q)(ByRef target As Q) As Boolean

        Select Case GetType(Q)
            Case GetType(QAgent)
                Dim mev As QAgent = Me.Value.Duplicate
                Dim obj As Object = mev
                target = obj
                Return True
        End Select

        Return False
    End Function

    Public Overrides Function CastFrom(source As Object) As Boolean
        Select Case source.GetType
            Case GetType(QAgent)
                Dim ql As QAgent = DirectCast(source, QAgent)
                ql = ql.Duplicate
                Me.Value = ql
                Return True
        End Select

        Return False
    End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        Dim alpha As Double = reader.GetDouble("Alpha")
        Dim gamma As Double = reader.GetDouble("Gamma")
        Dim rndseed As Integer = reader.GetInt64("RndSeed")
        Dim rndcount As Integer = reader.GetInt64("RndCount")
        Dim qm As Tensor = Owl.Core.IO.FromBinary(reader.GetByteArray("QMatrix"))

        Me.Value = New QAgent(qm, gamma, alpha, rndseed)
        Me.Value.Rnd = QAgent.RebuildRandom(rndseed, rndcount)
        Me.Value.RndCount = rndcount
        Me.Value.RndSeed = rndseed
        Return True
    End Function


    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        If Me.Value Is Nothing Then Return False
        writer.SetDouble("Alpha", Me.Value.Alpha)
        writer.SetDouble("Gamma", Me.Value.Gamma)
        writer.SetInt64("RndCount", Me.Value.RndCount)
        writer.SetInt64("RndSeed", Me.Value.RndSeed)
        writer.SetByteArray("QMatrix", Owl.Core.IO.ToBinary(Me.Value.QMatrix))

        Return True
    End Function

End Class
