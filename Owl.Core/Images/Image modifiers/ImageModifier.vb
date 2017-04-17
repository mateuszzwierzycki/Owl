Imports Owl.Core.Tensors

Namespace Convolutions

    Public MustInherit Class ImageModifier

        MustOverride Sub Apply(ByRef Image As Tensor)

        MustOverride Function Duplicate() As ImageModifier

        ''' <summary>
        ''' You can try the multithreaded method, but it was not debugged yet.
        ''' </summary>
        ''' <param name="ImageSet"></param>
        ''' <param name="Multithread"></param>
        Sub Apply(ByRef ImageSet As TensorSet, Optional Multithread As Boolean = False)
            Dim imgset As TensorSet = ImageSet


            'this multithreading approach might be faulty... expect the unexpected
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
