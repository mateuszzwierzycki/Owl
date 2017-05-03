Imports Owl.Core.Tensors

Namespace Convolutions

    Public Class Subsampling
        Inherits ImageModifier

        Private m_f As Double = 2

        Sub New(Factor As Double)
            m_f = Factor
        End Sub

        Public Overrides Function Duplicate() As ImageModifier
            Return New Subsampling(m_f)
        End Function

        Public Property Factor As Double
            Get
                Return m_f
            End Get
            Set(value As Double)
                m_f = value
            End Set
        End Property

        Public Overrides Sub Apply(ByRef Image As Tensor)
            Dim wsize As Double = Math.Ceiling(Image.Width / Factor)
            Dim hsize As Double = Math.Ceiling(Image.Height / Factor)

            If wsize = 0 Or hsize = 0 Then Return

            Dim nimg As New Tensor(CInt(wsize), CInt(hsize))

            Dim cntx As Integer = 0
            Dim cnty As Integer = 0

            For i As Integer = 0 To Image.Height - 1 Step Factor
                cntx = 0
                For j As Integer = 0 To Image.Width - 1 Step Factor
                    nimg.ValueAt(cntx, cnty) = Image.ValueAt(j, i)
                    cntx += 1
                Next
                cnty += 1
            Next

            Image.Dispose()
            Image = nimg
        End Sub

        Public Shared Function Create(Factor As Integer) As Subsampling
            Return New Subsampling(Factor)
        End Function

    End Class

End Namespace