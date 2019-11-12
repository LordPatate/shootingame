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
		Player:  CreatePlayer(screen),
		Level:   &Level_t{},
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

	game.Player.Update(screen, game.Level)
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
	foreground := getTexture(screen, fg, FromRGB(20, 17, 23))
	background := getTexture(screen, bg, FromRGB(65, 60, 55))

	if bg == "" {
		if err := screen.Renderer.Copy(foreground, nil, &sdl.Rect{W: WindowWidth, H: WindowHeight}); err != nil {
			panic(err)
		}
		if err := screen.Renderer.Copy(background, nil, game.Level.Bounds); err != nil {
			panic(err)
		}
	}

	for _, tile := range game.Level.Tiles {
		screen.Renderer.Copy(foreground, nil, tile.Rect)
	}
	foreground.Destroy()
	background.Destroy()
}

func getTexture(screen *Screen_t, src string, byDefault uint32) (texture *sdl.Texture) {
	var surface *sdl.Surface
	if src == "" {
		var err error
		surface, err = sdl.CreateRGBSurface(0, TileWidth, TileHeight, 32, 0, 0, 0, 0)
		if err != nil {
			panic(err)
		}
		if err := surface.FillRect(nil, byDefault); err != nil {
			panic(err)
		}
	}
	texture, err := screen.Renderer.CreateTextureFromSurface(surface)
	if err != nil {
		panic(err)
	}
	surface.Free()

	return
}
