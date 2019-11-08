package main

import (
	"math"

	"github.com/veandco/go-sdl2/sdl"
)

func FromRGB(r, g, b uint8) uint32 {
	return Red*uint32(r)/255 + Green*uint32(g)/255 + Blue*uint32(b)/255
}

func Max32(x, y int32) int32 {
	if x > y {
		return x
	}
	return y
}

func Min32(x, y int32) int32 {
	if x < y {
		return x
	}
	return y
}

func Dist(a, b *sdl.Point) float64 {
	square := func(x float64) float64 { return x * x }
	return math.Sqrt(square(float64(b.X-a.X)) + square(float64(b.Y-a.Y)))
}

func Cos(origin *sdl.Point, x, y int32) float64 {
	return float64(x-origin.X) / Dist(origin, &sdl.Point{x, y})
}

func Sin(origin *sdl.Point, x, y int32) float64 {
	return float64(y-origin.Y) / Dist(origin, &sdl.Point{x, y})
}
