Namespace Probability

    ''' <summary>
    ''' QLearning implementation.
    ''' </summary>
    Public Class QAgent

        Public QMatrix As Tensor = Nothing

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

        Public Property Rnd As Random = Nothing

        Private _rndcount As Integer = 0
        Private _rndseed As Integer = 123

        Public Sub New(States As Integer, Actions As Integer, gamma As Double, alpha As Double, Optional seed As Integer = 123, Optional InitValue As Double = Double.NaN)
            Me.QMatrix = QAgent.CreateQMatrix(States, Actions, InitValue)
            Me.Gamma = gamma
            Me.Alpha = alpha
            Me.RndSeed = seed
            Me.Rnd = New Random(seed)
        End Sub

        Public Sub New(QMatrix As Tensor,
                       gamma As Double,
                       alpha As Double,
                       Optional Seed As Integer = 123)

            Me.QMatrix = QMatrix.Duplicate
            Me.Gamma = gamma
            Me.Alpha = alpha
            Me.RndSeed = Seed
            Me.Rnd = New Random(Seed)
        End Sub

        Public Sub New(QMatrix As Tensor,
                       gamma As Double,
                       alpha As Double,
                       Seed As Integer,
                       RndCount As Integer)

            Me.QMatrix = QMatrix.Duplicate
            Me.Gamma = gamma
            Me.Alpha = alpha
            Me.Rnd = QAgent.RebuildRandom(Seed, RndCount)
            Me.RndSeed = Seed
        End Sub

        Private Sub New(Other As QAgent)
            Me.QMatrix = Other.QMatrix.Duplicate
            Me.RndSeed = Other.RndSeed
            Me.RndCount = Other.RndCount
            Me.Gamma = Other.Gamma
            Me.Alpha = Other.Alpha
            Me.Rnd = Other.DuplicateRandom
        End Sub

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

        Public Shared Function RebuildRandom(Seed As Integer, Count As Integer) As System.Random
            Dim nr As New Random(Seed)
            For i As Integer = 0 To Count - 1
                nr.Next()
            Next
            Return nr
        End Function

        Private Function DuplicateRandom() As System.Random
            Return RebuildRandom(RndSeed, RndCount)
        End Function

        Public Function Duplicate() As QAgent
            Return New QAgent(Me)
        End Function

        Public Shared Function CreateQMatrix(States As Integer, Actions As Integer, InitValue As Double) As Tensor
            Dim qm As New Tensor(States, Actions)

            For i As Integer = 0 To qm.Height - 1
                For j As Integer = 0 To qm.Width - 1
                    qm.ValueAt(i, j) = InitValue
                Next
            Next

            Return qm
        End Function

        ''' <summary>
        ''' Chooses the next Action based on the CurrentState.
        ''' </summary>
        ''' <returns></returns>
        Public Function ChooseAction(CurrentState As Integer, Epsilon As Double) As Integer
            Dim thisAction As Integer = -1

            If NextRandomDouble() < Epsilon Then                 'random exploration 
                Dim possibleActions As List(Of Integer) = GetActionIds(CurrentState)
                thisAction = possibleActions(NextRandom(possibleActions.Count))
            Else                                                    'going with the gradient flow
                If SumPossibleActions(CurrentState) <> 0 Then       'if possible reward or penalty in sight then follow or avoid it
                    Dim possibleQRewards As New List(Of Double)
                    For Each possibleAction As Integer In GetActionIds(CurrentState)
                        possibleQRewards.Add(QMatrix.ValueAt(CurrentState, possibleAction))
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
            Dim qsa As Double = QMatrix.ValueAt(CurrentState, Action)
            Dim actionValues As List(Of Double) = GetActions(NextState)
            Dim new_q = qsa + Alpha * (Reward + Gamma * actionValues(ArgMax(actionValues)) - qsa)
            QMatrix.ValueAt(CurrentState, Action) = new_q
        End Sub

        Public Function GetActionIds(State As Integer) As List(Of Integer)
            Dim ids As New List(Of Integer)
            For i As Integer = 0 To QMatrix.Width - 1
                If QMatrix.ValueAt(State, i) < 0 Then Continue For
                ids.Add(i)
            Next
            Return ids
        End Function

        Public Function GetActions(State As Integer) As List(Of Double)
            Dim val As New List(Of Double)
            For i As Integer = 0 To QMatrix.Width - 1
                If QMatrix.ValueAt(State, i) < 0 Then Continue For
                val.Add(QMatrix.ValueAt(State, i))
            Next
            Return val
        End Function

        ''' <summary>
        ''' Sums the rewards of all the possible Actions at State
        ''' </summary>
        ''' <param name="State"></param>
        ''' <returns></returns>
        Private Function SumPossibleActions(State As Integer) As Double
            Dim sum As Double = 0

            Dim acts As List(Of Double) = GetActions(State)
            For i As Integer = 0 To acts.Count - 1
                If QMatrix.ValueAt(State, i) < 0 Then Continue For
                sum += (acts(i))
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
