using System;

namespace Owl.Learning.Neuro
{
    public class BackPropagationLearning 
    {
        private ActivationNetwork network;
        private double learningRate = 0.1;
        private double momentum = 0.0;

        private double[][] neuronErrors = null;
        private double[][][] weightsUpdates = null;
        private double[][] thresholdsUpdates = null;
 
        public double LearningRate
        {
            get { return learningRate; }
            set
            {
                learningRate = Math.Max(0.0, Math.Min(1.0, value));
            }
        }
        public double Momentum
        {
            get { return momentum; }
            set
            {
                momentum = Math.Max(0.0, Math.Min(1.0, value));
            }
        }

        public BackPropagationLearning(ActivationNetwork network)
        {
            this.network = network;

            // create error and deltas arrays
            neuronErrors = new double[network.Layers.Length][];
            weightsUpdates = new double[network.Layers.Length][][];
            thresholdsUpdates = new double[network.Layers.Length][];

            // initialize errors and deltas arrays for each layer
            for (int i = 0; i < network.Layers.Length; i++)
            {
                ActivationLayer layer = network.Layers[i];

                neuronErrors[i] = new double[layer.Neurons.Length];
                weightsUpdates[i] = new double[layer.Neurons.Length][];
                thresholdsUpdates[i] = new double[layer.Neurons.Length];

                // for each neuron
                for (int j = 0; j < weightsUpdates[i].Length; j++)
                {
                    weightsUpdates[i][j] = new double[layer.InputsCount];
                }
            }
        }

        /// <summary>
        /// Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// <param name="output">Desired output vector.</param>
        /// 
        /// <returns>Returns squared error (difference between current network's output and
        /// desired output) divided by 2.</returns>
        /// 
        /// <remarks><para>Runs one learning iteration and updates neuron's
        /// weights.</para></remarks>
        ///
        public double Run(double[] input, double[] output)
        {
            // compute the network's output
            network.Compute(input);

            // calculate network error
            double error = CalculateError(output);

            // calculate weights updates
            CalculateUpdates(input);

            // update the network
            UpdateNetwork();

            return error;
        }

        /// <summary>
        /// Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        /// <param name="output">Array of output vectors.</param>
        /// 
        /// <returns>Returns summary learning error for the epoch. See <see cref="Run"/>
        /// method for details about learning error calculation.</returns>
        /// 
        /// <remarks><para>The method runs one learning epoch, by calling <see cref="Run"/> method
        /// for each vector provided in the <paramref name="input"/> array.</para></remarks>
        /// 
        public double RunEpoch(double[][] input, double[][] output)
        {
            double error = 0.0;

            // run learning procedure for all samples
            for (int i = 0; i < input.Length; i++)
            {
                error += Run(input[i], output[i]);
            }

            // return summary error
            return error;
        }


        /// <summary>
        /// Calculates error values for all neurons of the network.
        /// </summary>
        /// 
        /// <param name="desiredOutput">Desired output vector.</param>
        /// 
        /// <returns>Returns summary squared error of the last layer divided by 2.</returns>
        /// 
        private double CalculateError(double[] desiredOutput)
        {
            // current and the next layers
            ActivationLayer layer, layerNext;
            // current and the next errors arrays
            double[] errors, errorsNext;
            // error values
            double error = 0, e, sum;
            // neuron's output value
            double output;
            // layers count
            int layersCount = network.Layers.Length;

            // calculate error values for the last layer first
            layer = network.Layers[layersCount - 1];
            errors = neuronErrors[layersCount - 1];

            for (int i = 0; i < layer.Neurons.Length; i++)
            {
                output = layer.Neurons[i].Output;
                // error of the neuron
                e = desiredOutput[i] - output;
                // error multiplied with activation function's derivative
                errors[i] = e * network.ActivationFunction.Derivative2(output);
                // squre the error and sum it
                error += (e * e);
            }

            // calculate error values for other layers
            for (int j = layersCount - 2; j >= 0; j--)
            {
                layer = network.Layers[j];
                layerNext = network.Layers[j + 1];
                errors = neuronErrors[j];
                errorsNext = neuronErrors[j + 1];

                // for all neurons of the layer
                for (int i = 0; i < layer.Neurons.Length; i++)
                {
                    sum = 0.0;
                    // for all neurons of the next layer
                    for (int k = 0; k < layerNext.Neurons.Length; k++)
                    {
                        sum += errorsNext[k] * layerNext.Neurons[k].Weights[i];
                    }
                    errors[i] = sum * network.ActivationFunction.Derivative2(layer.Neurons[i].Output);
                }
            }

            // return squared error of the last layer divided by 2
            return error / 2.0;
        }

        /// <summary>
        /// Calculate weights updates.
        /// </summary>
        /// 
        /// <param name="input">Network's input vector.</param>
        /// 
        private void CalculateUpdates(double[] input)
        {
            // 1 - calculate updates for the first layer
            ActivationLayer layer = network.Layers[0];
            double[] errors = neuronErrors[0];
            double[][] layerWeightsUpdates = weightsUpdates[0];
            double[] layerThresholdUpdates = thresholdsUpdates[0];

            // cache for frequently used values
            double cachedMomentum = learningRate * momentum;
            double cached1mMomentum = learningRate * (1 - momentum);
            double cachedError;

            // for each neuron of the layer
            for (int i = 0; i < layer.Neurons.Length; i++)
            {
                cachedError = errors[i] * cached1mMomentum;
                double[] neuronWeightUpdates = layerWeightsUpdates[i];

                // for each weight of the neuron
                for (int j = 0; j < neuronWeightUpdates.Length; j++)
                {
                    // calculate weight update
                    neuronWeightUpdates[j] = cachedMomentum * neuronWeightUpdates[j] + cachedError * input[j];
                }

                // calculate threshold update
                layerThresholdUpdates[i] = cachedMomentum * layerThresholdUpdates[i] + cachedError;
            }

            // 2 - for all other layers
            for (int k = 1; k < network.Layers.Length; k++)
            {
                ActivationLayer layerPrev = network.Layers[k - 1];
                layer = network.Layers[k];
                errors = neuronErrors[k];
                layerWeightsUpdates = weightsUpdates[k];
                layerThresholdUpdates = thresholdsUpdates[k];

                // for each neuron of the layer
                for (int i = 0; i < layer.Neurons.Length; i++)
                {
                    cachedError = errors[i] * cached1mMomentum;
                    double[] neuronWeightUpdates = layerWeightsUpdates[i];

                    // for each synapse of the neuron
                    for (int j = 0; j < neuronWeightUpdates.Length; j++)
                    {
                        // calculate weight update
                        neuronWeightUpdates[j] = cachedMomentum * neuronWeightUpdates[j] + cachedError * layerPrev.Neurons[j].Output;
                    }

                    // calculate threshold update
                    layerThresholdUpdates[i] = cachedMomentum * layerThresholdUpdates[i] + cachedError;
                }
            }
        }

        /// <summary>
        /// Update network's weights.
        /// </summary>
        /// 
        private void UpdateNetwork()
        {
            // current neuron
            ActivationNeuron neuron;
            // current layer
            ActivationLayer layer;
            // layer's weights updates
            double[][] layerWeightsUpdates;
            // layer's thresholds updates
            double[] layerThresholdUpdates;
            // neuron's weights updates
            double[] neuronWeightUpdates;

            // for each layer of the network
            for (int i = 0; i < network.Layers.Length; i++)
            {
                layer = network.Layers[i];
                layerWeightsUpdates = weightsUpdates[i];
                layerThresholdUpdates = thresholdsUpdates[i];

                // for each neuron of the layer
                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    neuron = layer.Neurons[j] as ActivationNeuron;
                    neuronWeightUpdates = layerWeightsUpdates[j];

                    // for each weight of the neuron
                    for (int k = 0; k < neuron.Weights.Length; k++)
                    {
                        // update weight
                        neuron.Weights[k] += neuronWeightUpdates[k];
                    }
                    // update treshold
                    neuron.Threshold += layerThresholdUpdates[j];
                }
            }
        }
    }
}
