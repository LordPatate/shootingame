package main

import (
	"errors"
	"time"

	"github.com/veandco/go-sdl2/img"
	"github.com/veandco/go-sdl2/sdl"
	"github.com/veandco/go-sdl2/ttf"
)

func main() {
	if err := sdl.Init(sdl.INIT_EVERYTHING); err != nil {
		panic(err)
	}
	if err := ttf.Init(); err != nil {
		panic(err)
	}
	if img.Init(imgFlags) != imgFlags {
		panic(errors.New("IMG init failed"))
	}
	defer sdl.Quit()
	defer ttf.Quit()
	defer img.Quit()

	screen, err := CreateScreen()
	if err != nil {
		panic(err)
	}
	defer screen.Destroy()

	running := true
	askQuit := func() {
		popup := screen.PopupInit([]string{"Do you really want to quit?"}, "Yes", "No")
		if popup.Pop(screen) == "Yes" {
			running = false
		}
	}
	for running {
		screen.Update()
		for event := sdl.PollEvent(); event != nil; event = sdl.PollEvent() {
			switch e := event.(type) {
			case *sdl.KeyboardEvent:
				if e.Keysym.Sym == sdl.K_ESCAPE {
					askQuit()
				}
			case *sdl.QuitEvent:
				askQuit()
				break
			}
		}
		time.Sleep(100 * time.Millisecond)
	}
}
