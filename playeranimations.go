package main

import "github.com/veandco/go-sdl2/sdl"

type spriteCoord struct {
	Y, X uint8
}

var (
	idleCoords = []spriteCoord{
		{0, 0}, {0, 0}, {0, 1}, {0, 1}, {0, 2}, {0, 2}, {0, 3}, {0, 3},
	}
	runningCoords = []spriteCoord{
		{1, 1}, {1, 2}, {1, 3}, {1, 4}, {1, 5}, {1, 6},
	}
	jumpingCoords = []spriteCoord{
		{2, 0}, {2, 1}, {2, 2}, {2, 3}, {2, 4},
	}
	fallingCoords = []spriteCoord{
		{3, 1}, {3, 2},
	}
	wallSlidingCoords = []spriteCoord{
		{11, 2}, {11, 3},
	}
	wallJumpingCoords = []spriteCoord{
		{11, 0}, {11, 1},
	}
)

var dispatch map[PlayerState][]spriteCoord = map[PlayerState][]spriteCoord{
	Idle:        idleCoords,
	Running:     runningCoords,
	Jumping:     jumpingCoords,
	Falling:     fallingCoords,
	WallSliding: wallSlidingCoords,
	WallJumping: wallJumpingCoords,
}

func (player *Player_t) setTextureArea() {
	spriteCoordArray := dispatch[player.State]
	spriteCoord := spriteCoordArray[player.Frame/StepsPerFrame]

	player.TextureArea = &sdl.Rect{
		X: int32(spriteCoord.X) * CharacterWidth,
		Y: int32(spriteCoord.Y) * CharacterHeight,
		W: CharacterWidth,
		H: CharacterHeight,
	}

	player.Frame++
	player.Frame %= uint8(len(spriteCoordArray) * StepsPerFrame)
}
