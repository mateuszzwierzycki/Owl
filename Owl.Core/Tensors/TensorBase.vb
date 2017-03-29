Imports Owl.Core.Structures

Namespace Tensors

    ''' <summary>
    ''' This abstract class stores Tensor data as a 1D array of double, providing some simple methods.
    ''' </summary>
    <Serializable>
    Public MustInherit Class TensorBase
        Implements IDisposable
        Implements IEnumerable(Of Double)
        Implements IComparable(Of TensorBase)

        Private _array() As Double = Nothing

        Sub New()

        End Sub

        Sub New(Values() As Double)
            ReDim _array(Values.Length - 1)
            Buffer.BlockCopy(Values, 0, _array, 0, Values.Length * 8)
        End Sub

        Sub New(Values As IEnumerable(Of Double))
            ReDim _array(Values.Count - 1)

            For i As Integer = 0 To Values.Count - 1 Step 1
                TensorData(i) = Values(i)
            Next
        End Sub

        Sub New(Length As Integer)
            ReDim _array(Length - 1)
        End Sub

        MustOverride Function Duplicate() As TensorBase

        Default Public Property Item(index As Integer) As Double
            Get
                Return _array(index)
            End Get
            Set(value As Double)
                _array(index) = value
            End Set
        End Property

        Public ReadOnly Property Length As Integer
            Get
                Return Me.TensorData.Length
            End Get
        End Property

        ''' <summary>
        ''' Returns a direct pointer to the internal array. Won't copy the data.
        ''' </summary>
        ''' <returns></returns>
        Public Property TensorData As Double()
            Get
                Return _array
            End Get
            Set(value As Double())
                _array = value
            End Set
        End Property

        Public Function GetEnumerator() As IEnumerator(Of Double) Implements IEnumerable(Of Double).GetEnumerator
            Return _array.AsEnumerable.GetEnumerator
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _array.GetEnumerator
        End Function

        Public Function CompareTo(other As TensorBase) As Integer Implements IComparable(Of TensorBase).CompareTo
            If Me.Length < other.Length Then Return -1
            If Me.Length > other.Length Then Return 1

            For i As Integer = 0 To Me.Count - 1 Step 1
                If Me(i) < other(i) Then Return -1
                If Me(i) > other(i) Then Return 1
            Next

            Return 0
        End Function

        Public Sub TrimCeiling(Value As Double)
            For i As Integer = 0 To Me.Count - 1 Step 1
                TrimCeiling(Value, i)
            Next
        End Sub

        Public Sub TrimFloor(Value As Double)
            For i As Integer = 0 To Me.Count - 1 Step 1
                TrimFloor(Value, i)
            Next
        End Sub

        Public Sub TrimCeiling(Value As Double, Dimension As Integer)
            Me(Dimension) = Math.Min(Value, Me(Dimension))
        End Sub

        Public Sub TrimFloor(Value As Double, Dimension As Integer)
            Me(Dimension) = Math.Max(Value, Me(Dimension))
        End Sub

        Public Function GetRange() As Range
            Dim min As Double = Double.MaxValue
            Dim max As Double = Double.MinValue

            For i As Integer = 0 To Me.Count - 1 Step 1
                min = Math.Min(min, Me(i))
                max = Math.Max(max, Me(i))
            Next

            Return New Range(min, max)
        End Function

        Public Sub Remap(From As Range, [To] As Range, Dimension As Integer)
            If From.Length = 0 Then Me(Dimension) = From.From : Exit Sub
            Me(Dimension) = (((Me(Dimension) - From.From) * (1 / (From.To - From.From))) * ([To].To - [To].From)) + [To].From
        End Sub

        Public Sub Remap(From As Range, [To] As Range)
            If From.Length = 0 Then
                For i As Integer = 0 To Me.Count - 1 Step 1
                    Me(i) = From.From
                Next
                Exit Sub
            End If

            Dim denom As Double = 1 / (From.To - From.From)

            For i As Integer = 0 To Me.Count - 1 Step 1
                Me(i) = (((Me(i) - From.From) * denom) * ([To].To - [To].From)) + [To].From
            Next
        End Sub

        ''' <summary>
        ''' Finds the current range first.
        ''' </summary>
        ''' <param name="[To]"></param>
        Public Sub Remap([To] As Range)
            Dim from As Range = Me.GetRange

            If from.Length = 0 Then
                For i As Integer = 0 To Me.Count - 1 Step 1
                    Me(i) = from.From
                Next
                Exit Sub
            End If

            Dim denom As Double = 1 / (from.To - from.From)

            For i As Integer = 0 To Me.Count - 1 Step 1
                Me(i) = (((Me(i) - from.From) * denom) * ([To].To - [To].From)) + [To].From
            Next
        End Sub



#Region "IDisposable Support"
        Private disposedValue As Boolean

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                End If
                _array = Nothing
            End If
            disposedValue = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub

#End Region
    End Class

End Namespace
