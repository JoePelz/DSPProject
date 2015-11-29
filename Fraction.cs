using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comp3931_Project_JoePelz {
    public class Fraction {
        public int num;
        public int denom;
        private double epsilon;

        public Fraction(double value) {
            epsilon = 0.0001;
            fromDouble(value);
        }

        public Fraction(double value, double e) {
            epsilon = e;
            fromDouble(value);
        }

        private void fromDouble(double value) {
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
