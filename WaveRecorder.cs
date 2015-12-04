using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comp3931_Project_JoePelz {

    /// <summary>
    /// Messages handled in my thread message queue
    /// </summary>
    public enum RecorderMsg {
        WIM_OPEN,
        WIM_DATA,
        WIM_CLOSE,
        RECORDING_START,
        RECORDING_STOP,
        TERMINATING,
        EXIT
    }

    class WaveRecorder : IDisposable {
        private BlockingCollection<RecorderMsg> MsgQueue;
        private Thread recorder;
        
        private WaveFormat waveform;
        private byte[] pSaveBuffer;
        private int lastHeader;

        private WaveHdr waveHdr1;
        private byte[] pBuffer1;
        private GCHandle h_pBuffer1;

        private WaveHdr waveHdr2;
        private byte[] pBuffer2;
        private GCHandle h_pBuffer2;

        private IntPtr hWaveIn;
        private bool bRecording, bEnding;
        private static int INP_BUFFER_SIZE = 4096;

        private WinmmHook.WaveDelegate WaveInProc;
        private IntPtr parentHandle;

        public WaveRecorder(System.Windows.Forms.Form parent) {
            parentHandle = parent.Handle;
            MsgQueue = new BlockingCollection<RecorderMsg>();

            hWaveIn = new IntPtr();
            WaveInProc = new WinmmHook.WaveDelegate(WIM_proc);
            
            pBuffer1 = new byte[INP_BUFFER_SIZE];
            pBuffer2 = new byte[INP_BUFFER_SIZE];
            waveform = new WaveFormat(11025, 8, 1);
        }

        ~WaveRecorder() {
            Dispose();
        }

        /* 
        ===============  Public Recording Interface  =============== 
        */

        public bool Recording {
            get { return bRecording; }
        }

        public void RecordingStart() {
            MsgQueue.Add(RecorderMsg.RECORDING_START);
            if (recorder != null && recorder.IsAlive) {
                throw new Exception("Thread is still alive. Are you still recording?");
            }
            recorder = new Thread(new ThreadStart(threadProc));
            recorder.Start();
        }

        public void RecordingStop() {
            MsgQueue.Add(RecorderMsg.RECORDING_STOP);
        }

        public WaveFile getSamples() {
            if (recorder.IsAlive) {
                MsgQueue.Add(RecorderMsg.TERMINATING);
                recorder.Join();
            }

            //create a new wave file with the byte array.
            WaveFile result = null;
            if (pSaveBuffer != null && pSaveBuffer.Length > 0) {
                result = new WaveFile(8, 1, 11025, pSaveBuffer);
            }

            return result;
        }

        /* 
        ===============  Queue processing  =============== 
        */

        private void WIM_proc(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2) {
            switch (uMsg) {
                case WinmmHook.MM_WIM_OPEN:
                    MsgQueue.Add(RecorderMsg.WIM_OPEN);
                    break;
                case WinmmHook.MM_WIM_DATA:
                    if (wavhdr.Equals(waveHdr1)) {
                        lastHeader = 1;
                    } else if (wavhdr.Equals(waveHdr2)) {
                        lastHeader = 2;
                    } else {
                        lastHeader = 0;
                    }
                    MsgQueue.Add(RecorderMsg.WIM_DATA);
                    break;
                case WinmmHook.MM_WIM_CLOSE:
                    MsgQueue.Add(RecorderMsg.WIM_CLOSE);
                    break;
            }
        }

        private void threadProc() {
            RecorderMsg msg;
            msg = MsgQueue.Take();
            while (msg != RecorderMsg.EXIT) {
                switch (msg) {
                    case RecorderMsg.WIM_OPEN:
                        OnWimOpen();
                        break;
                    case RecorderMsg.WIM_DATA:
                        OnWimData();
                        break;
                    case RecorderMsg.WIM_CLOSE:
                        OnWimClose();
                        break;
                    case RecorderMsg.RECORDING_START:
                        OnRecordingStart();
                        break;
                    case RecorderMsg.RECORDING_STOP:
                        OnRecordingStop();
                        break;
                    case RecorderMsg.TERMINATING:
                        OnTerminating();
                        break;
                }
                msg = MsgQueue.Take();
            }
        }

        /* 
        ===============  Actions =============== 
        */

        private void OnWimOpen() {
            WinmmHook.waveInAddBuffer(hWaveIn, ref waveHdr1, Marshal.SizeOf(waveHdr1));
            WinmmHook.waveInAddBuffer(hWaveIn, ref waveHdr2, Marshal.SizeOf(waveHdr2));
            
            bRecording = true;
            bEnding = false;
            WinmmHook.PostMessage(parentHandle, WinmmHook.WM_USER + 1, (int)PlaybackStatus.Recording, 0);
            WinmmHook.waveInStart(hWaveIn);
        }

        private void OnWimData() {
            WaveHdr header;
            byte[] samples;
            if (lastHeader == 1) {
                header = waveHdr1;
                samples = pBuffer1;
            } else if (lastHeader == 2) {
                header = waveHdr2;
                samples = pBuffer2;
            } else {
                if (bEnding) {
                    WinmmHook.waveInClose(hWaveIn);
                }
                return;
            }
            
            byte[] result;
            int copyPos;
            if (pSaveBuffer == null) {
                result = new byte[header.dwBytesRecorded];
                copyPos = 0;
            } else {
                result = new byte[pSaveBuffer.Length + header.dwBytesRecorded];
                pSaveBuffer.CopyTo(result, 0);
                copyPos = pSaveBuffer.Length;
            }
            Array.Copy(samples, 0, result, copyPos, header.dwBytesRecorded);
            pSaveBuffer = result;
            
            if (bEnding) {
                WinmmHook.waveInReset(hWaveIn);
                WinmmHook.waveInClose(hWaveIn);
                return;
            }


            if (lastHeader == 1) {
                WinmmHook.waveInAddBuffer(hWaveIn, ref waveHdr1, Marshal.SizeOf(waveHdr1));
            } else if (lastHeader == 2) {
                WinmmHook.waveInAddBuffer(hWaveIn, ref waveHdr2, Marshal.SizeOf(waveHdr2));
            }
        }

        private void OnWimClose() {
            WinmmHook.waveInUnprepareHeader(hWaveIn, ref waveHdr1, Marshal.SizeOf(waveHdr1));
            WinmmHook.waveInUnprepareHeader(hWaveIn, ref waveHdr2, Marshal.SizeOf(waveHdr2));

            if (h_pBuffer1.IsAllocated) {
                h_pBuffer1.Free();
            }
            if (h_pBuffer2.IsAllocated) {
                h_pBuffer2.Free();
            }

            bRecording = false;
            WinmmHook.PostMessage(parentHandle, WinmmHook.WM_USER + 1, (int)PlaybackStatus.Stopped, 0);
            MsgQueue.Add(RecorderMsg.TERMINATING);
        }

        private void OnRecordingStart() {
            int bFailed = WinmmHook.waveInOpen(out hWaveIn, WinmmHook.WAVE_MAPPER, waveform, WaveInProc, 0, WinmmHook.CALLBACK_FUNCTION);
            if (bFailed != 0) {
                //could not open waveform audio
                return;
            }

            h_pBuffer1 = GCHandle.Alloc(pBuffer1, GCHandleType.Pinned);
            waveHdr1.lpData = h_pBuffer1.AddrOfPinnedObject();
            waveHdr1.dwBufferLength = INP_BUFFER_SIZE;
            waveHdr1.dwBytesRecorded = 0;
            waveHdr1.dwUser = new IntPtr(0); //(IntPtr)GCHandle.Alloc(this);
            waveHdr1.dwFlags = 0;
            waveHdr1.dwLoops = 1;
            waveHdr1.lpNext = new IntPtr(0);
            waveHdr1.reserved = 0;

            WinmmHook.waveInPrepareHeader(hWaveIn, ref waveHdr1, Marshal.SizeOf(waveHdr1));

            h_pBuffer2 = GCHandle.Alloc(pBuffer2, GCHandleType.Pinned);
            waveHdr2.lpData = h_pBuffer2.AddrOfPinnedObject();
            waveHdr2.dwBufferLength = INP_BUFFER_SIZE;
            waveHdr2.dwBytesRecorded = 0;
            waveHdr2.dwUser = new IntPtr(0); //(IntPtr)GCHandle.Alloc(this);
            waveHdr2.dwFlags = 0;
            waveHdr2.dwLoops = 1;
            waveHdr2.lpNext = new IntPtr(0);
            waveHdr2.reserved = 0;

            WinmmHook.waveInPrepareHeader(hWaveIn, ref waveHdr2, Marshal.SizeOf(waveHdr2));

        }

        private void OnRecordingStop() {
            bEnding = true;
            //WinmmHook.waveInReset(hWaveIn);
        }

        private void OnTerminating() {
            if (bRecording) {
                bEnding = true;
                //WinmmHook.waveInReset(hWaveIn);
                return;
            }

            if (h_pBuffer1.IsAllocated) {
                h_pBuffer1.Free();
            }
            if (h_pBuffer2.IsAllocated) {
                h_pBuffer2.Free();
            }
            MsgQueue.Add(RecorderMsg.EXIT);
        }

        public void Dispose() {
            if (recorder.IsAlive) {
                MsgQueue.Add(RecorderMsg.TERMINATING);
                recorder.Join();
            }
        }

    }
}
