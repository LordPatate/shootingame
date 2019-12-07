namespace shootingame
{
    public class Const
    {
        public const uint Red = 0x00ff0000, Green = 0x0000ff00,  Blue =  0x000000ff;

        public const bool Left = true, Right = false;

        public const int GameStepDuration = 15,
        StepsPerFrame = 7,
        WallJumpSoundDuration = 400,
        HookSoundStep = 900,
        PlayerStep = 5,
        FramesBeforeFullSpeed = 15,
        Gravity = 30,
        JumpPower = 850,
        AirMovePower = 20,
        AirSlow = 4,
        WallFriction = 10,
        InertiaPerPixel = 100,
        HookMaxRange = 170,
        GunCoolDown = 30;

        public const int NormalStateDimX = 20, NormalStateDimY = 37;

        public const int WindowWidth = 1120, WindowHeight = 630,
        TileWidth = 25, TileHeight = 25;

        public const string PlayerSpriteSheet = "resources/sprites/player/adventurer-v1.5-Sheet.png"; // see playeranimations.go
        public const int PlayerSpriteWidth = 50, PlayerSpriteHeight = 37,
        PlayerScalePercent = 120;

        public const string FontFile = "resources/fonts/DejaVuSans.ttf";
        public const uint FontSize = 18;

        public const string SoundFolder = "resources/sounds/";

        public const int ServerPort = 4242;
    }
}
