using System;
using System.IO;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace shootingame
{
    public class LevelInfos
    {
	public static string[] Init()
	{
	    DirectoryInfo levelDir = null;
	    var dir = new DirectoryInfo(@".");
	    DirectoryInfo[] directories;
	    for (int i = 0; i < 3; ++i)
	    {
		directories = dir.GetDirectories("*levels");
		if (directories.Length != 0) {
		    levelDir = directories[0];
		    break;
		}
		dir = dir.Parent;
	    }

	    if (levelDir == null)
		throw new DirectoryNotFoundException("unable to locate 'levels' folder");

	    FileInfo[] levels = levelDir.GetFiles();
	    string[] sourceFiles = new string[levels.Length];
	    for (int i = 0; i < levels.Length; ++i) {
		sourceFiles[i] = levels[i].FullName;
	    }

	    return sourceFiles;
	}
    }

    public class Tile
    {
        public IntRect Rect;
        public Tile(int x = 0, int y = 0) {
            Rect = new IntRect(x, y, Const.TileWidth, Const.TileHeight);
        }
    }

    public class Level
    {
        public IntRect Bounds;
        public List<Vector2i> SpawnPoints;
        public List<Tile> Tiles;

	public Level(LevelUpdateRequest state)
	{
	    Bounds = new IntRect(
		left: 0,
		top: 0,
		width: state.Dimensions.X,
		height: state.Dimensions.Y
	    );
	    Tiles = new List<Tile>();
	    for (int i = 0; i < state.TilePos.Length; ++i)
		Tiles.Add(new Tile(state.TilePos[i].X, state.TilePos[i].Y));
	    SpawnPoints = new List<Vector2i>();
	    for (int i = 0; i < state.SpawnPoints.Length; ++i)
		SpawnPoints.Add(new Vector2i(state.SpawnPoints[i].X, state.SpawnPoints[i].Y));
	}

        public Level(string sourceFile)
        {
            Image image = new Image(sourceFile);
            Vector2u size = image.Size;
            
            Bounds = new IntRect(
                left: 0,
                top: 0,
                width: (int)size.X*Const.TileWidth,
                height: (int)size.Y*Const.TileHeight
            );
            Tiles = new List<Tile>();
            SpawnPoints = new List<Vector2i>();

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
                        SpawnPoints.Add(new Vector2i(
                            x: Bounds.Left + i*Const.TileWidth,
                            y: Bounds.Top + j*Const.TileHeight
                        ));
                    }
                }
            if (SpawnPoints.Count == 0) {
                throw new Exception($"Invalid file \"{sourceFile}\": no player spawn point");
            }
        }
    }
}
