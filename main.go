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

	game := CreateGame(screen)
	defer game.Destroy()

	const done = true
	channel := make(chan bool)

	screen.ComputeShadows(game)
	for game.Running {
		screen.Update(game)

		update := !done
		go func() {
			sdl.PumpEvents()
			game.Update(screen)
			screen.ComputeShadows(game)

			channel <- done
		}()

		time.Sleep(GameStepDuration)
		for update != done {
			update = <-channel
		}
	}
}
