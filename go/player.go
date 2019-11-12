package main

import (
	"math"

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
	HookPoint   *sdl.Point
	JumpEnabled bool
}

var playerStateDim map[PlayerState]sdl.Point = map[PlayerState]sdl.Point{
	Idle: {X: 20, Y: 37}, Running: {X: 20, Y: 37}, Jumping: {X: 20, Y: 37}, Falling: {X: 20, Y: 37}, WallSliding: {X: 20, Y: 37}, WallJumping: {X: 20, Y: 37},
}

func scale(x int32) int32 {
	return x * PlayerScalePercent / 100
}

func CreatePlayer(screen *Screen_t) *Player_t {
	player := &Player_t{
		Rect:        &sdl.Rect{},
		Inertia:     &sdl.Point{},
		JumpEnabled: true,
	}
	dim := playerStateDim[player.State]
	player.Rect.W, player.Rect.H = scale(dim.X), scale(dim.Y)

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
		X: player.Rect.X + player.Rect.W/2 - scale(PlayerSpriteWidth)/2,
		Y: player.Rect.Y + player.Rect.H/2 - scale(PlayerSpriteHeight)/2,
		W: scale(PlayerSpriteWidth), H: scale(PlayerSpriteHeight),
	}
	if err := screen.Renderer.CopyEx(player.Texture, player.TextureArea, dst, 0, nil, flip); err != nil {
		panic(err)
	}
	if player.HookPoint != nil {
		if err := screen.Renderer.SetDrawColor(255, 0, 0, 255); err != nil {
			panic(err)
		}
		playerCOM := player.GetCOM()
		if err := screen.Renderer.DrawLine(playerCOM.X, playerCOM.Y, player.HookPoint.X, player.HookPoint.Y); err != nil {
			panic(err)
		}
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
	mouseX, mouseY, mouseState := sdl.GetMouseState()
	if mouseState&sdl.ButtonRMask() != 0 {
		if player.HookPoint == nil {
			if hookPoint, ok := player.hook(level, mouseX, mouseY); ok {
				player.HookPoint = hookPoint
			}
		}
		if player.HookPoint != nil {
			if Dist(player.GetCOM(), player.HookPoint) <= HookMaxRange {
				player.swing()
				player.MoveX(player.Inertia.X/InertiaPerPixel, level)
				player.MoveY(player.Inertia.Y/InertiaPerPixel, level)
				return
			}
			player.HookPoint = nil
		}
	} else {
		player.HookPoint = nil
	}

	state := sdl.GetKeyboardState()
	if !player.JumpEnabled && state[sdl.SCANCODE_W] != 1 {
		player.JumpEnabled = true
	}

	if player.Inertia.Y >= 0 {
		ground := player.getGround(level)
		if ground != -1 {
			player.Rect.Y = ground - player.Rect.H
			player.Inertia.Y = 0
			player.GroundControl(state, level)
			return
		}

		player.SetState(Falling)
	}

	player.AirControl(state, level)

	player.Inertia.Y += Gravity
	if player.Inertia.X > 0 {
		player.Inertia.X -= AirSlow
		player.Inertia.X = Max32(player.Inertia.X, 0)
	} else {
		player.Inertia.X += AirSlow
		player.Inertia.X = Min32(player.Inertia.X, 0)
	}
	player.MoveX(player.Inertia.X/InertiaPerPixel, level)
	player.MoveY(player.Inertia.Y/InertiaPerPixel, level)
}

func (player *Player_t) MoveX(delta int32, level *Level_t) {
	projection := &sdl.Rect{
		X: player.Rect.X + delta,
		Y: player.Rect.Y, W: player.Rect.W, H: player.Rect.H,
	}
	wallslide := func() {
		if player.State == Falling || player.State == WallSliding {
			player.SetState(WallSliding)
			player.Inertia.Y -= WallFriction
		}
	}

	player.Rect.X = func() int32 {
		for _, tile := range level.Tiles {
			if projection.HasIntersection(tile.Rect) {
				wallslide()
				if delta > 0 {
					player.Inertia.X = InertiaPerPixel
					return tile.Rect.X - player.Rect.W
				}
				player.Inertia.X = -InertiaPerPixel
				return tile.Rect.X + tile.Rect.W
			}
		}
		union := projection.Union(level.Bounds)
		if !union.Equals(level.Bounds) {
			wallslide()
			if delta > 0 {
				player.Inertia.X = InertiaPerPixel
				return level.Bounds.X + level.Bounds.W - player.Rect.W
			}
			player.Inertia.X = -InertiaPerPixel
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
				if delta > 0 {
					return tile.Rect.Y
				}
				return tile.Rect.Y + tile.Rect.H
			}
		}
		union := projection.Union(level.Bounds)
		if !union.Equals(level.Bounds) {
			player.Inertia.Y = 0
			if delta > 0 {
				return level.Bounds.Y + level.Bounds.H
			}
			return level.Bounds.Y
		}

		return player.Rect.Y + delta
	}()
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

func (player *Player_t) hook(level *Level_t, mouseX, mouseY int32) (hookPoint *sdl.Point, ok bool) {
	playerCOM := player.GetCOM()
	projX, projY := PointShade(level.Bounds, playerCOM, mouseX, mouseY)
	point := &sdl.Point{projX, projY}
	minDist := Dist(playerCOM, point)
	if minDist <= HookMaxRange {
		ok = true
		hookPoint = point
	}

	for _, tile := range level.Tiles {
		rect := AdaptRect(tile.Rect)
		if rect.IntersectLine(playerCOM.X, playerCOM.Y, projX, projY) {
			point = rect.HitPoint(playerCOM, &sdl.Point{projX, projY})
			dist := Dist(playerCOM, point)
			if dist <= HookMaxRange && dist < minDist {
				minDist = dist
				hookPoint = point
				ok = true
			}
		}
	}

	return
}

func (player *Player_t) swing() {
	x, y := player.HookPoint.X, player.HookPoint.Y
	playerCOM := player.GetCOM()
	player.SetState(WallJumping)
	cos := Cos(playerCOM, x, y)
	sin := Sin(playerCOM, x, y)
	if sin > 0 {
		return
	}
	pullX := int32(math.Round(Gravity * math.Abs(cos)))
	if cos < 0 {
		player.Inertia.X -= pullX
	} else {
		player.Inertia.X += pullX
	}
	pullY := int32(math.Round(Gravity * 2 * math.Abs(sin)))
	player.Inertia.Y -= pullY
	player.Inertia.Y += Gravity
}

func (player *Player_t) GetCOM() *sdl.Point {
	playerRect := player.Rect
	return &sdl.Point{
		X: playerRect.X + playerRect.W/2,
		Y: playerRect.Y + playerRect.H/2,
	}
}
