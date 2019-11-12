package main

type Character interface {
	Copy(screen *Screen_t)
	GetRect()
	Destroy()
}
