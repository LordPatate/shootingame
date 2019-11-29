# Shootingame
A small dumb 2D game where you get to shoot people.

## Install

* Find the release that you need depending on your OS.

    See release page: https://github.com/LordPatate/shootingame/releases.
Client releases are archives named `shootingame-<OS_TYPE>.zip` ;
server releases are archives named `shootingame-server-<OS_TYPE>.zip`.

* Get the resources archives `shootingame-resources.zip`.

* Extract all your archives in a new folder

* Make sure executables are 2 levels deeper than `levels/` and `resources/`. Your hierarchy should look like the following:
```
.
├── client
│   └── publish
│       ├── other files...
│       └── shootingame (shootingame.exe on Windows)
├── levels
│   ├── level0.png
│   └── level1.png
├── resources
│   ├── fonts
│   │   └── some files...
│   └── sprites
│       └── some files...
└── server
    └── publish
        ├── other files...
        └── server (server.exe on Windows)
```

## Launch

Make sure you are in the same folder as the executable you want to run.

**Server:** `server[.exe] [<LevelID>]`

**Client:** `shootingame[.exe] <IP address of the server>`
