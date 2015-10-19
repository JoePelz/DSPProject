using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace Comp3931_Project_JoePelz {

    public enum PlaybackStatus {
        Playing,
        Stopped,
        Paused,
        Disabled
    }

    internal class WaveOutHelper {
        public static void Try(int err) {
            if (err != WinmmHook.MMSYSERR_NOERROR)
                throw new Exception(err.ToString());
        }
    }

    class WavePlayer : IDisposable {
        private WaveFile wave;
        private WaveFormat waveform;
        private WaveHdr pWaveHdr1;
        private byte[] pbuffer;
        private GCHandle h_pbuffer;
        private IntPtr hWaveOut;
        private AutoResetEvent sem_donePlaying = new AutoResetEvent(false);
        private AutoResetEvent sem_closing = new AutoResetEvent(false);

        private WaveForm parentWindow;

        private bool bPlaying;
        private bool bPaused;

        private WinmmHook.WaveDelegate m_BufferProc;
        delegate void SetStatusCallback(PlaybackStatus update);

        private void WOM_proc(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2) {
            GCHandle h;
            WavePlayer player;

            switch (uMsg) {
                case WinmmHook.MM_WOM_OPEN:
                    break;
                case WinmmHook.MM_WOM_DONE:
                    h = (GCHandle)wavhdr.dwUser;
                    player = (WavePlayer)h.Target;
                    player.handle_WOM_DONE();
                    break;
                case WinmmHook.MM_WOM_CLOSE:
                    handle_WOM_CLOSE();
                    break;
            }
        }

        public WavePlayer(WaveForm parent) {
            parentWindow = parent;
            updateStatus(PlaybackStatus.Disabled);
            hWaveOut = new IntPtr();
            h_pbuffer = new GCHandle();
            m_BufferProc = new WinmmHook.WaveDelegate(WOM_proc);
        }

        private void updateStatus(PlaybackStatus update) {
            lock(this) {
                switch(update) {
                    case PlaybackStatus.Playing:
                        bPlaying = true;
                        bPaused = false;
                        break;
                    case PlaybackStatus.Paused:
                        bPlaying = true;
                        bPaused = true;
                        break;
                    case PlaybackStatus.Stopped:
                    case PlaybackStatus.Disabled:
                        bPlaying = false;
                        bPaused = false;
                        break;
                }

                //Causes crash when stop is pressed during playback.
                //  May have to do with cross-thread calls.
                //SetStatusCallback d = new SetStatusCallback(parentWindow.updatePlaybackStatus);
                //parentWindow.Invoke(d, update);
                //parentWindow.updatePlaybackStatus(update);
            }
        }

        public void handle_WOM_DONE() {
            sem_donePlaying.Set();
            updateStatus(PlaybackStatus.Stopped);
        }

        public void handle_WOM_CLOSE() {
            updateStatus(PlaybackStatus.Disabled);
            sem_closing.Set();
        }

        public void setWave(WaveFile source) {
            if (hWaveOut != IntPtr.Zero) {
                sem_closing.Reset();
                cleanup();
                sem_closing.WaitOne();
            }

            wave = source;
            waveform = new WaveFormat(wave.sampleRate, wave.bitDepth, wave.channels);
            WaveOutHelper.Try(WinmmHook.waveOutOpen(out hWaveOut, WinmmHook.WAVE_MAPPER, waveform, m_BufferProc, 0, WinmmHook.CALLBACK_FUNCTION));

            pbuffer = wave.getData();
            if (h_pbuffer.IsAllocated)
                h_pbuffer.Free();
            h_pbuffer = GCHandle.Alloc(pbuffer, GCHandleType.Pinned);  //handle (pointer) to the buffer

            pWaveHdr1.dwUser = (IntPtr)GCHandle.Alloc(this);    //pointer to this
            pWaveHdr1.dwBufferLength = pbuffer.Length;          //size of the buffer in bytes
            pWaveHdr1.lpData = h_pbuffer.AddrOfPinnedObject();  //IntPtr to buffer
            updateStatus(PlaybackStatus.Stopped);
        }

        public void play() {
            if (hWaveOut == IntPtr.Zero)
                return;
            sem_donePlaying.Reset();
            if (pWaveHdr1.dwFlags != 0) {
                WaveOutHelper.Try(WinmmHook.waveOutUnprepareHeader(hWaveOut, ref pWaveHdr1, Marshal.SizeOf(pWaveHdr1)));
            }
            pWaveHdr1.dwFlags = 0;
            WaveOutHelper.Try(WinmmHook.waveOutPrepareHeader(hWaveOut, ref pWaveHdr1, Marshal.SizeOf(pWaveHdr1)));
            WinmmHook.waveOutWrite(hWaveOut, ref pWaveHdr1, Marshal.SizeOf(pWaveHdr1));
            updateStatus(PlaybackStatus.Playing);
            //sem_playing.WaitOne(); //block while playing.
        }

        public void pause() {
            if (hWaveOut == IntPtr.Zero)
                return;
            if (!bPlaying)
                return;

            if (!bPaused) {
                WinmmHook.waveOutPause(hWaveOut);
                updateStatus(PlaybackStatus.Paused);
            } else {
                WinmmHook.waveOutRestart(hWaveOut);
                updateStatus(PlaybackStatus.Playing);
            }
        }

        public void stop() {
            if (hWaveOut == IntPtr.Zero)
                return;
            WinmmHook.waveOutReset(hWaveOut);
            updateStatus(PlaybackStatus.Stopped);
        }

        public bool isPlaying() {
            return bPlaying;
        }

        public bool isPaused() {
            return bPaused;
        }

        ~WavePlayer() {
            Dispose();
        }
        public void Dispose() {
            if (hWaveOut != IntPtr.Zero) {
                sem_closing.Reset();
                cleanup();
                sem_closing.WaitOne();
            }
        }

        private void cleanup() {
            if (hWaveOut == IntPtr.Zero)
                return;
            WinmmHook.waveOutReset(hWaveOut);
            WinmmHook.waveOutClose(hWaveOut);
            if (h_pbuffer.IsAllocated)
                h_pbuffer.Free();
            hWaveOut = IntPtr.Zero;
        }
    }
}
