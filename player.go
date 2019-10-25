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
	Direction   bool
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
	if player.Direction == Left {
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
			player.groundControl(state, level)
			return
		}

		player.SetState(Falling)
	}

	player.airControl(state, level)

	player.Inertia.Y += Gravity
	player.MoveY(player.Inertia.Y/InertiaPerPixel, level)
	if player.Inertia.X > 0 {
		player.Inertia.X -= AirSlow
		player.Inertia.X = Max32(player.Inertia.X, 0)
	} else {
		player.Inertia.X += AirSlow
		player.Inertia.X = Min32(player.Inertia.X, 0)
	}
	player.MoveX(player.Inertia.X/InertiaPerPixel, level)
}

func (player *Player_t) MoveX(delta int32, level *Level_t) {
	projection := &sdl.Rect{
		X: player.Rect.X + delta,
		Y: player.Rect.Y, W: player.Rect.W, H: player.Rect.H,
	}

	player.Rect.X = func() int32 {
		for _, tile := range level.Tiles {
			if projection.HasIntersection(tile.Rect) {
				if delta > 0 {
					return tile.Rect.X - player.Rect.W
				}
				return tile.Rect.X + tile.Rect.W
			}
		}
		union := projection.Union(level.Bounds)
		if !union.Equals(level.Bounds) {
			if delta > 0 {
				return level.Bounds.X + level.Bounds.W - player.Rect.W
			}
			return level.Bounds.X
		}

		return player.Rect.X + delta
	}()
}

func (player *Player_t) MoveY(delta int32, level *Level_t) {
	projection := &sdl.Rect{
		Y: player.Rect.Y + delta,
		X: player.Rect.X, W: player.Rect.W, H: player.Rect.H,
	}

	player.Rect.Y = func() int32 {
		for _, tile := range level.Tiles {
			if projection.HasIntersection(tile.Rect) {
				player.Inertia.Y = 0
				return tile.Rect.Y + tile.Rect.H
			}
		}
		union := projection.Union(level.Bounds)
		if !union.Equals(level.Bounds) {
			player.Inertia.Y = 0
			return level.Bounds.Y
		}

		return player.Rect.Y + delta
	}()
}

func (player *Player_t) Step(direction bool, level *Level_t) {
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

func (player *Player_t) groundControl(keyState []uint8, level *Level_t) {
	if keyState[sdl.SCANCODE_W] == 1 {
		player.SetState(Jumping)
		player.Inertia.Y = -JumpPower
		return
	}

	var step func(*Player_t, *Level_t)
	moveKeysPressed := 0
	for _, key := range []uint{sdl.SCANCODE_A, sdl.SCANCODE_D} {
		if keyState[key] == 1 {
			step = GroundSteps[sdl.Scancode(key)]
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

func (player *Player_t) airControl(keyState []uint8, level *Level_t) {
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
	union := groundHitBox.Union(level.Bounds)
	if !union.Equals(level.Bounds) {
		return level.Bounds.Y + level.Bounds.H
	}
	return -1
}
