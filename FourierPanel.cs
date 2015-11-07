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
    public partial class FourierPanel : System.Windows.Forms.Panel {
        private int drawHeight;
        private Complex[][] DFT;
        private int fourierN;

        public FourierPanel() {
            InitializeComponent();
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
            fourierN = 8;
            DFT = new Complex[1][];
            DFT[0] = new Complex[fourierN];
            for (int i = 0; i < fourierN; i++) {
                DFT[0][i] = new Complex(0, 0);
            }
        }

        public Complex[][] Fourier {
            set {
                DFT = value;
                fourierN = value[0].Length;
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics g = e.Graphics;
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
                    value = (float)Math.Sqrt(DFT[0][i].magnitude() / (fourierN / 2)) * Height;
                    g.FillRectangle(Brushes.Brown, start, Height - value, end - start, value);
                }
            } else {
                //more bars than pixels wide
                for (int i = 0; i < Width; i++) {
                    start = (int)((float)i / (float)Width * (fourierN - 1));
                    end = (int)((float)(i + 1) / (float)Width * (fourierN - 1));
                    value = 0;
                    for (int j = start; j < end; j++) {
                        value = Math.Max(value, (float)Math.Sqrt(DFT[0][j].magnitude() / (fourierN / 2)));
                    }
                    value *= Height;
                    g.DrawLine(Pens.Brown, i, Height, i, Height - Math.Abs(value));
                }
            }
        }
    }
}
