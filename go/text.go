package main

import "github.com/veandco/go-sdl2/sdl"

func (screen *Screen_t) CopyText(line string, frame *sdl.Rect, fg, bg sdl.Color) {
	surface, err := screen.Font.RenderUTF8Shaded(line, fg, bg)
	if err != nil {
		panic(err)
	}
	texture, err := screen.Renderer.CreateTextureFromSurface(surface)
	if err != nil {
		panic(err)
	}
	screen.Renderer.Copy(texture, nil, &sdl.Rect{
		X: frame.X + 10 + (frame.W-20)/2 - surface.W/2,
		Y: frame.Y + frame.H/2 - surface.H/2,
		W: surface.W,
		H: surface.H,
	})
	surface.Free()
	texture.Destroy()
}
