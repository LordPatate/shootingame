package main

import (
	"errors"

	"github.com/veandco/go-sdl2/gfx"
	"github.com/veandco/go-sdl2/sdl"
	"github.com/veandco/go-sdl2/ttf"
)

type Screen_t struct {
	Window    *sdl.Window
	Renderer  *sdl.Renderer
	GameScene *sdl.Texture
	Font      *ttf.Font
	Shades    []struct {
		vx, vy []int16
	}
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
	err = screen.Renderer.SetDrawBlendMode(sdl.BLENDMODE_BLEND)
	if err != nil {
		return
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
	if err := screen.Renderer.Copy(game.Background, nil, nil); err != nil {
		panic(err)
	}

	game.Player.Copy(screen)
	screen.castShadows()

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

func (screen *Screen_t) ComputeShadows(game *Game_t) {
	shades := []struct{ vx, vy []int16 }{}
	bounds := game.Level.Bounds
	playerRect := game.Player.Rect
	playerEye := &sdl.Point{
		X: playerRect.X + playerRect.W/2,
		Y: playerRect.Y + 10,
	}

	for _, tile := range game.Level.Tiles {

		// Compute the shade produced by each diagonal
		rect := tile.Rect
		x, y, w, h := rect.X, rect.Y, rect.W, rect.H
		vx := []int32{x, x + w}
		vy := []int32{y, y + h}
		for i := 0; i < 2; i++ {
			x1, y1 := PointShade(bounds, playerEye, vx[0], vy[i])
			x2, y2 := PointShade(bounds, playerEye, vx[1], vy[1-i])
			s := struct{ vx, vy []int16 }{
				vx: []int16{int16(vx[0]), int16(vx[1]), x2, x1},
				vy: []int16{int16(vy[i]), int16(vy[1-i]), y2, y1},
			}
			shades = append(shades, s)
		}
	}

	screen.Shades = shades
}

func (screen *Screen_t) castShadows() {
	for _, shade := range screen.Shades {
		gfx.FilledPolygonRGBA(screen.Renderer, shade.vx, shade.vy, 15, 15, 15, 255)
	}
}
