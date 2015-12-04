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
    
    public partial class Mixer : Form {
        WaveRecorder rec;
        List<WaveForm> children = new List<WaveForm>();
        WaveForm activeChild = null;
        ToolStripMenuItem[] childWindowMenuItems;

        public Mixer() {
            InitializeComponent();
        }

        protected override void OnFormClosed(FormClosedEventArgs e) {
            base.OnFormClosed(e);
            //ensure all children are destroyed properly
            // (children have subthreads)
            while (children.Count > 0) {
                children[0].Close();
            }
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == WinmmHook.WM_USER + 1) {
                PlaybackStatus status = (PlaybackStatus)(int)m.WParam;
                playbackUpdate(status);
            }
            base.WndProc(ref m);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        public void setActiveWindow(WaveForm child) {
            activeChild = child;
            updateWindowMenu();
            updateFidelityMenu();
            if (activeChild == null) {
                playbackUpdate(PlaybackStatus.Disabled);
            } else {
                playbackUpdate(activeChild.State);
            }
        }

        public void childDied(WaveForm child) {
            children.Remove(child);
            updateWindowMenu();
            if (children.Count == 1) {
                setActiveWindow(children[0]);
            } else {
                setActiveWindow(null);
            }
        }

        private void updateWindowMenu() {
            windowToolStripMenuItem.DropDownItems.Clear();
            childWindowMenuItems = new ToolStripMenuItem[children.Count];
            for (int i = 0; i < children.Count; i++) {
                childWindowMenuItems[i] = new ToolStripMenuItem(children[i].Text, null, new System.EventHandler(this.focusChild));
                childWindowMenuItems[i].Tag = i;
                if (children[i] == activeChild) {
                    childWindowMenuItems[i].Checked = true;
                }
                windowToolStripMenuItem.DropDownItems.Add(childWindowMenuItems[i]);
            }
        }

        private void focusChild(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            children[(int)menuItem.Tag].Focus();
        }

        private void updateFidelityMenu() {
            ToolStripMenuItem[] bitRates    = { menu_Fidel_Bits_8, menu_Fidel_Bits_16, menu_Fidel_Bits_24, menu_Fidel_Bits_32 };
            ToolStripMenuItem[] sampleRates = { menu_Fidel_Sample_11025, menu_Fidel_Sample_22050, menu_Fidel_Sample_44100, menu_Fidel_Sample_88200 };
            ToolStripMenuItem[] channels    = { menu_Fidel_channels_mono, menu_Fidel_channels_stereo };
            foreach (ToolStripMenuItem item in sampleRates) {
                if (activeChild != null && activeChild.SampleRate == (int)item.Tag) {
                    item.Checked = true;
                } else {
                    item.Checked = false;
                }
            }
            foreach (ToolStripMenuItem item in bitRates) {
                if (activeChild != null && activeChild.BitDepth == (int)item.Tag) {
                    item.Checked = true;
                } else {
                    item.Checked = false;
                }
            }
            foreach (ToolStripMenuItem item in channels) {
                if (activeChild != null && activeChild.Channels == (int)item.Tag) {
                    item.Checked = true;
                } else {
                    item.Checked = false;
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e) {
            createChildWindow();
        }

        private void createChildWindow(string path = null) {
            WaveFile wave;
            WaveForm baby;
            if (path == null) {
                wave = new WaveFile();
                baby = new WaveForm(this, wave);
                baby.updateReport("New sine waves generated.");
            } else {
                try {
                    wave = new WaveFile(path);
                    baby = new WaveForm(this, wave);
                    baby.updateReport(wave.getName()+" opened successfully!");
                } catch (Exception e) {
                    MessageBox.Show("Opening Failed: " + e.Message);
                    return;
                }
            }
            
            children.Add(baby);
            activeChild = baby;
            baby.Show();
            updateWindowMenu();
        }

        private void createChildWindow(WaveFile wave) {
            WaveForm baby;
            baby = new WaveForm(this, wave);
            
            children.Add(baby);
            activeChild = baby;
            baby.Show();
            updateWindowMenu();
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                if (activeChild != null) {
                    activeChild.setPath(saveFileDialog1.FileName);
                    activeChild.save();
                } else {
                    MessageBox.Show("Please select which wave file to save.");
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                createChildWindow(openFileDialog1.FileName);
            }
        }

        private void btnStop_Click(object sender, EventArgs e) {
            foreach (WaveForm wave in children) {
                wave.waveStop();
            }
        }

        public void playbackUpdate(PlaybackStatus update) {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mixer));
            switch (update) {
                case PlaybackStatus.Playing:
                    btnPlay.Enabled = true;
                    btnPlay.Image = Comp3931_Project_JoePelz.Properties.Resources.btnPause;
                    btnStop.Enabled = true;
                    btnStop.Image = Comp3931_Project_JoePelz.Properties.Resources.btnStop;
                    btnRecord.Enabled = false;
                    btnRecord.Image = Comp3931_Project_JoePelz.Properties.Resources.btnRecordDisabled;
                    btnNew.Enabled = false;
                    btnNew.Image = Comp3931_Project_JoePelz.Properties.Resources.btnNewDisabled;
                    btnOpen.Enabled = false;
                    btnOpen.Image = Comp3931_Project_JoePelz.Properties.Resources.btnOpenDisabled;
                    btnSave.Enabled = false;
                    btnSave.Image = Comp3931_Project_JoePelz.Properties.Resources.btnSaveDisabled;
                    break;
                case PlaybackStatus.Paused:
                    btnPlay.Image = Comp3931_Project_JoePelz.Properties.Resources.btnPlay;
                    break;
                case PlaybackStatus.Recording:
                    btnPlay.Enabled = false;
                    btnPlay.Image = Comp3931_Project_JoePelz.Properties.Resources.btnPlayDisabled;
                    btnStop.Enabled = false;
                    btnStop.Image = Comp3931_Project_JoePelz.Properties.Resources.btnStopDisabled;
                    btnRecord.Enabled = true;
                    btnRecord.Image = Comp3931_Project_JoePelz.Properties.Resources.btnRecord;
                    btnNew.Enabled = false;
                    btnNew.Image = Comp3931_Project_JoePelz.Properties.Resources.btnNewDisabled;
                    btnOpen.Enabled = false;
                    btnOpen.Image = Comp3931_Project_JoePelz.Properties.Resources.btnOpenDisabled;
                    btnSave.Enabled = false;
                    btnSave.Image = Comp3931_Project_JoePelz.Properties.Resources.btnSaveDisabled;
                    break;
                case PlaybackStatus.Stopped:
                    btnPlay.Enabled = true;
                    btnPlay.Image = Comp3931_Project_JoePelz.Properties.Resources.btnPlay;
                    btnStop.Enabled = true;
                    btnStop.Image = Comp3931_Project_JoePelz.Properties.Resources.btnStop;
                    btnRecord.Enabled = true;
                    btnRecord.Image = Comp3931_Project_JoePelz.Properties.Resources.btnRecord;
                    btnNew.Enabled = true;
                    btnNew.Image = Comp3931_Project_JoePelz.Properties.Resources.btnNew;
                    btnOpen.Enabled = true;
                    btnOpen.Image = Comp3931_Project_JoePelz.Properties.Resources.btnOpen;
                    btnSave.Enabled = true;
                    btnSave.Image = Comp3931_Project_JoePelz.Properties.Resources.btnSave;
                    break;
                case PlaybackStatus.Disabled:
                    btnPlay.Enabled = false;
                    btnPlay.Image = Comp3931_Project_JoePelz.Properties.Resources.btnPlayDisabled;
                    btnStop.Enabled = true;
                    btnStop.Image = Comp3931_Project_JoePelz.Properties.Resources.btnStop;
                    btnRecord.Enabled = true;
                    btnRecord.Image = Comp3931_Project_JoePelz.Properties.Resources.btnRecord;
                    btnNew.Enabled = true;
                    btnNew.Image = Comp3931_Project_JoePelz.Properties.Resources.btnNew;
                    btnOpen.Enabled = true;
                    btnOpen.Image = Comp3931_Project_JoePelz.Properties.Resources.btnOpen;
                    btnSave.Enabled = true;
                    btnSave.Image = Comp3931_Project_JoePelz.Properties.Resources.btnSave;
                    break;
            }
        }

        private void btnRecord_Click(object sender, EventArgs e) {
            if (rec == null) {
                rec = new WaveRecorder(this);
                rec.RecordingStart();
            } else {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mixer));
                btnRecord.Enabled = false;
                btnRecord.Image = (System.Drawing.Image)(resources.GetObject("btnRecordDisabled"));
                rec.RecordingStop();
                WaveFile result = rec.getSamples();
                if (result != null) {
                    createChildWindow(result);
                }
                rec.Dispose();
                rec = null;
            }
        }

        private void btnPlay_Click(object sender, EventArgs e) {
            if (activeChild == null) {
                return;
            }

            activeChild.wavePlayPause();
            btnPlay.Invalidate();
        }

        private void reverseToolStripMenuItem_Click(object sender, EventArgs e) {
            if (activeChild == null) {
                return;
            }
            activeChild.applyFX(DSP_FX.reverse, null);
        }

        private void menu_Fidel_Click(object sender, EventArgs e) {
            ToolStripMenuItem[] sampleRates = { menu_Fidel_Sample_11025, menu_Fidel_Sample_22050, menu_Fidel_Sample_44100, menu_Fidel_Sample_88200 };
            ToolStripMenuItem[] bitRates = { menu_Fidel_Bits_8, menu_Fidel_Bits_16, menu_Fidel_Bits_24, menu_Fidel_Bits_32 };
            ToolStripMenuItem[] channels = { menu_Fidel_channels_mono, menu_Fidel_channels_stereo };
            ToolStripMenuItem source = (ToolStripMenuItem)sender;

            if (sampleRates.Contains(source)) {
                int targetRate = (int)(source.Tag);
                if (targetRate > activeChild.SampleRate) {
                    DialogResult result = MessageBox.Show(String.Format("Are you sure you want to upsample {0} from {1}Hz to {2}Hz?", activeChild.Text, activeChild.SampleRate, targetRate), "Confirm Upsample",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes) {
                        activeChild.changeSampleRate(targetRate);
                    }
                } else if (targetRate < activeChild.SampleRate) {
                    DialogResult result = MessageBox.Show(String.Format("Are you sure you want to downsample {0} from {1}Hz to {2}Hz?", activeChild.Text, activeChild.SampleRate, targetRate), "Confirm Downsample",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes) {
                        activeChild.changeSampleRate(targetRate);
                    }
                } else {
                    MessageBox.Show("Sample is already at " + targetRate + "Hz!");
                }
            }

            if (bitRates.Contains(source)) {
                short targetRate = (short)(int)(source.Tag);
                activeChild.changeBitRate(targetRate);
            }

            if (channels.Contains(source)) {
                short nChannels = (short)(int)(source.Tag);
                if (nChannels == activeChild.Channels) {
                    return;
                }
                if (activeChild.Channels != 1) {
                    DialogResult result = MessageBox.Show(String.Format("Are you sure you want to Mix {0} to {1} channels?", activeChild.Text, nChannels), "Confirm Mix",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes) {
                        activeChild.changeChannels(nChannels);
                    }
                } else {
                    activeChild.changeChannels(nChannels);
                }
            }
            updateFidelityMenu();
        }

        private void maximizeAmplitudeToolStripMenuItem_Click(object sender, EventArgs e) {
            if (activeChild == null) {
                return;
            }
            activeChild.applyFX(DSP_FX.normalize, null);
        }

        private void PitchShift(object sender, EventArgs e) {
            ToolStripMenuItem source = (ToolStripMenuItem)sender;
            int shift = (int)source.Tag;
            object[] args = { shift };
            if (activeChild != null) {
                activeChild.applyFX(DSP_FX.pitchshift, args);
            }
        }
    }
}
