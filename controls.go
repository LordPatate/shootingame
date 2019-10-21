package main

import (
	"github.com/veandco/go-sdl2/img"
	"github.com/veandco/go-sdl2/sdl"
)

type Player_t struct {
	Rect        *sdl.Rect
	Texture     *sdl.Texture
	TextureArea *sdl.Rect
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

func (player *Player_t) Copy(screen Screen_t) {
	if err := screen.Renderer.SetRenderTarget(screen.Background); err != nil {
		panic(err)
	}
	if err := screen.Renderer.Copy(player.Texture, player.TextureArea, player.Rect); err != nil {
		panic(err)
	}
}

func (player *Player_t) GetRect() *sdl.Rect {
	return player.Rect
}

func (player *Player_t) Destroy() {
	player.Texture.Destroy()
}

func (player *Player_t) Step(left bool) {
	// FIXME
}
