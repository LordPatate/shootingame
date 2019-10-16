package main

import "github.com/veandco/go-sdl2/sdl"

type popup_t struct {
	Text    string
	Options []string
}

func (screen screen_t) pop(popup popup_t) (option string) {
	surface, err := screen.Window.GetSurface()
	if err != nil {
		panic(err)
	}
	surface.FillRect(nil, 0)

	rect := sdl.Rect{X: 0, Y: 0, W: 200, H: 200}
	if err := surface.FillRect(&rect, fromRGB(255, 255, 0)); err != nil {
		panic(err)
	}

	for option == "" {
		for event := sdl.PollEvent(); event != nil; event = sdl.PollEvent() {
			switch event.GetType() {
			case sdl.MOUSEBUTTONDOWN:
				option = popup.Options[0] // FIXME
				break
			}
		}
	}
	return
}
