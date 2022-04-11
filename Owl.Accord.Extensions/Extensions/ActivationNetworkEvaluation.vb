Imports System
Imports Accord.Neuro
Imports Accord.Neuro.Learning

Namespace Owl.Accord.Extensions
    Public Class ActivationNetworkEvaluation
        Implements ISupervisedLearning

        Private _network As ActivationNetwork
        Private _neuronErrors As Double()() = Nothing

        Public Sub New()
        End Sub

        Public Sub New(ByVal network As ActivationNetwork)
            _network = network
            _neuronErrors = New Double(network.Layers.Length - 1)() {}

            For i As Integer = 0 To network.Layers.Length - 1
                Dim layer As Layer = network.Layers(i)
                _neuronErrors(i) = New Double(layer.Neurons.Length - 1) {}
            Next
        End Sub

        ''' <summary>
        ''' Returns sum of errors
        ''' </summary>
        ''' <param name="input"></param>
        ''' <param name="output"></param>
        ''' <returns></returns>
        Public Function RunEpoch(input As Double()(), output As Double()()) As Double Implements ISupervisedLearning.RunEpoch
            Dim [error] As Double = 0.0

            For i As Integer = 0 To input.Length - 1
                [error] += Run(input(i), output(i))
            Next

            Return [error]
        End Function

        ''' <summary>
        ''' Returns the error value
        ''' </summary>
        ''' <param name="input"></param>
        ''' <param name="output"></param>
        ''' <returns></returns>
        Public Function Run(input As Double(), output As Double()) As Double Implements ISupervisedLearning.Run
            _network.Compute(input)
            Return CalculateError(output)
        End Function

        Private Function CalculateError(ByVal desiredOutput As Double()) As Double
            Dim layer, layerNext As Layer
            Dim errors, errorsNext As Double()
            Dim e, sum As Double, [error] As Double = 0
            Dim output As Double
            Dim layersCount As Integer = _network.Layers.Length
            layer = _network.Layers(layersCount - 1)
            errors = _neuronErrors(layersCount - 1)

            For i As Integer = 0 To layer.Neurons.Length - 1
                output = layer.Neurons(i).Output
                e = desiredOutput(i) - output
                errors(i) = e * (TryCast(layer.Neurons(i), ActivationNeuron)).ActivationFunction.Derivative2(output)
                [error] += (e * e)
            Next

            For j As Integer = layersCount - 2 To 0
                layer = _network.Layers(j)
                layerNext = _network.Layers(j + 1)
                errors = _neuronErrors(j)
                errorsNext = _neuronErrors(j + 1)

                For i As Integer = 0 To layer.Neurons.Length - 1
                    sum = 0.0

                    For k As Integer = 0 To layerNext.Neurons.Length - 1
                        sum += errorsNext(k) * layerNext.Neurons(k).Weights(i)
                    Next

                    errors(i) = sum * (TryCast(layer.Neurons(i), ActivationNeuron)).ActivationFunction.Derivative2(layer.Neurons(i).Output)
                Next
            Next

            Return [error] / 2.0
        End Function

    End Class
End Namespace
