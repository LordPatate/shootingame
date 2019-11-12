package main

import (
	"errors"

	"github.com/veandco/go-sdl2/img"
	"github.com/veandco/go-sdl2/sdl"
)

type LevelInfos_t struct {
	SourceFile    string
	BackgroundImg string
	ForegroundImg string
}

var LevelInfos []LevelInfos_t = []LevelInfos_t{
	{ // Level 0
		SourceFile:    "levels/level0.png",
		BackgroundImg: "",
		ForegroundImg: "",
	},
}

type Level_t struct {
	ID             uint8
	Bounds         *sdl.Rect
	PlayerStartPos *sdl.Point
	Tiles          []Tile_t
}

type Tile_t struct {
	Rect *sdl.Rect
}

func (level *Level_t) Init(infos LevelInfos_t) {
	surface, err := img.Load(infos.SourceFile)
	if err != nil {
		panic(err)
	}

	bounds := &sdl.Rect{
		X: WindowWidth/2 - surface.W*TileWidth/2,
		Y: WindowHeight/2 - surface.H*TileHeight/2,
		W: surface.W * TileWidth, H: surface.H * TileHeight,
	}
	level.Bounds = bounds

	for i := 0; int32(i) < surface.W; i++ {
		for j := 0; int32(j) < surface.H; j++ {
			pixels := surface.Pixels()
			x := int32(j)*surface.Pitch + int32(i)*int32(surface.Format.BytesPerPixel)
			r, g, b, a := pixels[x], pixels[x+1], pixels[x+2], pixels[x+3]
			if r|g|b == 0 && a == 255 { // black pixel
				level.Tiles = append(level.Tiles, Tile_t{
					Rect: &sdl.Rect{
						X: bounds.X + int32(i*TileWidth),
						Y: bounds.Y + int32(j*TileHeight),
						W: TileWidth, H: TileHeight,
					},
				})
				continue
			}
			if r&a == 255 && g|b == 0 { // red pixel
				if level.PlayerStartPos != nil {
					panic(errors.New("Invalid file: too many player spawn points"))
				}
				level.PlayerStartPos = &sdl.Point{
					X: bounds.X + int32(i*TileWidth),
					Y: bounds.Y + int32(j*TileHeight) - PlayerSpriteHeight,
				}
			}
		}
	}
	if level.PlayerStartPos == nil {
		panic(errors.New("Invalid file: no player spawn point"))
	}
}
