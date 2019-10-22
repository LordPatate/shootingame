package main

import "github.com/veandco/go-sdl2/sdl"

const left, right = true, false

var Movements map[sdl.Scancode]func(*Game_t) = map[sdl.Scancode]func(*Game_t){
	sdl.SCANCODE_A: func(game *Game_t) { game.Player.Step(left) },
	sdl.SCANCODE_D: func(game *Game_t) { game.Player.Step(right) },
}
