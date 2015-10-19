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
        Image play;
        Image pause;
        List<WaveForm> children = new List<WaveForm>();
        WaveForm activeChild = null;
        ToolStripMenuItem[] childWindowMenuItems;

        public Mixer() {
            InitializeComponent();
            play = btnPlay.Image;
            pause = btnPause.Image;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        public void setActiveWindow(WaveForm child) {
            activeChild = child;
            updateWindowMenu();
        }

        public void childDied(WaveForm child) {
            children.Remove(child);
            updateWindowMenu();
            if (children.Count == 1) {
                activeChild = children[0];
            } else {
                activeChild = null;
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

        private void btnNew_Click(object sender, EventArgs e) {
            createChildWindow();
        }

        private void createChildWindow(string path = null) {
            WaveFile wave;
            WaveForm baby;
            if (path == null) {
                wave = new WaveFile();
                baby = new WaveForm(this, wave);
                baby.report("New sine waves generated.");
            } else {
                try {
                    wave = new WaveFile(path);
                    baby = new WaveForm(this, wave);
                    baby.report(wave.getName()+" opened successfully!");
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

        private void btnSave_Click(object sender, EventArgs e) {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                MessageBox.Show("File to save: " + saveFileDialog1.FileName);
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
                //MessageBox.Show("Opening: " + openFileDialog1.FileName);
                createChildWindow(openFileDialog1.FileName);
            }
        }

        private void btnStop_Click(object sender, EventArgs e) {
            foreach (WaveForm wave in children) {
                wave.waveStop();
            }
            btnPlay.Image = play;
        }

        public void playbackUpdate(PlaybackStatus update) {
            switch (update) {
                case PlaybackStatus.Playing:
                    btnPlay.Image = pause;
                    btnPlay.Enabled = true;
                    btnStop.Enabled = true;
                    break;
                case PlaybackStatus.Paused:
                    btnPlay.Image = play;
                    btnPlay.Enabled = true;
                    btnStop.Enabled = true;
                    break;
                case PlaybackStatus.Stopped:
                    btnPlay.Image = play;
                    btnPlay.Enabled = true;
                    btnStop.Enabled = true;
                    break;
                case PlaybackStatus.Disabled:
                    btnPlay.Image = play;
                    btnPlay.Enabled = false;
                    btnStop.Enabled = false;
                    break;
            }
        }

        private void btnPlay_Click(object sender, EventArgs e) {
            if (activeChild == null) {
                return;
            }
            //pause doesn't work. but when it does, this can change the image.
            //if (btnPlay.Image == play) {
            //    btnPlay.Image = pause;
            //} else {
            //    btnPlay.Image = play;
            //}

            activeChild.wavePlayPause();
            btnPlay.Invalidate();
        }

        private void reverseToolStripMenuItem_Click(object sender, EventArgs e) {
            if (activeChild == null) {
                MessageBox.Show("No sample to reverse!");
                return;
            }
            activeChild.applyFX(DSP_FX.reverse);
        }
    }
}
