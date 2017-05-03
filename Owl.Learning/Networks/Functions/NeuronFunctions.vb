Namespace NeuronFunctions

    Public MustInherit Class NeuronFunctionBase

        MustOverride Function Evaluate(Parameter As Double) As Double
        MustOverride Sub Evaluate(Vector As Tensor)
        MustOverride Function Derivative(Parameter As Double) As Double
        MustOverride Function SecondDerivative(Parameter As Double) As Double
        MustOverride Function Duplicate() As NeuronFunctionBase

    End Class

    Public Class Sigmoid
        Inherits NeuronFunctionBase

        Sub New(Optional Alpha As Double = 1)
            Me.Alpha = Alpha
        End Sub

        Public Property Alpha As Double

        Public Overrides Function Evaluate(Parameter As Double) As Double
            Return 1 / (1 + Math.Exp(-Parameter * Alpha))
        End Function

        Public Overrides Sub Evaluate(Vector As Tensor)
            For i = 0 To Vector.Length - 1
                Vector(i) = Evaluate(Vector(i))
            Next
        End Sub

        Public Overrides Function Derivative(Parameter As Double) As Double
            Dim ev As Double = Evaluate(Parameter)
            Return (ev * (1 - ev))
        End Function

        Public Overrides Function SecondDerivative(Parameter As Double) As Double
            Return Double.NaN
        End Function

        Public Overrides Function Duplicate() As NeuronFunctionBase
            Return New Sigmoid(Me.Alpha)
        End Function
    End Class

End Namespace
