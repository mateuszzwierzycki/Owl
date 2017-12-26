Imports System

Namespace Probability

    ''' <summary>
    ''' QLearning implementation with sparse matrices.
    ''' </summary>
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

        ''' <summary>
        ''' Shared random number generator.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Property Rnd As Random = New Random()

        Public GoalStates As HashSet(Of Integer) = New HashSet(Of Integer)
        Public CurrentState As Integer = -1

        ''' <summary>
        ''' List of states, each state stores a list of possible actions 
        ''' </summary>
        Public QMatrix As New List(Of List(Of Integer))

        ''' <summary>
        ''' SortedList of Q Values. 
        ''' Tuple(State, Action). 
        ''' </summary>
        Public QValues As New SortedList(Of Tuple(Of Integer, Integer), Double)

        ''' <summary>
        ''' QMatrix - list of states, each stores a list of possible actions. 
        ''' </summary>
        ''' <param name="QMatrix"></param>
        Public Sub New(QMatrix As List(Of List(Of Integer)))
            Me.QMatrix = QMatrix
            Me.QValues = InitializeQValues(QMatrix, 0)
        End Sub

        Public Sub New(QMatrix As List(Of List(Of Integer)), InitialState As Integer)
            Me.QMatrix = QMatrix
            Me.QValues = InitializeQValues(QMatrix, 0)
            Me.CurrentState = InitialState
        End Sub

        Public Sub New(QMatrix As List(Of List(Of Integer)), QValues As SortedList(Of Tuple(Of Integer, Integer), Double))
            Me.QMatrix = QMatrix
            Me.QValues = QValues
        End Sub

        Public Sub New(QMatrix As List(Of List(Of Integer)),
                       InitialState As Integer,
                       gamma As Double,
                       alpha As Double,
                       epsilon As Double)
            Me.QMatrix = QMatrix
            Me.QValues = InitializeQValues(QMatrix, 0)
            Me.Gamma = gamma
            Me.Alpha = alpha
            Me.Epsilon = epsilon
            Me.CurrentState = InitialState
        End Sub

        Public Sub New(QMatrix As List(Of List(Of Integer)),
                       QValues As SortedList(Of Tuple(Of Integer, Integer), Double),
                       Goals As HashSet(Of Integer),
                       InitialState As Integer,
                       gamma As Double,
                       alpha As Double,
                       epsilon As Double)
            Me.QMatrix = QMatrix
            Me.QValues = QValues
            Me.GoalStates = Goals
            Me.Gamma = gamma
            Me.Alpha = alpha
            Me.Epsilon = epsilon
            Me.CurrentState = InitialState
        End Sub

        Public Function Duplicate() As QLearning
            If QMatrix Is Nothing Then Return Nothing
            If QValues Is Nothing Then Return Nothing

            Return New QLearning(DuplicateQMatrix(QMatrix), DuplicateQValues(QValues), New HashSet(Of Integer)(Me.GoalStates), CurrentState, Gamma, Alpha, Epsilon)
        End Function

        Public Function DuplicateQMatrix(MatrixToDuplicate As List(Of List(Of Integer))) As List(Of List(Of Integer))
            Dim nl As New List(Of List(Of Integer))

            For i As Integer = 0 To MatrixToDuplicate.Count - 1
                nl.Add(MatrixToDuplicate(i))
            Next

            Return nl
        End Function

        Public Function DuplicateQValues(QValuesToDuplicate As SortedList(Of Tuple(Of Integer, Integer), Double)) As SortedList(Of Tuple(Of Integer, Integer), Double)
            Dim nl As New SortedList(Of Tuple(Of Integer, Integer), Double)
            For Each key As Tuple(Of Integer, Integer) In QValuesToDuplicate.Keys
                nl.Add(key, QValuesToDuplicate(key))
            Next
            Return nl
        End Function

        ''' <summary>
        ''' Creates a dictionary of values based on the provided (State, Action) matrix. 
        ''' </summary>
        ''' <param name="QMatrix"></param>
        ''' <param name="InitialValue"></param>
        ''' <returns></returns>
        Public Shared Function InitializeQValues(QMatrix As List(Of List(Of Integer)), InitialValue As Double) As SortedList(Of Tuple(Of Integer, Integer), Double)
            Dim nl As New SortedList(Of Tuple(Of Integer, Integer), Double)
            For i As Integer = 0 To QMatrix.Count - 1
                For j As Integer = 0 To QMatrix(i).Count - 1
                    nl.Add(New Tuple(Of Integer, Integer)(i, QMatrix(i)(j)), 0)
                Next
            Next
            Return nl
        End Function

        ''' <summary>
        ''' If the CurrentState is a goal state, choose the next state randomly.
        ''' </summary>
        Public Sub AssureState()
            If Me.GoalStates.Contains(Me.CurrentState) Then
                Me.CurrentState = Rnd.Next(Me.QMatrix.Count)
            End If
        End Sub

        ''' <summary>
        ''' Chooses the next Action based on the CurrentState.
        ''' </summary>
        ''' <returns></returns>
        Public Function ChooseAction() As Integer
            Dim thisAction As Integer = -1

            If Rnd.NextDouble < Epsilon Then                 'random exploration 
                Dim possibleActions As List(Of Integer) = QMatrix(CurrentState)
                thisAction = possibleActions(Rnd.Next(possibleActions.Count))
            Else                                                    'going with the gradient flow
                If SumPossibleActions(CurrentState) <> 0 Then       'if possible reward or penalty in sight then follow or avoid it
                    Dim possibleQRewards As New List(Of Double)
                    For Each possibleAction As Integer In QMatrix(CurrentState)
                        possibleQRewards.Add(QValues(New Tuple(Of Integer, Integer)(CurrentState, possibleAction)))
                    Next
                    thisAction = QMatrix(CurrentState)(ArgMax(possibleQRewards))
                Else                                                'if no reward in sight, go random
                    Dim possibleActions = QMatrix(CurrentState)
                    thisAction = possibleActions(Rnd.Next(possibleActions.Count))
                End If
            End If

            Return thisAction
        End Function

        ''' <summary>
        ''' Updates the QValue(CurrentState, Action) using Reward, then assigns the NextState value to CurrentState. 
        ''' </summary>
        ''' <param name="CurrentState"></param>
        ''' <param name="Action"></param>
        ''' <param name="NextState"></param>
        ''' <param name="Reward"></param>
        Public Sub UpdateQ(CurrentState As Integer, Action As Integer, NextState As Integer, Reward As Double)
            Dim tk As New Tuple(Of Integer, Integer)(CurrentState, Action)
            Dim qsa = QValues(tk)

            Dim nextActions As List(Of Integer) = QMatrix(NextState)
            Dim nextValues As New List(Of Double)

            For Each nextAct As Integer In nextActions
                Dim nk As New Tuple(Of Integer, Integer)(NextState, nextAct)
                nextValues.Add(QValues(New Tuple(Of Integer, Integer)(NextState, nextAct)))
            Next

            Dim new_q = qsa + Alpha * (Reward + Gamma * nextValues(ArgMax(nextValues)) - qsa)
            QValues(tk) = new_q

            Me.CurrentState = NextState
        End Sub

        ''' <summary>
        ''' Sums the rewards of all the possible Actions at CurrentState
        ''' </summary>
        ''' <param name="CurrentState"></param>
        ''' <returns></returns>
        Private Function SumPossibleActions(CurrentState As Integer) As Double
            Dim sum As Double = 0
            For Each PossibleAction As Integer In QMatrix(CurrentState)
                sum += QValues(New Tuple(Of Integer, Integer)(CurrentState, PossibleAction))
            Next
            Return sum
        End Function

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

    End Class

End Namespace
