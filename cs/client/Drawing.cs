using SFML.Graphics;
using SFML.System;

namespace shootingame
{
    enum TextAlignment
    {
        Left, Center, Right
    }
    class Drawing
    {
        public static RectangleShape SpriteOf(Texture texture, IntRect rect)
        {
            return SpriteOf(WithTexture(texture), rect);
        }
        public static RectangleShape SpriteOf(RenderTexture render, IntRect rect)
        {
            render.Display();
            return SpriteOf(WithTexture(render.Texture), rect);
        }
        public static RectangleShape SpriteOf(Texture texture, IntRect rect, IntRect textureRect)
        {
            return SpriteOf(WithTexture(texture), rect, textureRect);
        }
        public static RectangleShape SpriteOf(RenderTexture render, IntRect rect, IntRect textureRect)
        {
            render.Display();
            return SpriteOf(WithTexture(render.Texture), rect, textureRect);
        }

        public static RectangleShape SpriteOf(RectangleShape shape, IntRect rect, IntRect textureRect)
        {
            shape.TextureRect = textureRect;
            
            return SpriteOf(shape, rect);
        }
        public static RectangleShape SpriteOf(RectangleShape shape, IntRect rect)
        {
            shape.Position = new Vector2f((float)rect.Left, (float)rect.Top);
            shape.Size = new Vector2f((float)rect.Width, (float)rect.Height);
            
            return shape;
        }
        public static RectangleShape WithTexture(Texture texture)
        {
            var shape = new RectangleShape();
            shape.Texture = texture;
            
            return shape;
        }


        public static void DrawText(
            RenderTarget dst, string line, IntRect frame,
            Color fg, Color bg, uint fontSize, TextAlignment alignment = TextAlignment.Center
        )
        {
            Text text = new Text(line, Screen.Font, fontSize*Screen.Height/600);
            text.FillColor = fg;
            text.OutlineColor = bg;

            FloatRect rect = text.GetLocalBounds();
            switch (alignment)
            {
                case TextAlignment.Left:
                    text.Position = new Vector2f(
                        x: frame.Left,
                        y: frame.Top
                    );
                    break;
                case TextAlignment.Center:
                    text.Position = new Vector2f(
                        x: frame.Left + frame.Width/2 - rect.Width/2,
                        y: frame.Top + frame.Height/2 - rect.Height/2
                    );
                    break;
                case TextAlignment.Right:
                    text.Position = new Vector2f(
                        x: frame.Left + frame.Width - rect.Width,
                        y: frame.Top + frame.Height - rect.Height
                    );
                    break;
            }
            
            dst.Draw(text);
        }
    }
}