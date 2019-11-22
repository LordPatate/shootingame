using System;
using System.Threading;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace shootingame
{
    class PopupOption
    {
        public string Text;
        public RenderTexture Texture;
        public IntRect Rect;
    }
    class Popup
    {
        public string[] Text;
        public RenderTexture Texture;
        public IntRect Rect;
        public PopupOption[] Options;
        private PopupOption ClickedOption;
        private IntRect PreviousRect;
        private string ChosenOption;

        public Popup(string[] text, params string[] options)
        {
            Text = text;
            Rect = Geometry.ScaleRect(new IntRect(0, 0, width: (int)Screen.Width, height: (int)Screen.Height), 80, 80);
            Options = new PopupOption[options.Length];

            var buttonRow = new IntRect(
                left: Rect.Left + Rect.Width*5/100,
                top: Rect.Top + Rect.Height*80/100,
                width: Rect.Width * 90 / 100,
                height: 60
            );
            int width = buttonRow.Width / options.Length;
            var buttonSpaces = new IntRect[options.Length];
            for (int i = 0; i < options.Length; ++i)
            {
                var space = buttonSpaces[i];
                space.Left = buttonRow.Left + width*i;
                space.Top = buttonRow.Top;
                space.Width = width;
                space.Height = buttonRow.Height;

                Options[i] = new PopupOption();
                Options[i].Rect = Geometry.ScaleRect(space, 75, 90);
                Options[i].Text = options[i];
            }

            CreateTextures();
        }

        public string Pop()
        {
            ClickedOption = null;
            PreviousRect = Options[0].Rect;
            
            Screen.Window.MouseButtonPressed += OnClick;
            Screen.Window.MouseButtonReleased += OnRelease;
            
            ChosenOption = "";
            while (ChosenOption == "")
            {
                Display();

                Screen.Window.DispatchEvents();

                Thread.Sleep(Const.GameStepDuration);
            }

            Screen.Window.MouseButtonPressed -= OnClick;
            Screen.Window.MouseButtonReleased -= OnRelease;

            return ChosenOption;
        }

        public void Destroy()
        {
            
        }

        private void Display()
        {
            Screen.Window.Draw(new Sprite(Screen.GameScene.Texture));
                        
            Screen.Window.Draw(Drawing.SpriteOf(Texture, Rect));
            
            foreach (PopupOption option in Options) {
                Screen.Window.Draw(Drawing.SpriteOf(option.Texture, option.Rect));
            }

            Screen.Window.Display();
        }

        private void OnClick(object sender, MouseButtonEventArgs mbe)
        {            
            if (mbe.Button == Mouse.Button.Right){
                ChosenOption = "right click";
                return;
            }
            if (mbe.Button == Mouse.Button.Left) {
                foreach (PopupOption opt in Options) {
                    if (opt.Rect.Contains(mbe.X, mbe.Y)) {
                        PreviousRect = opt.Rect;
                        opt.Rect = Geometry.ScaleRect(opt.Rect, 95, 95);
                        ClickedOption = opt;
                        return;
                    }
                }
            }
        }
        private void OnRelease(object sender, MouseButtonEventArgs mbe)
        {            
            if (mbe.Button == Mouse.Button.Left) {
                if (ClickedOption != null) {
                    ClickedOption.Rect = PreviousRect;
                }
                foreach (PopupOption opt in Options) {
                    if (opt.Rect.Contains(mbe.X, mbe.Y)) {
                        if (opt == ClickedOption)
                            ChosenOption = opt.Text;
                        
                        return;
                    }
                }
                ClickedOption = null;
            }
        }

        private void CreateTextures()
        {
            var bgColor = new Color(red: 110, green: 100, blue: 100);

            // frame
            Texture = new RenderTexture((uint)Rect.Width, (uint)Rect.Height);
            Texture.Clear(bgColor);

            // text body
            int lineHeight = 20;
            int sideMargin = 10;
            int topMargin = (Rect.Top*80/100)/2 - Text.Length*lineHeight/2;
            int i = 0;
            foreach (string line in Text)
            {
                var rect = new IntRect(
                    left: sideMargin,
                    top: topMargin + i*lineHeight, 
                    width: Rect.Width - 2*sideMargin,
                    height: lineHeight
                );
                DrawText(Texture, line, rect, new Color(0, 0, 0), bgColor);

                ++i;
            }

            // buttons
            var buttonFG = new Color(255, 255, 255);
            var buttonBG = new Color(0, 0, blue: 255);
            foreach (PopupOption option in Options)
            {
                option.Texture = new RenderTexture((uint)option.Rect.Width, (uint)option.Rect.Height);
                option.Texture.Clear(buttonBG);
                
                var rect = new IntRect(0, 0, width: option.Rect.Width, height: option.Rect.Height);
                DrawText(option.Texture, option.Text, rect, buttonFG, buttonBG);
            }
        }

        private static void DrawText(RenderTarget dst, string line, IntRect frame, Color fg, Color bg)
        {
            Text text = new Text(line, Screen.Font, Const.FontSize);
            text.FillColor = fg;
            text.OutlineColor = bg;

            FloatRect rect = text.GetLocalBounds();
            text.Position = new Vector2f(
                x: frame.Left + frame.Width/2 - rect.Width/2,
                y: frame.Top + frame.Height/2 - rect.Height/2
            );
            
            dst.Draw(text);
        }
    }
}