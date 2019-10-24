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

var playerStateDim map[PlayerState]sdl.Point = map[PlayerState]sdl.Point{
	Idle: {X: 20, Y: 37}, Running: {X: 20, Y: 37}, Jumping: {X: 20, Y: 37}, Falling: {X: 20, Y: 37}, WallSliding: {X: 20, Y: 37}, WallJumping: {X: 20, Y: 37},
}

func CreatePlayer(screen *Screen_t) *Player_t {
	player := &Player_t{
		Rect:    &sdl.Rect{},
		Inertia: &sdl.Point{},
	}
	dim := playerStateDim[player.State]
	player.Rect.W, player.Rect.H = dim.X, dim.Y

	surface, err := img.Load(PlayerSpriteSheet)
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
	dst := &sdl.Rect{
		X: player.Rect.X + player.Rect.W/2 - PlayerSpriteWidth/2,
		Y: player.Rect.Y + player.Rect.H/2 - PlayerSpriteHeight/2,
		W: PlayerSpriteWidth, H: PlayerSpriteHeight,
	}
	if err := screen.Renderer.CopyEx(player.Texture, player.TextureArea, dst, 0, nil, flip); err != nil {
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
	state := sdl.GetKeyboardState()

	if player.Inertia.Y >= 0 {
		ground := player.getGround(level)
		if ground != -1 {
			player.Rect.Y = ground - player.Rect.H
			player.Inertia.Y = 0
			player.Inertia.X = 0
			player.groundControl(state)
			return
		}

		player.SetState(Falling)
	}

	player.airControl(state)

	player.Inertia.Y += Gravity
	player.Rect.Y += player.Inertia.Y / InertiaPerPixel
	if player.Inertia.X > 0 {
		player.Inertia.X -= AirSlow
		player.Inertia.X = Max32(player.Inertia.X, 0)
	} else {
		player.Inertia.X += AirSlow
		player.Inertia.X = Min32(player.Inertia.X, 0)
	}
	player.Rect.X += player.Inertia.X / InertiaPerPixel
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

func (player *Player_t) groundControl(keyState []uint8) {
	if keyState[sdl.SCANCODE_W] == 1 {
		player.SetState(Jumping)
		player.Inertia.Y = -JumpPower
		if keyState[sdl.SCANCODE_A] == 1 {
			player.Inertia.X = -PlayerStep * InertiaPerPixel
		}
		if keyState[sdl.SCANCODE_D] == 1 {
			player.Inertia.X = PlayerStep * InertiaPerPixel
		}
		return
	}

	var step func(*Player_t)
	moveKeysPressed := 0
	for _, key := range []uint{sdl.SCANCODE_A, sdl.SCANCODE_D} {
		if keyState[key] == 1 {
			step = GroundSteps[sdl.Scancode(key)]
			moveKeysPressed++
		}
	}
	if moveKeysPressed == 1 {
		step(player)
	} else {
		player.SetState(Idle)
	}
}

func (player *Player_t) airControl(keyState []uint8) {
	if keyState[sdl.SCANCODE_A] == 1 {
		player.Inertia.X -= AirMovePower
		player.Inertia.X = Max32(player.Inertia.X, -PlayerStep*InertiaPerPixel)
	}
	if keyState[sdl.SCANCODE_D] == 1 {
		player.Inertia.X += AirMovePower
		player.Inertia.X = Min32(player.Inertia.X, PlayerStep*InertiaPerPixel)
	}
}

func (player *Player_t) getGround(level *Level_t) int32 {
	groundHitBox := &sdl.Rect{
		X: player.Rect.X,
		Y: player.Rect.Y + player.Rect.H + 1,
		W: player.Rect.W, H: player.Inertia.Y/InertiaPerPixel + 1,
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
