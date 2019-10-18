package main

func FromRGB(r, g, b uint8) uint32 {
	return Red*uint32(r)/255 + Green*uint32(g)/255 + Blue*uint32(b)/255
}
