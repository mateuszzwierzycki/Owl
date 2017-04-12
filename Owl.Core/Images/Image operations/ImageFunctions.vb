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

    End Module
End Namespace