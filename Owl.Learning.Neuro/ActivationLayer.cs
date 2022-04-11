using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2012
// contacts@aforgenet.com

namespace Owl.Learning.Neuro
{
    /// <summary>
    /// Activation layer.
    /// </summary>
    /// 
    /// <remarks>Activation layer is a layer of <see cref="ActivationNeuron">activation neurons</see>.
    /// The layer is usually used in multi-layer neural networks.</remarks>
    ///
    [Serializable]
    public class ActivationLayer
    {
         
        public ActivationLayer(int neuronsCount, int inputsCount) 
        {
            this.inputsCount = Math.Max(1, inputsCount);
            this.neuronsCount = Math.Max(1, neuronsCount);
            neurons = new ActivationNeuron[this.neuronsCount];

            for (int i = 0; i < neurons.Length; i++)
                neurons[i] = new ActivationNeuron(inputsCount );
        }

        protected int inputsCount = 0;
        protected int neuronsCount = 0;
        protected ActivationNeuron[] neurons;
        protected double[] output;

        public int InputsCount
        {
            get { return inputsCount; }
        }

        public ActivationNeuron[] Neurons
        {
            get { return neurons; }
        }

        public double[] Output
        {
            get { return output; }
        } 
 
        public virtual double[] Compute(double[] input, SigmoidFunction function)
        {
            for (int i = 0; i < neurons.Length; i++)
                this.output[i] = neurons[i].Compute(input, function);

            return output;
        }
 
        public virtual void Randomize()
        {
            foreach (ActivationNeuron neuron in neurons)
                neuron.Randomize();
        }
    }
}
