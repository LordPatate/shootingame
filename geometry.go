package main

import "github.com/veandco/go-sdl2/sdl"

func PointInRectangle(x, y int32, r *sdl.Rect) bool {
	return r.X < x && r.X+r.W > x &&
		r.Y < y && r.Y+r.H > y
}

func ScaleRect(rect *sdl.Rect, wPercent, hPercent int32) *sdl.Rect {
	width := rect.W * wPercent / 100
	height := rect.H * hPercent / 100
	return &sdl.Rect{
		X: rect.X + (rect.W-width)/2,
		Y: rect.Y + (rect.H-height)/2,
		W: width,
		H: height,
	}
}
