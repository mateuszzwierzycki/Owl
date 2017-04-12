Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports Owl.Core.Structures
Imports Owl.Core.IO

Namespace Tensors

    <Serializable>
    Public Class TensorSet
        Inherits CollectionBase
        Implements IEnumerable
        Implements IEnumerable(Of Tensor)

        Sub New()

        End Sub

        ''' <summary>
        ''' Creates a deep copy of the IEnumerable data.
        ''' </summary>
        ''' <param name="Other"></param>
        Sub New(Other As IEnumerable(Of Tensor))
            For i As Integer = 0 To Other.Count - 1 Step 1
                Me.Add(Other(i).Duplicate)
            Next
        End Sub

        Sub New(Other As TensorSet)
            For i As Integer = 0 To Other.Count - 1 Step 1
                Me.Add(Other(i).Duplicate)
            Next
        End Sub

        Sub New(TensorCount As Integer, Dimensionality As Integer)
            For i As Integer = 0 To TensorCount - 1 Step 1
                Me.Add(New Tensor(Dimensionality))
            Next
        End Sub

        Sub New(Filepath As String, Optional Separator As Char = " ")

            Dim thispath As String = Nothing
            Dim tup As Tuple(Of Integer, Integer) = TryToFindIndices(Filepath, thispath)

            If tup.Item1 = -1 Then thispath = Filepath
            If tup.Item1 <> -1 Then Me.AddRange(New TensorSet(thispath, tup.Item1, tup.Item2, Separator)) : Exit Sub

            Select Case Path.GetExtension(thispath)
                Case GetExtension(OwlFileFormat.TensorBinary)
                    AddRange(LoadTensorsBinary(thispath))
                Case GetExtension(OwlFileFormat.TensorText)
                    AddRange(LoadTensorsText(thispath, "*"))
                Case GetExtension(OwlFileFormat.IDX)
                    AddRange(LoadTensorsIDX(thispath))
                Case Else
                    'try to look for idx, they have some tricky extensions like idx3-ubyte
                    Dim thisext As String = Path.GetExtension(thispath)
                    If thisext.Contains("idx") Then Me.AddRange(LoadTensorsIDX(thispath))
            End Select

        End Sub

        Sub New(Filepath As String, From As Integer, Count As Integer, Optional Separator As Char = " ")
            Select Case Path.GetExtension(Filepath)
                Case GetExtension(OwlFileFormat.TensorBinary)
                    AddRange(LoadTensorsBinary(Filepath, From, Count))
                Case GetExtension(OwlFileFormat.TensorText)
                    AddRange(LoadTensorsText(Filepath, From, Count, "*"))
                Case GetExtension(OwlFileFormat.IDX)
                    AddRange(LoadTensorsIDX(Filepath, From, Count))
                Case Else
                    'try to look for idx, they have some tricky extensions like idx3-ubyte
                    Dim thisext As String = Path.GetExtension(Filepath)
                    If thisext.Contains("idx") Then Me.AddRange(LoadTensorsIDX(Filepath, From, Count))
            End Select
        End Sub

        Sub Sort()
            Me.InnerList.Sort(New TensorComparer())
        End Sub

        'Private _multi As Boolean = True

        '''' <summary>
        '''' True by default, performs some arithmetic operations using multithreading.
        '''' </summary>
        '''' <returns></returns>
        'Public Property RunParallel As Boolean
        '    Get
        '        Return _multi
        '    End Get
        '    Set(value As Boolean)
        '        _multi = value
        '    End Set
        'End Property

        'Private Function ParallelSets() As List(Of Range)
        '    Dim prc As Integer = Environment.ProcessorCount
        '    Dim nl As New List(Of Range)

        '    Dim howmuchdata As Long = Me.Count * Me(0).Length

        '    'such a small tensorset, no need for multithreading
        '    If howmuchdata < 10000 Then nl.Add(New Range(0, Me.Count - 1)) : Return nl
        '    If Me.Count = 1 Then nl.Add(New Range(0, Me.Count - 1)) : Return nl

        '    If Me.Count <= prc Then 'amount of data is big nuf, splitting one tensor per one thread
        '        For i As Integer = 0 To Me.Count - 1 Step 1
        '            nl.Add(New Range(i, i))
        '        Next
        '    End If

        '    '8 cpu, 9 tensors
        '    '9/8 = 1.125
        '    'floor = 1
        '    '1,1,1,1
        '    '1,1,1,2

        '    '8 cpu, 17 tensors
        '    '17/8 = 2.125
        '    'floor = 2 
        '    '2,2,2,2
        '    '2,2,2,3

        '    Dim cnt As Double = Math.Floor(Me.Count / prc)
        '    Dim pos As Integer = 0

        '    For i As Integer = 0 To prc - 1 Step 1
        '        If i < prc - 1 Then
        '            nl.Add(New Range(pos, pos + cnt - 1))
        '            pos += cnt
        '        Else
        '            nl.Add(New Range(pos, Me.Count - 1))
        '        End If
        '    Next

        '    Return nl
        'End Function

        ''' <summary>
        ''' Dumb implementation, can cause overflows.
        ''' </summary>
        ''' <param name="Values"></param>
        ''' <returns></returns>
        Public Shared Function Average(Values As IEnumerable(Of Tensor)) As Tensor
            Dim v As New Tensor(Values(0).Length)

            For i As Integer = 0 To Values.Count - 1 Step 1
                v += Values(i)
            Next

            v /= Values.Count
            Return v
        End Function

        Public Overrides Function ToString() As String
            Return "TensorSet (" & Me.Count & If(Me.Count = 1, " Tensor)", " Tensors)")
        End Function

        Public Sub Add(Value As Tensor)
            List.Add(Value)
        End Sub

        Sub AddRange(Values As IEnumerable(Of Tensor))
            InnerList.AddRange(Values)
        End Sub

        Public Overloads Sub RemoveAt(index As Integer)
            List.RemoveAt(index)
        End Sub

        Public Sub Remove(Value As Tensor)
            List.Remove(Value)
        End Sub

        Default Public Property Item(index As Integer) As Tensor
            Get
                Return CType(List.Item(index), Tensor)
            End Get
            Set(value As Tensor)
                List.Item(index) = value
            End Set
        End Property

        Public Function Duplicate() As TensorSet
            Return New TensorSet(Me)
        End Function

        ''' <summary>
        ''' Interpret the TensorSet as Double()(). Creates an Array(), but stores pointers to the Tensors.
        ''' </summary>
        ''' <returns></returns>
        Public Function AsArrayArray() As Double()()
            Dim arr(Me.Count - 1)() As Double

            For i As Integer = 0 To Me.Count - 1 Step 1
                arr(i) = Me(i).TensorData
            Next

            Return arr
        End Function

        Public Function IsHomogeneous() As Boolean
            If Me.Count = 0 Then Return True
            Dim cnt As Integer = Me(0).Length
            For i As Integer = 0 To Me.Count - 1 Step 1
                If cnt <> Me(i).Length Then Return False
            Next
            Return True
        End Function

        ''' <summary>
        ''' Creates a collection of new TensorSets with a shallow copy of the Tensors.
        ''' </summary>
        ''' <param name="Proportions">The TensorSet count gets rounded down.</param>
        ''' <returns>List of TensorSets</returns>
        Function Split(Proportions As IEnumerable(Of Double)) As List(Of TensorSet)
            Dim sum As Double

            For i As Integer = 0 To Proportions.Count - 1 Step 1
                sum += Proportions(i)
            Next

            sum = 1 / sum

            Dim counts As New List(Of Integer)
            Dim allcnt As Integer = 0

            For i As Integer = 0 To Proportions.Count - 1 Step 1
                counts.Add(Math.Floor(Proportions(i) * sum * Me.Count))
                allcnt += counts(i)
            Next

            counts(counts.Count - 1) += (Me.Count - allcnt)

            Dim vl As New List(Of TensorSet)

            Dim cnt As Integer = 0

            For i As Integer = 0 To counts.Count - 1 Step 1
                Dim ts As New TensorSet

                For j As Integer = 0 To counts(i) - 1 Step 1
                    ts.Add(Me(cnt))
                    cnt += 1
                Next

                vl.Add(ts)
            Next

            Return vl
        End Function

        Public Sub Remap(From As Range, [To] As Range)
            For i As Integer = 0 To Me.Count - 1 Step 1
                Me(i).Remap(From, [To])
            Next
        End Sub

        Public Sub Remap([To] As Range)
            Dim from As Range = GetRange()
            For i As Integer = 0 To Me.Count - 1 Step 1
                Me(i).Remap(from, [To])
            Next
        End Sub

        Public Sub Remap(From As Range, [To] As Range, Dimension As Integer)
            For i As Integer = 0 To Me.Count - 1 Step 1
                If Me(i).Length < Dimension + 1 Then Continue For
                Me(i).Remap(From, [To], Dimension)
            Next
        End Sub

        Public Sub Remap([To] As Range, Dimension As Integer)
            Dim from As Range = Me.GetRange(Dimension)
            For i As Integer = 0 To Me.Count - 1 Step 1
                If Me(i).Length < Dimension + 1 Then Continue For
                Me(i).Remap(from, [To], Dimension)
            Next
        End Sub

        Public Function GetRange() As Range
            Dim min As Double = Double.MaxValue
            Dim max As Double = Double.MinValue

            For i As Integer = 0 To Me.Count - 1 Step 1
                For j As Integer = 0 To Me(i).Count - 1 Step 1
                    min = Math.Min(min, Me(i)(j))
                    max = Math.Max(max, Me(i)(j))
                Next
            Next

            Return New Range(min, max)
        End Function

        Public Function GetRange(Dimension As Integer) As Range
            Dim min As Double = Double.MaxValue
            Dim max As Double = Double.MinValue

            For i As Integer = 0 To Me.Count - 1 Step 1
                If Me(i).Length < Dimension + 1 Then Continue For
                min = Math.Min(min, Me(i)(Dimension))
                max = Math.Max(max, Me(i)(Dimension))
            Next

            Return New Range(min, max)
        End Function

        Public Sub TrimCeiling(Value As Double)
            For i As Integer = 0 To Me.Count - 1 Step 1
                Me(i).TrimCeiling(Value)
            Next
        End Sub

        Public Sub TrimCeiling(Value As Double, Dimension As Integer)
            For i As Integer = 0 To Me.Count - 1 Step 1
                Me(i).TrimCeiling(Value, Dimension)
            Next
        End Sub

        Public Sub TrimFloor(Value As Double)
            For i As Integer = 0 To Me.Count - 1 Step 1
                Me(i).TrimFloor(Value)
            Next
        End Sub

        Public Sub TrimFloor(Value As Double, Dimension As Integer)
            For i As Integer = 0 To Me.Count - 1 Step 1
                Me(i).TrimFloor(Value, Dimension)
            Next
        End Sub

        Public Sub AddNoise(Amplitude As Double, Seed As Integer)
            Dim rnd As New Random(Seed)
            For i As Integer = 0 To Me.Count - 1 Step 1
                For j As Integer = 0 To Me(i).Count - 1 Step 1
                    Me(i)(j) += (rnd.NextDouble - 0.5) * Amplitude
                Next
            Next
        End Sub

        Private Function IEnumerable_GetEnumerator() As IEnumerator(Of Tensor) Implements IEnumerable(Of Tensor).GetEnumerator
            Return New TensorSetEnumerator(Me)
        End Function

    End Class

    Public Class TensorSetEnumerator
        Implements IEnumerator(Of Tensor)

        Dim s As TensorSet = Nothing
        Dim c As Integer = -1

        Sub New(TS As TensorSet)
            s = TS
        End Sub

        Public ReadOnly Property Current As Tensor Implements IEnumerator(Of Tensor).Current
            Get
                Return s.Item(c)
            End Get
        End Property

        Private ReadOnly Property IEnumerator_Current As Object Implements IEnumerator.Current
            Get
                Return s.Item(c)
            End Get
        End Property

        Public Sub Reset() Implements IEnumerator.Reset
            c = -1
        End Sub

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            c += 1
            Return c < s.Count
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    'dispose managed state (managed objects).
                End If

                'free unmanaged resources (unmanaged objects) and override Finalize() below.
                'set large fields to null.
                s = Nothing
            End If
            disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region


    End Class


End Namespace
