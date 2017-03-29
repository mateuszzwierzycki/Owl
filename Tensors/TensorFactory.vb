Namespace Tensors

    Public Module TensorFactory

        Private _seed As Integer = 123
        Private _rnd As New Random(Seed)

        Public Property Seed As Integer
            Get
                Return _seed
            End Get
            Set(value As Integer)
                _seed = value
                OnSeedChange()
            End Set
        End Property

        Private Sub OnSeedChange()
            _rnd = New Random(Seed)
        End Sub

        ''' <summary>
        ''' Produces a Tensor with one randomly picked value set to ActiveValue. 
        ''' RandomClassificationTensor(4, 1, 0) = {0, 1, 0, 0}
        ''' </summary>
        ''' <param name="ClassesCount"></param>
        ''' <param name="ActiveValue"></param>
        ''' <param name="InactiveValue"></param>
        ''' <returns></returns>
        Public Function RandomClassificationTensor(ClassesCount As Integer, Optional ActiveValue As Double = 1, Optional InactiveValue As Double = 0) As Tensor
            Dim nv As New Tensor(ClassesCount, InactiveValue)
            nv(_rnd.Next(0, nv.Count)) = 1
            Return nv
        End Function

        ''' <summary>
        ''' Produces a Tensor with all values set to InitialValue.
        ''' PlainTensor(4, 10) = {10, 10, 10, 10}
        ''' </summary>
        ''' <param name="Length"></param>
        ''' <param name="InitialValue"></param>
        ''' <returns></returns>
        Public Function PlainTensor(Length As Integer, InitialValue As Double) As Tensor
            Return New Tensor(Length, InitialValue)
        End Function


        ''' <summary>
        ''' Creates a random Tensor with coordinates values within the Range.
        ''' RandomTensor(4, 10) = {0.123, 0.234, 0.345, 0.456}
        ''' </summary>
        ''' <param name="Length"></param>
        ''' <param name="Range"></param>
        ''' <returns></returns>
        Public Function RandomTensor(Length As Integer, Range As Structures.Range) As Tensor
            Dim nv As New Tensor(Length)

            For i As Integer = 0 To Length - 1 Step 1
                nv(i) = Range.ValueAtParameter(_rnd.NextDouble)
            Next

            Return nv
        End Function

        Public Function RandomTensor(Shape As IEnumerable(Of Integer), Range As Structures.Range, Optional Jitter As Boolean = True) As Tensor
            Dim nv As New Tensor(Shape)

            For i As Integer = 0 To nv.Length - 1 Step 1
                nv(i) = Range.ValueAtParameter(_rnd.NextDouble)
            Next

            If Jitter Then nv = JitterTensor(nv)

            Return nv
        End Function

        ''' <summary>
        ''' Randomly misplaces the tensor values.
        ''' </summary>
        ''' <returns></returns>
        Public Function JitterTensor(Source As Tensor) As Tensor
            Dim nv As New Tensor(Source.GetShape)
            Dim ids(nv.Length - 1) As Integer
            Dim val(nv.Length - 1) As Double

            For i As Integer = 0 To nv.Length - 1 Step 1
                val(i) = _rnd.NextDouble()
                ids(i) = i
            Next

            Array.Sort(val, ids)

            For i As Integer = 0 To nv.Length - 1 Step 1
                nv(ids(i)) = Source(i)
            Next

            Return nv
        End Function

        ''' <summary>
        ''' Creates a new Tensor by interpolating values from the SourceTensor.
        ''' Output dependent on the TensorInterpolation.
        ''' </summary>
        ''' <param name="SourceTensor"></param>
        ''' <param name="Samples"></param>
        ''' <param name="Interpolation"></param>
        ''' <returns></returns>
        Public Function Interpolate1D(SourceTensor As Tensor, Samples As Integer, Interpolation As TensorInterpolation) As Tensor
            Dim n As New Tensor(Samples)

            Select Case Interpolation
                Case TensorInterpolation.Nearest_Neighbor
                    Dim denom As Double = 1 / (Samples - 1)

                    For i As Integer = 0 To Samples - 1 Step 1
                        Dim thisint As Integer = (i * denom) * (SourceTensor.Count - 1)
                        n(i) = (SourceTensor(thisint))
                    Next

            End Select

            Return n
        End Function

        ''' <summary>
        ''' Interpolation types for Tensor Interpolation.
        ''' </summary>
        Public Enum TensorInterpolation As Integer
            Nearest_Neighbor = 0
        End Enum

        ''' <summary>
        ''' Creates a Tensor based on the series of numbers.
        ''' SeriesTensor(0, 1, 0.25) = {0, 0.25, 0.5, 0.75, 1}
        ''' </summary>
        ''' <param name="From"></param>
        ''' <param name="[To]"></param>
        ''' <param name="[Step]"></param>
        ''' <returns></returns>
        Public Function SeriesTensor(From As Double, [To] As Double, Optional [Step] As Double = 1) As Tensor
            Dim cnt As Integer = 0
            For i As Double = From To [To] Step [Step]
                cnt += 1
            Next

            Dim nv As New Tensor(cnt)

            cnt = 0
            For i As Double = From To [To] Step [Step]
                nv(i) = i
            Next

            Return nv
        End Function


    End Module

End Namespace
