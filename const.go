package main

import "github.com/veandco/go-sdl2/img"

const (
	Red   = 0x00ff0000
	Green = 0x0000ff00
	Blue  = 0x000000ff

	WindowWidth  = 1120
	WindowHeight = 630

	CharacterWidth  = 37
	CharacterHeight = 50

	FontFile = "resources/fonts/DejaVuSans.ttf"
	FontSize = 18

	imgFlags = img.INIT_PNG

	PlayerSpriteSheet = "resources/sprites/player/adventurer-v1.5-Sheet.png"
)
