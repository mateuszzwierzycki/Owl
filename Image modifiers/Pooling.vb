Imports Owl.Core.Tensors

Namespace Convolutions

    ''' <summary>
    ''' TODO max pooling check
    ''' </summary>
    Public Class MaxPooling
        Inherits ImageModifier

        Private m_w As Integer
        Private m_h As Integer

        Sub New(Width As Integer, Height As Integer)
            m_w = Width
            m_h = Height
        End Sub

        Public Overrides Function Duplicate() As ImageModifier
            Return New MaxPooling(m_w, m_h)
        End Function

        Public Property Width As Integer
            Get
                Return m_w
            End Get
            Set(value As Integer)
                m_w = value
            End Set
        End Property

        Public Property Height As Integer
            Get
                Return m_h
            End Get
            Set(value As Integer)
                m_h = value
            End Set
        End Property

        Public Overrides Sub Apply(ByRef Image As Tensor)

            Dim rows As Integer = 0
            Dim cols As Integer = 0

            For i As Integer = 0 To Image.Height - 1 Step Me.Height
                rows += 1
            Next

            For j As Integer = 0 To Image.Width - 1 Step Me.Width
                cols += 1
            Next

            Dim nimg As New Tensor(cols, rows)

            rows = 0
            cols = 0

            For i As Integer = 0 To Image.Height - 1 Step Me.Height

                cols = 0

                For j As Integer = 0 To Image.Width - 1 Step Me.Width

                    Dim thismax As Byte = 0

                    For p As Integer = 0 To Me.Height - 1 Step 1
                        For q As Integer = 0 To Me.Width - 1 Step 1

                            If (j + q) < Image.Width And (i + p) < Image.Height Then
                                thismax = Math.Max(thismax, Image.ValueAt(j + q, i + p))
                            End If

                        Next
                    Next

                    nimg.ValueAt(cols, rows) = thismax

                    cols += 1
                Next

                rows += 1
            Next

            Image.Dispose()
            Image = nimg
        End Sub

        Public Shared Function Create(Width As Integer, Height As Integer) As MaxPooling
            Return New MaxPooling(Width, Height)
        End Function


    End Class

End Namespace
