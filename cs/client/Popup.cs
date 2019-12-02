using System;
using System.Threading;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace shootingame
{
    public class PopupText
    {
        public string[] Lines;
        public uint FontSize;
        public int LineHeight;
        public int SideMargin;
        public int TopSpace;
    }
    public class PopupOption
    {
        public string Text;
        public uint FontSize;
        public RenderTexture Texture;
        public IntRect Rect;
    }
    public class Popup
    {
        public static int ActivePopups = 0;
        public PopupText Text;
        public RenderTexture Texture;
        public IntRect Rect;
        public PopupOption[] Options;
        public bool IsActive;
        private PopupOption ClickedOption;
        private IntRect PreviousRect;
        private string ChosenOption;

        protected Popup() {}
        public Popup(string[] text, params string[] options)
        {
            Rect = Geometry.ScaleRect(new IntRect(0, 0, width: (int)Screen.Width, height: (int)Screen.Height), 80, 80);
            Text = new PopupText() { Lines = text, FontSize = Const.FontSize };
            Options = new PopupOption[options.Length];
            IsActive = false;
            ChosenOption = null;

            Text.LineHeight = 20;
            Text.SideMargin = 10;
            int maxHeight = Rect.Height*80/100;
            int paragraphSize = text.Length*Text.LineHeight;
            Text.TopSpace = maxHeight/2 - paragraphSize/2;

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

                Options[i] = new PopupOption() {
                    Rect = Geometry.ScaleRect(space, 75, 90),
                    Text = options[i],
                    FontSize = Const.FontSize
                };
            }

            CreateTextures();
        }

        public void Pop()
        {
            ClickedOption = null;
            PreviousRect = Options[0].Rect;
            
            Screen.Window.MouseButtonPressed += OnClick;
            Screen.Window.MouseButtonReleased += OnRelease;
            
            ChosenOption = null;
            IsActive = true;
            ++ActivePopups;
        }

        public void Display()
        {
            Screen.Window.Draw(new Sprite(Screen.GameScene.Texture));
                        
            Screen.Window.Draw(Drawing.SpriteOf(Texture, Rect));
            
            foreach (PopupOption option in Options) {
                Screen.Window.Draw(Drawing.SpriteOf(option.Texture, option.Rect));
            }

            Screen.Window.Display();
        }
        public string GetChoice()
        {
            if (ChosenOption == null) {
                return null;
            }
            string opt = ChosenOption;
            ChosenOption = null;
            return opt;
        }

        private void OnClick(object sender, MouseButtonEventArgs mbe)
        {            
            if (mbe.Button == Mouse.Button.Right){
                SetChosenOption("right click");
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
                            SetChosenOption(opt.Text);
                        
                        return;
                    }
                }
                ClickedOption = null;
            }
        }

        private void SetChosenOption(string text)
        {
            Screen.Window.MouseButtonPressed -= OnClick;
            Screen.Window.MouseButtonReleased -= OnRelease;
            IsActive = false;
            --ActivePopups;
            ChosenOption = text;
        }

        protected void CreateTextures()
        {
            var bgColor = new Color(red: 110, green: 100, blue: 100);

            // frame
            Texture = new RenderTexture((uint)Rect.Width, (uint)Rect.Height);
            Texture.Clear(bgColor);

            // text body
            int i = 0;
            foreach (string line in Text.Lines)
            {
                var rect = new IntRect(
                    left: Text.SideMargin,
                    top: Text.TopSpace + i*Text.LineHeight, 
                    width: Rect.Width - 2*Text.SideMargin,
                    height: Text.LineHeight
                );
                Drawing.DrawText(
                    Texture, line, rect,
                    new Color(0, 0, 0), bgColor, Text.FontSize
                );

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
                Drawing.DrawText(
                    option.Texture, option.Text, rect,
                    buttonFG, buttonBG, option.FontSize
                );
            }
        }
    }
}