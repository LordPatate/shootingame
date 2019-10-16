package main

func fromRGB(r, g, b uint8) uint32 {
	return red*uint32(r)/255 + green*uint32(g)/255 + blue*uint32(b)/255
}
