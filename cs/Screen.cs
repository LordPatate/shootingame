using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;

namespace shootingame
{
    class Screen
    {
        public struct ShadePolygon {int[] vx, vy;}
        public static RenderWindow Window;
        public static uint Width, Height;
        public static Texture GameScene;
        public static Font Font;
        public static List<ShadePolygon> Shades;

        
        public static void Init()
        {
            VideoMode videoMode = VideoMode.FullscreenModes[0];
            // Create the main window
            Window = new RenderWindow(
                videoMode,
                "Shootingame",
                Styles.Fullscreen
            );
            Window.Closed += new EventHandler(OnClose);
            Width = videoMode.Width; Height = videoMode.Height;

            GameScene = new Texture(Width, Height);
            
            // err = SDL.SDL_SetRenderTarget(Renderer, GameScene); Errors.Check(err);

            // Font = SDL_ttf.TTF_OpenFont(Const.FontFile, Const.FontSize);
        }

        private static void OnClose(object sender, EventArgs e)
        {
            if (Game.AskQuit.Pop() == "Yes")
                Game.Running = false;
        }
        public static void Quit()
        {
            // SDL.SDL_DestroyWindow(Window);
            // SDL.SDL_DestroyRenderer(Renderer);
            // SDL.SDL_DestroyTexture(GameScene);
            // SDL_ttf.TTF_CloseFont(Font);
        }

        public static void Update()
        {
            // int err; Errors.msg = "Screen.Update";
            // err = SDL.SDL_SetRenderTarget(Renderer, GameScene); Errors.Check(err);
            // err = SDL.SDL_RenderCopy(Renderer, Game.Background, IntPtr.Zero, IntPtr.Zero); Errors.Check(err);
            Game.Player.Copy();
            CastShadows();

            // err = SDL.SDL_SetRenderTarget(Renderer, IntPtr.Zero); Errors.Check(err);
            // err = SDL.SDL_RenderCopy(Renderer, GameScene, IntPtr.Zero, IntPtr.Zero); Errors.Check(err);
            // SDL.SDL_RenderPresent(Renderer);
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