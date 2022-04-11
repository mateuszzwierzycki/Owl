using System;

namespace Owl.Learning.Neuro
{

    [Serializable]
    public class SigmoidFunction :  ICloneable
    {
        public double Alpha { get; set; } = 2;

        public SigmoidFunction() { }

        public SigmoidFunction(double alpha)
        {
            this.Alpha = alpha;
        }

        public double Function(double x)
        {
            return 1 / (1 + Math.Exp(-Alpha * x));
        }

        public double Derivative(double x)
        {
            double y = Function(x);
            return Alpha * y * (1 - y);
        }
         
        public double Derivative2(double y)
        {
            return Alpha * y * (1 - y);
        }
         
        public object Clone()
        {
            return new SigmoidFunction(Alpha);
        }
    }
}
