Imports Owl.Core.Tensors

Namespace Convolutions

    Public MustInherit Class ImageModifier

        MustOverride Sub Apply(ByRef Image As Tensor)

        MustOverride Function Duplicate() As ImageModifier

        Sub Apply(ByRef ImageSet As TensorSet, Optional Multithread As Boolean = True)
            Dim imgset As TensorSet = ImageSet

            If Multithread Then
                Parallel.For(0, imgset.Count, Sub(index As Integer)
                                                  Me.Apply(imgset(index))
                                              End Sub)
            Else
                For i As Integer = 0 To imgset.Count - 1 Step 1
                    Me.Apply(imgset(i))
                Next
            End If

            ImageSet = imgset
        End Sub

    End Class

End Namespace
