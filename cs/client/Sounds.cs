using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SFML.Audio;

namespace shootingame
{
    class Sounds
    {
        public static SoundBuffer pop = makeSound("pop"),
        shot = makeSound("shot"),
        tic = makeSound("tic"),
        hook = makeSound("hook"),
        run = makeSound("run"),
        slide = makeSound("slide");

        public static void Init() {
            longSounds.AddRange(new Sound[] {
                new Sound(hook),
                new Sound(run),
                new Sound(slide)
            });
            foreach (var s in longSounds) {
                s.Loop = true;
            }
        }
        public static void Update() {
            shortSounds.RemoveAll((sound) => sound.Status == SoundStatus.Stopped);
        }
        public static void PlayShort(SoundBuffer buf) {
            Sound s = new Sound(buf);
            shortSounds.Add(s);
            s.Play();
        }
        public static void PlayLong(SoundBuffer buf) {
            Sound s = longSounds.Find((sound) => sound.SoundBuffer == buf);
            if (s.Status != SoundStatus.Playing)
                s.Play();
        }
        public static void PauseLong(SoundBuffer buf) {
            Sound s = longSounds.Find((sound) => sound.SoundBuffer == buf);
            s.Pause();
        }
        public static void PlayFor(SoundBuffer buf, int duration) {
            Sound s = longSounds.Find((sound) => sound.SoundBuffer == buf);
            if (s.Status != SoundStatus.Playing) {
                Task.Run(() => {
                    s.Play();
                    Thread.Sleep(duration);
                    s.Pause();
                });
            }
        }
        
        private static SoundBuffer makeSound(string name) {
            return new SoundBuffer(Const.SoundFolder + name + ".wav");
        }
        private static List<Sound> shortSounds = new List<Sound>();
        private static List<Sound> longSounds = new List<Sound>();
    }
}