Imports Owl.Core.Tensors

Namespace Images
    Public Module ImageOperators

        ''' <summary>
        ''' A(i) * B(i)
        ''' </summary>
        ''' <param name="A">2D Tensor</param>
        ''' <param name="B">2D Tensor</param>
        ''' <returns></returns>
        Public Function ImageMultiply(A As Tensor, B As Tensor) As Tensor
            If A.Width <> B.Width OrElse A.Height <> B.Height Then Return Nothing

            Dim ni As New Tensor(A)

            For i As Integer = 0 To B.Length - 1 Step 1
                ni(i) *= B(i)
            Next

            Return ni
        End Function

        ''' <summary>
        ''' Computes average value of all the Tensor values.
        ''' </summary>
        ''' <param name="A">2D Tensor</param>
        ''' <returns></returns>
        Public Function ImageAverage(A As Tensor) As Double
            Dim sum As Double = 0

            For i As Integer = 0 To A.Length - 1 Step 1
                sum += A(i)
            Next

            sum /= A.Length
            Return sum
        End Function


    End Module

End Namespace