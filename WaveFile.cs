using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comp3931_Project_JoePelz {
    [Serializable]
    public class WaveFile {
        public short bitDepth;
        public short channels;
        public int sampleRate;
        public double[][] samples = null;
        private string path; // C:/temp/myFile.wav
        private string name; // myFile.wav
        
        public void readFile() {
            System.IO.BinaryReader sr;

            try { //TODO: test, rather than try/fail?
                sr = new System.IO.BinaryReader(System.IO.File.Open(path, System.IO.FileMode.Open));
            } catch (System.IO.IOException) {
                throw;
            }

            //====================
            //RIFF chunk id
            char[] ckID = sr.ReadChars(4);
            String a = new string(ckID);
            if (a.CompareTo("RIFF") != 0) {
                throw new FormatException("RIFF chunkID missing. Found " + ckID[0] + ckID[1] + ckID[2] + ckID[3] + ".");
            }

            UInt32 RIFFSize = sr.ReadUInt32();

            //====================
            //WAVE chunk id
            ckID = sr.ReadChars(4);
            a = new string(ckID);
            if (a.CompareTo("WAVE") != 0) {
                throw new FormatException("WAVE chunkID missing. Found " + ckID[0] + ckID[1] + ckID[2] + ckID[3] + ".");
            }

            //====================
            //fmt_ chunk id
            ckID = sr.ReadChars(4);
            a = new string(ckID);
            UInt32 chunkSize = sr.ReadUInt32();
            while (a.CompareTo("fmt ") != 0) {
                sr.ReadBytes((int)chunkSize);
                ckID = sr.ReadChars(4);
                a = new string(ckID);
                chunkSize = sr.ReadUInt32();
            }
            Int16 wFormatTag = sr.ReadInt16();
            Int16 nChannels = sr.ReadInt16();
            Int32 nSamplesPerSec = sr.ReadInt32();
            Int32 nAvgBytesPerSec = sr.ReadInt32();
            Int16 nBlockAlign = sr.ReadInt16();
            Int16 wBitsPerSample = sr.ReadInt16();
            chunkSize -= 16;
            //there may be more bytes in fmt_ so skip those.
            sr.ReadBytes((int)chunkSize);

            if (wFormatTag != 0x0001) {
                throw new FormatException("Invalid wave format. Only PCM wave files supported.");
            }

            //====================
            //data chunk id
            ckID = sr.ReadChars(4);
            a = new string(ckID);
            chunkSize = sr.ReadUInt32();
            while (a.CompareTo("data") != 0) {
                sr.ReadBytes((int)chunkSize);
                ckID = sr.ReadChars(4);
                a = new string(ckID);
                chunkSize = sr.ReadUInt32();
            }

            channels = (short)nChannels;
            bitDepth = (short)wBitsPerSample;
            sampleRate = nSamplesPerSec;
            long numSamples = chunkSize / (bitDepth / 8) / channels;
            samples = new double[channels][];
            for (int c = 0; c < channels; c++) {
                samples[c] = new double[numSamples];
            }

            //======================
            // read samples
            if (bitDepth == 16) {
                for (int i = 0; i < numSamples; i++) {
                    for (int c = 0; c < channels; c++) {
                        //assuming signed
                        //normalized to -1.0..+1.0
                        samples[c][i] = (double)sr.ReadInt16() / 32768.0;
                    }
                }
            } else if (bitDepth == 8) {
                for (int i = 0; i < numSamples; i++) {
                    for (int c = 0; c < channels; c++) {
                        //assuming unsigned
                        //normalized to -1.0..+1.0
                        samples[c][i] = (double)sr.ReadByte() / 128.0 - 1.0;
                    }
                }
            } else {
                throw new FormatException("Bit depth must be one of 8 or 16 bits.");
            }
            sr.Close();
        }

        public void writeFile(string dest) {
            System.IO.BinaryWriter wr;

            try { //TODO: test, rather than try/fail?
                wr = new System.IO.BinaryWriter(System.IO.File.Open(dest, System.IO.FileMode.Create));
            } catch (System.IO.IOException) {
                throw;
            }

            //====================
            //RIFF chunk id
            wr.Write("RIFF".ToCharArray());
            wr.Write((Int32)(4 + (8 + 16) + (8 + (getNumSamples() * channels * bitDepth / 8)))); //TODO: 4 + size of fmt + size of data

            //====================
            //WAVE chunk id
            wr.Write("WAVE".ToCharArray());

            //====================
            //fmt_ chunk
            wr.Write("fmt ".ToCharArray());
            wr.Write((Int32)16);           //nChunkSize
            wr.Write((Int16)0x0001);       //wFormatTag
            wr.Write((Int16)channels);     //nChannels
            wr.Write((Int32)sampleRate);   //nSamplesPerSecond
            wr.Write((Int32)(sampleRate * channels * bitDepth / 8)); //nAvgBytesPerSecond
            wr.Write((Int16)(channels * bitDepth / 8));  //nBlockAlign
            wr.Write((Int16)bitDepth);     //wBitsPerSample

            //====================
            //data chunk
            wr.Write("data".ToCharArray());
            wr.Write((Int32)(getNumSamples() * channels * bitDepth / 8)); //TODO: total samples length in bytes

            if (bitDepth == 16) {
                for (int i = 0; i < getNumSamples(); i++) {
                    for (int c = 0; c < channels; c++) {
                        //assuming signed
                        wr.Write((short)(samples[c][i] * 32768));
                    }
                }
            } else if (bitDepth == 8) {
                for (int i = 0; i < getNumSamples(); i++) {
                    for (int c = 0; c < channels; c++) {
                        //assuming unsigned
                        wr.Write((byte)(samples[c][i] * 128 + 128));
                    }
                }
            }

            if ((getNumSamples() * channels * bitDepth / 8) % 2 == 1)
                wr.Write((byte)0); //padding byte, if the above equation is odd

            wr.Close();
        }

        public byte[] getData() {
            byte[] result = new byte[getNumSamples() * channels * (bitDepth / 8)];
            System.IO.MemoryStream ms = new System.IO.MemoryStream(result);
            System.IO.BinaryWriter wr = new System.IO.BinaryWriter(ms);

            if (bitDepth == 16) {
                for (int i = 0; i < getNumSamples(); i++) {
                    for (int c = 0; c < channels; c++) {
                        //assuming signed
                        wr.Write((short)(samples[c][i] * 32768));
                    }
                }
            } else if (bitDepth == 8) {
                for (int i = 0; i < getNumSamples(); i++) {
                    for (int c = 0; c < channels; c++) {
                        //assuming unsigned
                        wr.Write((byte)(samples[c][i] * 128 + 128));
                    }
                }
            }
            wr.Dispose();
            ms.Dispose();
            return result;
        }

        public void writeStream(System.IO.Stream outStream) {
            System.IO.BinaryWriter wr;

            try { //TODO: test, rather than try/fail?
                wr = new System.IO.BinaryWriter(outStream);
            } catch (System.IO.IOException) {
                throw;
            }

            //====================
            //RIFF chunk id
            wr.Write("RIFF".ToCharArray());
            wr.Write((Int32)(4 + (8 + 16) + (8 + (getNumSamples() * channels * bitDepth / 8)))); //TODO: 4 + size of fmt + size of data

            //====================
            //WAVE chunk id
            wr.Write("WAVE".ToCharArray());

            //====================
            //fmt_ chunk
            wr.Write("fmt ".ToCharArray());
            wr.Write((Int32)16);           //nChunkSize
            wr.Write((Int16)0x0001);       //wFormatTag
            wr.Write((Int16)channels);     //nChannels
            wr.Write((Int32)sampleRate);   //nSamplesPerSecond
            wr.Write((Int32)(sampleRate * channels * bitDepth / 8)); //nAvgBytesPerSecond
            wr.Write((Int16)(channels * bitDepth / 8));  //nBlockAlign
            wr.Write((Int16)bitDepth);     //wBitsPerSample

            //====================
            //data chunk
            wr.Write("data".ToCharArray());
            wr.Write((Int32)(getNumSamples() * channels * bitDepth / 8)); //TODO: total samples length in bytes

            if (bitDepth == 16) {
                for (int i = 0; i < getNumSamples(); i++) {
                    for (int c = 0; c < channels; c++) {
                        //assuming signed
                        wr.Write((short)(samples[c][i] * 32768));
                    }
                }
            } else if (bitDepth == 8) {
                for (int i = 0; i < getNumSamples(); i++) {
                    for (int c = 0; c < channels; c++) {
                        //assuming unsigned
                        wr.Write((byte)(samples[c][i] * 128 + 128));
                    }
                }
            }

            if ((getNumSamples() * channels * bitDepth / 8) % 2 == 1)
                wr.Write((byte)0); //padding byte, if the above equation is odd
        }

        public WaveFile() {
            generateSamples();
            path = null;
            name = "[Untitled File]";
        }

        public WaveFile(short bitDepth, short channels, int sampleRate, byte[] data) {
            this.bitDepth = bitDepth;
            this.channels = channels;
            this.sampleRate = sampleRate;
            path = null;
            name = "[Untitled Recording]";

            System.IO.MemoryStream ms = new System.IO.MemoryStream(data);
            System.IO.BinaryReader br = new System.IO.BinaryReader(ms);
            int numSamples = data.Length / (channels * (bitDepth / 8));
            samples = new double[channels][];
            for (int c = 0; c < channels; c++) {
                samples[c] = new double[numSamples];
            }

            if (bitDepth == 16) {
                for (int i = 0; i < numSamples; i++) {
                    for (int c = 0; c < channels; c++) {
                        //assuming signed
                        //normalized to -1.0..+1.0
                        samples[c][i] = (double)br.ReadInt16() / 32768.0;
                    }
                }
            } else if (bitDepth == 8) {
                for (int i = 0; i < numSamples; i++) {
                    for (int c = 0; c < channels; c++) {
                        //assuming unsigned
                        //normalized to -1.0..+1.0
                        samples[c][i] = (double)br.ReadByte() / 128.0 - 1.0;
                    }
                }
            } else {
                throw new FormatException("Bit depth must be one of 8 or 16 bits.");
            }

        }

        private WaveFile(short bitDepth, short channels, int sampleRate) {
            path = null;
            name = "[Untitled File]";
            this.bitDepth = bitDepth;
            this.channels = channels;
            this.sampleRate = sampleRate;
            samples = new double[channels][];
        }

        public WaveFile(string src) {
            path = src;
            name = path.Substring(path.LastIndexOf("\\") + 1);
            readFile();
        }

        private void generateSamples() {
            double A, f, phase;

            bitDepth = 16;
            channels = 1;
            sampleRate = 44100;

            samples = new double[channels][];
            for (int c = 0; c < channels; c++) {
                samples[c] = new double[sampleRate];
                for (int t = 0; t < sampleRate; t++) {
                    samples[c][t] = 0;

                    // Assignment 2, question 4
                    /*
                    samples[c][t] += 2 * Math.Cos(2 * Math.PI *  3 * t / 10);
                    samples[c][t] += 3 * Math.Sin(2 * Math.PI * 14 * t / 10);
                    */

                    // Assignment 2, question 3
                    /*
                    A = 1.0; f = 3; phase = Math.PI / 4;
                    samples[c][t] += A * Math.Cos(Math.PI * 2 * f * t / sampleRate + phase);

                    A = 1.5; f = 6; phase = Math.PI / 3;
                    samples[c][t] += A * Math.Cos(Math.PI * 2 * f * t / sampleRate + phase);
                    /**/

                    /*
                    A = 1.25; f = 25; phase = 0;
                    samples[c][t] += A * Math.Sin(Math.PI * 2 * f * t / sampleRate + phase);
                    A = 1.25; f = 75; phase = 0;
                    samples[c][t] += A * Math.Sin(Math.PI * 2 * f * t / sampleRate + phase);
                    */
                    
                    A = 1.0; f = 250; phase = 0;
                    samples[c][t] += A * Math.Sin(Math.PI * 2 * f * t / sampleRate + phase);
                    
                    A = 0.75; f = 600; phase = 0;
                    samples[c][t] += A * Math.Sin(Math.PI * 2 * f * t / sampleRate + phase);

                    A = 0.5; f = 800; phase = 0;
                    samples[c][t] += A * Math.Sin(Math.PI * 2 * f * t / sampleRate + phase);

                    A = 0.25; f = 1600; phase = 0;
                    samples[c][t] += A * Math.Sin(Math.PI * 2 * f * t / sampleRate + phase);
                    /**/
                    samples[c][t] = samples[c][t]
                        / 2.5;  //divide by total amplitude (new range: -1.0..1.0)
                }
            }
        }

        public int getNumSamples() {
            if (samples != null && samples[0] != null) {
                return samples[0].Length;
            }
            return 0;
        }

        public double getDuration() {
            return (double)getNumSamples() / sampleRate;
        }

        public string getPath() {
            return path;
        }

        public string getName() {
            return name;
        }

        public void setPath(string src) {
            path = src;
            name = path.Substring(path.LastIndexOf("\\") + 1);
        }

        public void save() {
            writeFile(path);
        }

        public bool inRange(int sample) {
            return (sample >= 0 && sample < getNumSamples());
        }

        private void dropSamples(int start, int end) {
            if (!inRange(start) || end <= start) {
                return;
            }

            double[][] result = new double[channels][];
            int i;

            if (end > getNumSamples()) {
                end = getNumSamples();
            }

            for (i = 0; i < channels; i++) {
                result[i] = new double[start + getNumSamples() - end];
            }

            for (int channel = 0; channel < channels; channel++) {
                //fill in pre
                for (i = 0; i < start; i++) {
                    result[channel][i] = samples[channel][i];
                }
                //fill in post
                for (i = end; i < getNumSamples(); i++) {
                    result[channel][i - (end - start)] = samples[channel][i];
                }
            }
            samples = result;
        }

        internal void pasteSelection(int destIndex, double[][] newSamples) {
            double[][] result = new double[channels][];
            int i;
            if (destIndex > getNumSamples()) {
                destIndex = getNumSamples();
            }
            int pre = destIndex;
            int mid = newSamples[0].Length;
            int post = getNumSamples() - destIndex;

            for (i = 0; i < channels; i++) {
                result[i] = new double[pre + mid + post];
            }

            for (int channel = 0; channel < channels; channel++) {
                //fill in pre
                for (i = 0; i < pre; i++) {
                    result[channel][i] = samples[channel][i];
                }
                //fill in mid
                for (i = pre; i < pre + mid; i++) {
                    result[channel][i] = newSamples[channel][i - pre];
                }
                //fill in post
                for (i = pre; i < pre + post; i++) {
                    result[channel][i + mid] = samples[channel][i];
                }
            }
            samples = result;
        }

        internal void pasteSelection(int destIndex, WaveFile newWave) {
            //TODO: newWave must be resampled to match sampling rate with current sample.
            double[][] result = new double[channels][];
            int i;
            if (destIndex > getNumSamples()) {
                destIndex = getNumSamples();
            }
            int pre = destIndex;
            int mid = newWave.getNumSamples();
            int post = getNumSamples() - destIndex;

            for (i = 0; i < channels; i++) {
                result[i] = new double[pre + mid + post];
            }

            for (int channel = 0; channel < channels; channel++) {
                //fill in pre
                for (i = 0; i < pre; i++) {
                    result[channel][i] = samples[channel][i];
                }
                //fill in mid
                for (i = pre; i < pre + mid; i++) {
                    result[channel][i] = newWave.samples[channel][i - pre];
                }
                //fill in post
                for (i = pre; i < pre + post; i++) {
                    result[channel][i + mid] = samples[channel][i];
                }
            }
            samples = result;
        }

        internal WaveFile copySelection(int start, int end) {
            if (!inRange(start) || end <= start) {
                return null;
            }
            if (end > getNumSamples()) {
                end = getNumSamples();
            }
            double[][] duplicate = new double[channels][];
            for (int i = 0; i < channels; i++) {
                duplicate[i] = new double[end - start];
            }
            for (int channel = 0; channel < channels; channel++) {
                for (int i = start; i < end; i++) {
                    duplicate[channel][i - start] = samples[channel][i];
                }
            }

            WaveFile copy = new WaveFile(bitDepth, channels, sampleRate);
            copy.pasteSelection(0, duplicate);
            return copy;
        }

        internal WaveFile cutSelection(int start, int end) {
            WaveFile copy = copySelection(start, end);
            dropSamples(start, end);
            return copy;
        }
    }
}
