using Accord.Math.Random;
using Accord.Statistics.Distributions.Univariate;
using System;

namespace Owl.Learning.Neuro
{

    [Serializable]
    public class ActivationNeuron
    {
        protected int inputsCount = 0;
        protected double[] weights = null;
        protected double output = 0;
        protected IRandomNumberGenerator<double> rand = new UniformContinuousDistribution();


        public ActivationNeuron(int inputs)
        {
            inputsCount = Math.Max(1, inputs);
            weights = new double[inputsCount];
            Randomize();
        }

        public IRandomNumberGenerator<double> RandGenerator
        {
            get { return rand; }
            set { rand = value; }
        }

        public int InputsCount
        {
            get { return inputsCount; }
        }

        public double Output
        {
            get { return output; }
        }

        public double[] Weights
        {
            get { return weights; }
        }

        protected double threshold = 0.0;

        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        public void Randomize()
        {
            rand.Generate(weights.Length, weights);
            threshold = rand.Generate();
        }

        public double Compute(double[] input, SigmoidFunction function)
        {
            if (input.Length != inputsCount)
                throw new ArgumentException("Wrong length of the input vector.");

            double sum = 0.0;

            for (int i = 0; i < weights.Length; i++)
                sum += weights[i] * input[i];
            sum += threshold;

            double output = function.Function(sum);
            this.output = output;
            return output;
        }
    }
}
