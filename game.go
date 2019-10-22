package main

import "github.com/veandco/go-sdl2/sdl"

type Game_t struct {
	Running bool
	Player  *Player_t
}

func CreateGame(playerStartPos *sdl.Point, screen *Screen_t) (game *Game_t) {
	game = &Game_t{
		Running: true,
		Player: CreatePlayer(
			&sdl.Rect{
				X: playerStartPos.X, Y: playerStartPos.Y,
				W: CharacterWidth, H: CharacterHeight,
			},
			PlayerSpriteSheet, screen),
	}

	return
}

func (game *Game_t) Destroy() {
	game.Player.Destroy()
}

func (game *Game_t) Update(screen *Screen_t) {
	askQuit := func() {
		popup := screen.PopupInit([]string{"Do you really want to quit?"}, "Yes", "No")
		if popup.Pop(screen) == "Yes" {
			game.Running = false
		}
	}

	for event := sdl.PollEvent(); event != nil; event = sdl.PollEvent() {
		switch e := event.(type) {
		case *sdl.KeyboardEvent:
			if e.Keysym.Sym == sdl.K_ESCAPE {
				askQuit()
				return
			}

		case *sdl.QuitEvent:
			askQuit()
			return
		}
	}
}
