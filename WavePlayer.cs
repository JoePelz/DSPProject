using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Media;

namespace Comp3931_Project_JoePelz {
    class WavePlayer {
        private WaveFile wave;
        private MemoryStream audioStream = null;
        private SoundPlayer player;
        private long startTime, timeElapsed, duration;
        public bool playing = false;

        public WavePlayer(WaveFile src) {
            
            setWave(src);
        }

        public void setWave(WaveFile src) {
            if (audioStream != null) {
                audioStream.Dispose();
                audioStream = null;
            }
            if (player != null) {
                player.Dispose();
                player = null;
            }

            wave = src;
            audioStream = new MemoryStream();
            wave.writeStream(audioStream);
            player = new SoundPlayer(audioStream);
            player.Stream.Seek(0, SeekOrigin.Begin);
            startTime = -1;
            timeElapsed = -1;
            duration = (long)(wave.getDuration() * 10000000);
        }

        public void Dispose() {
            if (audioStream != null) {
                audioStream.Dispose();
            }
            if (player != null) {
                player.Dispose();
            }
        }

        public void Stop() {
            if (player != null) {
                player.Stop();
                player.Stream.Seek(0, SeekOrigin.Begin); // rewind stream
            }
            timeElapsed = -1;
            startTime = -1;
            playing = false;
        }

        public bool isInRange() {
            if (timeElapsed == -1)
                return (DateTime.UtcNow.Ticks - startTime) < duration;
            if (playing) {
                return timeElapsed + (DateTime.UtcNow.Ticks - startTime) < duration;
            } else {
                return timeElapsed < duration;
            }
        }

        public void Play() {
            if (player == null) {
                return;
            }

            if (playing && !isInRange()) {
                Stop(); //rewind the wave file
            }

            startTime = DateTime.UtcNow.Ticks;
            player.Play();
            playing = true;
        }
    }
}
