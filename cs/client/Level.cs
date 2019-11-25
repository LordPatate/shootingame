using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace shootingame
{
    struct LevelInfos
    {
        public string SourceFile;
        public string BackgroundImg;
        public string ForegroundImg;

        public LevelInfos(string src, string bg = "", string fg = "") {
            SourceFile = Const.LevelFolder + src; BackgroundImg = bg; ForegroundImg = fg;
        }
    }

    class Tile
    {
        public IntRect Rect;
        public Tile(int x = 0, int y = 0) {
            Rect = new IntRect(x, y, Const.TileWidth, Const.TileHeight);
        }
    }

    class Level
    {
        public static readonly LevelInfos[] levelInfos = {
            new LevelInfos( // Level 0
                src: "level0.png"
            ),
        };

        public IntRect Bounds;
        public Vector2i PlayerStartPos;
        public List<Tile> Tiles;

        public void Init(LevelInfos infos)
        {
            Image image = new Image(infos.SourceFile);
            Vector2u size = image.Size;
            
            Bounds = new IntRect(
                left: (int)Screen.Width/2 - (int)size.X*Const.TileWidth/2,
                top: (int)Screen.Height/2 - (int)size.Y*Const.TileHeight/2,
                width: (int)size.X*Const.TileWidth,
                height: (int)size.Y*Const.TileHeight
            );
            Tiles = new List<Tile>();
            
            bool startPosFound = false;

            for (int i = 0; i < size.X; ++i)
                for (int j = 0; j < size.Y; ++j)
                {
                    byte[] pixels = image.Pixels;
                    long x = (j*size.X + i)*4;
                    byte r = pixels[x], g = pixels[x+1], b = pixels[x+2], a = pixels[x+3];

                    if ((r|g|b) == 0 && (a) == 255) { // black pixel
                        Tiles.Add(new Tile(
                            x: Bounds.Left + i*Const.TileWidth,
                            y: Bounds.Top + j*Const.TileHeight
                        ));
                        continue;
                    }
                    if ((r&a) == 255 && (g|b) == 0) { // red pixel
                        if (startPosFound) {
                            throw new Exception($"Invalid file \"{infos.SourceFile}\": too many player spawn points");
                        }
                        PlayerStartPos = new Vector2i(
                            x: Bounds.Left + i*Const.TileWidth,
                            y: Bounds.Top + j*Const.TileHeight
                        );
                        startPosFound = true;
                    }
                }
            if (!startPosFound) {
                throw new Exception($"Invalid file \"{infos.SourceFile}\": no player spawn point");
            }
        }
    }
}