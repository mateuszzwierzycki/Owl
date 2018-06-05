Imports System

Namespace Probability

    ''' <summary>
    ''' QLearning implementation with sparse matrices.
    ''' </summary>
    Public Class QAgent

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
        Public Property Rnd As Random = Nothing

        Private _rndcount As Integer = 0
        Private _rndseed As Integer = 123

        ''' <summary>
        ''' Counts how many times was the Rnd evaluated. 
        ''' </summary>
        ''' <returns></returns>
        Public Property RndCount As Integer
            Get
                Return _rndcount
            End Get
            Set(value As Integer)
                _rndcount = value
            End Set
        End Property

        Public Property RndSeed As Integer
            Get
                Return _rndseed
            End Get
            Set(value As Integer)
                _rndseed = value
            End Set
        End Property

        Public QMatrix(,) As Double = Nothing

        Public Sub New(QMatrix As List(Of List(Of Integer)),
                       gamma As Double,
                       alpha As Double,
                       epsilon As Double,
                       Seed As Integer)

            Dim statecount As Integer = QMatrix.Count
            Dim actioncount As Integer = -1

            For i As Integer = 0 To QMatrix.Count - 1
                For j As Integer = 0 To QMatrix(i).Count - 1
                    actioncount = Math.Max(actioncount, QMatrix(i)(j))
                Next
            Next

            ReDim Me.QMatrix(statecount - 1, actioncount)
            InitializeQValues(Me.QMatrix, 0)

            Me.Gamma = gamma
            Me.Alpha = alpha
            Me.Epsilon = epsilon
            Me.RndSeed = Seed
            Me.Rnd = New Random(Seed)
        End Sub

        Public Sub New(QMatrix As List(Of List(Of Integer)),
                       QValues As SortedList(Of Tuple(Of Integer, Integer), Double),
                       gamma As Double,
                       alpha As Double,
                       epsilon As Double,
                       Seed As Integer)

            Dim statecount As Integer = QMatrix.Count
            Dim actioncount As Integer = -1
            For i As Integer = 0 To QMatrix.Count - 1
                For j As Integer = 0 To QMatrix(i).Count - 1
                    actioncount = Math.Max(actioncount, QMatrix(i)(j))
                Next
            Next

            ReDim Me.QMatrix(statecount - 1, actioncount - 1)

            For i As Integer = 0 To QValues.Keys.Count - 1
                Dim tk As Tuple(Of Integer, Integer) = QValues.Keys(i)
                Me.QMatrix(tk.Item1, tk.Item2) = QValues(tk)
            Next

            Me.Gamma = gamma
            Me.Alpha = alpha
            Me.Epsilon = epsilon
            Me.RndSeed = Seed
            Me.Rnd = New Random(Seed)
        End Sub

        Private Sub New(Other As QAgent)
            Me.QMatrix = Other.QMatrix.Clone
            Me.RndSeed = Other.RndSeed
            Me.RndCount = Other.RndCount
            Me.Epsilon = Other.Epsilon
            Me.Gamma = Other.Gamma
            Me.Alpha = Other.Alpha
            Me.Rnd = Other.DuplicateRandom
        End Sub

        Public Shared Function RebuildRandom(Seed As Integer, Count As Integer) As System.Random
            Dim nr As New Random(Seed)
            For i As Integer = 0 To Count - 1
                nr.Next()
            Next
            Return nr
        End Function

        Private Function DuplicateRandom() As System.Random
            Return QAgent.RebuildRandom(RndSeed, RndCount)
        End Function

        Public Function Duplicate() As QAgent
            Return New QAgent(Me)
        End Function

        ''' <summary>
        ''' Creates a dictionary of values based on the provided (State, Action) matrix. 
        ''' </summary>
        ''' <param name="QMatrix"></param>
        ''' <param name="InitialValue"></param>
        Public Shared Sub InitializeQValues(ByRef QMatrix(,) As Double, InitialValue As Double)

            For i As Integer = 0 To QMatrix.GetUpperBound(0)
                For j As Integer = 0 To QMatrix.GetUpperBound(1)
                    QMatrix(i, j) = InitialValue
                Next
            Next

        End Sub

        ''' <summary>
        ''' Chooses the next Action based on the CurrentState.
        ''' </summary>
        ''' <returns></returns>
        Public Function ChooseAction(CurrentState As Integer) As Integer
            Dim thisAction As Integer = -1

            If NextRandomDouble() < Epsilon Then                 'random exploration 
                Dim possibleActions As List(Of Integer) = GetActionIds(CurrentState)
                thisAction = possibleActions(NextRandom(possibleActions.Count))
            Else                                                    'going with the gradient flow
                If SumPossibleActions(CurrentState) <> 0 Then       'if possible reward or penalty in sight then follow or avoid it
                    Dim possibleQRewards As New List(Of Double)
                    For Each possibleAction As Integer In GetActionIds(CurrentState)
                        possibleQRewards.Add(QMatrix(CurrentState, possibleAction))
                    Next
                    thisAction = GetActionIds(CurrentState)(ArgMax(possibleQRewards))
                Else                                                'if no reward in sight, go random
                    Dim possibleActions = GetActionIds(CurrentState)
                    thisAction = possibleActions(NextRandom(possibleActions.Count))
                End If
            End If

            Return thisAction
        End Function

        Private Function NextRandom(MaxValue As Integer) As Integer
            Dim ret As Integer = Rnd.Next(MaxValue)
            RndCount += 1
            Return ret
        End Function

        Private Function NextRandomDouble() As Double
            Dim retvalue As Double = Rnd.NextDouble
            RndCount += 1
            Return retvalue
        End Function

        ''' <summary>
        ''' Updates the QValue(CurrentState, Action) using Reward, then assigns the NextState value to CurrentState. 
        ''' </summary>
        ''' <param name="CurrentState"></param>
        ''' <param name="Action"></param>
        ''' <param name="NextState"></param>
        ''' <param name="Reward"></param>
        Public Sub UpdateQ(CurrentState As Integer, Action As Integer, NextState As Integer, Reward As Double)
            Dim qsa As Double = QMatrix(CurrentState, Action)
            Dim actionValues As New List(Of Double)
            Dim actionIds As New List(Of Integer)
            GetActions(NextState, actionIds, actionValues)
            Dim new_q = qsa + Alpha * (Reward + Gamma * actionValues(ArgMax(actionValues)) - qsa)
            QMatrix(CurrentState, Action) = new_q
        End Sub

        Public Function GetActionIds(State As Integer) As List(Of Integer)
            Dim ids As New List(Of Integer)

            For i As Integer = 0 To QMatrix.GetUpperBound(1)
                If QMatrix(State, i) <> -1 Then
                    ids.Add(i)
                End If
            Next

            Return ids
        End Function

        Public Sub GetActions(State As Integer, ByRef ActionID As List(Of Integer),
                                   ByRef ActionValue As List(Of Double))
            Dim ids As New List(Of Integer)
            Dim val As New List(Of Double)

            For i As Integer = 0 To QMatrix.GetUpperBound(1)
                If QMatrix(State, i) <> -1 Then
                    ids.Add(i)
                    val.Add(QMatrix(State, i))
                End If
            Next

            ActionID = ids
            ActionValue = val
        End Sub

        ''' <summary>
        ''' Sums the rewards of all the possible Actions at CurrentState
        ''' </summary>
        ''' <param name="CurrentState"></param>
        ''' <returns></returns>
        Private Function SumPossibleActions(CurrentState As Integer) As Double
            Dim sum As Double = 0

            For i As Integer = 0 To QMatrix.GetUpperBound(1)
                If QMatrix(CurrentState, i) <> -1 Then
                    sum += QMatrix(CurrentState, i)
                End If
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
