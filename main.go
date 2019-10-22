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

	game := CreateGame(&sdl.Point{500, 300}, screen)
	defer game.Destroy()

	for game.Running {
		screen.Update(game)
		game.Update(screen)

		time.Sleep(100 * time.Millisecond)
	}
}
