package main

import (
	"time"

	"github.com/veandco/go-sdl2/img"
)

const (
	Red, Green, Blue = 0x00ff0000, 0x0000ff00, 0x000000ff

	GameStepDuration = 10 * time.Millisecond
	StepsPerFrame    = 10

	WindowWidth, WindowHeight = 1120, 630

	CharacterWidth, CharacterHeight = 50, 37
	PlayerStep                      = 4

	FontFile, FontSize = "resources/fonts/DejaVuSans.ttf", 18

	imgFlags = img.INIT_PNG

	PlayerSpriteSheet = "resources/sprites/player/adventurer-v1.5-Sheet.png" // see playeranimations.go
)
