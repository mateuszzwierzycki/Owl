Imports Owl.Core.Tensors

Namespace Convolutions

    ''' <summary>
    ''' Any byte smaller than threshold becomes 0.
    ''' </summary>
    Public Class Rectification
        Inherits ImageModifier

        Private m_t As Double = 0.5

        Sub New(Threshold As Double)
            m_t = Threshold
        End Sub

        Public Overrides Function Duplicate() As ImageModifier
            Return New Rectification(m_t)
        End Function

        Public Property Threshold As Double
            Get
                Return m_t
            End Get
            Set(value As Double)
                m_t = value
            End Set
        End Property

        Public Overrides Sub Apply(ByRef Image As Tensor)
            For i As Integer = 0 To Image.Length - 1 Step 1
                Image(i) = If(Image(i) < m_t, 0, Image(i))
            Next
        End Sub

        Public Shared Function Create(Threshold As Byte) As Rectification
            Return New Rectification(Threshold)
        End Function
    End Class

End Namespace
