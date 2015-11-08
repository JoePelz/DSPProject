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

    public delegate void ReportEventHandler(object sender, ReportEventArgs e);

    public partial class WaveForm : Form {
        private WaveFile wave;
        private WavePlayer player2;
        private Complex[][] DFT;
        private DSP_Window sampleWindowing = DSP_Window.pass;
        private int fourierN = 882;
        private bool invalidPlayer = true;
        private int tSelStart;
        private int tSelEnd;
        private int fSelStart = 0;
        private int fSelEnd = 0;

        private Mixer parent = null;

        public WaveForm(Mixer parent, WaveFile wave) {
            this.wave = wave;
            this.parent = parent;
            InitializeComponent();
            panelFourier.setBottomMargin(statusBar.Height);
            this.Text = wave.getName();

            updateStatusBar();
            calculateDFT();
            panelWave.setSamples(wave.samples);
            panelFourier.SampleRate = wave.sampleRate;
        }
        
        protected override bool ProcessCmdKey(ref Message message, Keys keys) {
            switch (keys) {
                case Keys.C | Keys.Control:
                    copySelection();
                    return true;
                case Keys.V | Keys.Control:
                    pasteAtSelection();
                    return true;
                case Keys.X | Keys.Control:
                    cutSelection();
                    return true;
                case Keys.Space:
                    wavePlayPause();
                    return true;
                case Keys.Delete:
                    filterSelectedFrequencies();
                    return true;
            }
            if (panelWave.HotKeys(ref message, keys)) {
                return true;
            }

            // run base implementation
            return base.ProcessCmdKey(ref message, keys);
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == WinmmHook.WM_USER + 1) {
                PlaybackStatus status = (PlaybackStatus)(int)m.WParam;
                updatePlaybackStatus(status);
            }
            base.WndProc(ref m);
        }

        public void wavePlayPause() {
            if (player2 == null) {
                player2 = new WavePlayer(this);
            }

            if (invalidPlayer) {
                if (tSelEnd == tSelStart) {
                    player2.setWave(wave.copySelection(tSelStart, wave.getNumSamples()));
                } else {
                    player2.setWave(wave.copySelection(tSelStart, tSelEnd));
                }
                invalidPlayer = false;
            }

            if (player2.Playing) {
                player2.PlaybackPause();
            } else {
                player2.PlaybackStart();
            }
        }

        public void waveStop() {
            player2.PlaybackStop();
        }

        public void updatePlaybackStatus(PlaybackStatus update) {
            parent.playbackUpdate(update);
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
            panelFourier.Fourier = DFT;
            panelFourier.Invalidate();
        }

        private void updateStatusBar() {
            statusSampling.Text = String.Format("Sampling Rate: {0} Hz", wave.sampleRate);
            statusBits.Text = String.Format("Depth: {0}-bit", wave.bitDepth);
            statusLength.Text = String.Format("Length: {0:0.000}s ({1} samples)", (double)wave.getNumSamples() / wave.sampleRate, wave.getNumSamples());
            statusSelection.Text = String.Format("Selected: {0:0.000} seconds ({1}..{2})", (tSelEnd - tSelStart + 1) / (double)wave.sampleRate, tSelStart, tSelEnd);
        }

        public void updateReport(String msg) {
            statusReport.Text = msg;
        }

        private void updateSelection(Object sender, EventArgs e) {
            tSelStart = panelWave.SelectionStart;
            tSelEnd = panelWave.SelectionEnd;
            calculateDFT();
            panelFourier.Invalidate();
            updateStatusBar();
            invalidPlayer = true;
        }

        private void updateFreqSel(Object sender, EventArgs e) {
            fSelStart = panelFourier.SelectionStart;
            fSelEnd = panelFourier.SelectionEnd;
        }

        private void WaveForm_GotFocus(object sender, EventArgs e) {
            this.parent.setActiveWindow(this);
        }

        private void WaveForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (player2 != null) {
                player2.Dispose();
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
                updateReport((tSelEnd - tSelStart) + " samples copied to clipboard!");
            } else {
                MessageBox.Show("No samples selected to copy!");
            }
        }

        public void cutSelection() {
            int before = wave.getNumSamples();
            WaveFile cut = wave.cutSelection(tSelStart, tSelEnd);
            if (cut != null) {
                Clipboard.SetData("WaveFile", cut);
                updateReport((tSelEnd - tSelStart) + " samples cut to clipboard!");
            } else {
                MessageBox.Show("No samples selected to cut!");
            }
            int after = wave.getNumSamples();
            panelWave.SelectionEnd = tSelStart;
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
            updateReport(data.getNumSamples() + " samples pasted from the clipboard!");
            panelWave.SelectionEnd = tSelStart + data.getNumSamples();
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

        public void WaveForm_Report(Object sender, ReportEventArgs e) {
            updateReport(e.report);
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

    public class ReportEventArgs : EventArgs {
        public String report;
        public ReportEventArgs(String message) : base() {
            report = message;
        }
    }
}
