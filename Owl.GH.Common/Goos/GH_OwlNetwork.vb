Imports System.Drawing
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports GH_IO.Serialization
Imports Grasshopper.Kernel.Types
Imports Owl.Core.Tensors
Imports Owl.Learning.Networks
Imports Rhino.Geometry

Public Class GH_OwlNetwork
    Inherits GH_Goo(Of Network)

    Sub New()
        MyBase.New
    End Sub

    Sub New(Net As Network)
        MyBase.New(Net)
    End Sub

    Public Overrides ReadOnly Property IsValid As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TypeDescription As String
        Get
            Return "Owl.Learning.Network"
        End Get
    End Property

    Public Overrides ReadOnly Property TypeName As String
        Get
            Return "Owl.Learning.Network"
        End Get
    End Property

    Public Overrides Function Duplicate() As IGH_Goo
        Return New GH_OwlNetwork(Me.Value.Duplicate)
    End Function

    Public Overrides Function ToString() As String
        Return Me.Value.ToString
    End Function

    Public Overrides Function CastTo(Of Q)(ByRef target As Q) As Boolean

        Select Case GetType(Q)
            Case GetType(Network)
                Dim obj As Object = Me.Value.Duplicate
                target = obj
                Return True
        End Select

        Return False

    End Function

    'TODO casting network
    'Public Overrides Function CastFrom(source As Object) As Boolean
    '    Select Case source.GetType
    '        Case GetType(Tensor)
    '            Me.Value = DirectCast(source, Tensor)
    '            Return True

    '    End Select

    '    Return False
    'End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean

        Dim nn As New BinaryFormatter()

        Dim f As New List(Of Owl.Learning.NeuronFunctions.NeuronFunctionBase)
        Dim fc As Integer = reader.GetInt32("FunctionCount")
        Dim w As TensorSet = Nothing
        Dim b As TensorSet = Nothing

        For i As Integer = 0 To fc - 1 Step 1
            Using mstr As New MemoryStream(reader.GetByteArray("Function_" & i))
                f.Add(nn.Deserialize(mstr))
            End Using
        Next

        Using mstr As New MemoryStream(reader.GetByteArray("Biases"))
            b = Owl.Core.IO.ReadTensors(mstr)
        End Using

        Using mstr As New MemoryStream(reader.GetByteArray("Weigths"))
            w = Owl.Core.IO.ReadTensors(mstr)
        End Using

        Me.Value = New Owl.Learning.Networks.Network(w, b, f)
        Return True
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean

        Dim nn As New BinaryFormatter()

        writer.SetInt32("FunctionCount", Me.Value.NeuronFunctions.Count)

        For i As Integer = 0 To Me.Value.NeuronFunctions.Count - 1 Step 1
            Using mstr As New MemoryStream
                nn.Serialize(mstr, Me.Value.NeuronFunctions(i))
                writer.SetByteArray("Function_" & i, mstr.ToArray())
            End Using
        Next

        Using mstr As New MemoryStream
            Owl.Core.IO.WriteTensors(mstr, Me.Value.Biases)
            writer.SetByteArray("Biases", mstr.ToArray)
        End Using

        Using mstr As New MemoryStream
            Owl.Core.IO.WriteTensors(mstr, Me.Value.Weights)
            writer.SetByteArray("Weights", mstr.ToArray)
        End Using

        Return True
    End Function

End Class
