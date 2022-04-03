Imports System.Runtime.CompilerServices
Imports Accord.Neuro
Imports AN = Accord.Neuro
Imports Accord.Neuro.Learning
Imports Owl.Core.Tensors

Namespace Learning

    Public Module ActivationFunctions

        Public Class TFSigmoid
            Inherits SigmoidFunction
            Implements IActivationFunction

            Sub New()
                MyBase.New(1)
            End Sub
        End Class

    End Module


    Public Module NetworkExtensions

        Public Function ReconstructNetwork(Weights As TensorSet, Biases As TensorSet, ActFunction As IActivationFunction) As ActivationNetwork
            Dim ncounts As New List(Of Integer)

            For Each tens As Tensor In Biases
                ncounts.Add(tens.Length)
            Next

            Dim nn As New ActivationNetwork(ActFunction, Weights(0).ShapeAt(0), ncounts.ToArray)
            nn.SetWeights(Weights, True)
            nn.SetThresholds(Biases)

            Return nn
        End Function

        ''' <summary>
        ''' Removes layers counting from the start or from the end ogf the network.
        ''' </summary>
        ''' <param name="NetworkToTrim"></param>
        ''' <param name="LayersToTrim">Positive trims the input layers, negative the output layers.</param>
        ''' <returns></returns>
        <Extension()>
        Public Function TrimNetwork(NetworkToTrim As Network, LayersToTrim As Integer) As Network
            Dim neur As ActivationNeuron = NetworkToTrim.Layers(0).Neurons(0)
            Dim ncount As New List(Of Integer)
            Dim net As ActivationNetwork = Nothing

            Dim cnt As Integer = 0

            If LayersToTrim < 0 Then

                LayersToTrim = Math.Abs(LayersToTrim)

                For i As Integer = 0 To NetworkToTrim.Layers.Count - 1 - LayersToTrim
                    ncount.Add(NetworkToTrim.Layers(i).Neurons.Count)
                Next

                net = New ActivationNetwork(neur.ActivationFunction, NetworkToTrim.InputsCount, ncount.ToArray)

                For i As Integer = 0 To net.Layers.Count - 1 Step 1
                    For j As Integer = 0 To net.Layers(i).Neurons.Count - 1 Step 1

                        Dim aneu As ActivationNeuron = net.Layers(i).Neurons(j)
                        Dim that As ActivationNeuron = NetworkToTrim.Layers(i).Neurons(j)

                        aneu.Threshold = that.Threshold

                        For k As Integer = 0 To net.Layers(i).Neurons(j).Weights.Count - 1 Step 1
                            net.Layers(i).Neurons(j).Weights(k) = NetworkToTrim.Layers(i).Neurons(j).Weights(k)
                        Next
                    Next
                Next

            Else
                For i As Integer = LayersToTrim To NetworkToTrim.Layers.Count - 1 Step 1
                    ncount.Add(NetworkToTrim.Layers(i).Neurons.Count)
                Next

                net = New ActivationNetwork(neur.ActivationFunction, NetworkToTrim.Layers(LayersToTrim - 1).Neurons.Count, ncount.ToArray)

                For i As Integer = 0 To net.Layers.Count - 1 Step 1
                    For j As Integer = 0 To net.Layers(i).Neurons.Count - 1 Step 1
                        Dim aneu As ActivationNeuron = net.Layers(i).Neurons(j)
                        Dim that As ActivationNeuron = NetworkToTrim.Layers(i + LayersToTrim).Neurons(j)
                        aneu.Threshold = that.Threshold
                        For k As Integer = 0 To net.Layers(i).Neurons(j).Weights.Count - 1 Step 1
                            net.Layers(i).Neurons(j).Weights(k) = NetworkToTrim.Layers(i + LayersToTrim).Neurons(j).Weights(k)
                        Next
                    Next
                Next
            End If

            Return net
        End Function

        <Extension()>
        Public Function RunEpoch(Teacher As ISupervisedLearning, InputSamples As TensorSet, OutputSamples As TensorSet, FromSample As Integer, ToSample As Integer) As Double
            Dim [error] As Double = 0.0

            ' run learning procedure for all samples
            For i As Integer = FromSample To ToSample
                [error] += Teacher.Run(InputSamples(i).TensorData, OutputSamples(i).TensorData)
            Next

            ' return summary error
            Return [error]
        End Function

        <Extension()>
        Public Function RunEpoch(Teacher As ISupervisedLearning, InputSamples As TensorSet, OutputSamples As TensorSet) As Double
            Return RunEpoch(Teacher, InputSamples, OutputSamples, 0, InputSamples.Count)
        End Function

        <Extension()>
        Public Function DuplicateNetwork(Net As ActivationNetwork) As ActivationNetwork
            Dim aneur As ActivationNeuron = Net.Layers(0).Neurons(0)

            Dim lays As New List(Of Integer)

            For i As Integer = 0 To Net.Layers.Count - 1 Step 1
                Dim thislay As ActivationLayer = Net.Layers(i)
                lays.Add(thislay.Neurons.Count)
            Next

            Dim nn As New ActivationNetwork(aneur.ActivationFunction, Net.InputsCount, lays.ToArray)

            For i As Integer = 0 To nn.Layers.Count - 1 Step 1
                For j As Integer = 0 To nn.Layers(i).Neurons.Count - 1 Step 1
                    For k As Integer = 0 To nn.Layers(i).Neurons(j).Weights.Count - 1 Step 1
                        Dim thisn As ActivationNeuron = nn.Layers(i).Neurons(j)
                        Dim thatn As ActivationNeuron = Net.Layers(i).Neurons(j)

                        thisn.Threshold = thatn.Threshold

                        For p As Integer = 0 To thisn.Weights.Count - 1 Step 1
                            thisn.Weights(p) = thatn.Weights(p)
                        Next
                    Next
                Next
            Next

            Return nn
        End Function

        <Extension>
        Public Function ComputePerLayer(Net As ActivationNetwork, Input As Tensor) As List(Of Tensor)
            Dim tens() As Double = Input.TensorData

            Dim outs As New List(Of Tensor)({Input})

            For i As Integer = 0 To Net.Layers.Count - 1 Step 1
                Dim ntens() As Double = Net.Layers(i).Compute(tens)
                outs.Add(New Tensor(ntens))
                tens = ntens
            Next

            Return outs
        End Function

        <Extension()>
        Public Sub SetThresholds(Net As ActivationNetwork, Thresholds As TensorSet)
            For i As Integer = 0 To Net.Layers.Count - 1 Step 1
                Dim tens As Tensor = Thresholds(i)
                For j As Integer = 0 To Net.Layers(i).Neurons.Count - 1 Step 1
                    Dim act As ActivationNeuron = Net.Layers(i).Neurons(j)
                    act.Threshold = tens.ValueAt(j)
                Next
            Next
        End Sub

        <Extension()>
        Public Sub SetWeights(Net As ActivationNetwork, Weights As TensorSet, Optional FlipMatrix As Boolean = False)

            For i As Integer = 0 To Net.Layers.Count - 1 Step 1
                Dim tens As Tensor = Weights(i)
                For j As Integer = 0 To Net.Layers(i).Neurons.Count - 1 Step 1
                    For k As Integer = 0 To Net.Layers(i).Neurons(j).Weights.Length - 1 Step 1
                        Net.Layers(i).Neurons(j).Weights(k) = If(FlipMatrix, tens.ValueAt({k, j}), tens.ValueAt({j, k}))
                    Next
                Next
            Next

        End Sub

        <Extension()>
        Public Function ExtractWeights(Net As ActivationNetwork) As TensorSet
            'one layer one tensor 
            'tensor (neuron, connections) 

            Dim ts As New TensorSet

            For i As Integer = 0 To Net.Layers.Count - 1 Step 1
                Dim tens As New Tensor(New Integer() {Net.Layers(i).Neurons.Count, Net.Layers(i).Neurons(0).Weights.Length})
                For j As Integer = 0 To Net.Layers(i).Neurons.Count - 1 Step 1
                    For k As Integer = 0 To Net.Layers(i).Neurons(j).Weights.Length - 1 Step 1
                        tens.ValueAt(j, k) = Net.Layers(i).Neurons(j).Weights(k)
                    Next
                Next
                ts.Add(tens)
            Next

            Return ts
        End Function

        <Extension()>
        Public Function ExtractThresholds(Net As ActivationNetwork) As TensorSet
            Dim ts As New TensorSet

            'one layer one tensor 
            'tensor (neuron) 

            For i As Integer = 0 To Net.Layers.Count - 1 Step 1
                Dim tens As New Tensor(New Integer() {Net.Layers(i).Neurons.Count})
                For j As Integer = 0 To Net.Layers(i).Neurons.Count - 1 Step 1
                    Dim thisn As ActivationNeuron = Net.Layers(i).Neurons(j)
                    tens.ValueAt(j) = thisn.Threshold
                Next
                ts.Add(tens)
            Next

            Return ts
        End Function

    End Module


    'Public Module Testing

    '    Public Sub Test()

    '        Dim wpath As String = "C:\Users\Mateusz\PycharmProjects\OwlPy\Examples\Random100\Model\extraction_weights.tbin"
    '        Dim bpath As String = "C:\Users\Mateusz\PycharmProjects\OwlPy\Examples\Random100\Model\extraction_biases.tbin"
    '        Dim rpath As String = "C:\Users\Mateusz\PycharmProjects\OwlPy\Examples\Random100\Data\Random100_Query.idx"

    '        Dim tw As TensorSet = Owl.Core.IO.LoadTensorsBinary(wpath)
    '        Dim tb As TensorSet = Owl.Core.IO.LoadTensorsBinary(bpath)
    '        Dim tr As TensorSet = Owl.Core.IO.LoadTensorsIDX(rpath)

    '        Dim thisnet As ActivationNetwork = ReconstructNetwork(tw, tb, New TFSigmoid())

    '        Dim res As New TensorSet

    '        For Each tens As Tensor In tr
    '            res.Add(New Tensor(thisnet.Compute(tens.TensorData)))
    '        Next

    '        Owl.Core.IO.SaveTensorsBinary(res, "C:/tmp/owl", "recog")

    '    End Sub

    'End Module

End Namespace
