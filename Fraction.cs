using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comp3931_Project_JoePelz {
    /// <summary>
    /// This class represents a fraction.  It is used to convert a given rational number to a small fraction (within a tolerance, to result in smaller numbers).
    /// </summary>
    public class Fraction {
        public int num;
        public int denom;
        private double epsilon;

        /// <summary>
        /// Construct a fraction from a given double, to within 0.0001 of the original double.
        /// </summary>
        /// <param name="value"></param>
        public Fraction(double value) {
            epsilon = 0.0001;
            setValsFromDouble(value);
        }

        /// <summary>
        /// Construct a fraction from a rational number with the given tolerance.  
        /// Will be accurate within the tolerance, as far as I know. 
        /// Not extensively tested. May be inefficient.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="e"></param>
        public Fraction(double value, double e) {
            epsilon = e;
            setValsFromDouble(value);
        }

        /// <summary>
        /// Calculate a fraction that is within epsilon distance of the given double value.  Mutator.
        /// </summary>
        /// <param name="value"></param>
        private void setValsFromDouble(double value) {
            int whole = (int)Math.Floor(value);
            value -= whole;
            double a = 0;
            double b = 1;
            //inc num and denum until within epsilon of value.
            double temp = a / b;

            while (Math.Abs(temp - value) > epsilon) {
                if (temp < value) {
                    a += 1;
                } else {
                    b += 1;
                }
                temp = a / b;
            }
            num = (int)a + (whole * (int)b);
            denom = (int)b;
        }
    }
}
