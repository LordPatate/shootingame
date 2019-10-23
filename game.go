package main

import "github.com/veandco/go-sdl2/sdl"

type Game_t struct {
	Running    bool
	Player     *Player_t
	Background *sdl.Texture
	Level      *Level_t
}

func CreateGame(screen *Screen_t) (game *Game_t) {
	game = &Game_t{
		Running: true,
		Player: CreatePlayer(
			&sdl.Rect{W: PlayerSpriteWidth, H: PlayerSpriteHeight},
			PlayerSpriteSheet, screen),
		Level: &Level_t{},
	}
	game.LoadLevel(0, screen)

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

func (game *Game_t) LoadLevel(id uint8, screen *Screen_t) {
	levelInfos := LevelInfos[id]

	game.Level.Init(levelInfos)

	game.Player.Rect.X = game.Level.PlayerStartPos.X
	game.Player.Rect.Y = game.Level.PlayerStartPos.Y

	background, err := screen.Renderer.CreateTexture(sdl.PIXELFORMAT_RGBA8888, sdl.TEXTUREACCESS_TARGET, WindowWidth, WindowHeight)
	if err != nil {
		panic(err)
	}
	if err := screen.Renderer.SetRenderTarget(background); err != nil {
		panic(err)
	}

	game.drawBackground(screen, levelInfos.BackgroundImg, levelInfos.ForegroundImg)

	game.Background = background
	if err := screen.Renderer.SetRenderTarget(nil); err != nil {
		panic(err)
	}
}

func (game *Game_t) drawBackground(screen *Screen_t, bg, fg string) {
	if bg == "" {
		if err := screen.Renderer.SetDrawColor(0, 0, 0, 255); err != nil {
			panic(err)
		}
		if err := screen.Renderer.Clear(); err != nil {
			panic(err)
		}
	}

	var foregroundSurface *sdl.Surface
	if fg == "" {
		var err error
		foregroundSurface, err = sdl.CreateRGBSurface(0, TileWidth, TileHeight, 32, 0, 0, 0, 0)
		if err != nil {
			panic(err)
		}
		if err := foregroundSurface.FillRect(nil, FromRGB(100, 100, 100)); err != nil {
			panic(err)
		}
	}
	foreground, err := screen.Renderer.CreateTextureFromSurface(foregroundSurface)
	if err != nil {
		panic(err)
	}

	for _, tile := range game.Level.Tiles {
		screen.Renderer.Copy(foreground, nil, tile.Rect)
	}
}
