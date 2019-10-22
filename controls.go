package main

import "github.com/veandco/go-sdl2/sdl"

var Movements map[sdl.Scancode]func(*Game_t) = map[sdl.Scancode]func(*Game_t){
	sdl.SCANCODE_A: PlayerStepLeft,
	sdl.SCANCODE_D: PlayerStepRight,
}
