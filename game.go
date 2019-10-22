package main

import "github.com/veandco/go-sdl2/sdl"

type Game_t struct {
	Player *Player_t
}

func CreateGame(playerStartPos *sdl.Point, screen *Screen_t) (game *Game_t) {
	game = &Game_t{
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
