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
                FontSize = Const.FontSize*2/3
            };
            Options = new PopupOption[options.Length];

            Text.LineHeight = 30;
            Text.SideMargin = 10;
            int textMaxHeight = Rect.Height*20/100;
            Text.TopSpace = textMaxHeight/2 - Text.LineHeight/2;

            var buttonBox = new IntRect(
                left: Rect.Left + Rect.Width*10/100,
                top: Rect.Top + textMaxHeight,
                width: Rect.Width * 80 / 100,
                height: Rect.Height*80/100 - 10
            );
            int width = buttonBox.Width / options.Length;
            var buttonSpaces = new IntRect[options.Length];
            for (int i = 0; i < options.Length; ++i)
            {
                var space = buttonSpaces[i];
                space.Left = buttonBox.Left + width*i;
                space.Top = buttonBox.Top;
                space.Width = width;
                space.Height = buttonBox.Height;

                Options[i] = new PopupOption() {
                    Rect = Geometry.ScaleRect(space, 75, 90),
                    Text = options[i],
                    FontSize = Const.FontSize
                };
            }

            CreateTextures();
        }
    }
}