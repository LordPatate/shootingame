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
            Player.Destroy();
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
            LevelInfos infos = Level.levelInfos[id];
            Level.Init(infos);

            Player.Rect.X = Level.PlayerStartPos.X;
            Player.Rect.Y = Level.PlayerStartPos.Y;

            Background = SDL.SDL_CreateTexture(Screen.Renderer, SDL.SDL_PIXELFORMAT_RGBA8888, SDL.SDL_TEXTUREACCESS_TARGET,
                Screen.Width, Screen.Height);
            SDL.SDL_SetRenderTarget(Screen.Renderer, Background);

            DrawBackground(infos.BackgroundImg, infos.ForegroundImg);

            SDL.SDL_SetRenderTarget(Screen.Renderer, null);
        }

        private static void DrawBackground(string bg, string fg)
        {
            string foreground = GetTexture(fg, 20, 17, 23);
            string background = GetTexture(bg, 65, 60, 55);

            if (bg == "") {
                var rect = SDLFactory.MakeRect(W: Screen.Width, H: Screen.Height);
                SDL.SDL_RenderCopy(Screen.Renderer, foreground, null, rect);
                SDL.SDL_RenderCopy(Screen.Renderer, background, null, Game.Level.Bounds);
            }

            foreach (var tile in Game.Level.Tiles)
                SDL.SDL_RenderCopy(Screen.Renderer, foreground, null, tile.Rect);
            
            SDL.SDL_DestroyTexture(foreground);
            SDL.SDL_DestroyTexture(background);
        }

        private static IntPtr GetTexture(string src, uint defaultR, uint defaultG, uint defaultB)
        {
            SDL.SDL_Surface surface;
            if (src != "")
            {
                surface = SDL.SDL_CreateRGBSurface(0, Const.TileWidth, Const.TileHeight, 32, 0, 0, 0, 0);
                SDL.SDL_FillRect(surface, null, SDL.SDL_MapRGB(surface.format, defaultR, defaultG, defaultB));
            }
            else
            {
                // FIXME
            }

            IntPtr texture = SDL.SDL_CreateTextureFromSurface(surface);
            SDL.SDL_FreeSurface(surface);
        }
    }
}