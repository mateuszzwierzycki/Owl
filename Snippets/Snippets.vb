Imports System.IO
Imports Owl.Core.Tensors
Imports Owl.Core.IO


''' <summary>
''' Various examples for the fellow developers.
''' </summary>
<HideModuleName>
Module Snippets

#Region "Basics"

    ''' <summary>
    ''' Example showing the really basic way to create Tensors and add them to a TensorSet.
    ''' </summary>
    ''' <returns></returns>
    Private Function CreateTensors() As TensorSet
        Dim result As New TensorSet()
        Dim rnd As New Random(123)

        'We will create a 2D Tensor, those will be the dimensions. It will be square array.
        Dim tensorWidth As Integer = 10
        Dim tensorLength As Integer = tensorWidth ^ 2

        'The number of Tensors to create.
        Dim tensorCount As Integer = 15

        For i = 0 To tensorCount - 1
            'Tensor is essentially a multidimensional Array... 
            'The difference is that you can change the number and size of array dimensions whenever you like.

            'This will create a simple 1D Tensor, we can Reshape it later.
            Dim thisTens As New Tensor(tensorCount)

            'Thanks to the ability to Reshape dynamically, we can greatly simplify the way we create a random Tensor.
            For j = 0 To thisTens.Length - 1
                thisTens(j) = rnd.NextDouble
            Next

            'Now we can try to reshape the Tensor. If it's not possible (tensorWidth ^ 2 <> TensorLength) then we throw an error.
            If Not thisTens.TryReshape({tensorWidth, tensorWidth}) Then Throw New Exception("Wrong Tensor Shape") : Exit Function

            'The TensorSet is essentially a List of Tensors. 
            'While the Tensor is fixed with length (just like an Array)
            'It's way easier to work with dynamic collections in the end.
            result.Add(thisTens)
        Next

        'Let's normalize the TensorSet before returning. 
        result.Remap(New Structures.Range(0, 1))

        Return result
    End Function

#End Region

#Region "I/O"

    ''' <summary>
    ''' Example showing how to use streams with tensors.
    ''' </summary>
    ''' <returns></returns>
    Private Function StreamingTest() As Boolean
        Dim ts As New TensorSet()
        ts.Add(New Tensor(10))
        ts(0)(2) = 3

        Dim tsread As TensorSet = Nothing

        Using ms As New MemoryStream
            WriteTensors(ms, ts)
            ms.Position = 0
            tsread = ReadTensors(ms)
        End Using

        Debug.WriteLine(tsread(0).ToString)

        Return True
    End Function

#End Region

#Region "Expression example"

    ''' <summary>
    ''' This function will introduce a delegate Expression function.
    ''' </summary>
    ''' <returns></returns>
    Private Function RandomExpressionExample() As TensorSet
        ExpressionRandom = New Random(123)
        Dim exp As Tensor.EvaluateExpression1D = AddressOf RandomGen

        'We will use parallel loop to speed up the whole expression evaluation
        'TensorSet being a simple dynamic collection is not thread-safe, we will use an Array instead.
        Dim Tensors(9) As Tensor

        Parallel.For(0, Tensors.Length, Sub(i As Integer)
                                            Dim thisTens As New Tensor(100)
                                            Tensor.Evaluate(thisTens, exp)
                                            Tensors(i) = thisTens
                                        End Sub)

        Dim ts As New TensorSet(Tensors)
        ExpressionRandom = Nothing 'makes the GC's life a bit easier.

        Return ts
    End Function

    Private ExpressionRandom As Random = Nothing

    Private Function RandomGen(index As Integer, CurrentValue As Double) As Double
        If index Mod 2 = 0 Then
            Return -ExpressionRandom.NextDouble()
        Else
            Return ExpressionRandom.NextDouble()
        End If
    End Function

#End Region

End Module
