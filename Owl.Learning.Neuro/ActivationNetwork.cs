using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owl.Learning.Neuro
{
    [Serializable]
    public class ActivationNetwork
    {
        public SigmoidFunction ActivationFunction { get; set; } = new SigmoidFunction(2);
        public int InputsCount { get; set; }
        
        public ActivationLayer[] Layers { get; set; }
        
        public double[] Output { get; set; }

        public ActivationNetwork(SigmoidFunction function, int inputsCount, params int[] neuronsCount)
        {
            ActivationFunction = function;
            InputsCount = Math.Max(1, inputsCount);
            Layers = new ActivationLayer[neuronsCount.Length];

            // create each layer
            for (int i = 0; i < Layers.Length; i++)
                Layers[i] = new ActivationLayer(neuronsCount[i], (i == 0) ? inputsCount : neuronsCount[i - 1]);
        }

        public void Randomize()
        {
            foreach (ActivationLayer layer in Layers)
                layer.Randomize();
        }

        public double[] Compute(double[] input)
        {
            for (int i = 0; i < Layers.Length; i++)
                this.Output = Layers[i].Compute(input, ActivationFunction);
            return this.Output;
        }


    }
}
