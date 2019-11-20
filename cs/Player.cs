using System;
using SDL2;

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

    unsafe class Player
    {
        public SDL.SDL_Rect Rect;
        public IntPtr Texture;
        public SDL.SDL_Rect TextureArea;
        public PlayerState State;
        public uint Frame;
        public bool Direction;
        public SDL.SDL_Point Inertia;
        public SDL.SDL_Point* HookPoint;
        public bool JumpEnabled;

        public readonly SDL.SDL_Point NormalStateDim = SDLFactory.MakePoint(20, 37);

        public Player()
        {
            Func<int, int> scale = (x) => x * Const.PlayerScalePercent / 100;

            Rect = SDLFactory.MakeRect(w: scale(NormalStateDim.x), h: scale(NormalStateDim.y));
            JumpEnabled = true;
            Inertia = SDLFactory.MakePoint();
            HookPoint = null;

            IntPtr surfacePtr = SDL_image.IMG_Load(Const.PlayerSpriteSheet);
            Errors.CheckNull(surfacePtr, "Load PlayerSpriteSheet");
            Texture = SDL.SDL_CreateTextureFromSurface(Screen.Renderer, surfacePtr);
            Errors.CheckNull(Texture, "Player.Texture Creation");
            SDL.SDL_FreeSurface(surfacePtr);

            TextureArea = SDLFactory.MakeRect(w: Const.PlayerSpriteWidth, h: Const.PlayerSpriteHeight);
        }
        
        public void Destroy()
        {
            SDL.SDL_DestroyTexture(Texture);
        }

        public void Copy()
        {
            PlayerAnimations.SetTextureArea(this);

            SDL.SDL_RendererFlip flip = (Direction == Const.Left) ?
                SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL :
                SDL.SDL_RendererFlip.SDL_FLIP_NONE;
            
            Func<int, int> scale = (x) => x * Const.PlayerScalePercent / 100;
            int width = scale(Const.PlayerSpriteWidth), height = scale(Const.PlayerSpriteHeight);
            SDL.SDL_Rect dst = SDLFactory.MakeRect(
                x: Rect.x + Rect.w/2 - width/2,
                y: Rect.y + Rect.h/2 - height/2,
                w: width, h: height
            );

            int err; Errors.msg = "Player.Copy";
            err = SDL.SDL_RenderCopyEx(Screen.Renderer, Texture, ref TextureArea, ref dst, 0, IntPtr.Zero, flip); Errors.Check(err);
        }

        public void Update(Level level)
        {
            Controls.Update();

            if (!JumpEnabled && !Controls.Jump) {
                JumpEnabled = true;
            }

            if (Inertia.y >= 0) {
                if (OnGround(level)) {
                    Controls.OnGround(this, level);
                    return;
                }
                SetState(PlayerState.Falling);
            }

            MoveX(Inertia.x/Const.InertiaPerPixel, level);
            MoveY(Inertia.y/Const.InertiaPerPixel, level);
            
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
            SDL.SDL_Rect rightUnderFeet = SDLFactory.MakeRect(
                x: Rect.x,
                y: Rect.y + Rect.h,
                w: Rect.w,
                h: 1
            );
            return Controls.Collision(ref rightUnderFeet, level);
        }

        public void MoveX(int delta, Level level)
        {
            SDL.SDL_Rect projection = SDLFactory.MakeRect(
                x: Rect.x + delta,
                y: Rect.y, w: Rect.w, h: Rect.h
            );

            Controls.Collision(ref projection, level);
            if (delta > 0) {
                Rect.x = projection.x - Rect.w;
            } else {
                Rect.x = projection.x + projection.w;
            }
        }

        public void MoveY(int delta, Level level)
        {
            SDL.SDL_Rect projection = SDLFactory.MakeRect(
                x: Rect.x,
                y: Rect.y + delta,
                w: Rect.w, h: Rect.h
            );

            Controls.Collision(ref projection, level);
            if (delta > 0) {
                Rect.y = projection.y - Rect.h;
            } else {
                Rect.y = projection.y + projection.h;
            }
        }
    }
}