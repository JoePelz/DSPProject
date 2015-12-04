using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Mathematical complex number, using a pair of doubles.
/// </summary>
namespace Comp3931_Project_JoePelz {
    public class Complex {
        public double re, im;

        /// <summary>
        /// Constructs complex from real and imaginary components
        /// </summary>
        /// <param name="real"></param>
        /// <param name="imaginary"></param>
        public Complex(double real, double imaginary) {
            re = real;
            im = imaginary;
        }

        /// <summary>
        /// Factory function, builds a complex number from a given magnitude and angle in the complex plane.
        /// </summary>
        /// <param name="magnitude">sqrt(re^2 + im^2)</param>
        /// <param name="angle">Angle of the complex number in the complex plane. In radians.</param>
        /// <returns></returns>
        public static Complex fromMagAngle(double magnitude, double angle) {
            return new Complex(magnitude * Math.Cos(angle), magnitude * Math.Sin(angle));
        }

        /// <summary>
        /// Calculate the magnitude of a complex number via sqrt(re^2 + im^2).  Always returns a real number.
        /// </summary>
        /// <returns></returns>
        public double magnitude() {
            return Math.Sqrt(re * re + im * im);
        }

        /// <summary>
        /// Calculate the angle in the complex plane of this number. (atan2(im, re))
        /// </summary>
        /// <returns></returns>
        public double angle() {
            return Math.Atan2(im, re);
        }

        /// <summary>
        /// normalize the length of this complex number to the unit circle.
        /// </summary>
        public void normalize() {
            double length = magnitude();
            re /= length;
            im /= length;
        }
    }
}
