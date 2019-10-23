package main

import (
	"time"

	"github.com/veandco/go-sdl2/img"
)

const (
	Red, Green, Blue = 0x00ff0000, 0x0000ff00, 0x000000ff

	GameStepDuration = 10 * time.Millisecond
	StepsPerFrame    = 10
	PlayerStep       = 4

	WindowWidth, WindowHeight = 1120, 630

	PlayerSpriteSheet                     = "resources/sprites/player/adventurer-v1.5-Sheet.png" // see playeranimations.go
	PlayerSpriteWidth, PlayerSpriteHeight = 50, 37

	TileWidth, TileHeight = 25, 25

	FontFile, FontSize = "resources/fonts/DejaVuSans.ttf", 18

	imgFlags = img.INIT_PNG
)
