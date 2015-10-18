using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comp3931_Project_JoePelz {
    public enum DSP_Window { pass, triangle, cosine, blackman }
    public enum DSP_FX { reverse }
    
    class DSP {
        public static Complex[] DFT(ref double[] samples) {
            int N = samples.Length;
            Complex[] DFT = new Complex[N];

            double re, im;
            for (int f = 0; f < N; f++) {
                re = 0;
                im = 0;
                for (int t = 0; t < N; t++) {
                    re += (samples[t]) * Math.Cos(2 * Math.PI * f * t / N);
                    im -= (samples[t]) * Math.Sin(2 * Math.PI * f * t / N);
                }
                DFT[f] = new Complex(re, im);
            }

            return DFT;
        }

        public static double[] InverseDFT(ref Complex[] A) {
            int N = A.Length;
            double[] S = new double[N];
            
            double st;
            for (int t = 0; t < N; t++) {
                st = 0;
                for (int f = 0; f < N; f++) {
                    st += A[f].re * Math.Cos(2 * Math.PI * f * t / N);
                    st -= A[f].im * Math.Sin(2 * Math.PI * f * t / N);
                }
                S[t] = st / N;
            }

            return S;
        }

        public static double[] convolveFilter(double[] S, double[] filter) {
            int t;
            //create complex filter from simple filter
            Complex[] qFilter = new Complex[filter.Length];
            for (int af = 0; af < filter.Length; af++) {
                qFilter[af] = new Complex(filter[af], filter[af]);
            }

            //run complex filter through IDFT
            double[] w = InverseDFT(ref qFilter);

            //convolve signal with filter
            double[] result = new double[S.Length];
            for (t = 0; t < S.Length - w.Length; t++) {
                result[t] = 0;
                for (int i = 0; i < w.Length; i++) {
                    result[t] += S[t] * w[i];
                }
            }
            for (; t < S.Length; t++) {
                result[t] = 0;
                for (int i = 0; i < S.Length - t; i++) {
                    result[t] += S[t] * w[i];
                }
            }
            //TODO: test this. Does it work?
            return result;
        }

        public static double[] MergeChannels(double[][] samples) {
            double[] result = new double[samples[0].Length];
            for (int channel = 0; channel < samples.Length; channel++) {
                for (int t = 0; t< samples[0].Length; t++) {
                    result[t] = 0;
                }
            }
            throw new NotImplementedException();
        }

        public static void WindowPassthrough(ref double[] samples) {
            return;
        }

        public static void WindowTriangle(ref double[] samples) {
            double N = samples.Length;
            double a = (N - 1.0) / 2.0;
            double b = N / 2.0;
            double weight;

            for (int n = 0; n < N; n++) {
                weight = 1.0 - Math.Abs((n - a) / b);
                samples[n] *= weight;
            }
        }

        public static void WindowCosine(ref double[] samples) {
            double N = samples.Length;
            double a = 0.54;
            double b = 1.0 - a;
            double weight;

            for (int n = 0; n < N; n++) {
                weight = a - b * Math.Cos((2 * Math.PI * n) / (N - 1));
                samples[n] *= weight;
            }
        }

        public static void WindowBlackman(ref double[] samples) {
            double N = samples.Length;
            const double a = 0.16;
            const double a0 = (1.0 - a) / 2.0;
            const double a1 = 1.0 / 2.0;
            const double a2 = a / 2.0;
            double weight;

            for (int n = 0; n < N; n++) {
                weight 
                    = a0 
                    - a1 * Math.Cos((2 * Math.PI * n) / (N - 1)) 
                    + a2 * Math.Cos((4 * Math.PI * n) / (N - 1));
                samples[n] *= weight;
            }
        }

        public static void ApplyFX(DSP_FX effect, ref WaveFile data) {
            switch (effect) {
                case DSP_FX.reverse:
                    for (int channel = 0; channel < data.channels; channel++) {
                        Array.Reverse(data.samples[channel]);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
