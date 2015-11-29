using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Comp3931_Project_JoePelz {

    public delegate void FreqSelChangedEventHandler(object sender, EventArgs e);
    
    public partial class FourierPanel : System.Windows.Forms.Panel {
        private int drawHeight;
        private int drawBottomMargin;
        private Complex[][] DFT;
        private int fourierN;
        private int sampleRate;

        private int fMouseDown = 0;
        private int fMouseDrag = 0;
        private int fSelStart = 0;
        private int fSelEnd = 0;

        public event FreqSelChangedEventHandler SelChanged;
        public event ReportEventHandler Report;

        public FourierPanel() {
            InitializeComponent();
            DoubleBuffered = true;
            fourierN = 8;
            DFT = new Complex[1][];
            DFT[0] = new Complex[fourierN];
            for (int i = 0; i < fourierN; i++) {
                DFT[0][i] = new Complex(0, 0);
            }
        }

        public FourierPanel(IContainer container) {
            container.Add(this);

            InitializeComponent();
            DoubleBuffered = true;
            fourierN = 8;
            DFT = new Complex[1][];
            DFT[0] = new Complex[fourierN];
            for (int i = 0; i < fourierN; i++) {
                DFT[0][i] = new Complex(0, 0);
            }
            sampleRate = 44100;
        }

        /*
        ==============   Event Handlers  ================
        */

        protected virtual void OnSelChanged(EventArgs e) {
            if (SelChanged != null)
                SelChanged(this, e);
        }

        protected virtual void OnReport(ReportEventArgs e) {
            if (Report != null)
                Report(this, e);
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            drawHeight = Height - drawBottomMargin;
            Invalidate();
        }

        /*
        ==============   Properties  ================
        */

        public Complex[][] Fourier {
            set {
                DFT = value;
                fourierN = value[0].Length;
            }
        }
        
        public int SampleRate {
            set { sampleRate = value; }
        }

        public int SelectionStart {
            get { return fSelStart; }
            set { updateSelection(value, fSelEnd); }
        }
        public int SelectionEnd {
            get { return fSelEnd; }
            set { updateSelection(fSelStart, value); }
        }

        /*
        ==============   Painting the Frequency Domain  ================
        */

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            drawFreqSelection(g);
            drawAliasAxis(g);
            drawFourier(g);
        }

        private void drawFourier(Graphics g) {
            //TODO: handle multiple channels in fourier
            int start, end;
            float value;
            if (fourierN < Width) {
                //more pixels than fourier steps
                for (int i = 0; i < fourierN; i++) {
                    start = (int)(i / (double)fourierN * Width);
                    end = (int)((i + 1) / (double)fourierN * Width);
                    //value = (float)Math.Sqrt(DFT[0][i].magnitude() / (fourierN / 2)) * drawHeight;
                    value = (float)(DFT[0][i].magnitude() / (fourierN / 2) * 2) * drawHeight;
                    g.FillRectangle(Brushes.Brown, start, drawHeight - value, end - start, value);
                }
            } else {
                //more bars than pixels wide
                for (int i = 0; i < Width; i++) {
                    start = (int)((float)i / (float)Width * (fourierN - 1));
                    end = (int)((float)(i + 1) / (float)Width * (fourierN - 1));
                    value = 0;
                    for (int j = start; j < end; j++) {
                        //value = Math.Max(value, (float)Math.Sqrt(DFT[0][j].magnitude() / (fourierN / 2)));
                        value = Math.Max(value, (float)(DFT[0][j].magnitude() / (fourierN / 2) * 2));
                    }
                    value *= drawHeight;
                    g.DrawLine(Pens.Brown, i, drawHeight, i, drawHeight - Math.Abs(value));
                }
            }
        }

        private void drawFreqSelection(Graphics g) {
            if (fSelStart == fSelEnd) {
                return;
            }
            RectangleF r = new RectangleF();
            r.Y = 0;
            r.Height = drawHeight;
            r.X = Math.Max(1.0f / fourierN * Width, (float)(fSelStart) / fourierN * Width);
            r.Width = Math.Max(2, (float)(fSelEnd + 1) / fourierN * Width - r.X);
            g.FillRectangle(Brushes.PaleGoldenrod, r);
            r.X = Width - (Math.Max(0, (float)(fSelStart - 1) / fourierN * Width)) - r.Width;
            g.FillRectangle(Brushes.PaleGoldenrod, r);
        }

        private void drawAliasAxis(Graphics g) {
            Pen dashed = new Pen(Color.DarkSeaGreen, Math.Max(2, g.VisibleClipBounds.Width / fourierN / 6));
            float[] dashValues = { 3, 2 };
            dashed.DashPattern = dashValues;
            float mid = g.VisibleClipBounds.Width / 2.0f + (g.VisibleClipBounds.Width / fourierN) / 2.0f;
            g.DrawLine(dashed, mid, 0.0f, mid, drawHeight);
        }

        /*
        ==============   Selecting Frequencies  ================
        */

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Right) {
                return;
            }
            
            float position = (float)e.X / Width;
            int index = (int)(position * DFT[0].Length);
            updateSelection(index, index);
            fMouseDown = index;
            fMouseDrag = index;
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Right) {
                return;
            }
            float position = (float)e.X / Width;
            int index = (int)(position * DFT[0].Length);
            fMouseDrag = index;
            if (fMouseDrag == fMouseDown) {
                updateSelection(0, 0);
            } else {
                updateSelection(Math.Min(fMouseDown, fMouseDrag), Math.Max(fMouseDown, fMouseDrag));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            float position = (float)e.X / Width;
            int index = (int)(position * DFT[0].Length);
            int loval = (int)((float)index / DFT[0].Length * sampleRate);
            String info;
            
            if (index >= 0 && index <= fourierN - 1) {
                if (loval >= sampleRate / 2) {
                    lblDFTFreq.Text = String.Format("Frequency: {0}Hz [Aliased]", loval);
                } else {
                    lblDFTFreq.Text = String.Format("Frequency: {0}Hz", loval);
                }
                double amplitude = DFT[0][index].magnitude() / (fourierN / 2);
                double phase = 0;
                if (amplitude > 0.00001)
                    phase = DFT[0][index].angle() / Math.PI;
                if (e.Button != MouseButtons.Left) {
                    info = String.Format("Frequency: {0}Hz; Amplitude: {1:0.00}; Phase: {2:0.000}pi*rad", loval, amplitude, phase);
                    OnReport(new ReportEventArgs(info));
                    return;
                }
            }

            fMouseDrag = index;
            updateSelection(Math.Min(fMouseDown, fMouseDrag), Math.Max(fMouseDown, fMouseDrag));

            loval = (int)((float)fSelStart / DFT[0].Length * sampleRate);
            int hival = (int)((float)(fSelEnd + 1) / DFT[0].Length * sampleRate);
            info = String.Format("Selected: {0}Hz to {1}Hz", loval, hival);
            OnReport(new ReportEventArgs(info));
        }

        private void updateSelection(int low, int high) {
            fSelStart = Math.Max(0, low);
            fSelEnd = Math.Min(fourierN - 1, high);
            Invalidate();
            OnSelChanged(EventArgs.Empty);
        }
        
    }
}
