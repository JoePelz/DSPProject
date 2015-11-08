using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comp3931_Project_JoePelz {
    public enum PlayerMsg {
        WOM_OPEN,
        WOM_DONE,
        WOM_CLOSE,
        PLAYBACK_START,
        PLAYBACK_STOP,
        PLAYBACK_PAUSE,
        TERMINATING,
        EXIT
    }

    public enum PlaybackStatus {
        Playing,
        Stopped,
        Paused,
        Disabled
    }

    class WavePlayer : IDisposable {
        private BlockingCollection<PlayerMsg> MsgQueue;
        private Thread player;

        private WaveFile wave;
        private WaveFormat waveform;
        private WaveHdr pWaveHdr1;
        private byte[] pbuffer;
        private GCHandle h_pbuffer;
        private IntPtr hWaveOut;
        private bool bPlaying, bPaused, bTerminating;

        private WinmmHook.WaveDelegate WaveOutProc;
        private WaveForm parentWindow;
        private IntPtr parentHandle;

        public WavePlayer(WaveForm parent) {
            parentWindow = parent;
            parentHandle = parent.Handle;
            MsgQueue = new BlockingCollection<PlayerMsg>();

            hWaveOut = new IntPtr();
            h_pbuffer = new GCHandle();
            WaveOutProc = new WinmmHook.WaveDelegate(WOM_proc);

            player = new Thread(new ThreadStart(threadProc));
            player.Start();
        }

        ~WavePlayer() {
            Dispose();
        }

        public bool Playing {
            get { return bPlaying; }
        }

        public void setWave(WaveFile source) {
            wave = source;
            pbuffer = wave.getData();
        }

        public void PlaybackStart() {
            MsgQueue.Add(PlayerMsg.PLAYBACK_START);
        }

        public void PlaybackStop() {
            MsgQueue.Add(PlayerMsg.PLAYBACK_STOP);
        }

        public void PlaybackPause() {
            MsgQueue.Add(PlayerMsg.PLAYBACK_PAUSE);
        }

        private void WOM_proc(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2) {
            switch (uMsg) {
                case WinmmHook.MM_WOM_OPEN:
                    MsgQueue.Add(PlayerMsg.WOM_OPEN);
                    break;
                case WinmmHook.MM_WOM_DONE:
                    MsgQueue.Add(PlayerMsg.WOM_DONE);
                    break;
                case WinmmHook.MM_WOM_CLOSE:
                    MsgQueue.Add(PlayerMsg.WOM_CLOSE);
                    break;
            }
        }

        private void threadProc() {
            PlayerMsg msg;
            msg = MsgQueue.Take();
            while (msg != PlayerMsg.EXIT) {
                switch (msg) {
                    case PlayerMsg.WOM_OPEN:
                        OnWomOpen();
                        break;
                    case PlayerMsg.WOM_DONE:
                        OnWomDone();
                        break;
                    case PlayerMsg.WOM_CLOSE:
                        OnWomClose();
                        break;
                    case PlayerMsg.PLAYBACK_START:
                        OnPlaybackStart();
                        break;
                    case PlayerMsg.PLAYBACK_PAUSE:
                        OnPlaybackPause();
                        break;
                    case PlayerMsg.PLAYBACK_STOP:
                        OnPlaybackStop();
                        break;
                    case PlayerMsg.TERMINATING:
                        OnTerminating();
                        break;
                }
                msg = MsgQueue.Take();
            }
        }

        private void OnWomOpen() {
            if (h_pbuffer.IsAllocated)
                h_pbuffer.Free();
            h_pbuffer = GCHandle.Alloc(pbuffer, GCHandleType.Pinned);  //handle (pointer) to the buffer

            pWaveHdr1.dwUser = (IntPtr)GCHandle.Alloc(this);    //pointer to this
            pWaveHdr1.dwBufferLength = pbuffer.Length;          //size of the buffer in bytes
            pWaveHdr1.lpData = h_pbuffer.AddrOfPinnedObject();  //IntPtr to buffer
            pWaveHdr1.dwFlags = 0;

            WinmmHook.waveOutPrepareHeader(hWaveOut, ref pWaveHdr1, Marshal.SizeOf(pWaveHdr1));
            WinmmHook.waveOutWrite(hWaveOut, ref pWaveHdr1, Marshal.SizeOf(pWaveHdr1));
            
            bPlaying = true;
            WinmmHook.PostMessage(parentHandle, WinmmHook.WM_USER + 1, (int)PlaybackStatus.Playing, 0);
        }

        private void OnWomDone() {
            WinmmHook.waveOutUnprepareHeader(hWaveOut, ref pWaveHdr1, Marshal.SizeOf(pWaveHdr1));
            WinmmHook.waveOutClose(hWaveOut);
        }

        private void OnWomClose() {
            bPaused = false;
            bPlaying = false;
            WinmmHook.PostMessage(parentHandle, WinmmHook.WM_USER + 1, (int)PlaybackStatus.Stopped, 0);

            if (bTerminating) {
                MsgQueue.Add(PlayerMsg.TERMINATING);
            }
        }

        private void OnPlaybackStart() {
            waveform = new WaveFormat(wave.sampleRate, wave.bitDepth, wave.channels);

            int val = WinmmHook.waveOutOpen(out hWaveOut, WinmmHook.WAVE_MAPPER, waveform, WaveOutProc, 0, WinmmHook.CALLBACK_FUNCTION);

            if (val == 11) {
                //invalid parameter.
                return;
            }
        }

        private void OnPlaybackPause() {
            if (!bPaused) {
                WinmmHook.waveOutPause(hWaveOut);
                bPaused = true;
                WinmmHook.PostMessage(parentHandle, WinmmHook.WM_USER + 1, (int)PlaybackStatus.Paused, 0);
            } else {
                WinmmHook.waveOutRestart(hWaveOut);
                bPaused = false;
                WinmmHook.PostMessage(parentHandle, WinmmHook.WM_USER + 1, (int)PlaybackStatus.Playing, 0);
            }
        }

        private void OnPlaybackStop() {
            WinmmHook.waveOutReset(hWaveOut);
        }

        private void OnTerminating() {
            if (bPlaying) {
                bTerminating = true;
                WinmmHook.waveOutReset(hWaveOut);
                return;
            }

            if (h_pbuffer.IsAllocated)
                h_pbuffer.Free();
            MsgQueue.Add(PlayerMsg.EXIT);
        }

        public void Dispose() {
            if (player.IsAlive) {
                MsgQueue.Add(PlayerMsg.TERMINATING);
                player.Join();
            }
        }
    }
}
