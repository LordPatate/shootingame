package main

import (
	"github.com/veandco/go-sdl2/sdl"
)

type Popup_t struct {
	Text    []string
	Texture *sdl.Texture
	Rect    *sdl.Rect
	Options []PopupOption
}

type PopupOption struct {
	Text    string
	Texture *sdl.Texture
	Rect    *sdl.Rect
}

func (screen *Screen_t) PopupInit(text []string, options ...string) (popup *Popup_t) {
	optionNB := len(options)
	popup = &Popup_t{
		Text:    text,
		Rect:    ScaleRect(&sdl.Rect{W: WindowWidth, H: WindowHeight}, 80, 80),
		Options: make([]PopupOption, optionNB),
	}

	buttonRow := sdl.Rect{
		X: popup.Rect.X + popup.Rect.W*5/100,
		Y: popup.Rect.Y + popup.Rect.H*80/100,
		W: popup.Rect.W * 90 / 100,
		H: 60,
	}
	width := buttonRow.W / int32(optionNB)
	buttonSpaces := make([]sdl.Rect, optionNB)
	for i, space := range buttonSpaces {
		space.X = buttonRow.X + width*int32(i)
		space.Y = buttonRow.Y
		space.W = width
		space.H = buttonRow.H

		popup.Options[i].Rect = ScaleRect(&space, 75, 90)

		popup.Options[i].Text = options[i]
	}

	popup.createMainTexture(screen)
	popup.createButtonTextures(screen)

	return
}

func (popup *Popup_t) Pop(screen *Screen_t) (option string) {
	var clickedOption *PopupOption
	var previousRect sdl.Rect

	for option == "" {
		popup.display(screen)

		func() {
			for event := sdl.PollEvent(); event != nil; event = sdl.PollEvent() {
				switch e := event.(type) {
				case *sdl.MouseButtonEvent:
					if e.Button == sdl.BUTTON_RIGHT && e.Type == sdl.MOUSEBUTTONDOWN {
						option = "right click"
						return
					}
					if e.Button == sdl.BUTTON_LEFT {
						for i, opt := range popup.Options {
							if PointInRectangle(e.X, e.Y, opt.Rect) {
								if e.Type == sdl.MOUSEBUTTONDOWN {
									clickedOption = &popup.Options[i]
									previousRect = *opt.Rect
									clickedOption.Rect = ScaleRect(opt.Rect, 95, 95)
									return
								}
								// e.Type == sdl.MOUSEBUTTONUP
								if clickedOption.Text == opt.Text {
									option = opt.Text
									return
								}
							}
						}
						if clickedOption != nil {
							*clickedOption.Rect = previousRect
						}
						clickedOption = nil
					}
				}
			}
		}()
	}

	popup.destroy()
	return
}

func (popup *Popup_t) createTextures(screen *Screen_t) {
	var err error
	popup.Texture, err = screen.Renderer.CreateTexture(sdl.PIXELFORMAT_RGBA8888, sdl.TEXTUREACCESS_TARGET, popup.Rect.W, popup.Rect.H)
	if err != nil {
		panic(err)
	}
	if err := screen.Renderer.SetRenderTarget(popup.Texture); err != nil {
		panic(err)
	}

	// frame
	if err := screen.Renderer.SetDrawColor(200, 200, 200, 255); err != nil {
		panic(err)
	}
	if err := screen.Renderer.FillRect(nil); err != nil {
		panic(err)
	}

	// text body
	yPos := (popup.Rect.H*80/100)/2 - int32(len(popup.Text)*20/2)
	for i, line := range popup.Text {
		rect := &sdl.Rect{Y: yPos + int32(i*20), W: popup.Rect.W, H: 20}
		screen.CopyText(line, rect, sdl.Color{0, 0, 0, 255}, sdl.Color{200, 200, 200, 255})
	}

	// buttons
	if err := screen.Renderer.SetDrawColor(0, 0, 255, 255); err != nil {
		panic(err)
	}
	for i, option := range popup.Options {
		var err error
		popup.Options[i].Texture, err = screen.Renderer.CreateTexture(sdl.PIXELFORMAT_RGBA8888, sdl.TEXTUREACCESS_TARGET, option.Rect.W, option.Rect.H)
		if err != nil {
			panic(err)
		}
		if err := screen.Renderer.SetRenderTarget(popup.Options[i].Texture); err != nil {
			panic(err)
		}

		if err := screen.Renderer.FillRect(nil); err != nil {
			panic(err)
		}
		rect := &sdl.Rect{W: option.Rect.W, H: option.Rect.H}
		screen.CopyText(option.Text, rect, sdl.Color{255, 255, 255, 255}, sdl.Color{0, 0, 255, 255})
	}
}

func (popup *Popup_t) display(screen *Screen_t) {
	if err := screen.Renderer.SetRenderTarget(nil); err != nil {
		panic(err)
	}
	if err := screen.Renderer.Copy(screen.Background, nil, nil); err != nil {
		panic(err)
	}
	if err := screen.Renderer.Copy(popup.Texture, nil, popup.Rect); err != nil {
		panic(err)
	}
	for _, option := range popup.Options {
		if err := screen.Renderer.Copy(option.Texture, nil, option.Rect); err != nil {
			panic(err)
		}
	}

	screen.Renderer.Present()
}

func (popup *Popup_t) destroy() {
	popup.Texture.Destroy()

	for _, option := range popup.Options {
		if option.Texture != nil {
			option.Texture.Destroy()
		}
	}
}
