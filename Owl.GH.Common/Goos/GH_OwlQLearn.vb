Imports GH_IO.Serialization
Imports Grasshopper.Kernel.Types
Imports Owl.Learning.Probability

Public Class GH_OwlQLearn
    Inherits GH_Goo(Of QLearning)

    Sub New()
        MyBase.New()
    End Sub

    Sub New(QL As QLearning)
        MyBase.New(QL)
    End Sub

    Public Overrides ReadOnly Property IsValid As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TypeName As String
        Get
            Return "QLearn"
        End Get
    End Property

    Public Overrides ReadOnly Property TypeDescription As String
        Get
            Return "QLearn"
        End Get
    End Property

    Public Overrides Function Duplicate() As IGH_Goo
        Return New GH_OwlQLearn(Me.Value.Duplicate)
    End Function

    Public Overrides Function ToString() As String
        Return "QLearn"
    End Function

    Public Overrides Function CastTo(Of Q)(ByRef target As Q) As Boolean

        Select Case GetType(Q)
            Case GetType(QLearning)
                Dim mev As QLearning = Me.Value.Duplicate
                Dim obj As Object = mev
                target = obj
                Return True
        End Select

        Return False
    End Function

    Public Overrides Function CastFrom(source As Object) As Boolean

        Select Case source.GetType
            Case GetType(QLearning)
                Dim ql As QLearning = DirectCast(source, QLearning)
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
        Dim state As Integer = reader.GetInt64("State")
        Dim rnd As Integer = reader.GetInt64("Seed")

        Dim goals As New HashSet(Of Integer)
        Dim goalcount As Integer = reader.GetInt64("GoalCount")


        For i As Integer = 0 To goalcount - 1
            goals.Add(reader.GetInt64("Goal_" & i))
        Next


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

        Me.Value = New QLearning(qm, vm, goals, state, gamma, alpha, epsilon, rnd)

        Return True
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        If Me.Value Is Nothing Then Return False
        writer.SetDouble("Alpha", Me.Value.Alpha)
        writer.SetDouble("Epsilon", Me.Value.Epsilon)
        writer.SetDouble("Gamma", Me.Value.Gamma)
        writer.SetInt64("State", Me.Value.CurrentState)
        writer.SetInt64("Seed", Me.Value.RandomSeed)

        Dim goals As List(Of Integer) = Me.Value.GoalStates.ToList
        writer.SetInt64("GoalCount", goals.Count)

        For i As Integer = 0 To goals.Count - 1 Step 1
            writer.SetInt64("Goal_" & i, goals(i))
        Next


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
