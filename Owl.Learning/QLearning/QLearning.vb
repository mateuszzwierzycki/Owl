Imports Related
Imports Related.Edges
Imports Related.Graphs

Namespace Probability

    Public Class QLearning

        ''' <summary>
        ''' Discount factor.
        ''' </summary>
        ''' <returns></returns>
        Public Property Gamma As Double = 0.8

        ''' <summary>
        ''' Learning rate.
        ''' </summary>
        ''' <returns></returns>
        Public Property Alpha As Double = 1.0

        ''' <summary>
        ''' Probability of going random. 
        ''' </summary>
        ''' <returns></returns>
        Public Property Epsilon As Double = 0.05

        Private Property Randomness As Random = New Random(1999)

        Public RGraph As DirectedGraphEdgeT(Of Double) = Nothing
        Public QGraph As DirectedGraphEdgeT(Of Double) = Nothing

        Public GoalStates As HashSet(Of Integer) = New HashSet(Of Integer)

        'cached adjacency matrices 
        Private RAdjacency() As List(Of Integer) = Nothing
        Private QAdjacency() As List(Of Integer) = Nothing

        ''' <summary>
        ''' Initialize with default values. 
        ''' </summary>
        ''' <param name="RewardGraph"></param>
        Public Sub New(RewardGraph As DirectedGraphEdgeT(Of Double))
            RGraph = RewardGraph
            QGraph = ConstructQ(RGraph)
            RAdjacency = RGraph.GetAdjacencyMatrix
            QAdjacency = QGraph.GetAdjacencyMatrix
        End Sub

        Public Sub New(RewardMatrix As DirectedGraphEdgeT(Of Double),
                       gamma As Double,
                       alpha As Double,
                       epsilon As Double,
                       seed As Integer)
            Me.Gamma = gamma
            Me.Alpha = alpha
            Me.Epsilon = epsilon
            Randomness = New Random(seed)

            RGraph = RewardMatrix
            QGraph = ConstructQ(RGraph)
            RAdjacency = RGraph.GetAdjacencyMatrix
            QAdjacency = QGraph.GetAdjacencyMatrix
        End Sub

        ''' <summary>
        ''' Call this method whenever changing the RGraph topology or values.
        ''' </summary>
        Public Sub UpdateCache()
            Dim nQ As DirectedGraphEdgeT(Of Double) = ConstructQ(RGraph)

            For Each te As DirectedEdge(Of Double) In QGraph.Edges
                If nQ.Edges.Contains(te) Then 'containment checks only From and To props, Value is skipped.
                    nQ.Edges(te) = te 'eventually replaces the value stored in the edge.
                End If
            Next

            QGraph = nQ
            QAdjacency = QGraph.GetAdjacencyMatrix
            RAdjacency = RGraph.GetAdjacencyMatrix
        End Sub

        ''' <summary>
        ''' Copy the topology of the RewardMatrix and set the values to 0.
        ''' </summary>
        ''' <param name="RewardMatrix"></param>
        ''' <returns></returns>
        Public Shared Function ConstructQ(RewardMatrix As DirectedGraphEdgeT(Of Double)) As DirectedGraphEdgeT(Of Double)
            Dim QMatrix As New DirectedGraphEdgeT(Of Double)
            For Each v In RewardMatrix.Vertices
                QMatrix.Vertices.Add(0)
            Next
            For Each e As DirectedEdge(Of Double) In RewardMatrix.Edges
                Dim te As New DirectedEdge(Of Double)(e.From, e.To, 0)
                QMatrix.Edges.Add(te)
            Next
            Return QMatrix
        End Function

        ''' <summary>
        ''' Traverses the state graph without learning (updating the Q matrix, epsilion set to 0).  
        ''' </summary>
        ''' <param name="InitialState"></param>
        ''' <param name="MaxSteps"></param>
        ''' <returns></returns>
        Public Function EvaluateQ(InitialState As Integer, MaxSteps As Integer) As List(Of Integer)
            Dim currentState = InitialState
            Dim visited As New List(Of Integer)({currentState}) 'currentState is the starting point of the episode

            For i As Integer = 0 To MaxSteps - 1
                If GoalStates.Contains(currentState) Then Return visited

                Dim thisAction As Integer = -1
                Dim thisNextState As Integer = -1

                'going with the gradient flow
                If SumPossibleActions(currentState) > 0 Then        'if any possible reward in sight then follow it
                    Dim possibleQRewards As New List(Of Double)
                    For Each possibleAction As Integer In QGraph.GetAdjacent(currentState, QAdjacency)
                        possibleQRewards.Add(QGraph.Edges(New DirectedEdge(Of Double)(currentState, possibleAction, -1)).Value)
                    Next
                    thisAction = QAdjacency(currentState)(ArgMax(possibleQRewards))
                Else                                                'if no reward in sight, go random
                    Dim possibleActions = RGraph.GetAdjacent(currentState, RAdjacency)
                    thisAction = possibleActions(Randomness.Next(possibleActions.Count))
                End If

                thisNextState = thisAction
                currentState = thisNextState
                visited.Add(currentState)
            Next

            Return visited
        End Function

        ''' <summary>
        ''' Run once, starting from a specified state. 
        ''' </summary>
        ''' <param name="InitialState"></param>
        ''' <param name="MaxTrials"></param>
        ''' <returns></returns>
        Public Function RunOnce(InitialState As Integer, MaxTrials As Integer) As List(Of Integer)
            Dim currentState = InitialState
            Dim visited As New List(Of Integer)({currentState}) 'currentState is the starting point of the episode

            For i As Integer = 0 To MaxTrials - 1
                If GoalStates.Contains(currentState) Then Return visited

                Dim thisAction As Integer = -1
                Dim thisNextState As Integer = -1

                If Randomness.NextDouble < Epsilon Then                 'random exploration 
                    Dim possibleActions As List(Of Integer) = RGraph.GetAdjacent(currentState, RAdjacency)
                    thisAction = possibleActions(Randomness.Next(possibleActions.Count))
                Else                                                    'going with the gradient flow
                    If SumPossibleActions(currentState) <> 0 Then       'if possible reward or penalty in sight then follow or avoid it
                        Dim possibleQRewards As New List(Of Double)
                        For Each possibleAction As Integer In QGraph.GetAdjacent(currentState, QAdjacency)
                            possibleQRewards.Add(QGraph.Edges(New DirectedEdge(Of Double)(currentState, possibleAction, -1)).Value)
                        Next
                        thisAction = QAdjacency(currentState)(ArgMax(possibleQRewards))
                    Else                                                'if no reward in sight, go random
                        Dim possibleActions = RGraph.GetAdjacent(currentState, RAdjacency)
                        thisAction = possibleActions(Randomness.Next(possibleActions.Count))
                    End If
                End If

                thisNextState = thisAction
                UpdateQ(currentState, thisAction, thisNextState)
                currentState = thisNextState
                visited.Add(currentState)
            Next

            Return visited
        End Function

        ''' <summary>
        ''' Run once, starting from a randomly chosen state. 
        ''' </summary>
        ''' <param name="MaxTrials"></param>
        ''' <returns></returns>
        Public Function RunOnce(MaxTrials As Integer) As List(Of Integer)
            Return RunOnce(Randomness.Next(RGraph.VertexCount), MaxTrials)
        End Function

        ''' <summary>
        ''' Run multiple episodes, starting from random chosen states. 
        ''' </summary>
        ''' <param name="Episodes"></param>
        ''' <param name="MaxTrials"></param>
        ''' <returns></returns>
        Public Function Run(Episodes As Integer, MaxTrials As Integer) As List(Of List(Of Integer))
            Dim eps As New List(Of List(Of Integer))
            For i As Integer = 0 To Episodes - 1
                eps.Add(RunOnce(MaxTrials))
            Next
            Return eps
        End Function

        Private Function UpdateQ(CurrentState As Integer, Action As Integer, NextState As Integer) As Double
            Dim tk As New DirectedEdge(Of Double)(CurrentState, Action, -1)
            Dim rsa = RGraph.Edges(tk).Value
            Dim qsa = QGraph.Edges(tk).Value

            Dim nextConn As List(Of Integer) = QGraph.GetAdjacent(NextState, QAdjacency)
            Dim nextVals As New List(Of Double)
            For Each conn As Integer In nextConn
                nextVals.Add(QGraph.Edges(New DirectedEdge(Of Double)(NextState, conn, -1)).Value)
            Next

            Dim new_q = qsa + Alpha * (rsa + Gamma * nextVals(ArgMax(nextVals)) - qsa)
            QGraph.Edges(tk) = New DirectedEdge(Of Double)(CurrentState, Action, new_q)
            'RenormalizeRow(CurrentState)

            Return RGraph.Edges(New DirectedEdge(Of Double)(CurrentState, Action, -1)).Value
        End Function

        Private Sub RenormalizeRow(CurrentState As Integer)
            Dim sum As Double = 0

            For Each con As Integer In QGraph.GetAdjacent(CurrentState, QAdjacency)
                Dim tk As New DirectedEdge(Of Double)(CurrentState, con, -1)
                sum += QGraph.Edges(tk).Value
            Next

            If sum = 0 Then Exit Sub

            For Each con As Integer In QGraph.GetAdjacent(CurrentState, QAdjacency)
                Dim tk As New DirectedEdge(Of Double)(CurrentState, con, -1)
                Dim actualedge = QGraph.Edges(tk)
                QGraph.Edges(tk) = New DirectedEdge(Of Double)(actualedge.From, actualedge.To, actualedge.Value / sum)
            Next
        End Sub

        ''' <summary>
        ''' Returns the index of the largest value in the list. 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Collection"></param>
        ''' <returns></returns>
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

        ''' <summary>
        ''' Sums the rewards of all the possible Actions at CurrentState
        ''' </summary>
        ''' <param name="CurrentState"></param>
        ''' <returns></returns>
        Private Function SumPossibleActions(CurrentState As Integer) As Double
            Dim sum As Double = 0
            For Each PossibleAction As Integer In QAdjacency(CurrentState)
                sum += QGraph.Edges(New DirectedEdge(Of Double)(CurrentState, PossibleAction, -1)).Value
            Next
            Return sum
        End Function

        ''' <summary>
        ''' Utility to present the Q and R graphs in a matrix form. 
        ''' </summary>
        ''' <param name="MatrixToPrint"></param>
        ''' <returns></returns>
        Public Function PrintMatrix(MatrixToPrint As DirectedGraphEdgeT(Of Double), Optional StringFormat As String = "0.00") As String
            Dim mat(MatrixToPrint.VertexCount - 1, MatrixToPrint.VertexCount - 1) As Double

            For i As Integer = 0 To MatrixToPrint.VertexCount - 1
                For Each con As Integer In MatrixToPrint.GetAdjacent(i, QAdjacency)
                    Dim tk As New DirectedEdge(Of Double)(i, con, -1)
                    mat(i, con) = MatrixToPrint.Edges(tk).Value
                Next
            Next

            Dim sb As New Text.StringBuilder()
            For i As Integer = 0 To mat.GetUpperBound(1)
                Dim nl As New String("")

                For j As Integer = 0 To mat.GetUpperBound(0)
                    nl &= mat(i, j).ToString(StringFormat) & " "
                Next

                sb.AppendLine(nl)
            Next

            Return sb.ToString
        End Function

    End Class

    ''' <summary>
    ''' Example showing how to use the QLearning class. 
    ''' </summary>
    Module Testing
        Private Function Test() As String
            Dim nr As New DirectedGraphEdgeT(Of Double)

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
            nq.GoalStates.Add(0)

            nq.Run(100, 100)

            Return nq.PrintMatrix(nq.QGraph)
        End Function
    End Module

End Namespace