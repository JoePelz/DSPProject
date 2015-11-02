using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Comp3931_Project_JoePelz {
    class WavePanel : System.Windows.Forms.Panel {
        private float WavePanelHeight;
        private float WavePanelWidth;
        private double[][] samples;
        private int drawHeight;
        private int tViewStart;
        private int tViewEnd;
        private int tSelStart;
        private int tSelEnd;
        private int tMouseDown;
        private int tMouseDrag;
        private HScrollBar scrollerWave;
        private WaveForm parent;
        
        public WavePanel(WaveForm parent) : base() {
            InitializeComponent();
            samples = new double[1][];
            samples[0] = new double[2];
            //drawHeight = 100;
            tSelStart = 50;
            tSelEnd = 75;
            tViewStart = 0;
            tViewEnd = 1000;
            WavePanelHeight = Height;
            WavePanelWidth = Width;
            this.parent = parent;
            updateScrollRange();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys) {
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
                    focusAll();
                    Invalidate();
                    return true;
            }

            // run base implementation
            return base.ProcessCmdKey(ref message, keys);
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
            this.DoubleBuffered = true;
            this.ResumeLayout(false);

        }

        private void panelWave_Resize(object sender, System.EventArgs e) {
            Graphics g = ((Control)sender).CreateGraphics();
            if (g.VisibleClipBounds.Height != WavePanelHeight)
                ((Control)sender).Invalidate();

            int oldRange = tViewEnd - tViewStart;
            int newRange = (int)(oldRange * (float)g.VisibleClipBounds.Width / (float)WavePanelWidth);
            tViewEnd = Math.Min(samples[0].Length, tViewStart + newRange);
            tViewStart = tViewEnd - newRange;
            if (tViewStart < 0) {
                tViewStart = 0;
                tViewEnd = tViewStart + newRange;
            }
            WavePanelHeight = g.VisibleClipBounds.Height;
            WavePanelWidth = g.VisibleClipBounds.Width;
            updateScrollRange();
            Invalidate();
        }

        public void setSamples(double[][] data) {
            samples = data;
            updateScrollRange();
        }

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
            int viewRange = tViewEnd - tViewStart;
            r.Y = 0;
            r.Height = client.Height;
            r.X = Math.Max(0, (float)(tSelStart - tViewStart) / viewRange * client.Width);
            r.Width = Math.Max(2, (float)(tSelEnd - tViewStart) / viewRange * client.Width - r.X);
            g.FillRectangle(Brushes.DarkKhaki, r);
        }

        private void drawWave(Graphics g) {
            RectangleF r = g.VisibleClipBounds;
            r.Height = drawHeight;
            int viewRange = tViewEnd - tViewStart;
            int numPoints = Math.Min(viewRange, (int)Math.Ceiling(r.Width));
            if (numPoints <= 0) return;
            float timeStep = (float)(viewRange) / numPoints;
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
                    pathPoints[i] = new PointF((float)(index - tViewStart) / viewRange * r.Width, (1.0f - (float)(samples[0][index] * 0.5 + 0.5)) * r.Height);
                } else {
                    pathPoints[i] = new PointF(i * xStep, 0.5f * r.Height);
                }
            }
            g.DrawLines(p, pathPoints);

            p.Dispose();
        }
        
        private void updateScrollRange() {
            int viewRange = tViewEnd - tViewStart;
            int scrollRange = samples[0].Length - viewRange;
            if (scrollRange < 0) {
                scrollerWave.Maximum = 0;
                scrollerWave.Value = 0;
                scrollerWave.Enabled = false;
            } else {
                scrollerWave.Maximum = samples[0].Length - viewRange;
                scrollerWave.Value = tViewStart;
                scrollerWave.Enabled = true;
            }
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
            Invalidate();
        }

        private void scrollerWave_Scroll(object sender, ScrollEventArgs e) {
            scrollWave(0);
        }

        public void focusAll() {
            updateView(0, samples[0].Length);
            updateScrollRange();
        }

        private void updateView(int low, int high) {
            if (low < 0)
                tViewStart = 0;
            else if (low > samples[0].Length)
                tViewStart = samples[0].Length - 1;
            else
                tViewStart = low;
            if (high < 0)
                tViewEnd = 0;
            else if (high > samples[0].Length)
                tViewEnd = samples[0].Length - 1;
            else
                tViewEnd = high;

            Invalidate();
        }

        private void MouseWheel_Zoom(object sender, MouseEventArgs e) {
            float scrollAmount = e.Delta / 120.0f;
            if (scrollAmount < 0 && tViewEnd > samples[0].Length) {
                return;
            } else if (scrollAmount > 0 && (tViewEnd - tViewStart) <= 10) {
                return;
            }
            int mousePoint = (int)((double)e.X / this.Width * (tViewEnd - tViewStart) + tViewStart);
            int less = mousePoint - tViewStart;
            int more = tViewEnd - mousePoint;
            less = (int)(less * Math.Pow(0.83f, scrollAmount));
            more = (int)(more * Math.Pow(0.83f, scrollAmount));

            tViewEnd = mousePoint + more;
            tViewStart = mousePoint - less;
            
            //avoid scrolling off the ends, prefer to block at sample 0.
            int viewRange = tViewEnd - tViewStart;
            if (tViewEnd > samples[0].Length) {
                tViewEnd = samples[0].Length;
                tViewStart = tViewEnd - viewRange;
            }
            if (tViewStart < 0) {
                tViewStart = 0;
                tViewEnd = tViewStart + viewRange;
            }

            updateScrollRange();
            Invalidate();
        }

        private void panelWave_MouseDown(object sender, MouseEventArgs e) {
            //e.X and e.Y are in client coordinates
            int time = (int)((double)e.X / this.Width * (tViewEnd - tViewStart) + tViewStart);
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
            int tViewRange = tViewEnd - tViewStart;

            tMouseDrag = (int)((double)e.X * tViewRange / Width + tViewStart);
            tSelStart = Math.Min(tMouseDown, tMouseDrag);
            tSelEnd = Math.Max(tMouseDown, tMouseDrag);

            //update drag-selection
            if (tMouseDrag > tViewEnd) {
                scrollWave(tMouseDrag - tViewEnd);
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

            //TODO: send message to parent that selection changed.
            parent.updateSelection(tSelStart, tSelEnd);
            Invalidate();
        }

        public void getSelection(out int low, out int high) {
            low = tSelStart;
            high = tSelEnd;
        }
    }
}
