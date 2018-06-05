Imports GH_IO.Serialization
Imports Grasshopper.Kernel.Types
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
        Return "QLearn"
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
        Dim epsilon As Double = reader.GetDouble("Epsilon")
        Dim gamma As Double = reader.GetDouble("Gamma")
        Dim rndseed As Integer = reader.GetInt64("RndSeed")
        Dim rndcount As Integer = reader.GetInt64("RndCount")

        Dim qm As New List(Of List(Of Integer))
        Dim statecount As Integer = reader.GetInt64("StateCount")
        For i As Integer = 0 To statecount - 1
            Dim thisstate As New List(Of Integer)
            Dim thisstatecount As Integer = reader.GetInt64("State_" & i & "_ActionCount")

            For j As Integer = 0 To thisstatecount - 1
                thisstate.Add(reader.GetInt64("State_" & i & "_Action_" & j))
            Next
            qm.Add(thisstate)
        Next

        Dim vm As New SortedList(Of Tuple(Of Integer, Integer), Double)
        Dim valuecount As Integer = reader.GetInt64("ValueCount")

        For i As Integer = 0 To valuecount - 1
            Dim k1 As Integer = reader.GetInt64("KeyState_" & i)
            Dim k2 As Integer = reader.GetInt64("KeyAction_" & i)
            Dim v12 As Double = reader.GetDouble("SAValue_" & i)

            vm.Add(New Tuple(Of Integer, Integer)(k1, k2), v12)
        Next

        Me.Value = New QAgent(qm, vm, gamma, alpha, epsilon, rndseed)
        Me.Value.Rnd = QAgent.RebuildRandom(rndseed, rndcount)
        Me.Value.RndCount = rndcount
        Me.Value.RndSeed = rndseed
        Return True
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        If Me.Value Is Nothing Then Return False
        writer.SetDouble("Alpha", Me.Value.Alpha)
        writer.SetDouble("Epsilon", Me.Value.Epsilon)
        writer.SetDouble("Gamma", Me.Value.Gamma)
        writer.SetInt64("RndCount", Me.Value.RndCount)
        writer.SetInt64("RndSeed", Me.Value.RndSeed)

        writer.SetInt64("StateCount", Me.Value.QMatrix.Count)
        For i As Integer = 0 To Me.Value.QMatrix.Count - 1
            writer.SetInt64("State_" & i & "_ActionCount", Me.Value.QMatrix(i).Count)

            For j As Integer = 0 To Me.Value.QMatrix(i).Count - 1
                writer.SetInt64("State_" & i & "_Action_" & j, Me.Value.QMatrix(i)(j))
            Next
        Next

        writer.SetInt64("ValueCount", Me.Value.QValues.Count)
        Dim cnt As Integer = 0
        For Each tup As Tuple(Of Integer, Integer) In Me.Value.QValues.Keys

            writer.SetInt64("KeyState_" & cnt, tup.Item1)
            writer.SetInt64("KeyAction_" & cnt, tup.Item2)
            writer.SetDouble("SAValue_" & cnt, Me.Value.QValues(tup))

            cnt += 1
        Next

        Return True
    End Function

End Class
