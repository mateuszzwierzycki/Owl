Imports System.Drawing

Namespace Structures

    ''' <summary>
    ''' Source: https://www.programmingalgorithms.com/algorithm/hsl-to-rgb?lang=VB.Net
    ''' </summary>
    Public Structure ColorHSL
        Private _a As Single
        Private _h As Integer
        Private _s As Single
        Private _l As Single

        ''' <summary>
        ''' </summary>
        ''' <param name="a">1.0</param>
        ''' <param name="h">360</param>
        ''' <param name="s">1.0</param>
        ''' <param name="l">1.0</param>
        Public Sub New(a As Single, h As Integer, s As Single, l As Single)
            Me._h = h
            Me._s = s
            Me._l = l
        End Sub

        Public Property H() As Integer
            Get
                Return Me._h
            End Get
            Set(value As Integer)
                Me._h = value
            End Set
        End Property

        Public Property S() As Single
            Get
                Return Me._s
            End Get
            Set(value As Single)
                Me._s = value
            End Set
        End Property

        Public Property L() As Single
            Get
                Return Me._l
            End Get
            Set(value As Single)
                Me._l = value
            End Set
        End Property

        Public Property A As Single
            Get
                Return _a
            End Get
            Set(value As Single)
                _a = value
            End Set
        End Property

        Public Overloads Function Equals(hsl As ColorHSL) As Boolean
            Return (Me.A = hsl.A) AndAlso (Me.H = hsl.H) AndAlso (Me.S = hsl.S) AndAlso (Me.L = hsl.L)
        End Function

    End Structure

End Namespace

