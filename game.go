package main

import "github.com/veandco/go-sdl2/sdl"

type Game_t struct {
	Running    bool
	Player     *Player_t
	Background *sdl.Texture
}

func CreateGame(playerStartPos *sdl.Point, screen *Screen_t) (game *Game_t) {
	background, err := screen.Renderer.CreateTexture(sdl.PIXELFORMAT_RGBA8888, sdl.TEXTUREACCESS_TARGET, WindowWidth, WindowHeight)
	if err != nil {
		panic(err)
	}
	if err := screen.Renderer.SetRenderTarget(background); err != nil {
		panic(err)
	}
	if err := screen.Renderer.SetDrawColor(0, 0, 0, 255); err != nil {
		panic(err)
	}
	if err := screen.Renderer.Clear(); err != nil {
		panic(err)
	}
	if err := screen.Renderer.SetRenderTarget(nil); err != nil {
		panic(err)
	}

	game = &Game_t{
		Running: true,
		Player: CreatePlayer(
			&sdl.Rect{
				X: playerStartPos.X, Y: playerStartPos.Y,
				W: CharacterWidth, H: CharacterHeight,
			},
			PlayerSpriteSheet, screen),
		Background: background,
	}

	return
}

func (game *Game_t) Destroy() {
	game.Player.Destroy()
	game.Background.Destroy()
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

	state := sdl.GetKeyboardState()
	var movement func(*Game_t)
	pressedKeys := 0
	for key := 0; key < sdl.NUM_SCANCODES; key++ {
		if state[key] == 1 {
			m, present := Movements[sdl.Scancode(key)]
			if present {
				pressedKeys++
				movement = m
			}
		}
	}
	if pressedKeys == 1 {
		movement(game)
	} else {
		player := game.Player
		if player.State != Idle {
			player.State = Idle
			player.Frame = 0
		}

	}
}
