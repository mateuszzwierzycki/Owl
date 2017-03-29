Imports System.IO
Imports Owl.Core.Tensors
Imports Owl.Core.IO


''' <summary>
''' Various examples for the fellow developers.
''' </summary>
Module Snippets

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


End Module
