package main

import (
	"errors"
	"sync"

	"github.com/veandco/go-sdl2/sdl"
	"github.com/veandco/go-sdl2/ttf"
)

type Screen_t struct {
	Window      *sdl.Window
	Renderer    *sdl.Renderer
	GameScene   *sdl.Texture
	Font        *ttf.Font
	BlackPoints []bool
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
	screen.castShadows(game)

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
	bounds := game.Level.Bounds
	playerRect := game.Player.Rect
	playerEye := &sdl.Point{
		X: playerRect.X + playerRect.W/2,
		Y: playerRect.Y + 10,
	}
	isHidden := func(x, y int32) bool {
		for _, tile := range game.Level.Tiles {
			if PointInRectangle(x, y, tile.Rect) {
				return false
			}
		}
		for _, tile := range game.Level.Tiles {
			rect := AdaptRect(tile.Rect)
			if rect.IntersectLine(x, y, playerEye.X, playerEye.Y) {
				return true
			}
		}
		return false
	}
	w, h := bounds.W, bounds.H
	blackPoints := make([]bool, w*h)
	chunksPerThreads := int32(80)
	wg := sync.WaitGroup{}
	wg.Add(int(w / chunksPerThreads))
	routine := func(start, end int32) {
		for x := start; x < end; x += 3 {
			for y := bounds.Y; y < bounds.Y+h-3; y += 3 {
				if isHidden(x, y) {
					i, j := y-bounds.Y, x-bounds.X
					blackPoints = matrixTrueSquare(blackPoints, w, h, j, i)
				}
			}
		}
		wg.Done()
	}
	var start, end int32
	for start = bounds.X; start < bounds.X+w; start += chunksPerThreads {
		end = start + chunksPerThreads
		go routine(start, end)
	}
	wg.Wait()
	screen.BlackPoints = blackPoints
}

func (screen *Screen_t) castShadows(game *Game_t) {
	screen.Renderer.SetDrawColor(0, 0, 0, 200)

	bounds := game.Level.Bounds
	w, h := bounds.W, bounds.H
	for i := int32(0); i < h; i++ {
		for j := int32(0); j < w; j++ {
			if screen.BlackPoints[j+i*w] {
				x, y := j+bounds.X, i+bounds.Y
				screen.Renderer.DrawPoint(x, y)
			}
		}
	}
}

func matrixTrueSquare(mat []bool, w, h, x, y int32) []bool {
	for i := y; i < y+3; i++ {
		for j := x; j < x+3; j++ {
			mat[i*w+j] = true
		}
	}
	return mat
}
