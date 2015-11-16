using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comp3931_Project_JoePelz {
    public enum DSP_Window { pass, triangle, cosine, blackman }
    public enum DSP_FX { reverse }
    
    class DSP {
        private static Complex[] DFTHelper(object args) {
            //ref double[] samples, int startIndex, int stopIndex
            Array argArray = new object[3];
            argArray = (Array)args;
            double[] samples = (double[])argArray.GetValue(0);
            int low  = (int)argArray.GetValue(1);
            int high = (int)argArray.GetValue(2);

            int N = samples.Length;
            Complex[] DFT = new Complex[N];
            double re, im;

            for (int f = low; f < high; f++) {
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
        
        public static double[] convolve(double[] S, double[] kernel) {
            int t;
            double[] result = new double[S.Length];
            for (t = 0; t < S.Length - kernel.Length; t++) {
                result[t] = 0;
                for (int i = 0; i < kernel.Length; i++) {
                    result[t] += S[t+i] * kernel[i];
                }
                result[t] /= kernel.Length;
            }
            for (; t < S.Length; t++) {
                result[t] = 0;
                for (int i = 0; i < S.Length - t; i++) {
                    result[t] += S[t+i] * kernel[i];
                }
                result[t] /= (S.Length - t);
            }
            return result;
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
            for (t = 1; t < S.Length - w.Length; t++) {
                result[t] = 0;
                for (int i = 0; i < w.Length; i++) {
                    result[t] += S[t+i] * w[i];
                }
            }
            for (; t < S.Length; t++) {
                result[t] = 0;
                for (int i = 0; i < S.Length - t; i++) {
                    result[t] += S[t+i] * w[i];
                }
            }
            //DC component doesn't change.
            result[0] = S[0];
            return result;
        }

        public static double[] dftFilter(double[] S, double[] filter) {
            int N = filter.Length;
            double[] result = new double[S.Length];

            double[] sChunk = new double[N];
            Complex[] fChunk;
            int first;
            first = 0;

            while(first + N < S.Length) { 
                //load sChunk
                Array.Copy(S, first, sChunk, 0, N);
                //calc fChunk
                fChunk = DFT(ref sChunk);
                //filter fChunk
                for (int i = 0; i < N; i++) {
                    if (filter[i] == 0) {
                        fChunk[i].im = 0;
                        fChunk[i].re = 0;
                    }
                }
                //replace sChunk
                sChunk = InverseDFT(ref fChunk);
                //offload sChunk
                Array.Copy(sChunk, 0, result, first, N);
                first += N;
            }


            return result;
        }

        public static double[] StereoToMono(ref double[] sampleA, ref double[] sampleB) {
            double[] result = new double[Math.Max(sampleA.Length, sampleB.Length)];
            long limit = Math.Min(sampleA.Length, sampleB.Length);
            long t;
            for (t = 0; t < limit; t++) {
                result[t] = (sampleA[t] + sampleB[t]) / 2;
            }

            if (t < sampleA.Length) {
                //copy rest of A
                Array.Copy(sampleA, t, result, t, sampleA.Length - t);
            } else if (t < sampleB.Length) {
                //copy rest of B
                Array.Copy(sampleB, t, result, t, sampleB.Length - t);
            }
            return result;
        }

        public static double[] resample(ref double[] samples, int oldRate, int newRate) {
            double[] extendedSamples, result;
            int L, M;
            if (newRate == oldRate) {
                L = 1; M = 1;
            } else if (newRate == 2 * oldRate) {
                L = 2; M = 1;
            } else if (newRate == 4 * oldRate) {
                L = 4; M = 1;
            } else if (newRate == 8 * oldRate) {
                L = 8; M = 1;
            } else if (newRate * 2 == oldRate) {
                L = 1; M = 2;
            } else if (newRate * 4 == oldRate) {
                L = 1; M = 4;
            } else if (newRate * 8 == oldRate) {
                L = 1; M = 8;
            } else {
                return null;
            }

            //insert L-1 0s between each sample
            extendedSamples = new double[samples.Length * L];
            int i = 0;
            for (int s = 0; s < samples.Length; s++) {
                extendedSamples[i++] = samples[s];
                for (int extra = 0; extra < L-1; extra++) {
                    extendedSamples[i++ + extra] = 0;
                }
            }

            //lowpass filter 
            int S = Math.Min(oldRate, newRate);
            int N = 3000;
            double[] filter = new double[N];
            int limit = N / 2;
            for (int w = 0; w < filter.Length; w++) {
                if (w < limit) {
                    filter[w] = 1;
                } else {
                    filter[w] = 0;
                }
            }
            extendedSamples = convolveFilter(extendedSamples, filter);

            //select every Mth sample.
            result = new double[extendedSamples.Length / M];
            i = 0;
            for (int s = 0; s < extendedSamples.Length; s+=M) {
                result[i++] = extendedSamples[s];
            }

            return result;
        }

        /* Returns the highest amplitude found in the sample set. 
           Value is + or - accordingly.
        */
        private static double maxAmplitude(double[] samples) {
            double max = 0;
            for (int t = 0; t < samples.Length; t++) {
                if (Math.Abs(samples[t]) > max) {
                    max = samples[t];
                }
            }
            return max;
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
