Imports GH_IO.Serialization
Imports Grasshopper.Kernel.Types
Imports Owl.Tensors

Public Class GH_OwlTensor2D
    Inherits GH_Goo(Of TensorD)

    Sub New()
        MyBase.New
    End Sub

    Sub New(V As TensorD)
        MyBase.New(V)
    End Sub

    Public Overrides ReadOnly Property IsValid As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TypeDescription As String
        Get
            Return "Tensor2D"
        End Get
    End Property

    Public Overrides ReadOnly Property TypeName As String
        Get
            Return "Tensor2D"
        End Get
    End Property

    Public Overrides Function Duplicate() As IGH_Goo
        Return New GH_OwlTensor(Me.Value.Duplicate)
    End Function

    Public Overrides Function ToString() As String
        Return Me.Value.ToString
    End Function

    Public Overrides Function CastTo(Of Q)(ByRef target As Q) As Boolean

        Select Case GetType(Q)
            Case GetType(TensorD)
                Dim obj As Object = Me.Value.Duplicate
                target = obj
                Return True
            Case GetType(Tensor)
                Dim obj As Object = New Tensor(Me.Value.TensorData)
                target = obj
                Return True
            Case GetType(GH_OwlTensor)

            Case GetType(GH_OwlTensor2D)

            Case GetType(TensorSet)
            Case GetType(TensorSet)

        End Select

        Return False

    End Function

    Public Overrides Function CastFrom(source As Object) As Boolean
        Select Case source.GetType
            Case GetType(TensorD)
                Me.Value = DirectCast(source, TensorD)
                Return True
            Case GetType(Tensor)
                Dim tens As Tensor = source
                Me.Value = New TensorD(tens.Length, 1, tens.TensorData)
                Return True
        End Select

        Return False
    End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        Me.Value = New TensorD(reader.GetInt32("W"), reader.GetInt32("H"), reader.GetDoubleArray("Tensor"))
        Return True
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        writer.SetInt32("W", Me.Value.Width)
        writer.SetInt32("H", Me.Value.Height)
        writer.SetDoubleArray("Tensor", Me.Value.ToArray)
        Return True
    End Function

End Class
