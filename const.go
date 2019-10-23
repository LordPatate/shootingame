package main

import (
	"time"

	"github.com/veandco/go-sdl2/img"
)

const (
	Red   = 0x00ff0000
	Green = 0x0000ff00
	Blue  = 0x000000ff

	GameStepDuration = 10 * time.Millisecond
	StepsPerFrame    = 10

	WindowWidth  = 1120
	WindowHeight = 630

	CharacterWidth, CharacterHeight = 50, 37
	PlayerStep                      = 4

	FontFile = "resources/fonts/DejaVuSans.ttf"
	FontSize = 18

	imgFlags = img.INIT_PNG

	PlayerSpriteSheet = "resources/sprites/player/adventurer-v1.5-Sheet.png" // see playeranimations.go
)
