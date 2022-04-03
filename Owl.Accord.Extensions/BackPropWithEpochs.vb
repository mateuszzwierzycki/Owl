Imports System
Imports Accord.Neuro
Imports Accord.Neuro.Learning

Namespace Accord.Neuro.Learning
    Public Class BackPropagationLearning
        Implements ISupervisedLearning

        Private _network As ActivationNetwork
        Private _learningRate As Double = 0.1
        Private _momentum As Double = 0.0
        Private _neuronErrors As Double()() = Nothing
        Private _weightsUpdates As Double()()() = Nothing
        Private _thresholdsUpdates As Double()() = Nothing

        Public Sub New()
        End Sub

        Public Property LearningRate As Double
            Get
                Return _learningRate
            End Get
            Set(ByVal value As Double)
                _learningRate = Math.Max(0.0, Math.Min(1.0, value))
            End Set
        End Property

        Public Property Momentum As Double
            Get
                Return _momentum
            End Get
            Set(ByVal value As Double)
                _momentum = Math.Max(0.0, Math.Min(1.0, value))
            End Set
        End Property

        Public Sub New(ByVal network As ActivationNetwork)
            _network = network
            _neuronErrors = New Double(network.Layers.Length - 1)() {}
            _weightsUpdates = New Double(network.Layers.Length - 1)()() {}
            _thresholdsUpdates = New Double(network.Layers.Length - 1)() {}

            For i As Integer = 0 To network.Layers.Length - 1
                Dim layer As Layer = network.Layers(i)
                _neuronErrors(i) = New Double(layer.Neurons.Length - 1) {}
                _weightsUpdates(i) = New Double(layer.Neurons.Length - 1)() {}
                _thresholdsUpdates(i) = New Double(layer.Neurons.Length - 1) {}

                For j As Integer = 0 To _weightsUpdates(i).Length - 1
                    _weightsUpdates(i)(j) = New Double(layer.InputsCount - 1) {}
                Next
            Next
        End Sub

        Public Function Run(input As Double(), output As Double()) As Double Implements ISupervisedLearning.Run
            _network.Compute(input)
            Dim [error] As Double = CalculateError(output)
            CalculateUpdates(input)
            UpdateNetwork()
            Return [error]
        End Function

        Public Function RunEpoch(input As Double()(), output As Double()()) As Double Implements ISupervisedLearning.RunEpoch
            Dim [error] As Double = 0.0

            For i As Integer = 0 To input.Length - 1
                [error] += Run(input(i), output(i))
            Next

            Return [error]
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

        Private Sub CalculateUpdates(ByVal input As Double())
            Dim layer As Layer = _network.Layers(0)
            Dim errors As Double() = _neuronErrors(0)
            Dim layerWeightsUpdates As Double()() = _weightsUpdates(0)
            Dim layerThresholdUpdates As Double() = _thresholdsUpdates(0)
            Dim cachedMomentum As Double = learningRate * momentum
            Dim cached1mMomentum As Double = learningRate * (1 - momentum)
            Dim cachedError As Double

            For i As Integer = 0 To layer.Neurons.Length - 1
                cachedError = errors(i) * cached1mMomentum
                Dim neuronWeightUpdates As Double() = layerWeightsUpdates(i)

                For j As Integer = 0 To neuronWeightUpdates.Length - 1
                    neuronWeightUpdates(j) = cachedMomentum * neuronWeightUpdates(j) + cachedError * input(j)
                Next

                layerThresholdUpdates(i) = cachedMomentum * layerThresholdUpdates(i) + cachedError
            Next

            For k As Integer = 1 To _network.Layers.Length - 1
                Dim layerPrev As Layer = _network.Layers(k - 1)
                layer = _network.Layers(k)
                errors = _neuronErrors(k)
                layerWeightsUpdates = _weightsUpdates(k)
                layerThresholdUpdates = _thresholdsUpdates(k)

                For i As Integer = 0 To layer.Neurons.Length - 1
                    cachedError = errors(i) * cached1mMomentum
                    Dim neuronWeightUpdates As Double() = layerWeightsUpdates(i)

                    For j As Integer = 0 To neuronWeightUpdates.Length - 1
                        neuronWeightUpdates(j) = cachedMomentum * neuronWeightUpdates(j) + cachedError * layerPrev.Neurons(j).Output
                    Next

                    layerThresholdUpdates(i) = cachedMomentum * layerThresholdUpdates(i) + cachedError
                Next
            Next
        End Sub

        Private Sub UpdateNetwork()
            Dim neuron As ActivationNeuron
            Dim layer As Layer
            Dim layerWeightsUpdates As Double()()
            Dim layerThresholdUpdates As Double()
            Dim neuronWeightUpdates As Double()

            For i As Integer = 0 To _network.Layers.Length - 1
                layer = _network.Layers(i)
                layerWeightsUpdates = _weightsUpdates(i)
                layerThresholdUpdates = _thresholdsUpdates(i)

                For j As Integer = 0 To layer.Neurons.Length - 1
                    neuron = TryCast(layer.Neurons(j), ActivationNeuron)
                    neuronWeightUpdates = layerWeightsUpdates(j)

                    For k As Integer = 0 To neuron.Weights.Length - 1
                        neuron.Weights(k) += neuronWeightUpdates(k)
                    Next

                    neuron.Threshold += layerThresholdUpdates(j)
                Next
            Next
        End Sub


    End Class
End Namespace
