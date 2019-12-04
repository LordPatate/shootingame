using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace shootingame
{
    public class Menu : Popup
    {
        public Menu(string title, params string[] options)
        {
            Rect = Geometry.ScaleRect(new IntRect(0, 0, width: (int)Screen.Width, height: (int)Screen.Height), 60, 80);
            Text = new PopupText() {
                Lines = new string[] {title},
                FontSize = Screen.FontSize*2
            };
            Options = new PopupOption[options.Length];

            Text.LineHeight = Screen.FontSize*2 + 2;
            Text.SideMargin = 10;
            int textMaxHeight = Rect.Height*20/100;
            Text.TopSpace = textMaxHeight/2 - Text.LineHeight/2;

            var buttonBox = new IntRect(
                left: Rect.Left + Rect.Width*10/100,
                top: Rect.Top + textMaxHeight,
                width: Rect.Width * 80 / 100,
                height: Rect.Height*80/100 - 10
            );
            int height = buttonBox.Height / options.Length;
            var buttonSpaces = new IntRect[options.Length];
            for (int i = 0; i < options.Length; ++i)
            {
                var space = buttonSpaces[i];
                space.Left = buttonBox.Left;
                space.Top = buttonBox.Top + height*i;
                space.Width = buttonBox.Width;
                space.Height = height;

                Options[i] = new PopupOption() {
                    Rect = Geometry.ScaleRect(space, 90, 60),
                    Text = options[i],
                    FontSize = Screen.FontSize
                };
            }

            CreateTextures();
        }
    }
}