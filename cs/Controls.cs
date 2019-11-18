using System;
using System.Collections.Generic;
using SDL2;

namespace shootingame
{
    class Controls
    {
        public static bool Left, Right, Jump;
        public static bool Update()
        {
            IntPtr keyStatePtr = SDL.SDL_GetKeyboardState(out int numkeys);
            Errors.CheckNull(keyStatePtr, "Controls.Update");
            uint* keyStatePtr = *(uint*)keyStatePtr.ToPointer();
            
            Left = keyState[SDL.SDL_Scancode.SDL_SCANCODE_A] == 1;
            Right = keyState[SDL.SDL_Scancode.SDL_SCANCODE_D] == 1;
            Jump = keyState[SDL.SDL_Scancode.SDL_SCANCODE_W] == 1;
        }

        public static void OnGround(Player player, uint[] keyState, Level level)
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

        public static void InAir(Player player, uint[] keyState, Level level)
        {
            const int maxSpeed = Const.PlayerStep * Const.InertiaPerPixel;

            if (Left) {
                player.Inertia.x -= Const.AirMovePower;
                player.Inertia.x = Math.Max(player.Inertia.x, -maxSpeed);
            }
            if (Right) {
                player.Inertia.x += Const.AirMovePower;
                player.Inertia.x = Math.Min(player.Inertia.x, maxSpeed);
            }

            SDL.SDL_Rect projectionLeft = SDLFactory.MakeRect(
                x: player.Rect.x - 1,
                y: player.Rect.y, w: player.Rect.w, h: player.Rect.h
            );
            if (Collision(ref projectionLeft, level)) {
                if (Jump) {
                    WallJump(player, left);
                    return;
                }
                if (Left &&
                (player.State == PlayerState.Falling || player.State == PlayerState.WallSliding)) {
                    WallSlide(player, left);
                }
            }
            SDL.SDL_Rect projectionRight = SDLFactory.MakeRect(
                x: player.Rect.x + 1,
                y: player.Rect.y, w: player.Rect.w, h: player.Rect.h
            );
            if (Collision(ref projectionRight, level)) {
                if (Jump) {
                    WallJump(player, right);
                    return;
                }
                if (Right &&
                (player.State == PlayerState.Falling || player.State == PlayerState.WallSliding)) {
                    WallSlide(player, right);
                }
            }
        }

        public static bool Collision(ref SDL.SDL_Rect projection, Level level)
        {
            foreach (Tile tile in level.Tiles) {
                var rect = tile.Rect;
                if (SDL.SDL_HasIntersection(projection, rect)) {
                    projection.x = rect.x; projection.y = rect.y;
                    projection.w = rect.w; projection.h = rect.h;
                    return true;
                }
            }
            
            SDL.SDL_Rect union, bounds = level.Bounds;
            SDL.SDL_UnionRect(ref projection, ref level.Bounds, out union);
            if (!SDL.SDL_RectEquals(ref level.Bounds, ref union)) {
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
                player.MoveX(-PlayerStep, level);
                player.Inertia.x = -Const.PlayerStep * Const.InertiaPerPixel;
            } else {
                player.MoveX(PlayerStep, level);
                player.Inertia.x = Const.PlayerStep * Const.InertiaPerPixel;
            }
            player.Direction = direction;
        }

        private static void WallJump(Player player, bool direction)
        {
            player.SetState(PlayerState.WallJumping);
            player.Inertia.Y = - Const.JumpPower;
            player.JumpEnabled = false;
            player.Direction = direction;
            player.Inertia.x = Const.PlayerStep * Const.InertiaPerPixel;
            if (direction == Const.Left)
                player.Inertia.x *= -1;
        }

        private static void WallSlide(Player player, bool direction)
        {
            player.SetState(WallSliding);
            player.Inertia.Y -= WallFriction;
            player.Direction = direction;
        }
    }
}