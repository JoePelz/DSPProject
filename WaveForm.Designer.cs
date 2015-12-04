namespace Comp3931_Project_JoePelz
{
    partial class WaveForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelFourier = new Comp3931_Project_JoePelz.FourierPanel(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.windowingModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.passthroughToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cosineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blackmanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convolutionFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dFTFilteringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iIRLowpassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iIRHighpassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusSampling = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBits = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLength = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusSelection = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusReport = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelWave = new Comp3931_Project_JoePelz.WavePanel();
            this.panelFourier.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 160);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1000, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // panelFourier
            // 
            this.panelFourier.AutoScroll = true;
            this.panelFourier.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelFourier.ContextMenuStrip = this.contextMenuStrip1;
            this.panelFourier.Controls.Add(this.statusBar);
            this.panelFourier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFourier.Location = new System.Drawing.Point(0, 163);
            this.panelFourier.Name = "panelFourier";
            this.panelFourier.SelectionEnd = 0;
            this.panelFourier.SelectionStart = 0;
            this.panelFourier.Size = new System.Drawing.Size(1000, 98);
            this.panelFourier.TabIndex = 3;
            this.panelFourier.SelChanged += new Comp3931_Project_JoePelz.FreqSelChangedEventHandler(this.updateFreqSel);
            this.panelFourier.Report += new Comp3931_Project_JoePelz.ReportEventHandler(this.WaveForm_Report);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowingModeToolStripMenuItem,
            this.filterToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(170, 70);
            // 
            // windowingModeToolStripMenuItem
            // 
            this.windowingModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.passthroughToolStripMenuItem,
            this.triangleToolStripMenuItem,
            this.cosineToolStripMenuItem,
            this.blackmanToolStripMenuItem});
            this.windowingModeToolStripMenuItem.Name = "windowingModeToolStripMenuItem";
            this.windowingModeToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.windowingModeToolStripMenuItem.Text = "Windowing Mode";
            // 
            // passthroughToolStripMenuItem
            // 
            this.passthroughToolStripMenuItem.Name = "passthroughToolStripMenuItem";
            this.passthroughToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.passthroughToolStripMenuItem.Tag = Comp3931_Project_JoePelz.DSP_Window.pass;
            this.passthroughToolStripMenuItem.Text = "Passthrough";
            this.passthroughToolStripMenuItem.Click += new System.EventHandler(this.windowingToolStripMenuItem_Click);
            // 
            // triangleToolStripMenuItem
            // 
            this.triangleToolStripMenuItem.Name = "triangleToolStripMenuItem";
            this.triangleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.triangleToolStripMenuItem.Tag = Comp3931_Project_JoePelz.DSP_Window.triangle;
            this.triangleToolStripMenuItem.Text = "Triangle";
            this.triangleToolStripMenuItem.Click += new System.EventHandler(this.windowingToolStripMenuItem_Click);
            // 
            // cosineToolStripMenuItem
            // 
            this.cosineToolStripMenuItem.Name = "cosineToolStripMenuItem";
            this.cosineToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cosineToolStripMenuItem.Tag = Comp3931_Project_JoePelz.DSP_Window.cosine;
            this.cosineToolStripMenuItem.Text = "Cosine";
            this.cosineToolStripMenuItem.Click += new System.EventHandler(this.windowingToolStripMenuItem_Click);
            // 
            // blackmanToolStripMenuItem
            // 
            this.blackmanToolStripMenuItem.Name = "blackmanToolStripMenuItem";
            this.blackmanToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.blackmanToolStripMenuItem.Tag = Comp3931_Project_JoePelz.DSP_Window.blackman;
            this.blackmanToolStripMenuItem.Text = "Blackman";
            this.blackmanToolStripMenuItem.Click += new System.EventHandler(this.windowingToolStripMenuItem_Click);
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convolutionFilterToolStripMenuItem,
            this.dFTFilteringToolStripMenuItem,
            this.iIRLowpassToolStripMenuItem,
            this.iIRHighpassToolStripMenuItem});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.filterToolStripMenuItem.Text = "Filter";
            // 
            // convolutionFilterToolStripMenuItem
            // 
            this.convolutionFilterToolStripMenuItem.Name = "convolutionFilterToolStripMenuItem";
            this.convolutionFilterToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.convolutionFilterToolStripMenuItem.Tag = DSP_FILTER.convolution;
            this.convolutionFilterToolStripMenuItem.Text = "Convolution Filter";
            this.convolutionFilterToolStripMenuItem.Click += new System.EventHandler(this.filterToolStripMenuItem_Click);
            // 
            // dFTFilteringToolStripMenuItem
            // 
            this.dFTFilteringToolStripMenuItem.Name = "dFTFilteringToolStripMenuItem";
            this.dFTFilteringToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.dFTFilteringToolStripMenuItem.Tag = DSP_FILTER.DFT;
            this.dFTFilteringToolStripMenuItem.Text = "DFT Filter";
            this.dFTFilteringToolStripMenuItem.Click += new System.EventHandler(this.filterToolStripMenuItem_Click);
            // 
            // iIRLowpassToolStripMenuItem
            // 
            this.iIRLowpassToolStripMenuItem.Name = "iIRLowpassToolStripMenuItem";
            this.iIRLowpassToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.iIRLowpassToolStripMenuItem.Tag = DSP_FILTER.IIRLowpass;
            this.iIRLowpassToolStripMenuItem.Text = "IIR Lowpass";
            this.iIRLowpassToolStripMenuItem.Click += new System.EventHandler(this.filterToolStripMenuItem_Click);
            // 
            // iIRHighpassToolStripMenuItem
            // 
            this.iIRHighpassToolStripMenuItem.Name = "iIRHighpassToolStripMenuItem";
            this.iIRHighpassToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.iIRHighpassToolStripMenuItem.Tag = DSP_FILTER.IIRHighpass;
            this.iIRHighpassToolStripMenuItem.Text = "IIR Highpass";
            this.iIRHighpassToolStripMenuItem.Click += new System.EventHandler(this.filterToolStripMenuItem_Click);
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusSampling,
            this.statusBits,
            this.statusLength,
            this.statusSelection,
            this.statusReport});
            this.statusBar.Location = new System.Drawing.Point(0, 74);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1000, 24);
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "statusStrip1";
            // 
            // statusSampling
            // 
            this.statusSampling.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusSampling.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.statusSampling.Name = "statusSampling";
            this.statusSampling.Size = new System.Drawing.Size(96, 19);
            this.statusSampling.Text = "status_sampling";
            // 
            // statusBits
            // 
            this.statusBits.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusBits.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.statusBits.Name = "statusBits";
            this.statusBits.Size = new System.Drawing.Size(122, 19);
            this.statusBits.Text = "toolStripStatusLabel2";
            // 
            // statusLength
            // 
            this.statusLength.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusLength.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.statusLength.Name = "statusLength";
            this.statusLength.Size = new System.Drawing.Size(122, 19);
            this.statusLength.Text = "toolStripStatusLabel3";
            // 
            // statusSelection
            // 
            this.statusSelection.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusSelection.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.statusSelection.Name = "statusSelection";
            this.statusSelection.Size = new System.Drawing.Size(122, 19);
            this.statusSelection.Text = "toolStripStatusLabel4";
            // 
            // statusReport
            // 
            this.statusReport.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusReport.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.statusReport.Name = "statusReport";
            this.statusReport.Size = new System.Drawing.Size(122, 19);
            this.statusReport.Text = "toolStripStatusLabel1";
            // 
            // panelWave
            // 
            this.panelWave.AutoScroll = true;
            this.panelWave.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panelWave.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelWave.Location = new System.Drawing.Point(0, 0);
            this.panelWave.Name = "panelWave";
            this.panelWave.SelectionEnd = 0;
            this.panelWave.SelectionStart = 0;
            this.panelWave.Size = new System.Drawing.Size(1000, 160);
            this.panelWave.TabIndex = 1;
            this.panelWave.SelChanged += new Comp3931_Project_JoePelz.TimeSelChangedEventHandler(this.updateSelection);
            // 
            // WaveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 261);
            this.Controls.Add(this.panelFourier);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panelWave);
            this.Name = "WaveForm";
            this.Text = "[Untitled.wav]";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WaveForm_FormClosed);
            this.GotFocus += new System.EventHandler(this.WaveForm_GotFocus);
            this.panelFourier.ResumeLayout(false);
            this.panelFourier.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusSampling;
        private System.Windows.Forms.ToolStripStatusLabel statusBits;
        private System.Windows.Forms.ToolStripStatusLabel statusLength;
        private System.Windows.Forms.ToolStripStatusLabel statusSelection;
        private System.Windows.Forms.ToolStripStatusLabel statusReport;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem windowingModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem passthroughToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triangleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cosineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blackmanToolStripMenuItem;
        private Comp3931_Project_JoePelz.WavePanel panelWave;
        private Comp3931_Project_JoePelz.FourierPanel panelFourier;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convolutionFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dFTFilteringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iIRLowpassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iIRHighpassToolStripMenuItem;
    }
}

