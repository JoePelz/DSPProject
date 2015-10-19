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
        private int tViewStart = 0;
        private int tViewEnd = 800;
        private int tSelStart = 0;
        private int tSelEnd = 0;
        private bool invalidPlayer = true;
        private int tMouseDown = 0;
        private int tMouseDrag = 0;
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

            updateScrollRange();
            updateStatusBar();
            //calculateSimpleFourier();
            calculateDFT();
            waveViewWidth = panelWave.ClientSize.Width;
            waveViewHeight = panelWave.ClientSize.Height;
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
                    updateSelection(0, tSelEnd);
                    return true;
                case Keys.Home:
                    updateSelection(0, 0);
                    return true;
                case Keys.End | Keys.Shift:
                    updateSelection(tSelStart, wave.getNumSamples() - 1);
                    return true;
                case Keys.End:
                    updateSelection(wave.getNumSamples() - 1, wave.getNumSamples() - 1);
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
            //parent.playbackUpdate(update);
        }

        private void panelFourier_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            drawAliasAxis(g);
            drawFourier(g);
            fourierViewHeight = g.VisibleClipBounds.Height;
            fourierViewWidth = g.VisibleClipBounds.Width;
        }

        private void drawAliasAxis(Graphics g) {
            Pen dashed = new Pen(Color.DarkSeaGreen, 2);
            float[] dashValues = { 3, 2 };
            dashed.DashPattern = dashValues;
            float mid = g.VisibleClipBounds.Width / 2.0f;
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

        private void panelWave_Resize(object sender, System.EventArgs e) {
            Graphics g = ((Control)sender).CreateGraphics();
            if (g.VisibleClipBounds.Height != waveViewHeight)
                ((Control)sender).Invalidate();

            int oldRange = tViewEnd - tViewStart;
            int newRange = (int)(oldRange * (float)g.VisibleClipBounds.Width / (float)waveViewWidth);
            tViewEnd = Math.Min(wave.getNumSamples(), tViewStart + newRange);
            tViewStart = tViewEnd - newRange;
            if (tViewStart < 0) {
                tViewStart = 0;
                tViewEnd = tViewStart + newRange;
            }
            waveViewHeight = g.VisibleClipBounds.Height;
            waveViewWidth = g.VisibleClipBounds.Width;
            updateScrollRange();
            panelWave.Invalidate();
        }

        private void panelFourier_Resize(object sender, System.EventArgs e) {
            ((Control)sender).Invalidate();
        }

        private void panelWave_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            drawSelection(g);
            drawGrid(g);
            drawWave(g);
        }

        private void drawGrid(Graphics g) {
            RectangleF r = g.VisibleClipBounds;
            r.Height -= this.scrollerWave.Height;
            //x-axis line
            g.FillRectangle(Brushes.White, 0, r.Height / 2 - 1, r.Width, 2);
        }

        private void drawWave(Graphics g) {
            RectangleF r = g.VisibleClipBounds;
            r.Height -= this.scrollerWave.Height;
            int numPoints = Math.Min(tViewEnd - tViewStart, (int)Math.Ceiling(r.Width));
            if (numPoints <= 0) return;
            float timeStep = (float)(tViewEnd - tViewStart) / numPoints;
            float xStep = r.Width / numPoints;
            Pen p = new Pen(Color.Yellow, 2);
            int index;
            int viewRange = tViewEnd - tViewStart;

            //TODO: handle multiple channels

            //draw using DrawPolyLine
            /**/
            PointF[] pathPoints = new PointF[numPoints + 2];
            pathPoints[0] = new PointF(-1, 0.5f * r.Height);
            pathPoints[numPoints+1] = new PointF(r.Width, 0.5f * r.Height);
            for (int i = 1; i <= numPoints; i++) {
                index = (int)(Math.Round(i * timeStep + tViewStart));
                if (index >= 0 && index < wave.getNumSamples()) {
                    //pathPoints[i] = new PointF(i * xStep, (1.0f - (float)(wave.samples[0][index] * 0.5 + 0.5)) * r.Height);
                    pathPoints[i] = new PointF((float)(index - tViewStart) / viewRange * r.Width, (1.0f - (float)(wave.samples[0][index] * 0.5 + 0.5)) * r.Height);
                } else {
                    pathPoints[i] = new PointF(i * xStep, 0.5f * r.Height);
                }
            }
            g.DrawPolygon(p, pathPoints);
            //*/

            p.Dispose();
        }

        private void drawSelection(Graphics g) {
            RectangleF client = g.VisibleClipBounds;
            RectangleF r = new RectangleF();
            int viewRange = tViewEnd - tViewStart;
            r.Y = 0;
            r.Height = client.Height;
            r.X = Math.Max(0, (float)(tSelStart - tViewStart) / viewRange * client.Width);
            r.Width = Math.Max(2, (float)(tSelEnd - tViewStart) / viewRange * client.Width - r.X);
            g.FillRectangle(Brushes.DarkKhaki, r);
        }

        private void updateScrollRange() {
            int viewRange = tViewEnd - tViewStart;
            int scrollRange = wave.getNumSamples() - viewRange;
            if (scrollRange < 0) {
                scrollerWave.Maximum = 0;
                scrollerWave.Value = 0;
                scrollerWave.Enabled = false;
            } else {
                scrollerWave.Maximum = wave.getNumSamples() - viewRange;
                scrollerWave.Value = tViewStart;
                scrollerWave.Enabled = true;
            }
        }
        
        private void updateStatusBar() {
            statusSampling.Text = String.Format("Sampling Rate: {0} Hz", wave.sampleRate);
            statusBits.Text = String.Format("Depth: {0}-bit", wave.bitDepth);
            statusLength.Text = String.Format("Length: {0:0.000}s ({1} samples)", (double)wave.getNumSamples() / wave.sampleRate, wave.getNumSamples());
            statusSelection.Text = String.Format("Selected: {0:0.000} seconds ({1}..{2}", (tSelEnd - tSelStart) / (double)wave.sampleRate, tSelStart, tSelEnd);
        }

        private void updateSelection(int low, int high) {
            if (low < 0)
                tSelStart = 0;
            else if (low > wave.getNumSamples())
                tSelStart = wave.getNumSamples() - 1;
            else
                tSelStart = low;
            if (high < 0)
                tSelEnd = 0;
            else if (high > wave.getNumSamples())
                tSelEnd = wave.getNumSamples() - 1;
            else
                tSelEnd = high;

            calculateDFT();
            panelFourier.Invalidate();
            panelWave.Invalidate();
            updateStatusBar();
            invalidPlayer = true;
        }

        private void scrollerWave_Scroll(object sender, ScrollEventArgs e) {
            scrollWave(0);
        }

        private void scrollWave(int scrollValue) {
            int range = tViewEnd - tViewStart;
            if (scrollerWave.Value + scrollValue > scrollerWave.Maximum) {
                scrollerWave.Value = scrollerWave.Maximum;
            } else if (scrollerWave.Value + scrollValue < scrollerWave.Minimum) {
                scrollerWave.Value = scrollerWave.Minimum;
            } else {
                scrollerWave.Value += scrollValue;
            }
            tViewStart = scrollerWave.Value;
            tViewEnd = scrollerWave.Value + range;
            panelWave.Invalidate();
        }

        private void WaveForm_GotFocus(object sender, EventArgs e) {
            this.parent.setActiveWindow(this);
        }

        private void panelFourier_MouseMove(object sender, MouseEventArgs e) {
            Graphics g = ((Control)sender).CreateGraphics();
            float position = e.X / g.VisibleClipBounds.Width;
            int index = (int)(position * DFT[0].Length);
            int loval = (int)((float)index / DFT[0].Length * wave.sampleRate);
            g.Dispose();

            if (loval >= wave.sampleRate / 2) {
                lblFourierFreq.Text = String.Format("Frequency: {0}Hz [Aliased]", loval);
            } else {
                lblFourierFreq.Text = String.Format("Frequency: {0}Hz", loval);
            }
            double amplitude = DFT[0][index].magnitude() / (fourierN / 2);
            double phase = 0;
            if (amplitude > 0.00001)
                phase = DFT[0][index].angle() / Math.PI;

            String info = String.Format("Frequency: {0}Hz; Amplitude: {1:0.0}; Phase: {2:0.000}pi*rad", loval, amplitude, phase);
            report(info);
        }

        private void WaveForm_FormClosed(object sender, FormClosedEventArgs e) {
            parent.childDied(this);
        }
        
        private void panelWave_MouseWheel(object sender, MouseEventArgs e) {
            float scrollAmount = e.Delta / 120.0f;
            if (scrollAmount < 0 && tViewEnd > wave.getNumSamples()) {
                return;
            } else if (scrollAmount > 0 && (tViewEnd - tViewStart) <= 10) {
                return;
            }
            int mousePoint = (int)((double)e.X / panelWave.Width * (tViewEnd - tViewStart) + tViewStart);
            int less = mousePoint - tViewStart;
            int more = tViewEnd - mousePoint;
            less = (int)(less * Math.Pow(0.83f, scrollAmount));
            more = (int)(more * Math.Pow(0.83f, scrollAmount));

            tViewEnd = mousePoint + more;
            tViewStart = mousePoint - less;
            
            //avoid scrolling off the ends, prefer to block at sample 0.
            int viewRange = tViewEnd - tViewStart;
            if (tViewEnd > wave.getNumSamples()) {
                tViewEnd = wave.getNumSamples();
                tViewStart = tViewEnd - viewRange;
            }
            if (tViewStart < 0) {
                tViewStart = 0;
                tViewEnd = tViewStart + viewRange;
            }

            updateScrollRange();
            panelWave.Invalidate();
        }
        
        private void panelWave_MouseDown(object sender, MouseEventArgs e) {
            //e.X and e.Y are in client coordinates
            int time = (int)((double)e.X / panelWave.Width * (tViewEnd - tViewStart) + tViewStart);
            tSelStart = time;
            tSelEnd = time;
            tMouseDown = time;
            tMouseDrag = time;
        }

        private void panelWave_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) {
                return;
            }
            Point clientOrigin = new Point(0, 0);
            clientOrigin = panelWave.PointToScreen(clientOrigin);
            int tViewRange = tViewEnd - tViewStart;

            tMouseDrag = (int)((double)e.X * tViewRange / panelWave.Width + tViewStart);
            tSelStart = Math.Min(tMouseDown, tMouseDrag);
            tSelEnd = Math.Max(tMouseDown, tMouseDrag);

            //update drag-selection
            if (tMouseDrag > tViewEnd) {
                scrollWave(tMouseDrag - tViewEnd);
            } else if (tMouseDrag < tViewStart) {
                scrollWave(tMouseDrag - tViewStart);
            } else {
                panelWave.Invalidate();
            }
            panelWave.Update();
        }

        private void panelWave_MouseUp(object sender, MouseEventArgs e) {
            tSelStart = Math.Min(tMouseDown, tMouseDrag);
            tSelEnd = Math.Max(tMouseDown, tMouseDrag);
            updateSelection(Math.Min(tMouseDown, tMouseDrag), Math.Max(tMouseDown, tMouseDrag));
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
