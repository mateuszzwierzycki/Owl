Imports Owl.Core.Structures
Imports Owl.Core.Tensors

Namespace Convolutions

    ''' <summary>
    ''' Scales values to conform to 0-1 range.
    ''' </summary>
    Public Class Normalizer
        Inherits ImageModifier

        Public Overrides Function Duplicate() As ImageModifier
            Return New Normalizer()
        End Function

        Public Overrides Sub Apply(ByRef Image As Tensor)
            Image.Remap(New Range(0, 1))
        End Sub

        Public Shared Function Create() As Normalizer
            Return New Normalizer
        End Function
    End Class

End Namespace
