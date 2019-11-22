using System.Drawing;
using SFML.Graphics;
using SFML.System;

namespace shootingame
{
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
    }
}