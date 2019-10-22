package main

import (
	"errors"

	"github.com/veandco/go-sdl2/sdl"
	"github.com/veandco/go-sdl2/ttf"
)

type Screen_t struct {
	Window    *sdl.Window
	Renderer  *sdl.Renderer
	GameScene *sdl.Texture
	Font      *ttf.Font
}

func CreateScreen() (screen *Screen_t, err error) {
	screen = &Screen_t{}
	screen.Window, err = sdl.CreateWindow("Shootingame", sdl.WINDOWPOS_UNDEFINED, sdl.WINDOWPOS_UNDEFINED,
		WindowWidth, WindowHeight, sdl.WINDOW_SHOWN)
	if err != nil {
		return
	}
	if screen.Window == nil {
		err = errors.New("Failed to create Window")
	}

	screen.Renderer, err = sdl.CreateRenderer(screen.Window, -1, sdl.RENDERER_ACCELERATED)
	if err != nil {
		return
	}
	if screen.Renderer == nil {
		err = errors.New("Failed to create Renderer")
	}

	screen.GameScene, err = screen.Renderer.CreateTexture(sdl.PIXELFORMAT_RGBA8888, sdl.TEXTUREACCESS_TARGET, WindowWidth, WindowHeight)
	if err != nil {
		return
	}
	if screen.GameScene == nil {
		err = errors.New("Failed to create GameScene")
		return
	}
	err = screen.Renderer.SetRenderTarget(screen.GameScene)
	if err != nil {
		return
	}

	screen.Font, err = ttf.OpenFont(FontFile, FontSize)

	return
}

func (screen *Screen_t) Destroy() {
	screen.Window.Destroy()
	screen.Renderer.Destroy()
	screen.GameScene.Destroy()
	screen.Font.Close()
}

func (screen *Screen_t) Update(game *Game_t) {
	if err := screen.Renderer.SetDrawColor(0, 0, 0, 255); err != nil {
		panic(err)
	}
	if err := screen.Renderer.Clear(); err != nil {
		panic(err)
	}

	// Test space

	game.Player.Copy(screen)

	// Test space end

	if err := screen.Renderer.SetRenderTarget(nil); err != nil {
		panic(err)
	}
	if err := screen.Renderer.Copy(screen.GameScene, nil, nil); err != nil {
		panic(err)
	}
	screen.Renderer.Present()

	if err := screen.Renderer.SetRenderTarget(screen.GameScene); err != nil {
		panic(err)
	}
}
