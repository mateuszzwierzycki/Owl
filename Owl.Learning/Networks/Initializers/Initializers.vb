Imports Owl.Learning.Networks

Namespace Initializers

    Public MustInherit Class InitializerBase

        MustOverride Sub InitializeNetwork(Net As Network)
        MustOverride Function Duplicate() As InitializerBase

    End Class

    Public Class RandomInitializer
        Inherits InitializerBase

        Private _seed As Integer
        Private _gen As Random = Nothing

        Sub New(Spread As Range, Optional Seed As Integer = 123)
            Me.Seed = Seed
            Me.Spread = Spread
            Me._gen = New Random(Seed)
        End Sub

        Public Property Seed As Integer
            Get
                Return _seed
            End Get
            Set(value As Integer)
                _seed = value
                Me._gen = New Random(_seed)
            End Set
        End Property

        Public Property Spread As Range

        Public Overrides Sub InitializeNetwork(Net As Network)

            For Each tens As Tensor In Net.Weights
                For i As Integer = 0 To tens.Length - 1 Step 1
                    tens(i) = Me.Spread.ValueAtParameter(_gen.NextDouble)
                Next
            Next

            For Each tens As Tensor In Net.Biases
                For i As Integer = 0 To tens.Length - 1 Step 1
                    tens(i) = Me.Spread.ValueAtParameter(_gen.NextDouble)
                Next
            Next

        End Sub

        Public Overrides Function Duplicate() As InitializerBase
            Return New RandomInitializer(Me.Spread, Me.Seed)
        End Function
    End Class


End Namespace
