Imports Owl.Core.Tensors

Namespace Convolutions

    Public Class Convolution
        Inherits ImageModifier

        Private m_k(,) As Integer = Nothing
        Private m_d As Double = 1
        Private m_o As Double = 0

        Sub New(Kernel(,) As Integer, Optional Divisor As Double = 1, Optional Offset As Double = 0)
            ReDim m_k(Kernel.GetUpperBound(0), Kernel.GetUpperBound(1))

            m_d = Divisor
            m_o = Offset

            For i As Integer = 0 To Kernel.GetUpperBound(1)
                For j As Integer = 0 To Kernel.GetUpperBound(0)
                    m_k(j, i) = Kernel(j, i)
                Next
            Next
        End Sub

        Public Overrides Function Duplicate() As ImageModifier
            Return New Convolution(Me.m_k, Me.m_d, Me.m_o)
        End Function

        Sub New(Width As Integer, Height As Integer)
            ReDim m_k(Width - 1, Height - 1)
        End Sub

        Sub SetKernel(Kernel(,) As Integer)
            For i As Integer = 0 To Kernel.GetUpperBound(1)
                For j As Integer = 0 To Kernel.GetUpperBound(0)
                    m_k(j, i) = Kernel(j, i)
                Next
            Next
        End Sub

        Public Property Divisor As Double
            Get
                Return m_d
            End Get
            Set(value As Double)
                m_d = value
            End Set
        End Property

        Public Property Offset As Double
            Get
                Return m_o
            End Get
            Set(value As Double)
                m_o = value
            End Set
        End Property

        Public ReadOnly Property Width As Integer
            Get
                Return m_k.GetUpperBound(0) + 1
            End Get
        End Property

        Public ReadOnly Property Height As Integer
            Get
                Return m_k.GetUpperBound(1) + 1
            End Get
        End Property

        Default Public Property Value(Row As Integer, Column As Integer) As Integer
            Get
                Return m_k(Row, Column)
            End Get
            Set(value As Integer)
                m_k(Row, Column) = value
            End Set
        End Property

        Public Overrides Sub Apply(ByRef Image As Tensor)
            Dim woff As Integer = (Me.Width - 1) / 2
            Dim hoff As Integer = (Me.Height - 1) / 2

            Dim nim As New Tensor(Image.Width - (woff * 2), Image.Height - (hoff * 2))

            Dim denom As Double = 1 / Divisor

            For i As Integer = hoff To Image.Height - hoff - 1 Step 1
                For j As Integer = woff To Image.Width - woff - 1 Step 1
                    Dim sum As Double = 0

                    For k As Integer = hoff To -hoff Step -1
                        For p As Integer = -woff To woff Step 1
                            Dim thissourcepx As Double = Image.ValueAt(j + p, i + k)
                            thissourcepx *= Me(p + woff, k + hoff)
                            sum += thissourcepx
                        Next
                    Next

                    sum *= denom
                    sum += Offset
                    ' sum = Math.Min(Math.Max(0, sum), 255)
                    nim.ValueAt(j - woff, i - hoff) = sum
                Next
            Next

            Image = nim
        End Sub

        Public Overrides Function ToString() As String

            Dim str As New String("")

            str &= "<"

            For i As Integer = 0 To Me.Height - 1 Step 1

                str &= "("

                For j As Integer = 0 To Me.Width - 1 Step 1
                    str &= Me(i, j)
                    If j < Me.Width - 1 Then str &= " "
                Next

                str &= ")"

                If i < Me.Height - 1 Then str &= " "

            Next

            str &= ">"

            Return "Convolution " & str & " Divisor:" & Divisor.ToString("0.0") & " Offset:" & Offset.ToString("0.0")
        End Function

        ''' <summary>
        ''' Convolution({{1, 2, 1}, {2, 4, 2}, {1, 2, 1}}, 16, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GaussianBlur() As Convolution
            Return New Convolution({{1, 2, 1}, {2, 4, 2}, {1, 2, 1}}, 16, 0)
        End Function

        ''' <summary>
        ''' Convolution({{1, 1, 1}, {1, 1, 1}, {1, 1, 1}}, 9, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function BoxBlur() As Convolution
            Return New Convolution({{1, 1, 1}, {1, 1, 1}, {1, 1, 1}}, 9, 0)
        End Function

        ''' <summary>
        ''' Convolution({{-1, -1, -1}, {-1, 8, -1}, {-1, -1, -1}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function EdgeDetection1() As Convolution
            Return New Convolution({{-1, -1, -1}, {-1, 8, -1}, {-1, -1, -1}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{1, 0, -1}, {0, 0, 0}, {-1, 0, 1}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function EdgeDetection2() As Convolution
            Return New Convolution({{1, 0, -1}, {0, 0, 0}, {-1, 0, 1}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{0, 1, 0}, {1, -4, 1}, {0, 1, 0}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function EdgeDetection3() As Convolution
            Return New Convolution({{0, 1, 0}, {1, -4, 1}, {0, 1, 0}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{0, -1, 0}, {-1, 5, -1}, {0, -1, 0}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Sharpen() As Convolution
            Return New Convolution({{0, -1, 0}, {-1, 5, -1}, {0, -1, 0}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{-2, -1, 0}, {-1, 1, 1}, {0, 1, 2}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Emboss() As Convolution
            Return New Convolution({{-2, -1, 0}, {-1, 1, 1}, {0, 1, 2}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{0, 0, 0}, {-1, 1, 0}, {0, 0, 0}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function EdgeEnhance1() As Convolution
            Return New Convolution({{0, 0, 0}, {-1, 1, 0}, {0, 0, 0}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{0, -1, 0}, {0, 1, 0}, {0, 0, 0}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function EdgeEnhance2() As Convolution
            Return New Convolution({{0, -1, 0}, {0, 1, 0}, {0, 0, 0}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{-1, 0, 0}, {0, 1, 0}, {0, 0, 0}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function EdgeEnhance3() As Convolution
            Return New Convolution({{-1, 0, 0}, {0, 1, 0}, {0, 0, 0}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{1, -1,-1}, {-1, 1, -1}, {-1,-1,1}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Diagonal1() As Convolution
            Return New Convolution({{1, -1, -1}, {-1, 1, -1}, {-1, -1, 1}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{-1, -1, 1}, {-1, 1, -1}, {1,-1,-1}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Diagonal2() As Convolution
            Return New Convolution({{-1, -1, 1}, {-1, 1, -1}, {1, -1, -1}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{-1, -1, -1}, {1, 1, 1}, {-1, -1, -1}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Horizontal() As Convolution
            Return New Convolution({{-1, -1, -1}, {1, 1, 1}, {-1, -1, -1}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{-1, 1, -1}, {-1, 1, -1}, {-1, 1, -1}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Vertical() As Convolution
            Return New Convolution({{-1, 1, -1}, {-1, 1, -1}, {-1, 1, -1}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{1, 1, 1}, {1, -1, 1}, {1, 1, 1}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Ring() As Convolution
            Return New Convolution({{1, 1, 1}, {1, -1, 1}, {1, 1, 1}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{-1, -1, -1}, {-1, 1, -1}, {-1, -1, -1}}, 1, 0)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Dot() As Convolution
            Return New Convolution({{-1, -1, -1}, {-1, 1, -1}, {-1, -1, -1}}, 1, 0)
        End Function

        ''' <summary>
        ''' Convolution({{1, -1, 1}, {-1, 1, -1}, {1,-1,1}}, 1, 0)
        ''' </summary>S
        ''' <returns></returns>
        Public Shared Function XFilter() As Convolution
            Return New Convolution({{1, -1, 1}, {-1, 1, -1}, {1, -1, 1}}, 1, 0)
        End Function


    End Class

End Namespace