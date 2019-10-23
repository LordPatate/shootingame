package main

import (
	"github.com/veandco/go-sdl2/img"
	"github.com/veandco/go-sdl2/sdl"
)

type PlayerState uint8

const (
	Idle PlayerState = iota
	Running
	Jumping
	Falling
	WallSliding
	WallJumping
)

type Player_t struct {
	Rect        *sdl.Rect
	Texture     *sdl.Texture
	TextureArea *sdl.Rect
	State       PlayerState
	Frame       uint8
	Left        bool
	Inertia     *sdl.Point
}

func CreatePlayer(rect *sdl.Rect, imgPath string, screen *Screen_t) *Player_t {
	player := &Player_t{
		Rect:    rect,
		Inertia: &sdl.Point{},
	}

	surface, err := img.Load(imgPath)
	if err != nil {
		panic(err)
	}
	player.Texture, err = screen.Renderer.CreateTextureFromSurface(surface)
	if err != nil {
		panic(err)
	}
	player.TextureArea = &sdl.Rect{
		W: PlayerSpriteWidth,
		H: PlayerSpriteHeight,
	}

	return player
}

func (player *Player_t) Copy(screen *Screen_t) {
	player.setTextureArea()
	flip := sdl.FLIP_NONE
	if player.Left {
		flip = sdl.FLIP_HORIZONTAL
	}
	if err := screen.Renderer.CopyEx(player.Texture, player.TextureArea, player.Rect, 0, nil, flip); err != nil {
		panic(err)
	}
}

func (player *Player_t) GetRect() *sdl.Rect {
	return player.Rect
}

func (player *Player_t) Destroy() {
	player.Texture.Destroy()
}

func (player *Player_t) SetState(state PlayerState) {
	if player.State != state {
		player.State = state
		player.Frame = 0
	}
}

func (player *Player_t) Update(screen *Screen_t, level *Level_t) {
	if player.Inertia.Y >= 0 {
		ground := player.getGround(level)
		if ground != -1 {
			player.Rect.Y = ground - player.Rect.H
			player.Inertia.Y = 0
			player.groundControl()
			return
		}

		player.SetState(Falling)
	}

	player.Inertia.Y += int32(Gravity)
	player.Rect.Y += player.Inertia.Y / 10
}

func (player *Player_t) Step(left bool) {
	player.SetState(Running)

	if left {
		player.Rect.X -= PlayerStep
	} else {
		player.Rect.X += PlayerStep
	}
	player.Left = left
}

func (player *Player_t) groundControl() {
	state := sdl.GetKeyboardState()

	if state[sdl.SCANCODE_W] == 1 {
		player.SetState(Jumping)
		player.Inertia.Y = -JumpPower
		return
	}

	var movement func(*Player_t)
	moveKeysPressed := 0
	for key := 0; key < sdl.NUM_SCANCODES; key++ {
		if state[key] == 1 {
			m, present := Movements[sdl.Scancode(key)]
			if present {
				moveKeysPressed++
				movement = m
			}
		}
	}
	if moveKeysPressed == 1 {
		movement(player)
	} else {
		player.SetState(Idle)
	}
}

func (player *Player_t) getGround(level *Level_t) int32 {
	groundHitBox := &sdl.Rect{
		X: player.Rect.X,
		Y: player.Rect.Y + player.Rect.H + 1,
		W: player.Rect.W, H: player.Inertia.Y + 1,
	}
	for _, tile := range level.Tiles {
		if groundHitBox.HasIntersection(tile.Rect) {
			return tile.Rect.Y
		}
	}
	if !groundHitBox.HasIntersection(level.Bounds) {
		return level.Bounds.Y + level.Bounds.H
	}
	return -1
}
