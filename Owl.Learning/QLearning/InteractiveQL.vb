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

        Public QGraph As DGraphEdge(Of Double) = Nothing
        Public GoalStates As HashSet(Of Integer) = New HashSet(Of Integer)
        Public CurrentState As Integer = -1

        'cached adjacency matrices 
        Private QAdjacency() As List(Of Integer) = Nothing

        ''' <summary>
        ''' Initialize with default values. 
        ''' </summary>
        ''' <param name="QLearnGraph"></param>
        Public Sub New(QLearnGraph As DGraphEdge(Of Double), InitialState As Integer)
            QGraph = DuplicateGraph(QLearnGraph)
            QAdjacency = QGraph.GetAdjacencyMatrix
            CurrentState = InitialState
        End Sub

        Public Sub New(QLearnGraph As DGraphEdge(Of Double), InitialState As Integer,
                       gamma As Double,
                       alpha As Double,
                       epsilon As Double,
                       seed As Integer)
            Me.Gamma = gamma
            Me.Alpha = alpha
            Me.Epsilon = epsilon
            Me.CurrentState = InitialState
            Randomness = New Random(seed)

            QGraph = DuplicateGraph(QLearnGraph)
            QAdjacency = QGraph.GetAdjacencyMatrix
        End Sub

        Public Shared Function DuplicateGraph(GraphToDuplicate As DGraphEdge(Of Double)) As DGraphEdge(Of Double)

            Dim QMatrix As New DGraphEdge(Of Double)
            For Each v In GraphToDuplicate.Vertices
                QMatrix.Vertices.Add(True)
            Next

            For Each e As DEdge(Of Double) In GraphToDuplicate.Edges
                Dim te As New DEdge(Of Double)(e.From, e.To, e.Value)
                QMatrix.Edges.Add(te)
            Next

            Return QMatrix
        End Function

        Public Function ChooseAction() As Integer
            Dim thisAction As Integer = -1

            If Randomness.NextDouble < Epsilon Then                 'random exploration 
                Dim possibleActions As List(Of Integer) = QGraph.GetAdjacent(CurrentState, QAdjacency)
                thisAction = possibleActions(Randomness.Next(possibleActions.Count))
            Else                                                    'going with the gradient flow
                If SumPossibleActions(CurrentState) <> 0 Then       'if possible reward or penalty in sight then follow or avoid it
                    Dim possibleQRewards As New List(Of Double)
                    For Each possibleAction As Integer In QGraph.GetAdjacent(CurrentState, QAdjacency)
                        possibleQRewards.Add(QGraph.Edges(New DEdge(Of Double)(CurrentState, possibleAction, -1)).Value)
                    Next
                    thisAction = QAdjacency(CurrentState)(ArgMax(possibleQRewards))
                Else                                                'if no reward in sight, go random
                    Dim possibleActions = QGraph.GetAdjacent(CurrentState, QAdjacency)
                    thisAction = possibleActions(Randomness.Next(possibleActions.Count))
                End If
            End If

            Return thisAction
        End Function

        Public Sub UpdateQ(CurrentState As Integer, Action As Integer, NextState As Integer, EnvironmentReward As Double)
            Dim tk As New DEdge(Of Double)(CurrentState, Action, -1)
            Dim qsa = QGraph.Edges(tk).Value

            Dim nextConn As List(Of Integer) = QGraph.GetAdjacent(NextState, QAdjacency)
            Dim nextVals As New List(Of Double)
            For Each conn As Integer In nextConn
                nextVals.Add(QGraph.Edges(New DEdge(Of Double)(NextState, conn, -1)).Value)
            Next

            Dim new_q = qsa + Alpha * (EnvironmentReward + Gamma * nextVals(ArgMax(nextVals)) - qsa)
            QGraph.Edges(tk) = New DEdge(Of Double)(CurrentState, Action, new_q)

            Me.CurrentState = NextState
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
                sum += QGraph.Edges(New DEdge(Of Double)(CurrentState, PossibleAction, -1)).Value
            Next
            Return sum
        End Function

        ''' <summary>
        ''' Utility to present the Q graph in a matrix form. 
        ''' </summary>
        ''' <returns></returns>
        Public Function PrintMatrix(Optional StringFormat As String = "0.00") As String
            Dim mat(QGraph.VertexCount - 1, QGraph.VertexCount - 1) As Double

            For i As Integer = 0 To QGraph.VertexCount - 1
                For Each con As Integer In QGraph.GetAdjacent(i, QAdjacency)
                    Dim tk As New DEdge(Of Double)(i, con, -1)
                    mat(i, con) = QGraph.Edges(tk).Value
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

End Namespace
