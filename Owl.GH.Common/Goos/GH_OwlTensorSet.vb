Imports GH_IO.Serialization
Imports Grasshopper.Kernel.Types
Imports Owl.Core.Tensors
Imports Owl.Core.IO

Public Class GH_OwlTensorSet
    Inherits GH_Goo(Of TensorSet)

    Sub New()
        MyBase.New
    End Sub

    Sub New(V As TensorSet)
        MyBase.New(V)
    End Sub

    Public Overrides ReadOnly Property IsValid As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TypeDescription As String
        Get
            Return "TensorSet"
        End Get
    End Property

    Public Overrides ReadOnly Property TypeName As String
        Get
            Return "TensorSet"
        End Get
    End Property

    Public Overrides Function Duplicate() As IGH_Goo
        Return New GH_OwlTensorSet(Me.Value.Duplicate)
    End Function

    Public Overrides Function ToString() As String
        Return Me.Value.ToString
    End Function

    Public Overrides Function CastTo(Of Q)(ByRef target As Q) As Boolean

        Select Case GetType(Q)
            Case GetType(TensorSet)
                Dim mev As TensorSet = Me.Value.Duplicate
                Dim obj As Object = mev
                target = obj
                Return True
        End Select

        Return False

    End Function

    Public Overrides Function CastFrom(source As Object) As Boolean
        Select Case source.GetType
            Case GetType(TensorSet)
                Me.Value = DirectCast(source, TensorSet)
                Return True
            Case GetType(GH_String)
                Dim thiss As GH_String = source
                Me.Value = New TensorSet(thiss.Value)
                Return True
            Case GetType(String)
                Dim this As String = source
                Me.Value = New TensorSet(this)
                Return True
        End Select

        Return False
    End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        Dim ts As New TensorSet

        For i As Integer = 0 To reader.GetInt64("TensorSetCount") - 1 Step 1
            ts.Add(ReadOne(reader, i))
        Next

        Me.Value = ts

        Return True
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean

        writer.SetInt64("TensorSetCount", Me.Value.Count)

        For i As Integer = 0 To Me.Value.Count - 1 Step 1
            WriteOne(Me.Value(i), writer, i)
        Next

        Return True
    End Function

    Public Function ReadOne(reader As GH_IReader, Suffix As String) As Tensor
        Dim shape As New List(Of Integer)
        Dim cnt As Integer = reader.GetInt32("ShapeCount_" & Suffix)

        For i As Integer = 0 To cnt - 1 Step 1
            shape.Add(reader.GetInt32("S" & i & "_" & Suffix))
        Next

        Return New Tensor(shape, reader.GetDoubleArray("Data_" & Suffix))
    End Function

    Public Function WriteOne(tens As Tensor, writer As GH_IWriter, Suffix As String) As Boolean
        writer.SetInt32("ShapeCount_" & Suffix, tens.ShapeCount)
        Dim shape As List(Of Integer) = tens.GetShape

        For i As Integer = 0 To tens.ShapeCount - 1 Step 1
            writer.SetInt32("S" & i & "_" & Suffix, shape(i))
        Next

        writer.SetDoubleArray("Data_" & Suffix, tens.ToArray)
        Return True
    End Function

End Class
