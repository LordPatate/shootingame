using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;

namespace shootingame
{
    class Screen
    {
        public struct ShadePolygon {int[] vx, vy;}
        public static RenderWindow window;
        public static uint width, height;
        public static Texture gameScene;
        public static Font font;
        public static List<ShadePolygon> shades;

        
        public static void Init()
        {
            VideoMode videoMode = VideoMode.GetFullScreenModes()[0];
            // Create the main window
            RenderWindow window = new RenderWindow(
                videoMode,
                "Shootingame",
                Style.Fullscreen
            );
            window.Closed += new EventHandler(OnClose);
            width = videoMode.width; height = videoMode.height;

            GameScene = new Texture(width, height);
            
            // err = SDL.SDL_SetRenderTarget(Renderer, GameScene); Errors.Check(err);

            // Font = SDL_ttf.TTF_OpenFont(Const.FontFile, Const.FontSize);
        }

        private static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
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