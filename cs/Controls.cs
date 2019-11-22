using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace shootingame
{
    unsafe class Controls
    {
        public static bool Left, Right, Jump;
        public static Vector2i MousePos;
        public static bool LeftClick, RightClick;
        public static void Update()
        {
            // Keys
            Left = Keyboard.IsKeyPressed(Key.A) || Keyboard.IsKeyPressed(Key.Q);
            Right = Keyboard.IsKeyPressed(Key.D);
            Jump = Keyboard.IsKeyPressed(Key.W) || Keyboard.IsKeyPressed(Key.Z);

            // Mouse
            MousePos = Mouse.GetPosition();
            LeftClick = Mouse.IsButtonPressed(Mouse.Button.Left);
            RightClick = Mouse.IsButtonPressed(Mouse.Button.Right);
        }

        public static void OnGround(Player player, Level level)
        {
            if (Jump && player.JumpEnabled) {
                player.SetState(PlayerState.Jumping);
                player.Inertia.Y = -Const.JumpPower;
                player.JumpEnabled = false;
                return;
            }
            
            
            if (Left == Right) {
                player.SetState(PlayerState.Idle);
                player.Inertia.X = 0;
                return;
            }
            Step(player, Left == Const.Left, level);
        }

        public static void InAir(Player player, Level level)
        {
            IntRect projection = new IntRect(
                left: player.Rect.Left - 1,
                top: player.Rect.Top, width: player.Rect.Width, height: player.Rect.Height
            );
            if (Collision(ref projection, level)) {
                OnWall(player, Const.Left);
                return;
            }

            projection.x += player.Rect.Width + 2;
            if (Collision(ref projection, level)) {
                OnWall(player, Const.Right);
                return;
            }
            
            Const int maxSpeed = Const.PlayerStep * Const.InertiaPerPixel;

            // Movement
            if (Left) {
                player.Inertia.X -= Const.airMovePower;
                player.Inertia.X = Math.Max(player.Inertia.X, -maxSpeed);
            }
            if (Right) {
                player.Inertia.X += Const.airMovePower;
                player.Inertia.X = Math.Min(player.Inertia.X, maxSpeed);
            }
            
            // Slowing
            if (player.Inertia.X > 0) {
                player.Inertia.X -= Const.airSlow;
                player.Inertia.X = Math.Max(player.Inertia.X, 0);
            } else {
                player.Inertia.X += Const.airSlow;
                player.Inertia.X = Math.Min(player.Inertia.X, 0);
            }
            
            // Gravity
            player.Inertia.Y += Const.gravity;
        }

        public static bool Collision(ref IntRect projection, Level level)
        {
            foreach (Tile tile in level.tiles) {
                var rect = tile.Rect;
                if (projection.Intersects(rect) {
                    projection.x = rect.x; projection.y = rect.y;
                    projection.w = rect.w; projection.h = rect.h;
                    return true;
                }
            }
            
            IntRect bounds = level.Bounds;
            projection.Intersects(bounds, out overLap);
            if (projection.Equals(overLap)) {
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
                player.Inertia.X = -Const.PlayerStep * Const.InertiaPerPixel;
            } else {
                player.MoveX(Const.PlayerStep, level);
                player.Inertia.X = Const.PlayerStep * Const.InertiaPerPixel;
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
            player.Inertia.Y = - Const.JumpPower;
            player.JumpEnabled = false;
            player.Direction = direction;
            player.Inertia.X = Const.PlayerStep * Const.InertiaPerPixel;
            if (direction == Const.Left)
                player.Inertia.X *= -1;
        }

        private static void WallSlide(Player player, bool direction)
        {
            player.SetState(PlayerState.WallSliding);
            player.Inertia.Y -= Const.WallFriction;
            player.Direction = direction;
        }

    }
}