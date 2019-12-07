# Shootingame
A small dumb 2D game where you get to shoot people.

### Contents:
- Install
- Launch
- Edit you levels

## Install

* Find the release that you need depending on your OS.

    See release page: https://github.com/LordPatate/shootingame/releases.
Client releases are archives named `shootingame-<OS_TYPE>.zip` ;
server releases are archives named `shootingame-server-<OS_TYPE>.zip`.

* Extract your archive in a new folder

* Your hierarchy should look like the following:
```
shootingame
├── publish
│   ├── shootingame (shootingame.exe on Windows)
│   └── other files...
└── resources
    ├── fonts
    │   └── some files...
    └── sprites
        └── some files...

shootingame-server
├── publish
│   ├── server (server.exe on Windows)
│   └── other files...
└── levels
    ├── level0.png
    └── level1.png
```

## Launch

Make sure you are in the same folder as the executable you want to run.

**Client:** `shootingame[.exe] <IP address of the server> <Player Name>`

**Server:** `server[.exe] <Level ID>`

### Notes:

When launching a client:

If you don't specify a Player Name, you will be named "Player X" where X is an ID given by the server (first available).
If you don't specity the server address either, the default value will be 127.0.0.1 (localhost).

When lauching a server:

If you don't specify a level ID, default will be 0. Level ID must be an integer (i.e. 0, 1, 2 etc). It refers to the n-th file in the folder 'levels' in alphabetical order. If you specify an ID too big, it will loop back to the beginning: if you have two files and you ask for level 2, first file will be used; if you ask for 3, it will be the second file.

## Edit your levels

Editing levels for shootingame is stupidly easy. You might have noticed that files in 'levels' are png images. Try to open one, then zoom in a lot. Black pixels are solid tiles and red pixels are spawn points.

So grab you favorite image editor (good old Paint works just fine) and start adding / removing tiles or spawn points!
You can also change the level size by changing the image dimensions.

**Warning:** to be recognized as a black pixel, you must use full black color (#000000) for your tiles. Same goes for red pixels, use the reddest color (#FF0000) to mark spawn points. Alpha should be fully opaque.

You can create as many different levels as you want. Refer to *Launch* section to see how to launch your server with the right file.
