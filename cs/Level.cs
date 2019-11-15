using System;
using System.Collections.Generic;
using SDL2;

namespace shootingame
{
    struct LevelInfos
    {
        public string SourceFile;
        public string BackgroundImg;
        public string ForegroundImg;

        public LevelInfos(string src, string bg = "", string fg = "") {
            SourceFile = src; BackgroundImg = bg; ForegroundImg = fg;
        }
    }

    class Tile
    {
        public SDL.SDL_Rect Rect;
        public Tile(int x = 0, int y = 0) {
            Rect = SDLFactory.MakeRect(x, y, Const.TileWidth, Const.TileHeight);
        }
    }

    unsafe class Level
    {
        public static readonly LevelInfos[] levelInfos = {
            new LevelInfos( // Level 0
                src: "../levels/level0.png"
            ),
        };

        public uint id;
        public SDL.SDL_Rect Bounds;
        public SDL.SDL_Point PlayerStartPos;
        public List<Tile> Tiles;

        public void Init(LevelInfos infos)
        {
            IntPtr surfacePtr = SDL_image.IMG_Load(infos.SourceFile);
            if (surfacePtr == IntPtr.Zero) {
                throw new Exception("Problem loading image");
            }
            SDL.SDL_Surface surface = *(SDL.SDL_Surface*)surfacePtr.ToPointer();
            SDL.SDL_PixelFormat format = *(SDL.SDL_PixelFormat*)surface.format.ToPointer();

            Bounds = SDLFactory.MakeRect(
                x: Screen.Width/2 - surface.w*Const.TileWidth/2,
                y: Screen.Height/2 - surface.h*Const.TileHeight/2,
                w: surface.w*Const.TileWidth, h: surface.h*Const.TileHeight
            );
            bool startPosFound = false;

            for (int i = 0; i < surface.w; ++i)
                for (int j = 0; j < surface.h; ++j)
                {
                    byte* pixels = (byte*)surface.pixels.ToPointer();
                    int x = j*surface.pitch + i*format.BytesPerPixel;
                    byte r = pixels[x], g = pixels[x+1], b = pixels[x+2], a = pixels[x+3];

                    if ((r|g|b) == 0 && (a) == 255) { // black pixel
                        Tiles.Add(new Tile(
                            x: Bounds.x + i*Const.TileWidth,
                            y: Bounds.y + j*Const.TileHeight
                        ));
                        continue;
                    }
                    if ((r&a) == 255 && (g|b) == 0) { // red pixel
                        if (startPosFound) {
                            throw new Exception($"Invalid file \"{infos.SourceFile}\": too many player spawn points");
                        }
                        PlayerStartPos = SDLFactory.MakePoint(
                            x: Bounds.x + i*Const.TileWidth,
                            y: Bounds.y + j*Const.TileHeight - Const.PlayerSpriteHeight
                        );
                        startPosFound = true;
                    }
                }
            if (!startPosFound) {
                throw new Exception($"Invalid file \"{infos.SourceFile}\": no player spawn point");
            }

            SDL.SDL_FreeSurface(surfacePtr);
        }
    }
}