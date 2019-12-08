using SFML.Graphics;
using SFML.System;

namespace shootingame
{
    enum TextAlignment
    {
        Left, Center, Right
    }
    enum TextPosition
    {
	Top, Middle, Bottom
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
            Color fg, Color bg, uint fontSize = 0,
	    TextAlignment alignment = TextAlignment.Center, TextPosition position = TextPosition.Middle
        )
        {
	    if (fontSize == 0)
		fontSize = Screen.FontSize;

            Text text = new Text(line, Screen.Font, fontSize);
	    text.FillColor = fg;
	    text.OutlineColor = bg;

	    DrawText(dst, text, frame, alignment, position);
	}
	public static void DrawText(RenderTarget dst, Text text, IntRect frame,
	    TextAlignment alignment = TextAlignment.Center, TextPosition position = TextPosition.Middle
	)
	{
            FloatRect rect = text.GetLocalBounds();
	    float x = 0, y = 0;
            switch (alignment)
            {
                case TextAlignment.Left:
                    x = frame.Left;
                    break;
                case TextAlignment.Center:
                    x = (int)(frame.Left + frame.Width/2 - rect.Width/2);
                    break;
                case TextAlignment.Right:
                    x = (int)(frame.Left + frame.Width - rect.Width);
                    break;
            }
	    switch (position)
	    {
		case TextPosition.Top:
		    y = frame.Top;
		    break;
		case TextPosition.Middle:
		    y = (int)(frame.Top + frame.Height/2 - rect.Height/2);
		    break;
		case TextPosition.Bottom:
		    y = (int)(frame.Top + frame.Height - rect.Height) - 10;
		    break;
	    }
	    text.Position = new Vector2f(x, y);
            
            dst.Draw(text);
        }
    }
}
