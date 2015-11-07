using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Comp3931_Project_JoePelz {

    public delegate void SelectionChangedEventHandler(object sender, EventArgs e);

    class WavePanel : System.Windows.Forms.Panel {
        private float WavePanelHeight;
        private float WavePanelWidth;
        private double[][] samples;
        private int drawHeight;
        private int tViewStart;
        private int tViewRange;
        private int tSelStart;
        private int tSelEnd;
        private int tMouseDown;
        private int tMouseDrag;
        private HScrollBar scrollerWave;
        public event SelectionChangedEventHandler SelChanged;

        public WavePanel() : base() {
            InitializeComponent();
            this.DoubleBuffered = true;

            samples = new double[1][];
            samples[0] = new double[128];
            updateView(0, 1000);
            updateSelection(0, 0);
            WavePanelHeight = Height;
            WavePanelWidth = Width;
            updateScrollRange();
        }
        
        protected virtual void OnSelChanged(EventArgs e) {
            if (SelChanged != null)
                SelChanged(this, e);
        }

        public bool HotKeys(ref Message message, Keys keys) {
            switch (keys) {
                case Keys.Home | Keys.Shift:
                    updateSelection(0, tSelEnd);
                    return true;
                case Keys.Home:
                    updateSelection(0, 0);
                    return true;
                case Keys.End | Keys.Shift:
                    updateSelection(tSelStart, samples[0].Length - 1);
                    return true;
                case Keys.End:
                    updateSelection(samples[0].Length - 1, samples[0].Length - 1);
                    return true;
                case Keys.F:
                    updateView(0, samples[0].Length);
                    Invalidate();
                    return true;
            }
            return false;
        }

        private void InitializeComponent() {
            this.scrollerWave = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();
            // 
            // scrollerWave
            // 
            this.scrollerWave.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.scrollerWave.Location = new System.Drawing.Point(0, 83);
            this.scrollerWave.Name = "scrollerWave";
            this.scrollerWave.Size = new System.Drawing.Size(200, 17);
            this.scrollerWave.TabIndex = 0;
            this.scrollerWave.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollerWave_Scroll);
            // 
            // WavePanel
            // 
            this.Controls.Add(this.scrollerWave);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelWave_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelWave_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelWave_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MouseWheel_Zoom);
            this.Resize += new System.EventHandler(this.panelWave_Resize);
            this.ResumeLayout(false);

        }

        private void panelWave_Resize(object sender, System.EventArgs e) {
            Graphics g = ((Control)sender).CreateGraphics();
            if (g.VisibleClipBounds.Height != WavePanelHeight)
                ((Control)sender).Invalidate();
            
            int newRange = (int)(tViewRange * (float)g.VisibleClipBounds.Width / (float)WavePanelWidth);
            updateView(tViewStart, newRange);
            WavePanelHeight = g.VisibleClipBounds.Height;
            WavePanelWidth = g.VisibleClipBounds.Width;
            updateScrollRange();
        }

        public void setSamples(double[][] data) {
            samples = data;
            updateScrollRange();
        }
        
        /*
        ==============   Painting the wave  ================
        */

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            scrollerWave.Invalidate();
            scrollerWave.Refresh();
            drawHeight = Height - scrollerWave.ClientRectangle.Height;
            Graphics g = e.Graphics;
            drawSelection(g);
            drawGrid(g);
            drawWave(g);
        }

        private void drawGrid(Graphics g) {
            RectangleF r = g.VisibleClipBounds;
            //x-axis line
            g.FillRectangle(Brushes.White, 0, drawHeight / 2 - 1, r.Width, 2);
        }

        private void drawSelection(Graphics g) {

            RectangleF client = g.VisibleClipBounds;
            RectangleF r = new RectangleF();
            r.Y = 0;
            r.Height = client.Height;
            r.X = Math.Max(0, (float)(tSelStart - tViewStart) / tViewRange * client.Width);
            r.Width = Math.Max(2, (float)(tSelEnd - tViewStart) / tViewRange * client.Width - r.X);
            g.FillRectangle(Brushes.DarkKhaki, r);
        }

        private void drawWave(Graphics g) {
            RectangleF r = g.VisibleClipBounds;
            r.Height = drawHeight;
            int numPoints = Math.Min(tViewRange, (int)Math.Ceiling(r.Width));
            if (numPoints <= 0) return;
            float timeStep = (float)(tViewRange) / numPoints;
            float xStep = r.Width / numPoints;
            Pen p = new Pen(Color.Yellow, 2);
            int index;

            //TODO: handle multiple channels

            //draw using DrawPolyLine
            PointF[] pathPoints = new PointF[numPoints + 2];
            pathPoints[0] = new PointF(-1, 0.5f * r.Height);
            pathPoints[numPoints + 1] = new PointF(r.Width, 0.5f * r.Height);
            for (int i = 1; i <= numPoints; i++) {
                index = (int)(Math.Round(i * timeStep + tViewStart));
                if (index >= 0 && index < samples[0].Length) {
                    pathPoints[i] = new PointF((float)(index - tViewStart) / tViewRange * r.Width, (1.0f - (float)(samples[0][index] * 0.5 + 0.5)) * r.Height);
                } else {
                    pathPoints[i] = new PointF(i * xStep, 0.5f * r.Height);
                }
            }
            g.DrawLines(p, pathPoints);

            p.Dispose();
        }
        
        /*
        ==============   Visible region controls  ================
        */

        private void updateScrollRange() {
            int scrollRange = samples[0].Length - tViewRange;
            if (scrollRange < 0) {
                scrollerWave.Maximum = 0;
                scrollerWave.Value = 0;
                scrollerWave.Enabled = false;
            } else {
                scrollerWave.Maximum = samples[0].Length - tViewRange;
                scrollerWave.Value = tViewStart;
                scrollerWave.Enabled = true;
            }
        }

        private void scrollWave(int scrollValue) {
            if (scrollerWave.Value + scrollValue > scrollerWave.Maximum) {
                scrollerWave.Value = scrollerWave.Maximum;
            } else if (scrollerWave.Value + scrollValue < scrollerWave.Minimum) {
                scrollerWave.Value = scrollerWave.Minimum;
            } else {
                scrollerWave.Value += scrollValue;
            }
            updateView(scrollerWave.Value, tViewRange);
            Invalidate();
        }

        private void scrollerWave_Scroll(object sender, ScrollEventArgs e) {
            scrollWave(0);
        }
        
        private void updateView(int low, int range) {
            if (range > samples[0].Length) {
                range = samples[0].Length;
            }

            if (low < 0) {
                tViewStart = 0;
            } else if (low > samples[0].Length) {
                tViewStart = samples[0].Length - range;
            } else {
                tViewStart = low;
            }
            tViewRange = range;
            updateScrollRange();
            Invalidate();
        }
        
        private void MouseWheel_Zoom(object sender, MouseEventArgs e) {
            float zoomAmount = e.Delta / 120.0f;
            if (zoomAmount < 0 && tViewRange >= samples[0].Length) {
                return;
            } else if (zoomAmount > 0 && tViewRange <= 40) {
                return;
            }
            int tMousePoint = (int)((double)e.X / this.Width * tViewRange + tViewStart);
            int less = tMousePoint - tViewStart;
            less = (int)(less * Math.Pow(0.83f, zoomAmount));
            int newRange = (int)(tViewRange * Math.Pow(0.83f, zoomAmount));

            updateView(tMousePoint - less, newRange);
            updateScrollRange();
        }
        
        /*
        ==============   Selection, and Selecting  ================
        */

        private void panelWave_MouseDown(object sender, MouseEventArgs e) {
            //e.X and e.Y are in client coordinates
            int time = (int)((double)e.X / this.Width * tViewRange + tViewStart);
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
            clientOrigin = this.PointToScreen(clientOrigin);

            tMouseDrag = (int)((double)e.X * tViewRange / Width + tViewStart);
            tSelStart = Math.Min(tMouseDown, tMouseDrag);
            tSelEnd = Math.Max(tMouseDown, tMouseDrag);

            //update drag-selection
            if (tMouseDrag > (tViewStart + tViewRange)) {
                scrollWave(tMouseDrag - (tViewStart + tViewRange));
            } else if (tMouseDrag < tViewStart) {
                scrollWave(tMouseDrag - tViewStart);
            } else {
                Invalidate();
            }
            Update();
        }

        private void panelWave_MouseUp(object sender, MouseEventArgs e) {
            tSelStart = Math.Min(tMouseDown, tMouseDrag);
            tSelEnd = Math.Max(tMouseDown, tMouseDrag);
            updateSelection(Math.Min(tMouseDown, tMouseDrag), Math.Max(tMouseDown, tMouseDrag));
        }

        private void updateSelection(int low, int high) {
            if (low < 0)
                tSelStart = 0;
            else if (low > samples[0].Length)
                tSelStart = samples[0].Length - 1;
            else
                tSelStart = low;
            if (high < 0)
                tSelEnd = 0;
            else if (high > samples[0].Length)
                tSelEnd = samples[0].Length - 1;
            else
                tSelEnd = high;

            OnSelChanged(EventArgs.Empty);
            Invalidate();
        }

        public int SelectionStart {
            get { return tSelStart; }
            set { updateSelection(value, tSelEnd); }
        }
        public int SelectionEnd {
            get { return tSelEnd; }
            set { updateSelection(tSelStart, value); }
        }
    }
}
