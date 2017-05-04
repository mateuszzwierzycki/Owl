Imports Owl.Learning.Initializers
Imports Owl.Learning.NeuronFunctions

Namespace Networks

    Public Class Network

        Private _weights As New TensorSet
        Private _biases As New TensorSet
        Private _neur As NeuronFunctionBase

        ''' <summary>
        ''' The hardcore way.
        ''' </summary>
        Public Sub New()

        End Sub

        ''' <summary>
        ''' The direct way.
        ''' </summary>
        ''' <param name="Weights"></param>
        ''' <param name="Biases"></param>
        ''' <param name="NeuronFunction"></param>
        Public Sub New(Weights As TensorSet, Biases As TensorSet, NeuronFunction As NeuronFunctionBase)
            Me.Weights = Weights
            Me.Biases = Biases
            Me.NeuronFunction = NeuronFunction
        End Sub

        ''' <summary>
        ''' The usual way.
        ''' </summary>
        ''' <param name="NFunction"></param>
        ''' <param name="Inputs"></param>
        ''' <param name="Neurons"></param>
        ''' <param name="NetworkInitializer"></param>
        Sub New(NFunction As NeuronFunctionBase, Inputs As Integer, Neurons As IEnumerable(Of Integer), Optional NetworkInitializer As InitializerBase = Nothing)
            _neur = NFunction

            Dim nl As New List(Of Integer) From {Inputs}
            nl.AddRange(Neurons)

            For i As Integer = 1 To nl.Count - 1 Step 1
                Dim prev As Integer = nl(i - 1)
                Dim this As Integer = nl(i)

                _weights.Add(New Tensor(New List(Of Integer) From {prev, this}))
                _biases.Add(New Tensor(New List(Of Integer) From {this}))
            Next

            If NetworkInitializer IsNot Nothing Then NetworkInitializer.InitializeNetwork(Me)
        End Sub

        Public Function Duplicate() As Network
            Return New Network(Weights.Duplicate, Biases.Duplicate, NeuronFunction.Duplicate)
        End Function

        Public Function NeuronCounts() As List(Of Integer)
            Dim nl As New List(Of Integer)
            For i = 0 To Me.LayerCount - 1
                nl.Add(Me.NeuronCount(i))
            Next
            Return nl
        End Function

        Public ReadOnly Property LayerCount As Integer
            Get
                Return _weights.Count
            End Get
        End Property

        Public ReadOnly Property InputCount(LayerIndex As Integer) As Integer
            Get
                Return _weights(LayerIndex).ShapeAt(0)
            End Get
        End Property

        Public ReadOnly Property NeuronCount(LayerIndex As Integer) As Integer
            Get
                Return _weights(LayerIndex).ShapeAt(1)
            End Get
        End Property

        Public Property LayerWeights(layerIndex As Integer) As Tensor
            Get
                Return _weights(layerIndex)
            End Get
            Set(value As Tensor)
                _weights(layerIndex) = value
            End Set
        End Property

        Public Property LayerBiases(layerIndex As Integer) As Tensor
            Get
                Return _biases(layerIndex)
            End Get
            Set(value As Tensor)
                _biases(layerIndex) = value
            End Set
        End Property

        ''' <summary>
        ''' Direct access to the underlying arrays. Weights are stored in Tensors of form [Input, Neuron].
        ''' This implies the input Tensor for Compute has to be of shape [1, Input].
        ''' The ComputeLayer performs this reshaping before feeding the Tensor.
        ''' </summary>
        ''' <returns></returns>
        Public Property Weights As TensorSet
            Get
                Return _weights
            End Get
            Set(value As TensorSet)
                _weights = value
            End Set
        End Property

        ''' <summary>
        ''' Direct access to the underlying arrays
        ''' </summary>
        ''' <returns></returns>
        Public Property Biases As TensorSet
            Get
                Return _biases
            End Get
            Set(value As TensorSet)
                _biases = value
            End Set
        End Property

        ''' <summary>
        ''' Direct access to the underlying function
        ''' </summary>
        ''' <returns></returns>
        Public Property NeuronFunction As NeuronFunctionBase
            Get
                Return _neur
            End Get
            Set(value As NeuronFunctionBase)
                _neur = value
            End Set
        End Property

        Public Function Compute(InputTensor As Tensor) As Tensor
            Dim thistens As Tensor = InputTensor
            For i As Integer = 0 To Me.LayerCount - 1 Step 1
                thistens = ComputeLayer(thistens, i)
            Next
            Return thistens
        End Function

        ''' <summary>
        ''' Input tensor gets always reshaped into a [1, length] matrix.
        ''' </summary>
        ''' <param name="InputTensor"></param>
        ''' <param name="LayerIndex"></param>
        ''' <returns></returns>
        Public Function ComputeLayer(InputTensor As Tensor, LayerIndex As Integer) As Tensor
            InputTensor.TryReshape({1, InputTensor.Height})
            Dim tsum As Tensor = Tensor.MatMul(InputTensor, Me.Weights(LayerIndex))
            tsum.Add(Biases(LayerIndex))
            NeuronFunction.Evaluate(tsum)
            Return tsum
        End Function

    End Class

End Namespace

