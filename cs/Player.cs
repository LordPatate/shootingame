using System;
using SFML.Graphics;
using SFML.System;

namespace shootingame
{
    enum PlayerState
    {
        Idle,
        Running,
        Jumping,
        Falling,
        WallSliding,
        WallJumping
    }

    class Player
    {
        public IntRect Rect;
        public Texture Texture;
        public IntRect TextureArea;
        public PlayerState State;
        public uint Frame;
        public bool Direction;
        public Vector2i Inertia;
        public Vector2i HookPoint;
        public bool Hooked;
        public bool JumpEnabled;

        public readonly Vector2i NormalStateDim =  new Vector2i(20, 37);

        public Player()
        {
            Func<int, int> scale = (x) => x * Const.PlayerScalePercent / 100;

            Rect = new IntRect(0, 0, width: scale(NormalStateDim.X), height: scale(NormalStateDim.Y));
            Inertia =  new Vector2i();
            HookPoint = new Vector2i();
            Hooked = false;
            JumpEnabled = true;

            // IntPtr surfacePtr = SDL_image.IMG_Load(Const.PlayerSpriteSheet);
            // Errors.CheckNull(surfacePtr, "Load PlayerSpriteSheet");
            // Texture = SDL.SDL_CreateTextureFromSurface(Screen.Renderer, surfacePtr);
            // Errors.CheckNull(Texture, "Player.Texture Creation");
            // SDL.SDL_FreeSurface(surfacePtr);

            TextureArea = new IntRect(0, 0, width: Const.PlayerSpriteWidth, height: Const.PlayerSpriteHeight);
        }
        
        public void Destroy()
        {
            // SDL.SDL_DestroyTexture(Texture);
        }

        public void Copy()
        {
            PlayerAnimations.SetTextureArea(this);

            // SDL.SDL_RendererFlip flip = (Direction == Const.Left) ?
            //     SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL :
            //     SDL.SDL_RendererFlip.SDL_FLIP_NONE;
            
            Func<int, int> scale = (x) => x * Const.PlayerScalePercent / 100;
            int width = scale(Const.PlayerSpriteWidth), height = scale(Const.PlayerSpriteHeight);
            IntRect dst = new IntRect(
                left: Rect.Left + Rect.Width/2 - width/2,
                top: Rect.Top + Rect.Height/2 - height/2,
                width: width, height: height
            );

            // int err; Errors.msg = "Player.Copy";
            // err = SDL.SDL_RenderCopyEx(Screen.Renderer, Texture, ref TextureArea, ref dst, 0, IntPtr.Zero, flip); Errors.Check(err);
        }

        public void Update(Level level)
        {
            Controls.Update();

            if (!JumpEnabled && !Controls.Jump) {
                JumpEnabled = true;
            }

            if (Inertia.Y >= 0) {
                if (OnGround(level)) {
                    Controls.OnGround(this, level);
                    return;
                }
                SetState(PlayerState.Falling);
            }

            MoveX(Inertia.X/Const.InertiaPerPixel, level);
            MoveY(Inertia.Y/Const.InertiaPerPixel, level);
            
            Controls.InAir(this, level);
        }

        public void SetState(PlayerState state)
        {
            if (State != state) {
                State = state;
                Frame = 0;
            }
        }

        public bool OnGround(Level level) {
            IntRect rightUnderFeet = new IntRect(
                left: Rect.Left,
                top: Rect.Top + Rect.Height,
                width: Rect.Width,
                height: 1
            );
            return Controls.Collision(ref rightUnderFeet, level);
        }

        public void MoveX(int delta, Level level)
        {
            IntRect projection = new IntRect(
                left: Rect.Left + delta,
                top: Rect.Top, width: Rect.Width, height: Rect.Height
            );

            if (Controls.Collision(ref projection, level)) {
                if (delta > 0) {
                    Rect.Left = projection.Left - Rect.Width;
                } else {
                    Rect.Left = projection.Left + projection.Width;
                }
            }
            else
            {
                Rect.Left = projection.Left;
            }
        }

        public void MoveY(int delta, Level level)
        {
            IntRect projection = new IntRect(
                left: Rect.Left,
                top: Rect.Top + delta,
                width: Rect.Width, height: Rect.Height
            );

            if (Controls.Collision(ref projection, level)) {
                if (delta > 0) {
                    Rect.Top = projection.Top - Rect.Height;
                } else {
                    Rect.Top = projection.Top + projection.Height;
                }
            }
            else
            {
                Rect.Top = projection.Top;
            }
        }
    }
}