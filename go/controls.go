package main

import "github.com/veandco/go-sdl2/sdl"

var groundSteps map[sdl.Scancode]func(*Player_t, *Level_t) = map[sdl.Scancode]func(*Player_t, *Level_t){
	sdl.SCANCODE_A: func(player *Player_t, level *Level_t) { player.step(Left, level) },
	sdl.SCANCODE_D: func(player *Player_t, level *Level_t) { player.step(Right, level) },
}

func (player *Player_t) step(direction bool, level *Level_t) {
	player.SetState(Running)

	if direction == Left {
		player.MoveX(-PlayerStep, level)
		player.Inertia.X = -PlayerStep * InertiaPerPixel
	} else {
		player.MoveX(PlayerStep, level)
		player.Inertia.X = PlayerStep * InertiaPerPixel
	}
	player.Direction = direction
}

func (player *Player_t) GroundControl(keyState []uint8, level *Level_t) {
	if keyState[sdl.SCANCODE_W] == 1 && player.JumpEnabled {
		player.SetState(Jumping)
		player.Inertia.Y = -JumpPower
		player.JumpEnabled = false
		return
	}

	var step func(*Player_t, *Level_t)
	moveKeysPressed := 0
	for _, key := range []uint{sdl.SCANCODE_A, sdl.SCANCODE_D} {
		if keyState[key] == 1 {
			step = groundSteps[sdl.Scancode(key)]
			moveKeysPressed++
		}
	}
	if moveKeysPressed == 1 {
		step(player, level)
	} else {
		player.SetState(Idle)
		player.Inertia.X = 0
	}
}

func (player *Player_t) AirControl(keyState []uint8, level *Level_t) {
	if keyState[sdl.SCANCODE_A] == 1 {
		player.Inertia.X -= AirMovePower
		player.Inertia.X = Max32(player.Inertia.X, -PlayerStep*InertiaPerPixel)
	}
	if keyState[sdl.SCANCODE_D] == 1 {
		player.Inertia.X += AirMovePower
		player.Inertia.X = Min32(player.Inertia.X, PlayerStep*InertiaPerPixel)
	}

	collision := func(projection *sdl.Rect) bool {
		for _, tile := range level.Tiles {
			if projection.HasIntersection(tile.Rect) {
				return true
			}
		}
		union := projection.Union(level.Bounds)
		if !union.Equals(level.Bounds) {
			return true
		}
		return false
	}

	projectionLeft := &sdl.Rect{
		X: player.Rect.X - 1,
		Y: player.Rect.Y, W: player.Rect.W, H: player.Rect.H,
	}
	if collision(projectionLeft) && keyState[sdl.SCANCODE_W] == 1 && player.JumpEnabled {
		player.SetState(WallJumping)
		player.Inertia.Y = -JumpPower
		player.JumpEnabled = false
		player.Direction = Right
		player.Inertia.X = PlayerStep * InertiaPerPixel
	}
	projectionRight := &sdl.Rect{
		X: player.Rect.X + 1,
		Y: player.Rect.Y, W: player.Rect.W, H: player.Rect.H,
	}
	if collision(projectionRight) && keyState[sdl.SCANCODE_W] == 1 && player.JumpEnabled {
		player.SetState(WallJumping)
		player.Inertia.Y = -JumpPower
		player.JumpEnabled = false
		player.Direction = Left
		player.Inertia.X = -PlayerStep * InertiaPerPixel
	}
}
