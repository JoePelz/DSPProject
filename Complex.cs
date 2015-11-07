using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comp3931_Project_JoePelz {
    public class Complex {
        public double re, im;

        public Complex(double real, double imaginary) {
            re = real;
            im = imaginary;
        }

        public double magnitude() {
            return Math.Sqrt(re * re + im * im);
        }

        public double angle() {
            return Math.Atan2(im, re);
        }
    }
}
