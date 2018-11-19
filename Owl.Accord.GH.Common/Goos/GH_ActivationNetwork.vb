Imports GH_IO.Serialization
Imports Grasshopper.Kernel.Types
Imports System.IO
Imports Accord.Neuro


Public Class GH_ActivationNetwork
    Inherits GH_Goo(Of ActivationNetwork)

    Sub New()
        MyBase.New
    End Sub

    Sub New(N As ActivationNetwork)
        MyBase.New(N)
    End Sub

    Public Overrides ReadOnly Property IsValid As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TypeDescription As String
        Get
            Return "ActivationNetwork"
        End Get
    End Property

    Public Overrides ReadOnly Property TypeName As String
        Get
            Return "ActivationNetwork"
        End Get
    End Property

    Public Overrides Function Duplicate() As IGH_Goo
        Return New GH_ActivationNetwork(Me.DuplicateNetwork)
    End Function

    Public Function DuplicateNetwork() As ActivationNetwork
        Return Owl.Accord.Extensions.Learning.DuplicateNetwork(Me.Value)
    End Function

    Public Overrides Function ToString() As String
        Dim str As String = "ActivationNetwork {"

        Dim ints As New List(Of Integer)

        str &= (Me.Value.InputsCount)

        For i As Integer = 0 To Me.Value.Layers.Count - 1 Step 1
            str &= ", " & (Me.Value.Layers(i).Neurons.Count)
        Next

        str &= "}"

        Return str
    End Function

    Public Overrides Function CastTo(Of Q)(ByRef target As Q) As Boolean

        Select Case GetType(Q)
            Case GetType(ActivationNetwork)
                Dim mev As ActivationNetwork = Me.DuplicateNetwork
                Dim obj As Object = mev
                target = obj
                Return True
        End Select

        Return False

    End Function

    Public Overrides Function CastFrom(source As Object) As Boolean

        Select Case source.GetType
            Case GetType(ActivationNetwork)
                Me.Value = DirectCast(source, ActivationNetwork)
                Return True
        End Select

        Return False
    End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean

        Dim bytes() As Byte = reader.GetByteArray("Network")

        Using bs As System.IO.MemoryStream = New MemoryStream(bytes)
            Me.Value = ActivationNetwork.Load(bs)
        End Using

        Return True
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean

        Using bs As System.IO.MemoryStream = New MemoryStream()
            Me.Value.Save(bs)
            writer.SetByteArray("Network", bs.GetBuffer())
        End Using

        Return True
    End Function

End Class
