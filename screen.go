package main

import "github.com/veandco/go-sdl2/sdl"

type screen_t struct {
	Window *sdl.Window
}

func createScreen() (screen screen_t, err error) {
	screen.Window, err = sdl.CreateWindow("shootingame", sdl.WINDOWPOS_UNDEFINED, sdl.WINDOWPOS_UNDEFINED,
		800, 600, sdl.WINDOW_SHOWN)
	if err != nil {
		return
	}

	return
}

func (screen screen_t) destroy() {
	screen.Window.Destroy()
}

func (screen screen_t) update() {
	screen.Window.UpdateSurface()
}
