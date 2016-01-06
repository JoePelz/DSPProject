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

    /// <summary>
    /// delegate to handle my custom "report" event.  (Report events get posted to the rightmost end of the status bar)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ReportEventHandler(object sender, ReportEventArgs e);

    public partial class WaveForm : Form {
        private WaveFile wave;
        private WavePlayer player2;
        private DSP_Window sampleWindowing = DSP_Window.pass;
        private PlaybackStatus statusPlayback = PlaybackStatus.Stopped;
        private int fourierN = 882;
        private bool invalidPlayer = true;
        private int tSelStart;
        private int tSelEnd;
        private int fSelStart = 0;
        private int fSelEnd = 0;

        private Mixer parent = null;

        /// <summary>
        /// Constructor:  can only create a WaveForm with a WaveFile supplied.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="wave"></param>
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
        
        /// <summary>
        /// Handle hotkeys here. Overrides existing function, but calls super if keys are unhandled.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Handle messages from the playback thread. (updates current playback status.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m) {
            if (m.Msg == WinmmHook.WM_USER + 1) {
                PlaybackStatus status = (PlaybackStatus)(int)m.WParam;
                statusPlayback = status;
                parent.playbackUpdate(status);
            }
            base.WndProc(ref m);
        }
        /*
        ================  Accessors/Mutators  ================
        */

        public int SampleRate {
            get { return wave.sampleRate; }
        }

        public int BitDepth {
            get { return wave.bitDepth; }
        }

        public int Channels {
            get { return wave.channels; }
        }

        public PlaybackStatus State {
            get { return statusPlayback; }
        }

        public void setPath(String path) {
            wave.setPath(path);
            this.Text = wave.getName();
        }
        
        public void changeSampleRate(int newRate) {
            if (newRate == wave.sampleRate) {
                return;
            }

            for (int channel = 0; channel < wave.channels; channel++) {
                wave.samples[channel] = DSP.resample(ref wave.samples[channel], wave.sampleRate, newRate);
            }
            wave.sampleRate = newRate;
            updateStatusBar();
            calculateDFT();
            panelWave.setSamples(wave.samples);
            panelWave.Invalidate();
        }

        public void changeBitRate(short newBitDepth) {
            wave.bitDepth = newBitDepth;
            updateStatusBar();
        }

        public void changeChannels(short nChannels) {
            wave.samples = DSP.matchChannels(wave.samples, nChannels);
            wave.channels = nChannels;
            updateStatusBar();
            panelWave.setSamples(wave.samples);
            panelWave.Invalidate();
        }
        
        /*
        ================  Interaction Callbacks  ================
        */

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
            if (player2 != null) {
                player2.PlaybackStop();
            }
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
            panelWave.setSamples(wave.samples);
            waveStop();
            invalidPlayer = true;
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
            //match number of channels
            data.samples = DSP.matchChannels(data.samples, wave.channels);
            data.channels = wave.channels;

            //match sampling rate
            for (int channel = 0; channel < data.channels; channel++) {
                data.samples[channel] = DSP.resample(ref data.samples[channel], data.sampleRate, wave.sampleRate);
            }
            data.sampleRate = wave.sampleRate;

            wave.cutSelection(tSelStart, tSelEnd);
            wave.pasteSelection(tSelStart, data);
            updateReport(data.getNumSamples() + " samples pasted from the clipboard!");
            panelWave.SelectionEnd = tSelStart + data.getNumSamples();
            panelWave.setSamples(wave.samples);
            waveStop();
            invalidPlayer = true;
        }

        /*
        ================  Other Callbacks  ================
        */

        private void WaveForm_GotFocus(object sender, EventArgs e) {
            this.parent.setActiveWindow(this);
        }

        private void WaveForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (player2 != null) {
                player2.Dispose();
            }
            parent.childDied(this);
        }

        private void windowingToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem src = (ToolStripMenuItem) sender;
            sampleWindowing = (DSP_Window) src.Tag;
            calculateDFT();
        }

        private void filterToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem src = (ToolStripMenuItem) sender;
            filterSelectedFrequencies((DSP_FILTER) src.Tag);
        }


        /*
        ================  UI Update Helpers  ================
        */

        private void calculateDFT() {
            Complex[][] DFT = new Complex[wave.channels][];
            double[] samples = new double[fourierN];

            //This is for display, so it (intentionally) grabs the first channel only
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
            }
            //if DSP.WindowPassthrough or unknown,
            //  pass through unchanged

            DFT[0] = DSP.DFT(ref samples);
            panelFourier.SampleRate = wave.sampleRate;
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
            waveStop();
            invalidPlayer = true;
        }

        private void updateFreqSel(Object sender, EventArgs e) {
            fSelStart = panelFourier.SelectionStart;
            fSelEnd = panelFourier.SelectionEnd;
        }

        /*
        ================  Miscellaneous  ================
        */

        public void filterSelectedFrequencies(DSP_FILTER method = DSP_FILTER.convolution) {
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
            
            double criticalPoint = 0;
            if (method == DSP_FILTER.IIRLowpass)
                criticalPoint = fSelStart * SampleRate / fourierN;
            else
                criticalPoint = fSelEnd * SampleRate / fourierN;

            for (int channel = 0; channel < wave.channels; channel++) {
                switch (method) {
                    case DSP_FILTER.convolution:
                        wave.samples[channel] = DSP.convolveFilter(ref wave.samples[channel], filter);
                        break;
                    case DSP_FILTER.DFT:
                        wave.samples[channel] = DSP.dftFilter(wave.samples[channel], filter);
                        break;
                    case DSP_FILTER.IIRLowpass: //low pass
                        // LowPass IIRFilter  ( samples, 1x freq, 0x freq, 0, SampleRate );
                        wave.samples[channel] = DSP.IIRFilter(wave.samples[channel], Math.Min(criticalPoint + 5000, SampleRate / 2), Math.Max(0, criticalPoint - 5000), 0, SampleRate);
                        break;
                    case DSP_FILTER.IIRHighpass: //high pass
                        // HighPass IIRFilter  ( samples, 0x freq, 1x freq, 1, SampleRate );
                        wave.samples[channel] = DSP.IIRFilter(wave.samples[channel], Math.Max(0, criticalPoint - 7000), Math.Min(criticalPoint + 3000, SampleRate/2), 1, SampleRate);
                        break;
                    default:
                        MessageBox.Show("something went wrong.");
                        break;
                }
            }
            panelWave.setSamples(wave.samples);
            panelWave.Invalidate();
            calculateDFT();
            invalidPlayer = true;
        }

        public void applyFX(DSP_FX effect, object[] args) {
            int startIndex = Math.Max(tSelStart, 0);
            int endIndex;
            if (tSelEnd != tSelStart) {
                endIndex = tSelEnd;
            } else {
                endIndex = wave.getNumSamples();
            }

            WaveFile data = wave.cutSelection(startIndex, endIndex);
            DSP.ApplyFX(effect, args, ref data);
            wave.pasteSelection(startIndex, data.samples);
            panelWave.setSamples(wave.samples);
            panelWave.Invalidate();
            invalidPlayer = true;
        }

        /// <summary>
        /// Event handler for my custom "Report" event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void WaveForm_Report(Object sender, ReportEventArgs e) {
            updateReport(e.report);
        }
    }

    /// <summary>
    /// Custom report event, carries a message to display.
    /// </summary>
    public class ReportEventArgs : EventArgs {
        public String report;
        public ReportEventArgs(String message) : base() {
            report = message;
        }
    }
}
