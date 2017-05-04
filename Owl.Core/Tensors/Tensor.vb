Imports System.Globalization
Imports System.IO
Imports Owl.Core.Tensors

Namespace Tensors

    ''' <summary>
    ''' Shapeable Tensor.
    ''' </summary>
    <Serializable>
    Public Class Tensor
        Inherits Tensors.TensorBase
        Implements IComparable(Of Tensor)

        Private _dims As New List(Of Integer)
        Private _mpliers As New List(Of Integer)

#Region "Constructors"

        Sub New()

        End Sub

        Sub New(Length As Integer)
            MyBase.New(Length)
            OnShapeChange({Length})
        End Sub

        Sub New(Values As IEnumerable(Of Double))
            MyBase.New(Values)
            OnShapeChange({Values.Count})
        End Sub

        Sub New(Shape As IEnumerable(Of Integer))
            MyBase.New(ShapeVolume(Shape))
            OnShapeChange(Shape)
        End Sub

        Sub New(Shape As IEnumerable(Of Integer), Values As IEnumerable(Of Double))
            MyBase.New(ShapeVolume(Shape))
            OnShapeChange(Shape)

            For i As Integer = 0 To Values.Count - 1 Step 1
                TensorData(i) = Values(i)
            Next
        End Sub

        Sub New(Shape As IEnumerable(Of Integer), Values() As Double)
            MyBase.New(Values)
            OnShapeChange(Shape)
        End Sub

        Sub New(Width As Integer, Height As Integer)
            MyBase.New(Width * Height)
            OnShapeChange({Height, Width})
        End Sub

        Sub New(Length As Integer, Value As Double)
            MyBase.New(Length)
            OnShapeChange({Length})
            For i As Integer = 0 To Me.Count - 1 Step 1
                Me(i) = Value
            Next
        End Sub

        Sub New(Width As Integer, Height As Integer, Values As IEnumerable(Of Byte))
            MyBase.New(Width * Height)
            OnShapeChange({Height, Width})

            For i As Integer = 0 To Values.Count - 1 Step 1
                TensorData(i) = Values(i)
            Next
        End Sub

        Sub New(Width As Integer, Height As Integer, Values As IEnumerable(Of Double))
            MyBase.New(Width * Height)
            OnShapeChange({Height, Width})

            For i As Integer = 0 To Values.Count - 1 Step 1
                TensorData(i) = Values(i)
            Next
        End Sub

        Sub New(Width As Integer, Height As Integer, Values() As Double)
            MyBase.New(Width * Height)
            OnShapeChange({Height, Width})

            ReDim Me.TensorData(Values.Length - 1)
            Array.Copy(Values, Me.TensorData, Values.Length)
        End Sub

        Sub New(Other As Tensor)
            MyBase.New(Other.TensorData)
            Shape.AddRange(Other.Shape)
            Multipliers.AddRange(Other.Multipliers)
        End Sub


        ''' <summary>
        ''' 3 10 12 32 0.0123 0.234 0.5532 ...
        ''' Shape Count, Shape, Shape, Shape, Value, Value ... 
        ''' </summary>
        ''' <param name="Text"></param>
        ''' <param name="Value"></param>
        ''' <returns></returns>
        Public Shared Function TryParse(Text As String, ByRef Value As Tensor, Optional Separator As Char = " ") As Boolean
            Dim alltext() As String = Text.Split(Separator)
            Dim shapecount As Integer
            Dim position As Integer = 0

            If Not Integer.TryParse(alltext(0), shapecount) Then Return False
            position += 1

            Dim shape As New List(Of Integer)
            For i As Integer = 0 To shapecount - 1 Step 1
                Dim tp As Integer
                If Not Integer.TryParse(alltext(position), tp) Then Return False
                shape.Add(tp)
                position += 1
            Next

            Dim cnt As Integer = Tensor.ShapeVolume(shape)
            If cnt <> (alltext.Length - position) Then Return False

            Dim vals(cnt - 1) As Double

            For i As Integer = 0 To cnt - 1 Step 1
                vals(i) = Val(alltext(position))
                position += 1
            Next

            Value = New Tensor(shape, vals)
            If Value Is Nothing Then Return False
            Return True
        End Function

#End Region

#Region "Private Stuff"

        Private Sub OnShapeChange(NewShape As IEnumerable(Of Integer))
            Shape.Clear()
            Shape.AddRange(NewShape)
            UpdateMultipliers()
        End Sub

        Public Property Multipliers As List(Of Integer)
            Get
                Return _mpliers
            End Get
            Set(value As List(Of Integer))
                _mpliers = value
            End Set
        End Property

        Private Sub UpdateMultipliers()
            Multipliers.Clear()
            Multipliers.Add(1)

            For i As Integer = ShapeCount - 1 To 1 Step -1
                Multipliers.Add(Shape(i) * Multipliers(Multipliers.Count - 1))
            Next

            Multipliers.Reverse()
        End Sub

        Private Property Shape As List(Of Integer)
            Get
                Return _dims
            End Get
            Set(value As List(Of Integer))
                _dims = value
            End Set
        End Property

        ''' <summary>
        ''' Returns the number of elements in a Tensor with the given shape.
        ''' </summary>
        ''' <param name="Shape"></param>
        ''' <returns></returns>
        Private Shared Function ShapeVolume(Shape As IEnumerable(Of Integer)) As Integer
            Dim vol As Long = 1

            For Each d As Integer In Shape
                vol *= d
            Next

            Return vol
        End Function

#End Region

#Region "Properties"

        ''' <summary>
        ''' It's just the name for the second array dimension, useful for bitmaps.
        ''' AKA Column count
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Width As Integer 'DIMENSIONS=================================================================
            Get
                If Me.ShapeCount < 2 Then Return 1
                Return ShapeAt(1)
            End Get
        End Property

        ''' <summary>
        ''' It' s just a name for the first array dimension, useful for bitmaps.
        ''' AKA Row count
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Height As Integer
            Get
                Return ShapeAt(0)
            End Get
        End Property

        Public ReadOnly Property ShapeAt(Dimension As Integer) As Integer 'SHAPE=====================================================================
            Get
                Return _dims(Dimension)
            End Get
        End Property

        Public ReadOnly Property ShapeCount() As Integer
            Get
                Return _dims.Count
            End Get
        End Property

        Public Property ValueAt(index As Integer) As Double 'VALUES============================================================
            Get
                Return TensorData(index)
            End Get
            Set(value As Double)
                TensorData(index) = value
            End Set
        End Property

        ''' <summary>
        ''' This is only valid when the Tensor has 2D Shape
        ''' </summary>
        ''' <param name="Column"></param>
        ''' <param name="Row"></param>
        ''' <returns></returns>
        Public Property ValueAt(Column As Integer, Row As Integer) As Double
            Get
                If Column > Me.Width - 1 Then Throw New IndexOutOfRangeException("Outside of the image bounds")
                Return TensorData(Column + (Row * Multipliers(0)))
            End Get
            Set(value As Double)
                TensorData(Column + (Row * Multipliers(0))) = value
            End Set
        End Property

        Public Property ValueAt(Coordinates As IEnumerable(Of Integer)) As Double
            Get
                Return Me.TensorData(CoordinateToIndex(Coordinates))
            End Get
            Set(value As Double)
                Me.TensorData(CoordinateToIndex(Coordinates)) = value
            End Set
        End Property

#End Region

#Region "Functions"

        ''' <summary>
        ''' Splits the Tensor into a TensorSet by the leftmost shape dimension. 
        ''' Tensor(2,3) will be splitted into 2 Tensors(3)
        ''' </summary>
        ''' <returns></returns>
        Public Function SplitIntoSet() As TensorSet
            Dim ts As New TensorSet
            Dim leftmost As Integer = Me.ShapeAt(0)

            Dim tlen As Integer = Me.Length / leftmost
            Dim tshp As New List(Of Integer)(Me.GetShape)
            tshp.RemoveAt(0)

            For i As Integer = 0 To leftmost - 1 Step 1
                Dim vals(tlen - 1) As Double
                Array.Copy(Me.TensorData, i * tlen, vals, 0, vals.Length)
                ts.Add(New Tensor(tshp, vals))
            Next

            Return ts
        End Function

        Public Function DistanceTo(Other As Tensor) As Double
            Return Math.Sqrt(Me.DistanceTo2(Other))
        End Function

        ''' <summary>
        ''' Returns squared distance value (no sqrt).
        ''' </summary>
        ''' <param name="Other"></param>
        ''' <returns></returns>
        Public Function DistanceTo2(Other As Tensor) As Double
            Dim dist As Double = 0

            For i As Integer = 0 To Me.Count - 1 Step 1
                dist += (Me(i) - Other(i)) ^ 2
            Next

            Return dist
        End Function

        Public Function GetShape() As List(Of Integer)
            Dim nl As New List(Of Integer)(Me.Shape)
            Return nl
        End Function

        Public Function CoordinateToIndex(Coordinates As IEnumerable(Of Integer)) As Long
            Dim ind As Long = 0

            For i As Integer = 0 To Coordinates.Count - 1 Step 1
                If Coordinates(i) > Shape(i) - 1 Then Throw New IndexOutOfRangeException("Coordinate outside of the Tensor bounds") : Exit Function
                ind += Coordinates(i) * Multipliers(i)
            Next

            Return ind
        End Function

        Public Overrides Function Duplicate() As TensorBase
            Return New Tensor(Me)
        End Function

        Public Function TryReshape(Shape As IEnumerable(Of Integer)) As Boolean
            Dim vols As Int64 = 1

            For Each d As Integer In Shape
                vols *= d
            Next

            If vols <> Me.TensorData.Length Then Return False
            OnShapeChange(Shape)

            Return True
        End Function

        Public Overrides Function ToString() As String
            If Me.ShapeCount = 1 Then Return ToStringSimple()
            Dim strout As String = "Tensor ("

            For i As Integer = 0 To Me.ShapeCount - 1 Step 1
                strout &= Me.ShapeAt(i) & If(i = Me.ShapeCount - 1, "", " by ")
            Next

            strout &= ")"
            Return strout
        End Function

        Private Function ToStringSimple() As String
            Dim strout As String = "Tensor ("

            If Me.Count <= LibrarySettings.TensorPrintLength Then
                For i As Integer = 0 To Me.Count - 1 Step 1
                    strout &= Me(i).ToString(LibrarySettings.TensorFormat) & If(i < Me.Count - 1, ", ", "")
                Next
            Else
                strout &= Me.Count & If(Me.Count = 1, " Dimension", " Dimensions")
            End If

            strout &= ")"
            Return strout
        End Function

#End Region

#Region "Methods"

        Public Delegate Function EvaluateExpression2D(X As Integer, Y As Integer, CurrentValue As Double) As Double
        Public Delegate Function EvaluateExpression1D(Index As Integer, CurrentValue As Double) As Double

        ''' <summary>
        ''' Apply more complex functions (convolutions ?) to the Tensor
        ''' </summary>
        ''' <param name="A">Tensor</param>
        ''' <param name="Expression"></param>
        Public Shared Sub Evaluate(ByRef A As Tensor, Expression As EvaluateExpression1D)
            For i As Integer = 0 To A.Length - 1 Step 1
                Dim val As Double = A.ValueAt(i)
                val = Expression(i, val)
                A.ValueAt(i) = val
            Next
        End Sub

        ''' <summary>
        ''' Apply more complex functions (convolutions ?) to the Tensor
        ''' </summary>
        ''' <param name="A">2D Tensor</param>
        ''' <param name="Expression"></param>
        Public Shared Sub Evaluate(ByRef A As Tensor, Expression As EvaluateExpression2D)
            For i As Integer = 0 To A.Height - 1 Step 1
                For j As Integer = 0 To A.Width - 1 Step 1
                    Dim val As Double = A.ValueAt(j, i)
                    val = Expression(j, i, val)
                    A.ValueAt(j, i) = val
                Next
            Next
        End Sub

#End Region

#Region "Operators"

        ''' <summary>
        ''' Supports only up to 2 dimensional matrices... sorry.
        ''' </summary>
        ''' <param name="A"></param>
        ''' <param name="B"></param>
        ''' <returns></returns>
        Public Shared Function MatMul(A As Tensor, B As Tensor) As Tensor
            Dim AB As New Tensor(B.Width, A.Height)

            For i As Integer = 0 To AB.Height - 1 Step 1
                For j As Integer = 0 To AB.Width - 1 Step 1
                    Dim thisval As Double = 0

                    For p As Integer = 0 To A.Width - 1 Step 1
                        thisval += B.ValueAt(j, p) * A.ValueAt(p, i)
                    Next

                    AB.ValueAt(j, i) = thisval
                Next
            Next

            Return AB
        End Function

        Public Shared Operator =(A As Tensor, B As Tensor) As Boolean
            If A.Count <> B.Count Then Return False
            For i As Integer = 0 To A.Count - 1 Step 1
                If A(i) <> B(i) Then Return False
            Next
            Return True
        End Operator

        Public Shared Operator <>(A As Tensor, B As Tensor) As Boolean
            Return Not A = B
        End Operator

        Public Shared Operator +(A As Tensor, B As Tensor) As Tensor
            Dim C As New Tensor(A)

            For i As Integer = 0 To A.Count - 1 Step 1
                C(i) += B(i)
            Next

            Return C
        End Operator

        Public Sub Add(Value As Tensor)
            For i As Integer = 0 To Me.Length - 1 Step 1
                Me(i) += Value(i)
            Next
        End Sub

        Public Shared Operator +(A As Tensor, V As Double) As Tensor
            Dim C As New Tensor(A)

            For i As Integer = 0 To A.Count - 1 Step 1
                C(i) += V
            Next

            Return C
        End Operator

        Public Shared Operator -(A As Tensor, V As Double) As Tensor
            Dim C As New Tensor(A)

            For i As Integer = 0 To A.Count - 1 Step 1
                C(i) -= V
            Next

            Return C
        End Operator

        Public Shared Operator -(A As Tensor, B As Tensor) As Tensor
            Dim C As New Tensor(A)

            For i As Integer = 0 To A.Count - 1 Step 1
                C(i) -= B(i)
            Next

            Return C
        End Operator

        Public Shared Operator *(A As Tensor, V As Double) As Tensor
            Dim C As New Tensor(A)

            For i As Integer = 0 To C.Count - 1 Step 1
                C(i) *= V
            Next

            Return C
        End Operator

        Public Shared Operator /(A As Tensor, V As Double) As Tensor
            If V = 0 Then Return Nothing
            Dim denom As Double = 1 / V

            Dim C As New Tensor(A)

            For i As Integer = 0 To C.Count - 1 Step 1
                C(i) *= denom
            Next

            Return C
        End Operator

        Public Overloads Function CompareTo(other As Tensor) As Integer Implements IComparable(Of Tensor).CompareTo
            Return MyBase.CompareTo(other)
        End Function

#End Region

    End Class

    Public Class TensorComparer
        Implements IComparer(Of Tensor)
        Implements IComparer

        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
            Dim ts As Tensor = x
            Dim tt As Tensor = y
            Return Compare(ts, tt)
        End Function

        Public Function Compare(x As Tensor, y As Tensor) As Integer Implements IComparer(Of Tensor).Compare
            If x.Length < y.Length Then Return -1
            If x.Length > y.Length Then Return 1

            For i As Integer = 0 To x.Count - 1 Step 1
                If x(i) < y(i) Then Return -1
                If x(i) > y(i) Then Return 1
            Next

            Return 0
        End Function
    End Class

End Namespace