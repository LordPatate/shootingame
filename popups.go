package main

import (
	"github.com/veandco/go-sdl2/sdl"
)

type Popup_t struct {
	Text    []string
	Rect    *sdl.Rect
	Options []PopupOption
}

type PopupOption struct {
	Text string
	Rect *sdl.Rect
}

func PopupInit(text []string, options ...string) (popup *Popup_t) {
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

	return
}

func (popup *Popup_t) display(screen *Screen_t) {
	if err := screen.Renderer.Copy(screen.Background, nil, nil); err != nil {
		panic(err)
	}

	// frame
	if err := screen.Renderer.SetDrawColor(200, 200, 200, 255); err != nil {
		panic(err)
	}
	if err := screen.Renderer.FillRect(popup.Rect); err != nil {
		panic(err)
	}

	// text body
	for _, line := range popup.Text {
		screen.CopyText(line, popup.Rect, sdl.Color{0, 0, 0, 255}, sdl.Color{200, 200, 200, 255})
	}

	// buttons
	if err := screen.Renderer.SetDrawColor(0, 0, 255, 255); err != nil {
		panic(err)
	}
	for _, option := range popup.Options {
		if err := screen.Renderer.FillRect(option.Rect); err != nil {
			panic(err)
		}
		screen.CopyText(option.Text, option.Rect, sdl.Color{255, 255, 255, 255}, sdl.Color{0, 0, 255, 255})
	}

	screen.Renderer.Present()
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
	return
}
