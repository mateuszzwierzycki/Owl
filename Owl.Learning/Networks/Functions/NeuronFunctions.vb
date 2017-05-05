Namespace NeuronFunctions

    <Serializable>
    Public MustInherit Class NeuronFunctionBase

        MustOverride Function Evaluate(Parameter As Double) As Double
        MustOverride Sub Evaluate(Vector As Tensor)
        MustOverride Function Derivative(Parameter As Double) As Double
        MustOverride Function SecondDerivative(Parameter As Double) As Double
        MustOverride Function Duplicate() As NeuronFunctionBase

    End Class

    'TODO function
    <Serializable>
    Public Class Tanh
        Inherits NeuronFunctionBase

        Public Overrides Sub Evaluate(Vector As Tensor)
            For i = 0 To Vector.Length - 1
                Vector(i) = Evaluate(Vector(i))
            Next
        End Sub

        Public Overrides Function Evaluate(Parameter As Double) As Double
            Return Math.Tanh(Parameter)
        End Function

        Public Overrides Function Derivative(Parameter As Double) As Double
            Return 1 - Evaluate(Parameter) ^ 2
        End Function

        Private Function Sech(x As Double)
            Return 1 / Math.Cosh(x)
        End Function

        Public Overrides Function SecondDerivative(Parameter As Double) As Double
            Return -2 * (Sech(Parameter) ^ 2) * Math.Tanh(Parameter)
        End Function

        Public Overrides Function Duplicate() As NeuronFunctionBase
            Return New Tanh
        End Function
    End Class

    <Serializable>
    Public Class Linear
        Inherits NeuronFunctionBase

        Public Overrides Sub Evaluate(Vector As Tensor)
            For i = 0 To Vector.Length - 1
                Vector(i) = Evaluate(Vector(i))
            Next
        End Sub

        Public Overrides Function Evaluate(Parameter As Double) As Double
            Return Parameter
        End Function

        Public Overrides Function Derivative(Parameter As Double) As Double
            Return 1
        End Function

        Public Overrides Function SecondDerivative(Parameter As Double) As Double
            Return 0
        End Function

        Public Overrides Function Duplicate() As NeuronFunctionBase
            Return New Linear
        End Function
    End Class

    <Serializable>
    Public Class Relu
        Inherits NeuronFunctionBase

        Public Overrides Sub Evaluate(Vector As Tensor)
            For i = 0 To Vector.Length - 1
                Vector(i) = Evaluate(Vector(i))
            Next
        End Sub

        Public Overrides Function Evaluate(Parameter As Double) As Double
            If Parameter < 0 Then Return 0
            Return Parameter
        End Function

        Public Overrides Function Derivative(Parameter As Double) As Double
            If Parameter < 0 Then Return 0
            Return 1
        End Function

        Public Overrides Function SecondDerivative(Parameter As Double) As Double
            Return 0
        End Function

        Public Overrides Function Duplicate() As NeuronFunctionBase
            Return New Relu
        End Function
    End Class

    <Serializable>
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
