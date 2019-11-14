using System;
using SDL2;

namespace shootingame
{
    unsafe class Screen
    {
        public struct ShadePolygon {int[] vx, vy;}
        public static IntPtr Window;
        public static IntPtr Renderer;
        public static IntPtr GameScene;
        public static IntPtr Font;
        public static ShadePolygon Shades;

        
        public static void Init()
        {
            SDL.SDL_CreateWindowAndRenderer(Const.WindowWidth, Const.WindowHeight,
                SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP,
                out Window, out Renderer);
            
            SDL.SDL_SetWindowTitle(Window, "Shootingame");

            SDL.SDL_SetRenderDrawBlendMode(Renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

            GameScene = SDL.SDL_CreateTexture(Renderer, SDL.SDL_PIXELFORMAT_RGBA8888,
                (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET,
                Const.WindowWidth, Const.WindowHeight);
            
            SDL.SDL_SetRenderTarget(Renderer, GameScene);

            Font = SDL_ttf.TTF_OpenFont(Const.FontFile, Const.FontSize);
        }

        public static void Quit()
        {
            SDL.SDL_DestroyWindow(Window);
            SDL.SDL_DestroyRenderer(Renderer);
            SDL.SDL_DestroyTexture(GameScene);
            SDL_ttf.TTF_CloseFont(Font);
        }

        public static void Update()
        {
            
        }
    }
}