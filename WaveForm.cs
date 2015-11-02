using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Comp3931_Project_JoePelz {
    public partial class WaveForm : Form {
        private WaveFile wave;
        private WavePlayer player;
        private Complex[][] DFT;
        private DSP_Window sampleWindowing = DSP_Window.pass;
        private int fourierN = 882;
        private bool invalidPlayer = true;
        private int tSelStart;
        private int tSelEnd;
        private int fMouseDown = 0;
        private int fMouseDrag = 0;
        private int fSelStart = 0;
        private int fSelEnd = 0;
        private float waveViewHeight = 0;
        private float waveViewWidth = 0;
        private float fourierViewHeight = 0;
        private float fourierViewWidth = 0;

        private Mixer parent = null;

        public WaveForm(Mixer parent, WaveFile wave) {
            this.wave = wave;
            this.parent = parent;
            InitializeComponent();
            this.Text = wave.getName();

            updateStatusBar();
            calculateDFT();
            waveViewWidth = panelWave.ClientSize.Width;
            waveViewHeight = panelWave.ClientSize.Height;
            panelWave.setSamples(wave.samples);
            //panelWave.setPanelDrawHeight(panelWave.ClientSize.Height - scrollerWave.ClientSize.Height);
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys) {
            switch (keys) {
                case Keys.C | Keys.Control:
                    copySelection();
                    return true; // signal that we've processed this key
                case Keys.V | Keys.Control:
                    pasteAtSelection();
                    return true;
                case Keys.X | Keys.Control:
                    cutSelection();
                    return true;
                case Keys.Space:
                    wavePlayPause();
                    return true;
                case Keys.Home | Keys.Shift:
                    panelWave.TriggerHotkeys(ref message, keys);
                    return true;
                case Keys.Home:
                    //updateSelection(0, 0);
                    return true;
                case Keys.End | Keys.Shift:
                    //updateSelection(tSelStart, wave.getNumSamples() - 1);
                    return true;
                case Keys.End:
                    updateSelection(wave.getNumSamples() - 1, wave.getNumSamples() - 1);
                    return true;
                case Keys.Delete:
                    //filterSelectedFrequencies();
                    return true;
                case Keys.F:
                    panelWave.focusAll();
                    Invalidate();
                    return true;
            }

            // run base implementation
            return base.ProcessCmdKey(ref message, keys);
        }

        public void wavePlayPause() {
            if (player == null) {
                player = new WavePlayer(this);
            }

            if (invalidPlayer) {
                if (player != null) {
                    player.stop();
                    player.Dispose();
                }
                if (tSelEnd == tSelStart)
                    player.setWave(wave.copySelection(tSelStart, wave.getNumSamples()));
                else
                    player.setWave(wave.copySelection(tSelStart, tSelEnd));
                invalidPlayer = false;
            }
            
            if (player.isPlaying()) {
                player.pause(); //toggles paused/unpaused
            } else {
                player.play();
            }
        }

        public void waveStop() {
            if (player != null) {
                player.stop();
            }
        }

        public void updatePlaybackStatus(PlaybackStatus update) {
            parent.playbackUpdate(update);
        }

        private void panelFourier_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            drawFreqSelection(g);
            drawAliasAxis(g);
            drawFourier(g);
            fourierViewHeight = g.VisibleClipBounds.Height;
            fourierViewWidth = g.VisibleClipBounds.Width;
        }

        private void drawFreqSelection(Graphics g) {
            if (fSelStart == fSelEnd) {
                return;
            }
            RectangleF client = g.VisibleClipBounds;
            RectangleF r = new RectangleF();
            int w = (int)g.VisibleClipBounds.Width;
            r.Y = 0;
            r.Height = client.Height;
            r.X = Math.Max(1.0f / fourierN * client.Width, (float)(fSelStart) / fourierN * client.Width);
            r.Width = Math.Max(2, (float)(fSelEnd+1) / fourierN * client.Width - r.X);
            g.FillRectangle(Brushes.PaleGoldenrod, r);
            r.X = client.Width - (Math.Max(0, (float)(fSelStart-1) / fourierN * client.Width)) - r.Width;
            g.FillRectangle(Brushes.PaleGoldenrod, r);
        }

        private void drawAliasAxis(Graphics g) {
            Pen dashed = new Pen(Color.DarkSeaGreen, Math.Max(2, g.VisibleClipBounds.Width / fourierN / 6));
            float[] dashValues = { 3, 2 };
            dashed.DashPattern = dashValues;
            float mid = g.VisibleClipBounds.Width / 2.0f + (g.VisibleClipBounds.Width / fourierN) / 2.0f;
            g.DrawLine(dashed, mid, 0.0f, mid, g.VisibleClipBounds.Height);
        }

        private void calculateDFT() {
            DFT = new Complex[wave.channels][];
            double[] samples = new double[fourierN];

            //TODO: handle multiple channels. (currently grabs first channel only)
            for (int i = 0; i < samples.Length; i++) {
                samples[i] = 0;
            }
            int startIndex = Math.Max(tSelStart, 0);
            
            int N = Math.Min(wave.getNumSamples() - startIndex, fourierN);
            Array.Copy(wave.samples[0], startIndex, samples, 0, N);

            if (sampleWindowing == DSP_Window.triangle) {
                DSP.WindowTriangle(ref samples);
            } else if (sampleWindowing == DSP_Window.cosine) {
                DSP.WindowCosine(ref samples);
            } else if (sampleWindowing == DSP_Window.blackman) {
                DSP.WindowBlackman(ref samples);
            } else {
                DSP.WindowPassthrough(ref samples);
            }

            DFT[0] = DSP.DFT(ref samples);
            panelFourier.Invalidate();
        }

        private void drawFourier(Graphics g) {
            //TODO: handle multiple channels in fourier
            int w = (int)g.VisibleClipBounds.Width;
            int h = (int)g.VisibleClipBounds.Height - statusBar.Height - 1;
            int start, end;
            float value;
            if (fourierN < w) {
                //more pixels than fourier steps
                for (int i = 0; i < fourierN; i++) {
                    start = (int)(i / (double)fourierN * w);
                    end = (int)((i+1) / (double)fourierN * w);
                    value = (float)Math.Sqrt(DFT[0][i].magnitude() / (fourierN/2)) * h;
                    g.FillRectangle(Brushes.Brown, start, h - value, end - start, value);
                }
            } else {
                //more bars than pixels wide
                for (int i = 0; i < w; i++) {
                    start = (int)((float)i / (float)w * (fourierN - 1));
                    end   = (int)((float)(i+1) / (float)w * (fourierN - 1));
                    value = 0;
                    for (int j = start; j < end; j++) {
                        value = Math.Max(value, (float)Math.Sqrt(DFT[0][j].magnitude() / (fourierN / 2)));
                    }
                    value *= h;
                    g.DrawLine(Pens.Brown, i, h, i, h - Math.Abs(value));
                }
            }
        }
        
        private void panelFourier_Resize(object sender, System.EventArgs e) {
            ((Control)sender).Invalidate();
        }
        
        private void updateStatusBar() {
            statusSampling.Text = String.Format("Sampling Rate: {0} Hz", wave.sampleRate);
            statusBits.Text = String.Format("Depth: {0}-bit", wave.bitDepth);
            statusLength.Text = String.Format("Length: {0:0.000}s ({1} samples)", (double)wave.getNumSamples() / wave.sampleRate, wave.getNumSamples());
            statusSelection.Text = String.Format("Selected: {0:0.000} seconds ({1}..{2}", (tSelEnd - tSelStart) / (double)wave.sampleRate, tSelStart, tSelEnd);
        }

        //TODO: this isn't safe, and can go out of bounds.
        public void updateSelection(int low, int high) {
            tSelStart = low;
            tSelEnd = high;
            calculateDFT();
            panelFourier.Invalidate();
            updateStatusBar();
            invalidPlayer = true;
        }

        private void WaveForm_GotFocus(object sender, EventArgs e) {
            this.parent.setActiveWindow(this);
        }

        private void panelFourier_MouseDown(object sender, MouseEventArgs e) {
            Graphics g = ((Control)sender).CreateGraphics();
            float position = e.X / g.VisibleClipBounds.Width;
            int index = (int)(position * DFT[0].Length);
            fSelStart = index;
            fSelEnd = index;
            fMouseDown = index;
            fMouseDrag = index;
        }

        private void panelFourier_MouseUp(object sender, MouseEventArgs e) {
            Graphics g = ((Control)sender).CreateGraphics();
            float position = e.X / g.VisibleClipBounds.Width;
            int index = (int)(position * DFT[0].Length);
            fMouseDrag = index;
            if (fMouseDrag == fMouseDown) {
                updateFreqSelection(-2, -1);
            } else {
                updateFreqSelection(Math.Min(fMouseDown, fMouseDrag), Math.Max(fMouseDown, fMouseDrag));
            }
        }

        private void panelFourier_MouseMove(object sender, MouseEventArgs e) {
            Graphics g = ((Control)sender).CreateGraphics();
            float position = e.X / g.VisibleClipBounds.Width;
            int index = (int)(position * DFT[0].Length);
            int loval = (int)((float)index / DFT[0].Length * wave.sampleRate);
            String info;
            g.Dispose();

            if (index >= 0 && index <= fourierN-1) {
                if (loval >= wave.sampleRate / 2) {
                    lblFourierFreq.Text = String.Format("Frequency: {0}Hz [Aliased]", loval);
                } else {
                    lblFourierFreq.Text = String.Format("Frequency: {0}Hz", loval);
                }
                double amplitude = DFT[0][index].magnitude() / (fourierN / 2);
                double phase = 0;
                if (amplitude > 0.00001)
                    phase = DFT[0][index].angle() / Math.PI;
                if (e.Button != MouseButtons.Left) {
                    info = String.Format("Frequency: {0}Hz; Amplitude: {1:0.0}; Phase: {2:0.000}pi*rad", loval, amplitude, phase);
                    report(info);
                    return;
                }
            }
            
            fMouseDrag = index;
            updateFreqSelection(Math.Min(fMouseDown, fMouseDrag), Math.Max(fMouseDown, fMouseDrag));

            loval = (int)((float)fSelStart / DFT[0].Length * wave.sampleRate);
            int hival = (int)((float)(fSelEnd+1) / DFT[0].Length * wave.sampleRate);
            info = String.Format("Selected: {0}Hz to {1}Hz", loval, hival);
            report(info);
        }

        private void updateFreqSelection(int low, int high) {
            fSelStart = Math.Max(0, low);
            fSelEnd = Math.Min(fourierN-1, high);
            panelFourier.Invalidate();
        }

        private void WaveForm_FormClosed(object sender, FormClosedEventArgs e) {
            waveStop();
            if (player != null) {
                player.Dispose();
            }
            parent.childDied(this);
        }

        public void setPath(String path) {
            wave.setPath(path);
            this.Text = wave.getName();
        }

        public void save() {
            wave.save();
        }

        public void copySelection() {
            WaveFile copy = wave.copySelection(tSelStart, tSelEnd);
            if (copy != null) {
                Clipboard.SetData("WaveFile", copy);
                report((tSelEnd - tSelStart) + " samples copied to clipboard!");
            } else {
                MessageBox.Show("No samples selected to copy!");
            }
        }

        public void cutSelection() {
            int before = wave.getNumSamples();
            WaveFile cut = wave.cutSelection(tSelStart, tSelEnd);
            if (cut != null) {
                Clipboard.SetData("WaveFile", cut);
                report((tSelEnd - tSelStart) + " samples cut to clipboard!");
            } else {
                MessageBox.Show("No samples selected to cut!");
            }
            int after = wave.getNumSamples();
            updateSelection(tSelStart, tSelStart);
        }

        public void pasteAtSelection() {
            if (!Clipboard.ContainsData("WaveFile")) {
                MessageBox.Show("No samples in clipboard.");
                return;
            }
            WaveFile data = (WaveFile)Clipboard.GetData("WaveFile");
            if (data == null) {
                MessageBox.Show("Error reading clipboard.");
                return;
            }
            wave.cutSelection(tSelStart, tSelEnd);
            wave.pasteSelection(tSelStart, data);
            report(data.getNumSamples() + " samples pasted from the clipboard!");
            updateSelection(tSelStart, tSelStart + data.getNumSamples());
        }

        public void filterSelectedFrequencies() {
            if (fSelStart == fSelEnd)
                return;
            
            double[] filter = new double[fourierN];
            for (int fbin = 0; fbin < filter.Length; fbin++) {
                if ((fbin >= fSelStart && fbin <= fSelEnd) || (fbin >= fourierN - fSelEnd && fbin <= fourierN - fSelStart)) {
                    filter[fbin] = 0;
                } else {
                    filter[fbin] = 1;
                }
            }

            for (int channel = 0; channel < wave.channels; channel++) {
                wave.samples[channel] = DSP.convolveFilter(wave.samples[channel], filter);
            }
            panelWave.Invalidate();
            calculateDFT();
            panelFourier.Invalidate();
            invalidPlayer = true;
        }

        public void applyFX(DSP_FX effect) {
            int startIndex = Math.Max(tSelStart, 0);
            int endIndex;
            if (tSelEnd != tSelStart) {
                endIndex = tSelEnd;
            } else {
                endIndex = wave.getNumSamples();
            }

            WaveFile data = wave.cutSelection(startIndex, endIndex);
            DSP.ApplyFX(effect, ref data);
            wave.pasteSelection(startIndex, data.samples);
            panelWave.Invalidate();
            invalidPlayer = true;
        }

        public void report(String msg) {
            labelReport.Text = msg;
        }

        private void passthroughToolStripMenuItem_Click(object sender, EventArgs e) {
            sampleWindowing = DSP_Window.pass;
            calculateDFT();
        }

        private void triangleToolStripMenuItem_Click(object sender, EventArgs e) {
            sampleWindowing = DSP_Window.triangle;
            calculateDFT();
        }

        private void cosineToolStripMenuItem_Click(object sender, EventArgs e) {
            sampleWindowing = DSP_Window.cosine;
            calculateDFT();
        }

        private void blackmanToolStripMenuItem_Click(object sender, EventArgs e) {
            sampleWindowing = DSP_Window.blackman;
            calculateDFT();
        }
    }
}
