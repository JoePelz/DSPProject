﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comp3931_Project_JoePelz {
    /// <summary>
    /// Windowing modes (for frequency-domain viewing).  Combined to reduce redundant functions.
    /// </summary>
    public enum DSP_Window { pass, triangle, cosine, blackman }
    /// <summary>
    /// Special FX capabilities. Combined in an enum to reduce redundant functions.
    /// </summary>
    public enum DSP_FX { reverse, normalize, pitchshift }
    /// <summary>
    /// Filtering modes. Combined in an enum to reduce redundant functions.
    /// </summary>
    public enum DSP_FILTER { convolution, DFT, IIRLowpass, IIRHighpass }

    class DSP {
        /// <summary>
        /// Discrete Fourier Transform.  Transforms time-domain to frequency domain.
        /// N == sample size.
        /// </summary>
        /// <param name="samples">Time-domain data to use</param>
        /// <returns></returns>
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

        /// <summary>
        /// Inverse Discrete Fourier Transform.  Transforms frequency domain data to time domain.
        /// Sample length == N (number of frequency bins)
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Performs convolution on a sample set, given a convolution kernel.  returns a copy of S, convolved.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="kernel"></param>
        /// <returns></returns>
        public static double[] convolve(ref double[] S, double[] kernel) {
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

        /// <summary>
        /// Filter a given set of samples (S) via convolution.  
        /// The filter must contain multipliers in the frequency domain.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="filter">A set of multipliers in the frequency domain.</param>
        /// <returns></returns>
        public static double[] convolveFilter(ref double[] S, double[] filter) {
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
            //Treat the samples that run off the end as 0s.
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

        /// <summary>
        /// Constructs a biquad IIR filter to filter the given samples. zeroFreq, highFreq, and mode modify the shape of the filter.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="zeroFreq"></param>
        /// <param name="highFreq"></param>
        /// <param name="mode">Highpass (1) or Lowpass (0) or reference frequency.</param>
        /// <param name="samplingRate"></param>
        /// <returns></returns>
        public static double[] IIRFilter(double[] S, double zeroFreq, double highFreq, double mode, double samplingRate) {
            double[] a = new double[2];
            double[] b = new double[3];

            //Using a biquad filter equation

            Complex z1 = Complex.fromMagAngle(1, zeroFreq / samplingRate * Math.PI * 2);
            Complex z2 = Complex.fromMagAngle(1, -zeroFreq / samplingRate * Math.PI * 2);
            Complex p1 = Complex.fromMagAngle(0.66, highFreq / samplingRate * Math.PI * 2);
            Complex p2 = Complex.fromMagAngle(0.66, -highFreq / samplingRate * Math.PI * 2);

            b[0] = 1;
            b[1] = -z1.re - z2.re;
            b[2] = z1.re * z2.re - z1.im * z2.im;

            a[0] = -p1.re - p2.re;
            a[1] = p1.re * p2.re - p1.im * p2.im;

            //reference frequency
            double Zr = 0; // reference frequency
            double Wr = 0; // reference angle
            if (mode == 0) {
                Zr = 0; //low pass
                Wr = 0;
            } else if (mode == 1) {
                Zr = samplingRate / 2; //high pass
                Wr = Math.PI;
            } else {
                Zr = mode; 
                Wr = Zr / samplingRate * Math.PI * 2;
            }
            //reference frequency as angle

            //scale factor
            double c = 1;
            if (mode == 0) {
                //lowpass can be simplified for calculating c
                c = (1 + a[0] + a[1]) / (1 + b[1] + b[2]);
            } else if (mode == 1) {
                //highpass can also be simplified for calculating c
                c = (1 - a[0] + a[1]) / (1 - b[1] + b[2]);
            } else {
                c = Math.Sqrt(
                    (Math.Pow(1 + a[0] * Math.Cos(Wr) + a[1] * Math.Cos(2 * Wr), 2) + Math.Pow(a[0] * Math.Sin(Wr) + a[1] * Math.Sin(2 * Wr), 2))
                    /
                    (Math.Pow(1 + b[1] * Math.Cos(Wr) + b[2] * Math.Cos(2 * Wr), 2) + Math.Pow(b[1] * Math.Sin(Wr) + b[2] * Math.Sin(2 * Wr), 2))
                );
            }
            
            b[0] = c;
            b[1] = c * (-z1.re - z2.re);
            b[2] = c * (z1.re * z2.re - z1.im * z2.im);

            a[0] = -p1.re - p2.re;
            a[1] = p1.re * p2.re - p1.im * p2.im;
            
            return processIIRFilter(ref S, a, b);
        }

        /// <summary>
        /// Runs a set of samples (S) in the time domain through an IIR filter, described by a and b. `a` represents multiplied historic results in the filtering process. `b` represents multipliers from the sample set.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="a">Array of influence factors for the feedback loop, going back in time the length of a.</param>
        /// <param name="b">Array of influence factors for the input samples, going back in time the length of b.</param>
        /// <returns></returns>
        private static double[] processIIRFilter(ref double[] S, double[] a, double[] b) {
            double[] result = new double[S.Length];

            int shortest = Math.Min(a.Length, b.Length);
            int n = 0;
            //skip the first few so that we don't go out of bounds.
            //They're unlikely to make a difference anyway.
            for (; n < shortest; n++) {
                result[n] = S[n];
            }

            for (; n < S.Length; n++) {
                result[n] = 0;
                for (int i = 0; i < b.Length; i++) {
                    result[n] += S[n - i] * b[i];
                }
                for (int i = 0; i < a.Length; i++) {
                    result[n] -= result[n - 1 - i] * a[i];
                }
            }

            return result;
        }

        /// <summary>
        /// Filters a sample set by converting to frequency domain, multiplying by factors, and reverting to time domain.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Given a multi-channel sample array, match the number of channels to the given newChannels.  (clone existing channels if needed)
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="newChannels"></param>
        /// <returns></returns>
        public static double[][] matchChannels(double[][] samples, short newChannels) {
            if (samples.Length == newChannels) {
                return samples;
            }
            double[] data = samples[0];

            //downmix to 1 channel
            for (int i = 0; i < samples.Length; i++) {
                data = StereoToMono(ref samples[i], ref data);
            }

            //duplicate channel to every other channel.
            samples = new double[newChannels][];
            for (int channel = 0; channel < newChannels; channel++) {
                samples[channel] = data;
            }

            return samples;
        }

        /// <summary>
        /// Take two sample arrays and add them together. Arrays may be different lengths, and the longer length is used..
        /// </summary>
        /// <param name="sampleA"></param>
        /// <param name="sampleB"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Resample audio from one sample rate to another. 
        /// Works by inserting 0s between existing samples at a particular ratio, 
        /// running a lowpass filter, 
        /// and selecting samples at a different ratio.
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="oldRate"></param>
        /// <param name="newRate"></param>
        /// <returns></returns>
        public static double[] resample(ref double[] samples, int oldRate, int newRate) {
            double[] extendedSamples, result;
            int L, M;
            if (newRate == oldRate) {
                return samples;
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
                extendedSamples[i] = samples[s];
                i++;
                for (int extra = 0; extra < L-1; extra++) {
                    extendedSamples[i] = 0;
                    i++;
                }
            }

            //lowpass filter 
            int S = Math.Min(oldRate, newRate);
            extendedSamples = DSP.IIRFilter(extendedSamples, S/2 + 5000, S/2 - 5000, 0, S*2);


            //select every Mth sample.
            result = new double[extendedSamples.Length / M];
            i = 0;
            for (int s = 0; s < extendedSamples.Length; s+=M) {
                result[i++] = extendedSamples[s];
            }

            return result;
        }

        /// <summary>
        /// Reinterpret the sample rate of a given set of samples to make it seem as though the pitch has been shifted by the given number of semitones.
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="sampleRate"></param>
        /// <param name="semitones"></param>
        /// <returns></returns>
        public static double[] pitchShift(ref double[] samples, int sampleRate, int semitones) {
            double[] extendedSamples, result;
            double freqRatio = Math.Pow(2, (double)semitones / 12.0);
            Fraction frac = new Fraction(freqRatio);
            int M = frac.num;
            int L = frac.denom;

            //insert L-1 0s between each sample
            extendedSamples = new double[samples.Length * L];
            int i = 0;
            for (int s = 0; s < samples.Length; s++) {
                extendedSamples[i] = samples[s];
                for (int extra = 1; extra <= L - 1; extra++) {
                    extendedSamples[i + extra] = extendedSamples[i + extra - 1];
                }
                i += L;
            }

            //select every Mth sample.
            result = new double[(int)Math.Ceiling(extendedSamples.Length / (double)M)];
            i = 0;
            for (int s = 0; s < extendedSamples.Length; s += M) {
                result[i++] = extendedSamples[s];
            }

            return result;
        }


        /// <summary>
        /// Returns the highest amplitude found in the sample set. Only returns positive numbers.
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        private static double maxAmplitude(double[] samples) {
            double max = 0;
            for (int t = 0; t < samples.Length; t++) {
                if (Math.Abs(samples[t]) > max) {
                    max = Math.Abs(samples[t]);
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

        /// <summary>
        /// Administer some special effects. Wheee!
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="args"></param>
        /// <param name="data"></param>
        public static void ApplyFX(DSP_FX effect, object[] args, ref WaveFile data) {
            double factor;
            switch (effect) {
                case DSP_FX.reverse:
                    for (int channel = 0; channel < data.channels; channel++) {
                        Array.Reverse(data.samples[channel]);
                    }
                    break;
                case DSP_FX.normalize:
                    for (int channel = 0; channel < data.channels; channel++) {
                        factor = maxAmplitude(data.samples[channel]) * 1.1;
                        for (int sample = 0; sample < data.samples[channel].Length; sample++) {
                            data.samples[channel][sample] = data.samples[channel][sample] / factor;
                        }
                    }
                    break;
                case DSP_FX.pitchshift:
                    for (int channel = 0; channel < data.channels; channel++) {
                        data.samples[channel] = pitchShift(ref data.samples[channel], data.sampleRate, (int)args[0]);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
