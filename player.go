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
}

func CreatePlayer(rect *sdl.Rect, imgPath string, screen *Screen_t) *Player_t {
	player := &Player_t{Rect: rect}

	surface, err := img.Load(imgPath)
	if err != nil {
		panic(err)
	}
	player.Texture, err = screen.Renderer.CreateTextureFromSurface(surface)
	if err != nil {
		panic(err)
	}
	player.TextureArea = &sdl.Rect{
		W: CharacterWidth,
		H: CharacterHeight,
	}

	return player
}

func (player *Player_t) Copy(screen *Screen_t) {
	if err := screen.Renderer.SetRenderTarget(screen.Background); err != nil {
		panic(err)
	}

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

func PlayerStepLeft(game *Game_t) {
	player := game.Player
	if player.State != Running {
		player.State = Running
		player.Frame = 0
	}

	player.Rect.X -= PlayerStep
	player.Left = true
}

func PlayerStepRight(game *Game_t) {
	player := game.Player
	if player.State != Running {
		player.State = Running
		player.Frame = 0
	}

	player.Rect.X += PlayerStep
	player.Left = false
}
