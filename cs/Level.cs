using System;
using System.Collections.Generic;
using SDL2;

namespace shootingame
{
    struct LevelInfos
    {
        string SourceFile;
        string BackgroundImg;
        string ForegroundImg;
    }

    class Tile
    {
        public SDL.SDL_Rect Rect;
        public Tile(int X = 0, int Y = 0) {
            Rect = SDLFactory.MakeRect(X, Y, Const.TileWidth, Const.TileHeight);
        }
    }

    class Level
    {
        public const LevelInfos levelInfos = {
            { // Level 0
                "../levels/level0.png",
                "", ""
            },
        };

        public uint id;
        public SDL.SDL_Rect Bounds;
        public SDL.SDL_Point PlayerStartPos;
        public List<Tile> Tiles;

        public void Init(LevelInfos infos)
        {
            SDL.SDL_Surface surface = SDL_image.IMG_Load(infos.SourceFile);

            Bounds = SDLFactory.MakeRect(
                X: Screen.Width/2 - surface.W*Const.TileWidth/2,
                Y: Screen.Height/2 - surface.H*Const.TileHeight/2,
                W: surface.W*Const.TileWidth, H: surface.H*Const.TileHeight
            );

            for (int i = 0; i < surface.W; ++i)
                for (int j = 0; j < surface.H; ++j)
                {
                    pixels = surface.Pixels();
                    int x = j*surface.Pitch + i*surface.Format.BytesPerPixel;
                    int r = pixels[x], g = pixels[x+1], b = pixels[x+2], a = pixels[x+3];

                    if (r|g|b == 0 && a == 255) { // black pixel
                        Tiles.Add(new Tile(
                            X: Bounds.X + i*Const.TileWidth,
                            Y: Bounds.Y + j*TileHeight
                        ));
                        continue;
                    }
                    if (r&a == 255 && g|b == 0) { //red pixel
                        if (PlayerStartPos != null) {
                            throw new Exception($"Invalid file \"{infos.SourceFile}\": too many player spawn points");
                        }
                        PlayerStartPos = SDLFactory.MakePoint(
                            X: Bounds.X + i*Const.TileWidth,
                            Y: Bounds.Y + j*Const.TileHeight - Const.PlayerSpriteHeight
                        );
                    }
                }
            if (PlayerStartPos == null) {
                throw new Exception($"Invalid file \"{infos.SourceFile}\": no player spawn point");
            }
        }
    }
}