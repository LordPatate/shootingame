package main

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
