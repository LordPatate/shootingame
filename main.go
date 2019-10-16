package main

import "github.com/veandco/go-sdl2/sdl"

func main() {
	if err := sdl.Init(sdl.INIT_EVERYTHING); err != nil {
		panic(err)
	}
	defer sdl.Quit()

	screen, err := createScreen()
	if err != nil {
		panic(err)
	}
	defer screen.destroy()

	//Test space
	surface, err := screen.Window.GetSurface()
	if err != nil {
		panic(err)
	}
	surface.FillRect(nil, 0)

	rect := sdl.Rect{X: 0, Y: 0, W: 200, H: 200}
	surface.FillRect(&rect, fromRGB(255, 255, 0))

	//Test space end

	running := true
	for running {
		screen.update()
		for event := sdl.PollEvent(); event != nil; event = sdl.PollEvent() {
			switch event.(type) {
			case *sdl.QuitEvent:
				popup := popup_t{
					Text: "Do you really want to quit?",
					Options: {
						"Yes",
						"No",
					},
				}
				if screen.pop(popup) == "Yes" {
					running = false
				}
				break
			}
		}
	}
}
