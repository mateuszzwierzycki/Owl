Imports System.Drawing
Imports GH_IO.Serialization
Imports Grasshopper.Kernel.Types
Imports Owl.Core.Tensors
Imports Rhino.Geometry

Public Class GH_OwlTensor
    Inherits GH_Goo(Of Tensor)

    Sub New()
        MyBase.New
    End Sub

    Sub New(V As Tensor)
        MyBase.New(V)
    End Sub

    Public Overrides ReadOnly Property IsValid As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TypeDescription As String
        Get
            Return "Tensor"
        End Get
    End Property

    Public Overrides ReadOnly Property TypeName As String
        Get
            Return "Tensor"
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
            Case GetType(Tensor)
                Dim obj As Object = Me.Value.Duplicate
                target = obj
                Return True
        End Select

        Return False

    End Function

    Public Overrides Function CastFrom(source As Object) As Boolean
        Select Case source.GetType
            Case GetType(Tensor)
                Me.Value = DirectCast(source, Tensor)
                Return True
            Case GetType(TensorSet)
                Dim ts As TensorSet = DirectCast(source, TensorSet)
                If ts.Count <> 1 Then Return False
                Me.Value = ts(0)
                Return True
            Case GetType(Point3d)
                Dim asp As Point3d = source
                Me.Value = New Tensor({asp.X, asp.Y, asp.Z})
                Return True
            Case GetType(GH_Point)
                Dim asp As GH_Point = source
                Me.Value = New Tensor({asp.Value.X, asp.Value.Y, asp.Value.Z})
                Return True
            Case GetType(GH_Vector)
                Dim asp As GH_Vector = source
                Me.Value = New Tensor({asp.Value.X, asp.Value.Y, asp.Value.Z})
                Return True
            Case GetType(Vector3d)
                Dim asp As Vector3d = source
                Me.Value = New Tensor({asp.X, asp.Y, asp.Z})
                Return True
            Case GetType(Color)
                Dim asp As Color = source
                Me.Value = New Tensor({CDbl(asp.A), CDbl(asp.R), CDbl(asp.G), CDbl(asp.B)})
                Return True
            Case GetType(GH_Colour)
                Dim asp As GH_Colour = source
                Me.Value = New Tensor({CDbl(asp.Value.A), CDbl(asp.Value.R), CDbl(asp.Value.G), CDbl(asp.Value.B)})
                Return True
            Case GetType(Line)
                Dim asp As Line = source
                Me.Value = New Tensor({asp.From.X, asp.From.Y, asp.From.Z, asp.To.X, asp.To.Y, asp.To.Z})
                Return True
            Case GetType(GH_Line)
                Dim thisv As GH_Line = source
                Dim asp As Line = thisv.Value
                Me.Value = New Tensor({asp.From.X, asp.From.Y, asp.From.Z, asp.To.X, asp.To.Y, asp.To.Z})
                Return True
        End Select

        Return False
    End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        Dim shape As New List(Of Integer)
        Dim cnt As Integer = reader.GetInt32("ShapeCount")

        For i As Integer = 0 To cnt - 1 Step 1
            shape.Add(reader.GetInt32("S" & i))
        Next

        Me.Value = New Tensor(shape, reader.GetDoubleArray("Data"))
        Return True
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        writer.SetInt32("ShapeCount", Me.Value.ShapeCount)
        Dim shape As List(Of Integer) = Me.Value.GetShape

        For i As Integer = 0 To Me.Value.ShapeCount - 1 Step 1
            writer.SetInt32("S" & i, shape(i))
        Next

        writer.SetDoubleArray("Data", Me.Value.ToArray)
        Return True
    End Function

End Class
