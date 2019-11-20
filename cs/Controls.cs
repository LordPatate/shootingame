using System;
using System.Collections.Generic;
using SDL2;

namespace shootingame
{
    unsafe class Controls
    {
        public static bool Left, Right, Jump;
        public static int MouseX, MouseY;
        public static bool LeftClick, RightClick;
        public static void Update()
        {
            // Keys
            IntPtr keyStatePtr = SDL.SDL_GetKeyboardState(out int numkeys);
            Errors.CheckNull(keyStatePtr, "Controls.Update");
            uint* keyState = (uint*)keyStatePtr.ToPointer();
            
            Left = keyState[(int)SDL.SDL_Scancode.SDL_SCANCODE_A] == 1;
            Right = keyState[(int)SDL.SDL_Scancode.SDL_SCANCODE_D] == 1;
            Jump = keyState[(int)SDL.SDL_Scancode.SDL_SCANCODE_W] == 1;

            // Mouse
            UInt32 mouseState = SDL.SDL_GetMouseState(out MouseX, out MouseY);
            LeftClick = (mouseState & SDL.SDL_BUTTON_LMASK) != 0;
            RightClick = (mouseState & SDL.SDL_BUTTON_RMASK) != 0;
        }

        public static void OnGround(Player player, Level level)
        {
            if (Jump && player.JumpEnabled) {
                player.SetState(PlayerState.Jumping);
                player.Inertia.y = -Const.JumpPower;
                player.JumpEnabled = false;
                return;
            }
            
            
            if (Left == Right) {
                player.SetState(PlayerState.Idle);
                player.Inertia.x = 0;
                return;
            }
            Step(player, Left == Const.Left, level);
        }

        public static void InAir(Player player, Level level)
        {
            SDL.SDL_Rect projection = SDLFactory.MakeRect(
                x: player.Rect.x - 1,
                y: player.Rect.y, w: player.Rect.w, h: player.Rect.h
            );
            if (Collision(ref projection, level)) {
                OnWall(player, Const.Left);
                return;
            }

            projection.x += player.Rect.w + 2;
            if (Collision(ref projection, level)) {
                OnWall(player, Const.Right);
                return;
            }
            
            const int maxSpeed = Const.PlayerStep * Const.InertiaPerPixel;

            // Movement
            if (Left) {
                player.Inertia.x -= Const.AirMovePower;
                player.Inertia.x = Math.Max(player.Inertia.x, -maxSpeed);
            }
            if (Right) {
                player.Inertia.x += Const.AirMovePower;
                player.Inertia.x = Math.Min(player.Inertia.x, maxSpeed);
            }
            
            // Slowing
            if (player.Inertia.x > 0) {
                player.Inertia.x -= Const.AirSlow;
                player.Inertia.x = Math.Max(player.Inertia.x, 0);
            } else {
                player.Inertia.x += Const.AirSlow;
                player.Inertia.x = Math.Min(player.Inertia.x, 0);
            }
            
            // Gravity
            player.Inertia.y += Const.Gravity;
        }

        public static bool Collision(ref SDL.SDL_Rect projection, Level level)
        {
            foreach (Tile tile in level.Tiles) {
                var rect = tile.Rect;
                if ((int)SDL.SDL_HasIntersection(ref projection, ref rect) != 0) {
                    projection.x = rect.x; projection.y = rect.y;
                    projection.w = rect.w; projection.h = rect.h;
                    return true;
                }
            }
            
            SDL.SDL_Rect union, bounds = level.Bounds;
            SDL.SDL_UnionRect(ref projection, ref level.Bounds, out union);
            if ((int)SDL.SDL_RectEquals(ref level.Bounds, ref union) == 0) {
                projection.x = bounds.x + bounds.w; projection.y = bounds.y + bounds.h;
                projection.w = -bounds.w; projection.h = -bounds.h;
                return true;
            }
            
            return false;
        }

        private static void Step(Player player, bool direction, Level level)
        {
            player.SetState(PlayerState.Running);

            if (direction == Const.Left) {
                player.MoveX(-Const.PlayerStep, level);
                player.Inertia.x = -Const.PlayerStep * Const.InertiaPerPixel;
            } else {
                player.MoveX(Const.PlayerStep, level);
                player.Inertia.x = Const.PlayerStep * Const.InertiaPerPixel;
            }
            player.Direction = direction;
        }

        private static void OnWall(Player player, bool wallSide)
        {
            if (Jump) {
                WallJump(player, !wallSide);
                return;
            }
            if ((Left && (wallSide == Const.Left) || (Right && (wallSide == Const.Right))) && 
            (player.State == PlayerState.Falling || player.State == PlayerState.WallSliding)) {
                WallSlide(player, wallSide);
            }
        }

        private static void WallJump(Player player, bool direction)
        {
            player.SetState(PlayerState.WallJumping);
            player.Inertia.y = - Const.JumpPower;
            player.JumpEnabled = false;
            player.Direction = direction;
            player.Inertia.x = Const.PlayerStep * Const.InertiaPerPixel;
            if (direction == Const.Left)
                player.Inertia.x *= -1;
        }

        private static void WallSlide(Player player, bool direction)
        {
            player.SetState(PlayerState.WallSliding);
            player.Inertia.y -= Const.WallFriction;
            player.Direction = direction;
        }

    }
}