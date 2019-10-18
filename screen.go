package main

import "github.com/veandco/go-sdl2/sdl"

type Screen_t struct {
	Window *sdl.Window
}

func CreateScreen() (screen *Screen_t, err error) {
	screen = &Screen_t{}
	screen.Window, err = sdl.CreateWindow("shootingame", sdl.WINDOWPOS_UNDEFINED, sdl.WINDOWPOS_UNDEFINED,
		WindowWidth, WindowHeight, sdl.WINDOW_SHOWN)
	if err != nil {
		return
	}

	return
}

func (screen *Screen_t) Destroy() {
	screen.Window.Destroy()
}

func (screen *Screen_t) Update() {

	//Test space
	surface, err := screen.Window.GetSurface()
	if err != nil {
		panic(err)
	}
	surface.FillRect(nil, 0)

	rect := sdl.Rect{X: 0, Y: 0, W: 200, H: 200}
	surface.FillRect(&rect, FromRGB(255, 255, 0))

	//Test space end

	screen.Window.UpdateSurface()
}
