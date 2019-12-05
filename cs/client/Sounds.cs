using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SFML.Audio;

namespace shootingame
{
    class Sounds
    {
        public static void Init() {
            var soundNames = new string[] {
                "pop", "shot", "tic", "hook", "run", "slide"
            };
            for (int i = 0; i < soundNames.Length; ++i) {
                buffers.Add(soundNames[i], makeBuffer(soundNames[i]));
            }

            longSounds.AddRange(new Sound[] {
                new Sound(buffers["hook"]),
                new Sound(buffers["run"]),
                new Sound(buffers["slide"])
            });
            foreach (var s in longSounds) {
                s.Loop = true;
            }
        }
        public static void Update() {
            shortSounds.RemoveAll((sound) => sound.Status == SoundStatus.Stopped);
        }
        public static void Quit() {
            shortSounds.Clear();
            longSounds.Clear();
            buffers.Clear();
        }
        public static void PlayShort(string name) {
            Sound s = new Sound(buffers[name]);
            shortSounds.Add(s);
            s.Play();
        }
        public static void PlayLong(string name) {
            Sound s = longSounds.Find((sound) => sound.SoundBuffer == buffers[name]);
            if (s.Status != SoundStatus.Playing)
                s.Play();
        }
        public static void PauseLong(string name) {
            Sound s = longSounds.Find((sound) => sound.SoundBuffer == buffers[name]);
            s.Pause();
        }
        public static void PlayFor(string name, int duration) {
            Sound s = longSounds.Find((sound) => sound.SoundBuffer == buffers[name]);
            if (s.Status != SoundStatus.Playing) {
                Task.Run(() => {
                    s.Play();
                    Thread.Sleep(duration);
                    s.Pause();
                });
            }
        }
        
        private static SoundBuffer makeBuffer(string name) {
            return new SoundBuffer(Const.SoundFolder + name + ".wav");
        }
        private static Dictionary<string, SoundBuffer> buffers = new Dictionary<string, SoundBuffer>();
        private static List<Sound> shortSounds = new List<Sound>();
        private static List<Sound> longSounds = new List<Sound>();
    }
}