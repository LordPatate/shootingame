using System;
using SDL2;

namespace shootingame
{
    class Game
    {
        public static bool Running;
        public static Player Player;
        public static IntPtr Background;
        public static Level Level;
        private static Popup AskQuit;

        public static void Init()
        {
            Running = true;
            Player = new Player();
            LoadLevel(0);
            AskQuit = new Popup();
        }

        public static void Quit()
        {
            SDL.SDL_DestroyTexture(Background);
        }

        public static void Update()
        {
            SDL.SDL_Event e;
            while (SDL.SDL_PollEvent(out e) != 0)
            {
                switch (e.type) {
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE) {
                            if (AskQuit.Pop() == "yes") {
                                Running = false;
                            }
                            return;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_QUIT:
                        if (AskQuit.Pop() == "yes") {
                            Running = false;
                        }
                        return;
                }
            }

            Player.Update();
        }

        public static void LoadLevel(uint id)
        {

        }
    }
}