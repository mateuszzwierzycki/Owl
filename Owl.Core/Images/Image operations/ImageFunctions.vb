Imports Owl.Core.Tensors


Namespace Images
    Public Module ImageFunctions

        Public Function Crop(Tens As Tensor, X As Integer, Y As Integer, W As Integer, H As Integer) As Tensor
            Dim nts As New Tensor(W, H)

            For i As Integer = 0 To H - 1 Step 1
                For j As Integer = 0 To W - 1 Step 1
                    nts.ValueAt(j, i) = Tens.ValueAt(X + j, Y + i)
                Next
            Next

            Return nts
        End Function

        Public Function Mirror(Tens As Tensor, Optional Vertical As Boolean = True) As Tensor
            Dim newtens As New Tensor(Tens.GetShape)

            If Vertical Then
                Dim rev(Tens.TensorData.Length - 1) As Double
                Tens.TensorData.CopyTo(rev, 0)
                Array.Reverse(rev)

                For i As Integer = 0 To Tens.Height - 1 Step 1
                    Array.Copy(rev, i * Tens.Width, newtens.TensorData, (Tens.Height - i - 1) * Tens.Width, Tens.Width)
                Next
            Else
                For i As Integer = 0 To Tens.Height - 1 Step 1
                    Array.Copy(Tens.TensorData, i * Tens.Width, newtens.TensorData, (Tens.Height - i - 1) * Tens.Width, Tens.Width)
                Next
            End If

            Return newtens
        End Function

        ''' <summary>
        ''' Rotating a 2D Tensor 180 degrees is equivalent to reversing it's internal array.
        ''' </summary>
        ''' <param name="Tens"></param>
        ''' <returns></returns>
        Public Function Rotate180(Tens As Tensor) As Tensor
            Dim nt As New Tensor(Tens.Width, Tens.Height, Tens.TensorData)
            Array.Reverse(nt.TensorData)
            Return nt
        End Function

        Public Function Rotate90(Tens As Tensor, Optional Clockwise As Boolean = False) As Tensor
            Dim nt As New Tensor(Tens.Height, Tens.Width)

            If Clockwise Then
                For i As Integer = 0 To Tens.Height - 1 Step 1
                    For j As Integer = 0 To Tens.Width - 1 Step 1
                        Dim tensval As Double = Tens.ValueAt(j, i)
                        nt.ValueAt(nt.Width - i - 1, j) = tensval
                    Next
                Next
            Else
                For i As Integer = 0 To Tens.Height - 1 Step 1
                    For j As Integer = 0 To Tens.Width - 1 Step 1
                        Dim tensval As Double = Tens.ValueAt(j, i)
                        nt.ValueAt(i, nt.Height - j - 1) = tensval
                    Next
                Next
            End If

            Return nt
        End Function

        Public Function Transpose(Tens As Tensor) As Tensor
            Dim nt As New Tensor(Tens.Height, Tens.Width)

            For i As Integer = 0 To Tens.Height - 1 Step 1
                For j As Integer = 0 To Tens.Width - 1 Step 1
                    nt.ValueAt(i, j) = Tens.ValueAt(j, i)
                Next
            Next

            Return nt
        End Function

    End Module
End Namespace