using System;
using SDL2;

namespace shootingame
{
    unsafe class Game
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
            Level = new Level();
            AskQuit = new Popup(
                new string[] {
                    "Do you really want to quit?",
                }, "Yes", "No"
            );

            LoadLevel(0);
        }

        public static void Quit()
        {
            Player.Destroy();
            SDL.SDL_DestroyTexture(Background);
            AskQuit.Destroy();
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
            LevelInfos infos = Level.levelInfos[id];
            Level.Init(infos);

            Player.Rect.x = Level.PlayerStartPos.x;
            Player.Rect.y = Level.PlayerStartPos.y;

            Background = SDL.SDL_CreateTexture(Screen.Renderer, SDL.SDL_PIXELFORMAT_RGBA8888,
                (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET,
                Screen.Width, Screen.Height);
            SDL.SDL_SetRenderTarget(Screen.Renderer, Background);

            DrawBackground(infos.BackgroundImg, infos.ForegroundImg);

            SDL.SDL_SetRenderTarget(Screen.Renderer, IntPtr.Zero);
        }

        private static void DrawBackground(string bg, string fg)
        {
            IntPtr foreground = GetTexture(fg, 20, 17, 23);
            IntPtr background = GetTexture(bg, 65, 60, 55);

            if (bg == "") {
                var rect = SDLFactory.MakeRect(w: Screen.Width, h: Screen.Height);
                SDL.SDL_RenderCopy(Screen.Renderer, foreground, IntPtr.Zero, ref rect);
                SDL.SDL_RenderCopy(Screen.Renderer, background, IntPtr.Zero, ref Game.Level.Bounds);
            }

            foreach (var tile in Game.Level.Tiles)
                SDL.SDL_RenderCopy(Screen.Renderer, foreground, IntPtr.Zero, ref tile.Rect);
            
            SDL.SDL_DestroyTexture(foreground);
            SDL.SDL_DestroyTexture(background);
        }

        private static IntPtr GetTexture(string src, byte defaultR, byte defaultG, byte defaultB)
        {
            IntPtr surfacePtr = IntPtr.Zero;
            SDL.SDL_Surface surface;
            if (src != "")
            {
                surfacePtr = SDL.SDL_CreateRGBSurface(0, Const.TileWidth, Const.TileHeight, 32, 0, 0, 0, 0);
                surface = *(SDL.SDL_Surface*)surfacePtr.ToPointer();
                SDL.SDL_FillRect(surfacePtr, IntPtr.Zero, SDL.SDL_MapRGB(surface.format, defaultR, defaultG, defaultB));
            }
            else
            {
                // FIXME
            }

            IntPtr texture = SDL.SDL_CreateTextureFromSurface(Screen.Renderer, surfacePtr);
            SDL.SDL_FreeSurface(surfacePtr);

            return texture;
        }
    }
}