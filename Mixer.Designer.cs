namespace Comp3931_Project_JoePelz
{
    partial class Mixer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mixer));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fidelityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeSampleRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Fidel_Sample_11025 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Fidel_Sample_22050 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Fidel_Sample_44100 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Fidel_Sample_88200 = new System.Windows.Forms.ToolStripMenuItem();
            this.changeBitRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Fidel_Bits_8 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Fidel_Bits_16 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Fidel_Bits_24 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Fidel_Bits_32 = new System.Windows.Forms.ToolStripMenuItem();
            this.fXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reverseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBarMain = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRecord = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.toolBarMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.fidelityToolStripMenuItem,
            this.fXToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(334, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripSeparator1,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(100, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // fidelityToolStripMenuItem
            // 
            this.fidelityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeSampleRateToolStripMenuItem,
            this.changeBitRateToolStripMenuItem});
            this.fidelityToolStripMenuItem.Name = "fidelityToolStripMenuItem";
            this.fidelityToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.fidelityToolStripMenuItem.Text = "F&idelity";
            // 
            // changeSampleRateToolStripMenuItem
            // 
            this.changeSampleRateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_Fidel_Sample_11025,
            this.menu_Fidel_Sample_22050,
            this.menu_Fidel_Sample_44100,
            this.menu_Fidel_Sample_88200});
            this.changeSampleRateToolStripMenuItem.Name = "changeSampleRateToolStripMenuItem";
            this.changeSampleRateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.changeSampleRateToolStripMenuItem.Text = "Sample Rate";
            // 
            // menu_Fidel_Sample_11025
            // 
            this.menu_Fidel_Sample_11025.Name = "menu_Fidel_Sample_11025";
            this.menu_Fidel_Sample_11025.Size = new System.Drawing.Size(152, 22);
            this.menu_Fidel_Sample_11025.Tag = 11025;
            this.menu_Fidel_Sample_11025.Text = "11025";
            this.menu_Fidel_Sample_11025.Click += new System.EventHandler(this.menu_Fidel_Click);
            // 
            // menu_Fidel_Sample_22050
            // 
            this.menu_Fidel_Sample_22050.Name = "menu_Fidel_Sample_22050";
            this.menu_Fidel_Sample_22050.Size = new System.Drawing.Size(152, 22);
            this.menu_Fidel_Sample_22050.Tag = 22050;
            this.menu_Fidel_Sample_22050.Text = "22050";
            this.menu_Fidel_Sample_22050.Click += new System.EventHandler(this.menu_Fidel_Click);
            // 
            // menu_Fidel_Sample_44100
            // 
            this.menu_Fidel_Sample_44100.Name = "menu_Fidel_Sample_44100";
            this.menu_Fidel_Sample_44100.Size = new System.Drawing.Size(152, 22);
            this.menu_Fidel_Sample_44100.Tag = 44100;
            this.menu_Fidel_Sample_44100.Text = "44100";
            this.menu_Fidel_Sample_44100.Click += new System.EventHandler(this.menu_Fidel_Click);
            // 
            // menu_Fidel_Sample_88200
            // 
            this.menu_Fidel_Sample_88200.Name = "menu_Fidel_Sample_88200";
            this.menu_Fidel_Sample_88200.Size = new System.Drawing.Size(152, 22);
            this.menu_Fidel_Sample_88200.Tag = 88200;
            this.menu_Fidel_Sample_88200.Text = "88200";
            this.menu_Fidel_Sample_88200.Click += new System.EventHandler(this.menu_Fidel_Click);
            // 
            // changeBitRateToolStripMenuItem
            // 
            this.changeBitRateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_Fidel_Bits_8,
            this.menu_Fidel_Bits_16,
            this.menu_Fidel_Bits_24,
            this.menu_Fidel_Bits_32});
            this.changeBitRateToolStripMenuItem.Name = "changeBitRateToolStripMenuItem";
            this.changeBitRateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.changeBitRateToolStripMenuItem.Text = "Bit Rate";
            // 
            // menu_Fidel_Bits_8
            // 
            this.menu_Fidel_Bits_8.Name = "menu_Fidel_Bits_8";
            this.menu_Fidel_Bits_8.Size = new System.Drawing.Size(86, 22);
            this.menu_Fidel_Bits_8.Text = "8";
            // 
            // menu_Fidel_Bits_16
            // 
            this.menu_Fidel_Bits_16.Checked = true;
            this.menu_Fidel_Bits_16.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menu_Fidel_Bits_16.Name = "menu_Fidel_Bits_16";
            this.menu_Fidel_Bits_16.Size = new System.Drawing.Size(86, 22);
            this.menu_Fidel_Bits_16.Text = "16";
            // 
            // menu_Fidel_Bits_24
            // 
            this.menu_Fidel_Bits_24.Name = "menu_Fidel_Bits_24";
            this.menu_Fidel_Bits_24.Size = new System.Drawing.Size(86, 22);
            this.menu_Fidel_Bits_24.Text = "24";
            // 
            // menu_Fidel_Bits_32
            // 
            this.menu_Fidel_Bits_32.Name = "menu_Fidel_Bits_32";
            this.menu_Fidel_Bits_32.Size = new System.Drawing.Size(86, 22);
            this.menu_Fidel_Bits_32.Text = "32";
            // 
            // fXToolStripMenuItem
            // 
            this.fXToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reverseToolStripMenuItem});
            this.fXToolStripMenuItem.Name = "fXToolStripMenuItem";
            this.fXToolStripMenuItem.Size = new System.Drawing.Size(32, 20);
            this.fXToolStripMenuItem.Text = "F&X";
            // 
            // reverseToolStripMenuItem
            // 
            this.reverseToolStripMenuItem.Name = "reverseToolStripMenuItem";
            this.reverseToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.reverseToolStripMenuItem.Text = "&Reverse";
            this.reverseToolStripMenuItem.Click += new System.EventHandler(this.reverseToolStripMenuItem_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "&Window";
            // 
            // toolBarMain
            // 
            this.toolBarMain.BackColor = System.Drawing.SystemColors.Control;
            this.toolBarMain.ImageScalingSize = new System.Drawing.Size(48, 48);
            this.toolBarMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnSave,
            this.btnOpen,
            this.toolStripSeparator2,
            this.btnRecord,
            this.btnStop,
            this.btnPlay});
            this.toolBarMain.Location = new System.Drawing.Point(0, 24);
            this.toolBarMain.Name = "toolBarMain";
            this.toolBarMain.Size = new System.Drawing.Size(334, 55);
            this.toolBarMain.TabIndex = 3;
            this.toolBarMain.Text = "toolStrip1";
            // 
            // btnNew
            // 
            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
            this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(52, 52);
            this.btnNew.Text = "New Sample";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(52, 52);
            this.btnSave.Text = "Save Sample";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(52, 52);
            this.btnOpen.Text = "Open Sample";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AutoSize = false;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(12, 55);
            // 
            // btnRecord
            // 
            this.btnRecord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRecord.Image = ((System.Drawing.Image)(resources.GetObject("btnRecord.Image")));
            this.btnRecord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(52, 52);
            this.btnRecord.Text = "Record a sample";
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // btnStop
            // 
            this.btnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(52, 52);
            this.btnStop.Text = "Stop recording and playing";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.AutoSize = false;
            this.btnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPlay.Image = ((System.Drawing.Image)(resources.GetObject("btnPlay.Image")));
            this.btnPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(52, 52);
            this.btnPlay.Text = "Play active sample";
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Wave Files|*.wav|All files|*.*";
            this.openFileDialog1.InitialDirectory = "Documents";
            this.openFileDialog1.Title = "Open a wave file";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "wav";
            this.saveFileDialog1.Filter = "Wave Files|*.wav|All files|*.*";
            this.saveFileDialog1.InitialDirectory = "Documents";
            // 
            // Mixer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(334, 80);
            this.Controls.Add(this.toolBarMain);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Mixer";
            this.Text = "Mixer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolBarMain.ResumeLayout(false);
            this.toolBarMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fidelityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeSampleRateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menu_Fidel_Sample_11025;
        private System.Windows.Forms.ToolStripMenuItem menu_Fidel_Sample_22050;
        private System.Windows.Forms.ToolStripMenuItem menu_Fidel_Sample_44100;
        private System.Windows.Forms.ToolStripMenuItem menu_Fidel_Sample_88200;
        private System.Windows.Forms.ToolStripMenuItem changeBitRateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menu_Fidel_Bits_8;
        private System.Windows.Forms.ToolStripMenuItem menu_Fidel_Bits_16;
        private System.Windows.Forms.ToolStripMenuItem menu_Fidel_Bits_24;
        private System.Windows.Forms.ToolStripMenuItem menu_Fidel_Bits_32;
        private System.Windows.Forms.ToolStripMenuItem fXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reverseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolBarMain;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnRecord;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStripButton btnPlay;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}