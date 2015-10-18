using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace Comp3931_Project_JoePelz {

    internal class WaveOutHelper {
        public static void Try(int err) {
            if (err != WinmmHook.MMSYSERR_NOERROR)
                throw new Exception(err.ToString());
        }
    }

    class WavePlayer2 : IDisposable {
        private WaveFile wave;
        private WaveFormat waveform;
        private WaveHdr pWaveHdr1;
        private byte[] pbuffer;
        private GCHandle h_pbuffer;
        private IntPtr hWaveOut;
        private AutoResetEvent m_PlayEvent = new AutoResetEvent(false);

        private bool bPlaying;
        private bool bPaused;
        private bool bEnding;
        //private bool bFinished;

        private WinmmHook.WaveDelegate m_BufferProc = new WinmmHook.WaveDelegate(WavePlayer2.WaveOutProc);

        internal static void WaveOutProc(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2) {
            GCHandle h;
            WavePlayer2 player;
            if (uMsg == WinmmHook.MM_WOM_DONE) {
                h = (GCHandle)wavhdr.dwUser;
                player = (WavePlayer2)h.Target;
                player.handle_WOM_DONE();
            }
        }

        public WavePlayer2() {
            bEnding = false;
            bPlaying = false;
            bPaused = false;
            hWaveOut = new IntPtr();
        }

        public void handle_WOM_DONE() {
            m_PlayEvent.Set();
            cleanup();
        }

        public void setWave(WaveFile source) {
            if (hWaveOut != IntPtr.Zero) {
                stop();
                while (bEnding)
                    ; //TODO: Active waiting. How do I semaphore again?
            }
            wave = source;
            waveform = new WaveFormat(wave.sampleRate, wave.bitDepth, wave.channels);

            pbuffer = wave.getData();

            pWaveHdr1.dwUser = (IntPtr)GCHandle.Alloc(this);
            pWaveHdr1.dwBufferLength = pbuffer.Length;          //size of the buffer in bytes
        }

        public void play() {
            WaveOutHelper.Try(WinmmHook.waveOutOpen(out hWaveOut, WinmmHook.WAVE_MAPPER, waveform, m_BufferProc, 0, WinmmHook.CALLBACK_FUNCTION));


            h_pbuffer = GCHandle.Alloc(pbuffer, GCHandleType.Pinned);
            pWaveHdr1.lpData = h_pbuffer.AddrOfPinnedObject();  //IntPtr to buffer
            WaveOutHelper.Try(WinmmHook.waveOutPrepareHeader(hWaveOut, ref pWaveHdr1, Marshal.SizeOf(pWaveHdr1)));

            m_PlayEvent.Reset();
            WinmmHook.waveOutWrite(hWaveOut, ref pWaveHdr1, Marshal.SizeOf(pWaveHdr1));
            m_PlayEvent.WaitOne();
        }

        public void pause() {
            if (!bPaused) {
                WinmmHook.waveOutPause(hWaveOut);
            } else {
                WinmmHook.waveOutRestart(hWaveOut);
            }
        }

        public void stop() {
            bEnding = true;
            WinmmHook.waveOutReset(hWaveOut);
        }

        public bool isPlaying() {
            return bPlaying;
        }

        public bool isPaused() {
            return bPaused;
        }

        ~WavePlayer2() {
            Dispose();
        }
        public void Dispose() {
            if (h_pbuffer.IsAllocated)
                h_pbuffer.Free();
            GC.SuppressFinalize(this);
        }

        private void cleanup() {
            WinmmHook.waveOutUnprepareHeader(hWaveOut, ref pWaveHdr1, Marshal.SizeOf(pWaveHdr1));
            WinmmHook.waveOutReset(hWaveOut);
            WinmmHook.waveOutClose(hWaveOut);
            bEnding = false; //finished ending.
            if (h_pbuffer.IsAllocated)
                h_pbuffer.Free();
            hWaveOut = IntPtr.Zero;
        }
    }
}
