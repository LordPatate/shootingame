package main

import (
	"time"

	"github.com/veandco/go-sdl2/sdl"
)

func main() {
	if err := sdl.Init(sdl.INIT_EVERYTHING); err != nil {
		panic(err)
	}
	defer sdl.Quit()

	screen, err := CreateScreen()
	if err != nil {
		panic(err)
	}
	defer screen.Destroy()

	running := true
	for running {
		screen.Update()
		for event := sdl.PollEvent(); event != nil; event = sdl.PollEvent() {
			switch event.(type) {
			case *sdl.QuitEvent:
				popup := PopupInit([]string{"Do you really want to quit?"}, "Yes", "No")
				if popup.Pop(screen) == "Yes" {
					running = false
				}
				break
			}
		}
		time.Sleep(100 * time.Millisecond)
	}
}
