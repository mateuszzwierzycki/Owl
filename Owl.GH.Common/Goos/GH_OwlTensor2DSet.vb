Imports GH_IO.Serialization
Imports Grasshopper.Kernel.Types
Imports Owl.Tensors

Public Class GH_OwlTensor2DSet
    Inherits GH_Goo(Of Tensor2DSet)

    Sub New()
        MyBase.New
    End Sub

    Sub New(V As Tensor2DSet)
        MyBase.New(V)
    End Sub

    Public Overrides ReadOnly Property IsValid As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TypeDescription As String
        Get
            Return "Tensor2DSet"
        End Get
    End Property

    Public Overrides ReadOnly Property TypeName As String
        Get
            Return "Tensor2DSet"
        End Get
    End Property

    Public Overrides Function Duplicate() As IGH_Goo
        Return New GH_OwlTensor2DSet(Me.Value.Duplicate)
    End Function

    Public Overrides Function ToString() As String
        Return Me.Value.ToString
    End Function

    Public Overrides Function CastTo(Of Q)(ByRef target As Q) As Boolean

        Select Case GetType(Q)
            Case GetType(Tensor2DSet)
                Dim mev As Tensor2DSet = Me.Value.Duplicate
                Dim obj As Object = mev
                target = obj
                Return True
            Case GetType(TensorSet)
                Dim mev As New TensorSet
                For i As Integer = 0 To Me.Value.Count - 1 Step 1
                    mev.Add(New Tensor(Me.Value(i).TensorData))
                Next
                Dim obj As Object = mev
                target = obj
                Return True
        End Select

        Return False

    End Function

    Public Overrides Function CastFrom(source As Object) As Boolean
        Select Case source.GetType
            Case GetType(Tensor2DSet)
                Me.Value = DirectCast(source, Tensor2DSet)
                Return True
            Case GetType(TensorSet)
                Dim ts As TensorSet = source
                Dim thisv As New Tensor2DSet
                For i As Integer = 0 To ts.Count - 1 Step 1
                    thisv.Add(New TensorD(ts(i).Length, 1, ts(i).TensorData))
                Next
                Return True
        End Select

        Return False
    End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean

        Dim vs As New Tensor2DSet

        For i As Integer = 0 To reader.GetInt64("TensorSetCount") - 1 Step 1
            vs.Add(New TensorD(reader.GetInt32("W_" & i), reader.GetInt32("H_" & i), reader.GetDoubleArray("Tensor_" & i)))
        Next

        Me.Value = vs

        Return True
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean

        writer.SetInt64("TensorSetCount", Me.Value.Count)

        For i As Integer = 0 To Me.Value.Count - 1 Step 1
            writer.SetInt32("W_" & i, Me.Value(i).Width)
            writer.SetInt32("H_" & i, Me.Value(i).Height)
            writer.SetDoubleArray("Tensor_" & i, Me.Value(i).ToArray)
        Next

        Return True
    End Function

End Class
