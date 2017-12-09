Imports Related
Imports Related.Edges

Public Class QLearning

    Public Property Gamma As Double = 0.8
    Public Property Alpha As Double = 1.0
    Public Property Epsilon As Double = 0.05
    Public Property Randomness As Random = New Random(1999)

    Public Property Episodes As New List(Of List(Of Integer))
    Public Property StoreEpisodes As Boolean = True

    Public r As Graphs.DirectedGraphEdgeT(Of Double) = Nothing
    public q As Graphs.DirectedGraphEdgeT(Of Double) = Nothing

    Dim rAdjacency() As List(Of Integer) = Nothing
    Dim qAdjacency() As List(Of Integer) = Nothing

    Public Sub New(RewardMatrix As Graphs.DirectedGraphEdgeT(Of Double))
        Dim QMatrix As New Graphs.DirectedGraphEdgeT(Of Double)

        For Each v In RewardMatrix.Vertices
            QMatrix.Vertices.Add(0)
        Next

        For Each e As DirectedEdge(Of Double) In RewardMatrix.Edges
            Dim te As New DirectedEdge(Of Double)(e.From, e.To, 0)
            QMatrix.Edges.Add(te)
        Next

        r = RewardMatrix
        q = QMatrix

        rAdjacency = r.GetAdjacencyMatrix
        qAdjacency = q.GetAdjacencyMatrix
    End Sub

    Public Sub New(RewardMatrix As Graphs.DirectedGraphEdgeT(Of Double), gamma As Double, alpha As Double, epsilon As Double, seed As Integer)
        Me.Gamma = gamma
        Me.Alpha = alpha
        Me.Epsilon = epsilon
        Me.Randomness = New Random(seed)

        Dim QMatrix As New Graphs.DirectedGraphEdgeT(Of Double)

        For Each v In RewardMatrix.Vertices
            QMatrix.Vertices.Add(0)
        Next

        For Each e As DirectedEdge(Of Double) In RewardMatrix.Edges
            Dim te As New DirectedEdge(Of Double)(e.From, e.To, 0)
            QMatrix.Edges.Add(te)
        Next

        r = RewardMatrix
        q = QMatrix

        rAdjacency = r.GetAdjacencyMatrix
        qAdjacency = q.GetAdjacencyMatrix
    End Sub

    Public Sub RunOnce(MaxTrials As Integer)
        Dim currentState = Randomness.Next(r.VertexCount)
        Dim goal = False
        Dim safeswitch As Integer = MaxTrials

        Dim visited As New List(Of Integer)({currentState}) 'here the currentState is the starting point of the episode

        While Not goal

            safeswitch -= 1
            If safeswitch < 0 Then Exit While

            Dim thisAction As Integer = -1
            Dim thisNextState As Integer = -1

            If Randomness.NextDouble < Epsilon Then
                Dim possibleActions As List(Of Integer) = r.GetAdjacent(currentState, rAdjacency)
                thisAction = possibleActions(Randomness.Next(possibleActions.Count))
            Else
                If SumPossibleActions(currentState) > 0 Then
                    Dim possibleQRewards As New List(Of Double)
                    For Each possibleAction As Integer In q.GetAdjacent(currentState, qAdjacency)
                        possibleQRewards.Add(q.Edges(New DirectedEdge(Of Double)(currentState, possibleAction, -1)).Value)
                    Next
                    thisAction = qAdjacency(currentState)(ArgMax(possibleQRewards))
                Else
                    Dim possibleActions = r.GetAdjacent(currentState, rAdjacency)
                    thisAction = possibleActions(Randomness.Next(possibleActions.Count))
                End If
            End If

            thisNextState = thisAction

            Dim reward As Double = UpdateQ(currentState, thisAction, thisNextState)
            If reward > 1 Then
                goal = True
            End If

            currentState = thisNextState
            visited.Add(currentState)

        End While

        Episodes.Add(visited)
    End Sub

    Public Sub Run(Episodes As Integer, MaxTrials As Integer)
        For i As Integer = 0 To Episodes - 1
            Me.RunOnce(MaxTrials)
        Next
    End Sub

    Private Function UpdateQ(CurrentState As Integer, Action As Integer, NextState As Integer) As Double
        Dim tk As New DirectedEdge(Of Double)(CurrentState, Action, -1)
        Dim rsa = r.Edges(tk).Value
        Dim qsa = q.Edges(tk).Value

        Dim nextConn As List(Of Integer) = q.GetAdjacent(NextState, qAdjacency)
        Dim nextVals As New List(Of Double)
        For Each conn As Integer In nextConn
            nextVals.Add(q.Edges(New DirectedEdge(Of Double)(NextState, conn, -1)).Value)
        Next

        Dim new_q = qsa + Alpha * (rsa + Gamma * nextVals(ArgMax(nextVals)) - qsa)
        q.Edges(tk) = New DirectedEdge(Of Double)(CurrentState, Action, new_q)
        RenormalizeRow(CurrentState)

        Return r.Edges(New DirectedEdge(Of Double)(CurrentState, Action, -1)).Value
    End Function

    Private Sub RenormalizeRow(CurrentState As Integer)
        Dim sum As Double = 0

        For Each con As Integer In q.GetAdjacent(CurrentState, qAdjacency)
            Dim tk As New DirectedEdge(Of Double)(CurrentState, con, -1)
            sum += q.Edges(tk).Value
        Next

        If sum = 0 Then Exit Sub

        For Each con As Integer In q.GetAdjacent(CurrentState, qAdjacency)
            Dim tk As New DirectedEdge(Of Double)(CurrentState, con, -1)
            Dim actualedge = q.Edges(tk)
            q.Edges(tk) = New DirectedEdge(Of Double)(actualedge.From, actualedge.To, actualedge.Value / sum)
        Next
    End Sub

    Private Function ArgMax(Of T As IComparable)(Collection As IList(Of T)) As Integer
        Dim maxind = 0
        Dim maxval = Collection(0)

        For i As Integer = 1 To Collection.Count - 1
            If Collection(i).CompareTo(maxval) > 0 Then
                maxind = i
                maxval = Collection(i)
            End If
        Next

        Return maxind
    End Function

    Private Function SumPossibleActions(CurrentState As Integer) As Double
        Dim sum As Double = 0
        For Each PossibleAction As Integer In qAdjacency(CurrentState)
            sum += q.Edges(New DirectedEdge(Of Double)(CurrentState, PossibleAction, -1)).Value
        Next
        Return sum
    End Function

    Public Function PrintMatrix(MatrixToPrint As Graphs.DirectedGraphEdgeT(Of Double)) As String
        Dim mat(MatrixToPrint.VertexCount - 1, MatrixToPrint.VertexCount - 1) As Double

        For i As Integer = 0 To MatrixToPrint.VertexCount - 1
            For Each con As Integer In MatrixToPrint.GetAdjacent(i, qAdjacency)
                Dim tk As New DirectedEdge(Of Double)(i, con, -1)
                Dim val As Double = MatrixToPrint.Edges(tk).Value
                mat(i, con) = val
            Next
        Next

        Dim sb As New Text.StringBuilder()
        For i As Integer = 0 To mat.GetUpperBound(1)
            Dim nl As New String("")

            For j As Integer = 0 To mat.GetUpperBound(0)
                nl &= mat(i, j) & " "
            Next

            sb.AppendLine(nl)
        Next

        Return sb.ToString
    End Function

End Class

Public Module testing
    Public Function Test() As String
        Dim nr As New Graphs.DirectedGraphEdgeT(Of Double)

        nr.Vertices.Add(True)
        nr.Vertices.Add(True)
        nr.Vertices.Add(True)
        nr.Vertices.Add(True)
        nr.Vertices.Add(True)

        nr.Edges.Add(New DirectedEdge(Of Double)(0, 1, 1))
        nr.Edges.Add(New DirectedEdge(Of Double)(1, 2, 1))
        nr.Edges.Add(New DirectedEdge(Of Double)(2, 3, 1))
        nr.Edges.Add(New DirectedEdge(Of Double)(3, 4, 1))
        nr.Edges.Add(New DirectedEdge(Of Double)(4, 0, 1))

        nr.Edges.Add(New DirectedEdge(Of Double)(0, 2, 0))
        nr.Edges.Add(New DirectedEdge(Of Double)(1, 4, 1))
        nr.Edges.Add(New DirectedEdge(Of Double)(2, 0, 10))
        nr.Edges.Add(New DirectedEdge(Of Double)(1, 3, 0))
        nr.Edges.Add(New DirectedEdge(Of Double)(4, 1, 1))

        Dim nq As New QLearning(nr, 0.8, 1, 0.8, 1234)
        nq.Run(100, 100)

        Return nq.PrintMatrix(nq.q)
    End Function
End Module
