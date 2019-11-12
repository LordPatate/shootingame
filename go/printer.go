package main

import (
	"fmt"

	"github.com/veandco/go-sdl2/sdl"
)

func PrintRect(r *sdl.Rect) {
	fmt.Printf("Rect{x: %d, y: %d, w: %d, h: %d}\n", r.X, r.Y, r.W, r.H)
}
