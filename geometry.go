package main

import (
	"math"

	"github.com/veandco/go-sdl2/sdl"
)

type FloatPoint struct {
	X, Y float64
}

func PointToFloat(x, y int32) FloatPoint {
	return FloatPoint{float64(x), float64(y)}
}

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

type myRect sdl.Rect

func AdaptRect(rect *sdl.Rect) myRect {
	return myRect{
		X: rect.X - 1, Y: rect.Y - 1,
		W: rect.W + 2, H: rect.H + 2,
	}
}

func (rect *myRect) IntersectLine(x1, y1, x2, y2 int32) bool {
	rectX1 := rect.X
	rectY1 := rect.Y
	rectX2 := rect.X + rect.W - 1
	rectY2 := rect.Y + rect.H - 1

	// easy cases
	if (x1 < rectX1 && x2 < rectX1) || (x1 > rectX2 && x2 > rectX2) ||
		(y1 < rectY1 && y2 < rectY1) || (y1 > rectY2 && y2 > rectY2) {
		return false
	}

	if y1 == y2 { // horizontal line
		return y1 >= rect.Y && y1 < rect.Y+rect.H
	}
	if x1 == x2 { // vertical line
		return x1 >= rect.X && x1 < rect.X+rect.W
	}

	a1, a2 := PointToFloat(x1, y1), PointToFloat(x2, y2)

	// top edge
	topX := HorizontalIntersection(a1, a2, rectY1)
	if topX > rectX1 && topX < rectX2 {
		return true
	}

	// bottom edge
	bottomX := HorizontalIntersection(a1, a2, rectY2)
	if bottomX > rectX1 && bottomX < rectX2 {
		return true
	}

	// left edge
	leftY := VerticalIntersection(a1, a2, rectX1)
	return leftY > rectY1 && leftY < rectY2
}

func HorizontalIntersection(a1, a2 FloatPoint, y int32) int32 {
	coefA := (a1.Y - a2.Y) / (a1.X - a2.X)
	ordA := a1.Y - a1.X*coefA

	return int32(math.Round((float64(y) - ordA) / coefA))
}

func VerticalIntersection(a1, a2 FloatPoint, x int32) int32 {
	coefA := (a1.Y - a2.Y) / (a1.X - a2.X)
	ordA := a1.Y - a1.X*coefA

	return int32(math.Round(coefA*float64(x) + ordA))
}

func PointShade(bounds *sdl.Rect, playerEye *sdl.Point, x, y int32) (sx, sy int16) {
	a1, a2 := PointToFloat(x, y), PointToFloat(playerEye.X, playerEye.Y)

	if playerEye.X > x { // shadow on left bound
		sx = int16(bounds.X)
	} else { // shadow on right bound
		sx = int16(bounds.X + bounds.W)
	}
	sy = int16(VerticalIntersection(a1, a2, int32(sx)))
	if sy >= int16(bounds.Y) && sy <= int16(bounds.Y+bounds.H) {
		return
	}

	if playerEye.Y > y { // shadow on top edge
		sy = int16(bounds.Y)
	} else { //shadow on bottom edge
		sy = int16(bounds.Y + bounds.H)
	}
	sx = int16(HorizontalIntersection(a1, a2, int32(sy)))
	if sx >= int16(bounds.X) && sx <= int16(bounds.X+bounds.W) {
		return
	}
	return int16(x), sy
}
