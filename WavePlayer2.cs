using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Comp3931_Project_JoePelz {
    class WavePlayer2 {
        private WaveFile wave;
        private WaveFormat waveform;
        private WaveHdr pWaveHdr1;
        private byte[] pbuffer;
        private GCHandle h_pbuffer;
        private GCHandle h_pWaveHdr1;
        private bool bPlaying;
        private bool bPaused;
        private bool bEnding;
        private IntPtr hWaveOut;
        private WinmmHook.WaveDelegate m_BufferProc = new WinmmHook.WaveDelegate(WavePlayer2.WaveOutProc);

        internal static void WaveOutProc(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2) {
            GCHandle h = (GCHandle)wavhdr.dwUser;
            WavePlayer2 player = (WavePlayer2)h.Target;
            switch (uMsg) {
                case WinmmHook.MM_WOM_OPEN:
                    player.handle_WOM_OPEN();
                    break;
                case WinmmHook.MM_WOM_DONE:
                    player.handle_WOM_DONE();
                    break;
                case WinmmHook.MM_WOM_CLOSE:
                    player.handle_WOM_CLOSE();
                    break;
            }
        }

        public WavePlayer2() {
            bEnding = false;
            bPlaying = false;
            bPaused = false;
            hWaveOut = new IntPtr();
        }

        public void handle_WOM_OPEN() {
            WinmmHook.waveOutPrepareHeader(hWaveOut, ref pWaveHdr1, Marshal.SizeOf(pWaveHdr1));
            WinmmHook.waveOutWrite(hWaveOut, ref pWaveHdr1, Marshal.SizeOf(pWaveHdr1)); 

            bEnding = false;
            bPlaying = true;
            bPaused = false;
        }

        public void handle_WOM_DONE() {
            cleanup();
        }

        public void handle_WOM_CLOSE() {
            bPaused = false;
            bPlaying = false;
        }

        public void setWave(WaveFile source) {
            if (hWaveOut != IntPtr.Zero) {
                stop();
                cleanup();
            }
            wave = source;
            waveform = new WaveFormat(wave.sampleRate, wave.bitDepth, wave.channels);

            h_pWaveHdr1 = GCHandle.Alloc(pWaveHdr1, GCHandleType.Pinned);

            pbuffer = wave.getData();
            h_pbuffer = GCHandle.Alloc(h_pWaveHdr1, GCHandleType.Pinned);

            pWaveHdr1 = new WaveHdr();
            pWaveHdr1.lpData = h_pbuffer.AddrOfPinnedObject();  //IntPtr to buffer
            pWaveHdr1.dwBufferLength = pbuffer.Length;          //size of the buffer in bytes
            pWaveHdr1.dwBytesRecorded = 0;
            pWaveHdr1.dwUser = (IntPtr)GCHandle.Alloc(this);
            pWaveHdr1.dwFlags = 0;
            pWaveHdr1.dwLoops = 0;
            pWaveHdr1.lpNext = new IntPtr(0);
            pWaveHdr1.reserved = 0;
        }

        public void play() {
            if (WinmmHook.waveOutOpen(out hWaveOut, WinmmHook.WAVE_MAPPER, waveform, m_BufferProc, 0, WinmmHook.CALLBACK_FUNCTION) != WinmmHook.MMSYSERR_NOERROR) {
                throw new Exception("Error during waveOutOpen.");
            }
        }

        public bool pause() {
            if (!bPaused) {
                WinmmHook.waveOutPause(hWaveOut);
                return true;
            } else {
                WinmmHook.waveOutRestart(hWaveOut);
                return false;
            }
        }

        public void stop() {
            bEnding = true;
            WinmmHook.waveOutReset(hWaveOut);
        }

        private void cleanup() {
            WinmmHook.waveOutUnprepareHeader(hWaveOut, ref pWaveHdr1, Marshal.SizeOf(pWaveHdr1));
            WinmmHook.waveOutClose(hWaveOut);
        }
    }
}
