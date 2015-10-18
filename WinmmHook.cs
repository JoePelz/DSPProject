using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Comp3931_Project_JoePelz {

    [StructLayout(LayoutKind.Sequential)]
    public struct WaveHdr {
        public IntPtr lpData;       // pointer to locked data buffer
        public int dwBufferLength;  // length of data buffer
        public int dwBytesRecorded; // used for input only
        public IntPtr dwUser;       // for client's use
        public int dwFlags;         // assorted flags (see defines)
        public int dwLoops;         // loop control counter
        public IntPtr lpNext;       // PWaveHdr, reserved for driver
        public int reserved;        // reserved for driver
    }

    [StructLayout(LayoutKind.Sequential)]
    public class WaveFormat {
        public short wFormatTag;        //should be WAVE_FORMAT_PCM (0x01
        public short nChannels;         //channels to record (probably 1)
        public int nSamplesPerSec;      //11025, 22050, 44100, 88200
        public int nAvgBytesPerSec;     //bits/sample * channels * samples/second
        public short nBlockAlign;       //channels * bits/8
        public short wBitsPerSample;    //8-bit, 16-bit, 24-bit
        public short cbSize;            //chunk size (how many bytes of data)

        public WaveFormat(int rate, int bits, int channels) {
            wFormatTag = (short)1;
            nChannels = (short)channels;
            nSamplesPerSec = rate;
            wBitsPerSample = (short)bits;
            cbSize = 0;

            nBlockAlign = (short)(channels * (bits / 8));
            nAvgBytesPerSec = nSamplesPerSec * nBlockAlign;
        }
    }

    class WinmmHook {
        // consts
        public const int MMSYSERR_NOERROR = 0; // no error

        public const int MM_WOM_OPEN = 0x3BB;
        public const int MM_WOM_CLOSE = 0x3BC;
        public const int MM_WOM_DONE = 0x3BD;

        public const int CALLBACK_FUNCTION = 0x00030000;    // dwCallback is a FARPROC 

        public const int WAVE_MAPPER = -1;

        public const int TIME_MS = 0x0001;  // time in milliseconds 
        public const int TIME_SAMPLES = 0x0002;  // number of wave samples 
        public const int TIME_BYTES = 0x0004;  // current byte offset 

        // callbacks
        public delegate void WaveDelegate(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2);
        
        private const string mmdll = "winmm.dll";
        // native calls
        [DllImport(mmdll)]
        public static extern int waveOutGetNumDevs();
        [DllImport(mmdll)]
        public static extern int waveOutPrepareHeader(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, int uSize);
        [DllImport(mmdll)]
        public static extern int waveOutUnprepareHeader(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, int uSize);
        [DllImport(mmdll)]
        public static extern int waveOutWrite(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, int uSize);
        [DllImport(mmdll)]
        public static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, WaveFormat lpFormat, WaveDelegate dwCallback, int dwInstance, int dwFlags);
        [DllImport(mmdll)]
        public static extern int waveOutReset(IntPtr hWaveOut);
        [DllImport(mmdll)]
        public static extern int waveOutClose(IntPtr hWaveOut);
        [DllImport(mmdll)]
        public static extern int waveOutPause(IntPtr hWaveOut);
        [DllImport(mmdll)]
        public static extern int waveOutRestart(IntPtr hWaveOut);
        [DllImport(mmdll)]
        public static extern int waveOutGetPosition(IntPtr hWaveOut, out int lpInfo, int uSize);
        [DllImport(mmdll)]
        public static extern int waveOutSetVolume(IntPtr hWaveOut, int dwVolume);
        [DllImport(mmdll)]
        public static extern int waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);
    }
}
