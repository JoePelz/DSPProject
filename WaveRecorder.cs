using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Comp3931_Project_JoePelz {

    internal class WaveInHelper {
        public static void Try(int err) {
            if (err != WinmmHook.MMSYSERR_NOERROR)
                throw new Exception(err.ToString());
        }
    }

    class WaveRecorder : IDisposable {
        private const int BuffSize = 16384;
        private WaveFormat waveFormat;
        private byte[] data;
        private IntPtr hWaveIn;

        private WaveHdr waveHdr1;
        private byte[] buffer1;
        private GCHandle h_buffer1;

        private WaveHdr waveHdr2;
        private byte[] buffer2;
        private GCHandle h_buffer2;

        private bool bEnding;
        private bool bRecording;
        private WinmmHook.WaveDelegate wimproc;


        public WaveRecorder() {
            hWaveIn = new IntPtr();
            h_buffer1 = new GCHandle();
            h_buffer2 = new GCHandle();
            wimproc = new WinmmHook.WaveDelegate(WIM_proc);

            waveFormat = new WaveFormat(11025, 8, 1);
            WaveInHelper.Try(WinmmHook.waveInOpen(out hWaveIn, WinmmHook.WAVE_MAPPER, waveFormat, wimproc, 0, WinmmHook.CALLBACK_FUNCTION));

        }

        private void WIM_proc(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2) {
            switch (uMsg) {
                case WinmmHook.MM_WIM_OPEN:
                    handle_WIM_OPEN();
                    break;
                case WinmmHook.MM_WIM_DATA:
                    handle_WIM_DATA(ref wavhdr);
                    break;
                case WinmmHook.MM_WIM_CLOSE:
                    //this never runs. Why?!?!
                    handle_WIM_CLOSE();
                    break;
                default:
                    handle_WIM_CLOSE();
                    break;
            }
        }

        public void handle_WIM_OPEN() {
            data = null;
        }

        public void handle_WIM_DATA(ref WaveHdr waveHeader) {
            byte[] result;
            int copyPos;
            if (data == null) {
                result = new byte[waveHeader.dwBytesRecorded];
                copyPos = 0;
            } else {
                result = new byte[data.Length + waveHeader.dwBytesRecorded];
                data.CopyTo(result, 0);
                copyPos = data.Length;
            }

            byte[] samples;
            if (waveHeader.Equals(waveHdr1))
                samples = buffer1;
            else if (waveHeader.Equals(waveHdr2))
                samples = buffer2;
            else
                throw new Exception("Wave header doesn't exist??");
            Array.Copy(samples, 0, result, copyPos, waveHeader.dwBytesRecorded);
            data = result;

            if (bEnding) {
                WaveInHelper.Try(WinmmHook.waveInUnprepareHeader(hWaveIn, ref waveHeader, Marshal.SizeOf(waveHeader)));
                WinmmHook.waveInClose(hWaveIn);
                
                if (waveHeader.Equals(waveHdr1))
                    buffer1 = null;
                else if (waveHeader.Equals(waveHdr2))
                    buffer2 = null;
                return;
            }

            WinmmHook.waveInAddBuffer(hWaveIn, ref waveHeader, Marshal.SizeOf(waveHeader));

        }

        public void handle_WIM_CLOSE() {
            //this never gets called. WHY?
            bRecording = false;
            //WaveInHelper.Try(WinmmHook.waveInUnprepareHeader(hWaveIn, ref waveHdr1, Marshal.SizeOf(waveHdr1)));
            //WaveInHelper.Try(WinmmHook.waveInUnprepareHeader(hWaveIn, ref waveHdr2, Marshal.SizeOf(waveHdr2)));
        }

        public void beginRecording() {
            //Setup buffers and bufferHeaders
            buffer1 = new byte[BuffSize];
            h_buffer1 = GCHandle.Alloc(buffer1, GCHandleType.Pinned);
            waveHdr1.lpData = h_buffer1.AddrOfPinnedObject();
            waveHdr1.dwBufferLength = BuffSize;
            waveHdr1.dwBytesRecorded = 0;
            waveHdr1.dwUser = (IntPtr)GCHandle.Alloc(this);
            waveHdr1.dwFlags = 0;
            waveHdr1.dwLoops = 1;
            waveHdr1.lpNext = new IntPtr(0);
            waveHdr1.reserved = 0;

            WaveInHelper.Try(WinmmHook.waveInPrepareHeader(hWaveIn, ref waveHdr1, Marshal.SizeOf(waveHdr1)));

            buffer2 = new byte[BuffSize];
            h_buffer2 = GCHandle.Alloc(buffer2, GCHandleType.Pinned);
            waveHdr2.lpData = h_buffer2.AddrOfPinnedObject();
            waveHdr2.dwBufferLength = BuffSize;
            waveHdr2.dwBytesRecorded = 0;
            waveHdr2.dwUser = (IntPtr)GCHandle.Alloc(this);
            waveHdr2.dwFlags = 0;
            waveHdr2.dwLoops = 1;
            waveHdr2.lpNext = new IntPtr(0);
            waveHdr2.reserved = 0;

            WaveInHelper.Try(WinmmHook.waveInPrepareHeader(hWaveIn, ref waveHdr2, Marshal.SizeOf(waveHdr2)));
            
            WaveInHelper.Try(WinmmHook.waveInAddBuffer(hWaveIn, ref waveHdr1, Marshal.SizeOf(waveHdr1)));
            WaveInHelper.Try(WinmmHook.waveInAddBuffer(hWaveIn, ref waveHdr2, Marshal.SizeOf(waveHdr2)));

            bEnding = false;
            bRecording = true;
            WinmmHook.waveInStart(hWaveIn);
        }

        public void stopRecording() {
            bEnding = true;
            WaveInHelper.Try(WinmmHook.waveInStop(hWaveIn));
            WaveInHelper.Try(WinmmHook.waveInReset(hWaveIn));
        }

        public WaveFile getResult() {
            WaveFile result = null;
            if (data != null) {
                result = new WaveFile(8, 1, 11025, data);
            }
            return result;
        }

        ~WaveRecorder() {
            Dispose();
        }
        public void Dispose() {
        }
    }
}
