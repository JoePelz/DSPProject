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
            this.panelWave = new System.Windows.Forms.Panel();
            this.scrollerWave = new System.Windows.Forms.HScrollBar();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelFourier = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.windowingModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.passthroughToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cosineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblFourierFreq = new System.Windows.Forms.Label();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusSampling = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBits = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLength = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusSelection = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelReport = new System.Windows.Forms.ToolStripStatusLabel();
            this.blackmanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelWave.SuspendLayout();
            this.panelFourier.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelWave
            // 
            this.panelWave.AutoScroll = true;
            this.panelWave.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panelWave.Controls.Add(this.scrollerWave);
            this.panelWave.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelWave.Location = new System.Drawing.Point(0, 0);
            this.panelWave.Name = "panelWave";
            this.panelWave.Size = new System.Drawing.Size(1000, 160);
            this.panelWave.TabIndex = 1;
            this.panelWave.Paint += new System.Windows.Forms.PaintEventHandler(this.panelWave_Paint);
            this.panelWave.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelWave_MouseDown);
            this.panelWave.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelWave_MouseMove);
            this.panelWave.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelWave_MouseUp);
            this.panelWave.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panelWave_MouseWheel);
            this.panelWave.Resize += new System.EventHandler(this.panelWave_Resize);
            // 
            // scrollerWave
            // 
            this.scrollerWave.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.scrollerWave.Location = new System.Drawing.Point(0, 143);
            this.scrollerWave.Name = "scrollerWave";
            this.scrollerWave.Size = new System.Drawing.Size(1000, 17);
            this.scrollerWave.TabIndex = 0;
            this.scrollerWave.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollerWave_Scroll);
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
            this.panelFourier.Controls.Add(this.lblFourierFreq);
            this.panelFourier.Controls.Add(this.statusBar);
            this.panelFourier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFourier.Location = new System.Drawing.Point(0, 163);
            this.panelFourier.Name = "panelFourier";
            this.panelFourier.Size = new System.Drawing.Size(1000, 98);
            this.panelFourier.TabIndex = 3;
            this.panelFourier.Paint += new System.Windows.Forms.PaintEventHandler(this.panelFourier_Paint);
            this.panelFourier.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelFourier_MouseMove);
            this.panelFourier.Resize += new System.EventHandler(this.panelFourier_Resize);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowingModeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(170, 48);
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
            this.passthroughToolStripMenuItem.Text = "Passthrough";
            this.passthroughToolStripMenuItem.Click += new System.EventHandler(this.passthroughToolStripMenuItem_Click);
            // 
            // triangleToolStripMenuItem
            // 
            this.triangleToolStripMenuItem.Name = "triangleToolStripMenuItem";
            this.triangleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.triangleToolStripMenuItem.Text = "Triangle";
            this.triangleToolStripMenuItem.Click += new System.EventHandler(this.triangleToolStripMenuItem_Click);
            // 
            // cosineToolStripMenuItem
            // 
            this.cosineToolStripMenuItem.Name = "cosineToolStripMenuItem";
            this.cosineToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cosineToolStripMenuItem.Text = "Cosine";
            this.cosineToolStripMenuItem.Click += new System.EventHandler(this.cosineToolStripMenuItem_Click);
            // 
            // lblFourierFreq
            // 
            this.lblFourierFreq.AutoSize = true;
            this.lblFourierFreq.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFourierFreq.Location = new System.Drawing.Point(3, 3);
            this.lblFourierFreq.Name = "lblFourierFreq";
            this.lblFourierFreq.Size = new System.Drawing.Size(87, 19);
            this.lblFourierFreq.TabIndex = 1;
            this.lblFourierFreq.Text = "Frequency:";
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusSampling,
            this.statusBits,
            this.statusLength,
            this.statusSelection,
            this.labelReport});
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
            // labelReport
            // 
            this.labelReport.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.labelReport.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.labelReport.Name = "labelReport";
            this.labelReport.Size = new System.Drawing.Size(122, 19);
            this.labelReport.Text = "toolStripStatusLabel1";
            // 
            // blackmanToolStripMenuItem
            // 
            this.blackmanToolStripMenuItem.Name = "blackmanToolStripMenuItem";
            this.blackmanToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.blackmanToolStripMenuItem.Text = "Blackman";
            this.blackmanToolStripMenuItem.Click += new System.EventHandler(this.blackmanToolStripMenuItem_Click);
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
            this.panelWave.ResumeLayout(false);
            this.panelFourier.ResumeLayout(false);
            this.panelFourier.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelWave;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panelFourier;
        private System.Windows.Forms.HScrollBar scrollerWave;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusSampling;
        private System.Windows.Forms.ToolStripStatusLabel statusBits;
        private System.Windows.Forms.ToolStripStatusLabel statusLength;
        private System.Windows.Forms.ToolStripStatusLabel statusSelection;
        private System.Windows.Forms.Label lblFourierFreq;
        private System.Windows.Forms.ToolStripStatusLabel labelReport;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem windowingModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem passthroughToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triangleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cosineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blackmanToolStripMenuItem;
    }
}

