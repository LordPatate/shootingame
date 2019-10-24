package main

import "github.com/veandco/go-sdl2/sdl"

const left, right = true, false

var GroundSteps map[sdl.Scancode]func(*Player_t, *Level_t) = map[sdl.Scancode]func(*Player_t, *Level_t){
	sdl.SCANCODE_A: func(player *Player_t, level *Level_t) { player.Step(left, level) },
	sdl.SCANCODE_D: func(player *Player_t, level *Level_t) { player.Step(right, level) },
}
