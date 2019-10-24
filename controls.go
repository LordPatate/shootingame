package main

import "github.com/veandco/go-sdl2/sdl"

const left, right = true, false

var GroundSteps map[sdl.Scancode]func(*Player_t) = map[sdl.Scancode]func(*Player_t){
	sdl.SCANCODE_A: func(player *Player_t) { player.Step(left) },
	sdl.SCANCODE_D: func(player *Player_t) { player.Step(right) },
}
