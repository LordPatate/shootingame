package main

import (
	"errors"
	"math"

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
	topLeft, topRight := &sdl.Point{bounds.X, bounds.Y}, &sdl.Point{bounds.X + bounds.W, bounds.Y}
	botLeft, botRight := &sdl.Point{bounds.X, bounds.Y + bounds.H}, &sdl.Point{bounds.X + bounds.W, bounds.Y + bounds.H}
	playerRect := game.Player.Rect
	playerEye := &sdl.Point{
		X: playerRect.X + playerRect.W/2,
		Y: playerRect.Y + 10,
	}

	for _, tile := range game.Level.Tiles {
		// Compute the shade produced by each diagonal

		rect := tile.Rect
		x, y, w, h := rect.X, rect.Y, rect.W, rect.H
		var x1, x2, y1, y2 int32
		var s struct{ vx, vy []int16 }
		// Checks if the corner specified is inside the shadow
		checkCorner := func(corner *sdl.Point, above, right bool) {
			square := func(x float64) float64 { return x * x }
			computeAngle := func(x, y int32) float64 {
				val := math.Acos(float64(x-playerEye.X) / math.Sqrt(square(float64(x-playerEye.X))+square(float64(y-playerEye.Y))))
				if playerEye.Y < y {
					val = -val
				}
				if val < 0 && right {
					val += math.Pi * 2
				}
				return val
			}
			upper := computeAngle(x1, y1)
			lower := computeAngle(x2, y2)
			angle := computeAngle(corner.X, corner.Y)
			if above {
				if upper < angle && angle < lower {
					s.vx = append(s.vx, int16(corner.X))
					s.vy = append(s.vy, int16(corner.Y))
				}
				return
			}
			if upper > angle && angle > lower {
				s.vx = append(s.vx, int16(corner.X))
				s.vy = append(s.vy, int16(corner.Y))
			}
		}

		// TopLeft - BotRight diagonal
		x1, y1 = PointShade(bounds, playerEye, x, y)
		x2, y2 = PointShade(bounds, playerEye, x+w, y+h)
		s = struct{ vx, vy []int16 }{
			vx: []int16{int16(x1), int16(x), int16(x + w), int16(x2)},
			vy: []int16{int16(y1), int16(y), int16(y + h), int16(y2)},
		}
		above := playerEye.Y < playerEye.X+(y-x)
		checkCorner(botRight, above, above)
		if above {
			checkCorner(botLeft, above, above)
		} else {
			checkCorner(topRight, above, above)
		}
		checkCorner(topLeft, above, above)

		shades = append(shades, s)

		// BotLeft - TopRight diagonal
		x1, y1 = PointShade(bounds, playerEye, x, y+h)
		x2, y2 = PointShade(bounds, playerEye, x+w, y)
		s = struct{ vx, vy []int16 }{
			vx: []int16{int16(x1), int16(x), int16(x + w), int16(x2)},
			vy: []int16{int16(y1), int16(y + h), int16(y), int16(y2)},
		}
		above = playerEye.Y < -playerEye.X+(y+h+x)
		checkCorner(topRight, above, !above)
		if above {
			checkCorner(botRight, above, !above)
		} else {
			checkCorner(topLeft, above, !above)
		}
		checkCorner(botLeft, above, !above)

		shades = append(shades, s)
	}

	screen.Shades = shades
}

func (screen *Screen_t) castShadows() {
	for _, shade := range screen.Shades {
		gfx.FilledPolygonRGBA(screen.Renderer, shade.vx, shade.vy, 15, 15, 15, 255)
	}
}
