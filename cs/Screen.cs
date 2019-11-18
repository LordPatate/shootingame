using System;
using System.Collections.Generic;
using SDL2;

namespace shootingame
{
    unsafe class Screen
    {
        public struct ShadePolygon {int[] vx, vy;}
        public static IntPtr Window;
        public static int Width, Height;
        public static IntPtr Renderer;
        public static IntPtr GameScene;
        public static IntPtr Font;
        public static List<ShadePolygon> Shades;

        
        public static void Init()
        {
            int err; Errors.msg = "Screen.Init";
            err = SDL.SDL_CreateWindowAndRenderer(Const.WindowWidth, Const.WindowHeight,
                SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP,
                out Window, out Renderer);
            Errors.Check(err);
            
            SDL.SDL_SetWindowTitle(Window, "Shootingame");
            SDL.SDL_GetWindowSize(Window, out Width, out Height);

            err = SDL.SDL_SetRenderDrawBlendMode(Renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND); Errors.Check(err);

            GameScene = SDL.SDL_CreateTexture(Renderer, SDL.SDL_PIXELFORMAT_RGBA8888,
                (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET,
                Const.WindowWidth, Const.WindowHeight);
            Errors.CheckNull(GameScene);
            
            err = SDL.SDL_SetRenderTarget(Renderer, GameScene); Errors.Check(err);

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
            int err; Errors.msg = "Screen.Update";
            err = SDL.SDL_SetRenderTarget(Renderer, GameScene); Errors.Check(err);
            err = SDL.SDL_RenderCopy(Renderer, Game.Background, IntPtr.Zero, IntPtr.Zero); Errors.Check(err);
            Game.Player.Copy();
            CastShadows();

            err = SDL.SDL_SetRenderTarget(Renderer, IntPtr.Zero); Errors.Check(err);
            err = SDL.SDL_RenderCopy(Renderer, GameScene, IntPtr.Zero, IntPtr.Zero); Errors.Check(err);
            SDL.SDL_RenderPresent(Renderer);
        }

        public static void ComputeShadows()
        {
            Shades = new List<ShadePolygon>();
        }

        public static void CastShadows()
        {
            foreach (ShadePolygon shade in Shades)
            {
                //SDL_gfx.SDL_FilledPolygonRGBA
            }
        }
    }
}