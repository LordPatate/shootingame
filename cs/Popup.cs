using System;
using System.Threading;
using SDL2;

namespace shootingame
{
    unsafe class PopupOption
    {
        public string Text;
        public IntPtr Texture;
        public SDL.SDL_Rect Rect;
    }
    unsafe class Popup
    {
        public string[] Text;
        public IntPtr Texture;
        public SDL.SDL_Rect Rect;
        public PopupOption[] Options;

        public Popup(string[] text, params string[] options)
        {
            Text = text;
            Rect = Geometry.ScaleRect(SDLFactory.MakeRect(w: Screen.Width, h: Screen.Height), 80, 80);
            Options = new PopupOption[options.Length];

            var buttonRow = SDLFactory.MakeRect(
                x: Rect.x + Rect.w*5/100,
                y: Rect.y + Rect.h*80/100,
                w: Rect.w * 90 / 100,
                h: 60
            );
            int width = buttonRow.w / options.Length;
            var buttonSpaces = new SDL.SDL_Rect[options.Length];
            for (int i = 0; i < options.Length; ++i)
            {
                var space = buttonSpaces[i];
                space.x = buttonRow.x + width*i;
                space.y = buttonRow.y;
                space.w = width;
                space.h = buttonRow.h;

                Options[i] = new PopupOption();
                Options[i].Rect = Geometry.ScaleRect(space, 75, 90);
                Options[i].Text = options[i];
            }

            CreateTextures();
        }

        public string Pop()
        {
            PopupOption clickedOption = null;
            SDL.SDL_Rect previousRect = Options[0].Rect;

            string option = "";

            while (option == "")
            {
                Display();

                Action inner = () => {
                    SDL.SDL_Event e;
                    while (SDL.SDL_PollEvent(out e) != 0) {
                        if ((int)e.type >> 8 == 0x4) // MouseEvent
                        {
                            var be = e.button;
                            if (be.button == SDL.SDL_BUTTON_RIGHT && e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN) {
                                option = "right click";
                                return;
                            }
                            if (be.button == SDL.SDL_BUTTON_LEFT)
                            {
                                int i = 0;
                                foreach (PopupOption opt in Options)
                                {
                                    if (Geometry.PointInRectangle(be.x, be.y, opt.Rect))
                                    {
                                        if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN) {
                                            clickedOption = Options[i];
                                            previousRect = opt.Rect;
                                            clickedOption.Rect = Geometry.ScaleRect(opt.Rect, 95, 95);
                                            return;
                                        }
                                        // e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONUP
                                        if (clickedOption != null && clickedOption.Text == opt.Text) {
                                            option = opt.Text;
                                            return;
                                        }
                                    }
                                    ++i;
                                }
                                if (clickedOption != null) {
                                    clickedOption.Rect = previousRect;
                                }
                                clickedOption = null;
                            }
                        }
                    }
                };
                inner();

                Thread.Sleep(Const.GameStepDuration);
            }

            return option;
        }

        public void Destroy()
        {
            SDL.SDL_DestroyTexture(Texture);
            foreach (PopupOption option in Options)
                if (option.Texture != IntPtr.Zero)
                    SDL.SDL_DestroyTexture(option.Texture);
        }

        private void Display()
        {
            int err; Errors.msg = "Popup.Display";
            
            err = SDL.SDL_RenderCopy(Screen.Renderer, Screen.GameScene, IntPtr.Zero, IntPtr.Zero); Errors.Check(err);
            err = SDL.SDL_RenderCopy(Screen.Renderer, Texture, IntPtr.Zero, ref Rect); Errors.Check(err);
            foreach (PopupOption option in Options) {
                err = SDL.SDL_RenderCopy(Screen.Renderer, option.Texture, IntPtr.Zero, ref option.Rect); Errors.Check(err);
            }

            SDL.SDL_RenderPresent(Screen.Renderer);
        }

        private void CreateTextures()
        {
            int err; Errors.msg = "Popup.CreateTextures";
            
            Texture = SDL.SDL_CreateTexture(Screen.Renderer, SDL.SDL_PIXELFORMAT_RGBA8888,
                (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET,
                Rect.w, Rect.h);
            Errors.CheckNull(Texture);
            err = SDL.SDL_SetRenderTarget(Screen.Renderer, Texture); Errors.Check(err);

            var bgColor = SDLFactory.MakeColor(r: 110, g: 100, b: 100);

            // frame
            err = SDL.SDL_SetRenderDrawColor(Screen.Renderer, bgColor.r, bgColor.g, bgColor.b, bgColor.a); Errors.Check(err);
            err = SDL.SDL_RenderFillRect(Screen.Renderer, IntPtr.Zero); Errors.Check(err);

            // text body
            int yPos = (Rect.h*80/100)/2 - Text.Length*20/2;
            int i = 0;
            foreach (string line in Text)
            {
                var rect = SDLFactory.MakeRect(y: yPos + i*20, w: Rect.w, h: 20);
                CopyText(line, rect, SDLFactory.MakeColor(), bgColor);

                ++i;
            }

            // buttons
            err = SDL.SDL_SetRenderDrawColor(Screen.Renderer, 0, 0, 255, 255); Errors.Check(err);
            i = 0;
            foreach (PopupOption option in Options)
            {
                Options[i].Texture = SDL.SDL_CreateTexture(Screen.Renderer, SDL.SDL_PIXELFORMAT_RGBA8888,
                    (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET,
                    option.Rect.w, option.Rect.h);
                Errors.CheckNull(Options[i].Texture);
                err = SDL.SDL_SetRenderTarget(Screen.Renderer, Options[i].Texture); Errors.Check(err);

                err = SDL.SDL_RenderFillRect(Screen.Renderer, IntPtr.Zero); Errors.Check(err);
                var rect = SDLFactory.MakeRect(w: option.Rect.w, h: option.Rect.h);
                CopyText(option.Text, rect, SDLFactory.MakeColor(255,255,255), SDLFactory.MakeColor(b:255));

                ++i;
            }

            err = SDL.SDL_SetRenderTarget(Screen.Renderer, IntPtr.Zero); Errors.Check(err);
        }

        private static void CopyText(string line, SDL.SDL_Rect frame, SDL.SDL_Color fg, SDL.SDL_Color bg)
        {
            int err; Errors.msg = "Popup.CopyText";
           
            IntPtr surfacePtr = SDL_ttf.TTF_RenderUTF8_Shaded(Screen.Font, line, fg, bg);
            Errors.CheckNull(surfacePtr);
            var surface = *(SDL.SDL_Surface*)surfacePtr.ToPointer();
            
            IntPtr texture = SDL.SDL_CreateTextureFromSurface(Screen.Renderer, surfacePtr);
            Errors.CheckNull(texture);
            var rect = SDLFactory.MakeRect(
                x: frame.x + 10 + (frame.w-20)/2 - surface.w/2,
                y: frame.y + frame.h/2 - surface.h/2,
                w: surface.w, h: surface.h
            );
            err = SDL.SDL_RenderCopy(Screen.Renderer, texture, IntPtr.Zero, ref rect); Errors.Check(err);
            
            SDL.SDL_FreeSurface(surfacePtr);
            SDL.SDL_DestroyTexture(texture);
        }
    }
}