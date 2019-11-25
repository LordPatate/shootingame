using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static SFML.Window.Keyboard;

namespace shootingame
{
    class Controls
    {
        public static bool Left, Right, Jump;
        public static Vector2i MousePos;
        public static bool LeftClick, RightClick;
        public static void Update()
        {
            // Keys
            Left = IsKeyPressed(Key.A) || IsKeyPressed(Key.Q);
            Right = IsKeyPressed(Key.D);
            Jump = IsKeyPressed(Key.W) || IsKeyPressed(Key.Z);

            // Mouse
            MousePos = Mouse.GetPosition(Screen.Window);
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
            }

            projection.Left += player.Rect.Width + 2;
            if (Collision(ref projection, level)) {
                OnWall(player, Const.Right);
            }
            
            int maxSpeed = Const.PlayerStep * Const.InertiaPerPixel;

            // Movement
            if (Left) {
                player.Inertia.X -= Const.AirMovePower;
                player.Inertia.X = Math.Max(player.Inertia.X, -maxSpeed);
            }
            if (Right) {
                player.Inertia.X += Const.AirMovePower;
                player.Inertia.X = Math.Min(player.Inertia.X, maxSpeed);
            }
            
            // Slowing
            if (player.Inertia.X > 0) {
                player.Inertia.X -= Const.AirSlow;
                player.Inertia.X = Math.Max(player.Inertia.X, 0);
            } else {
                player.Inertia.X += Const.AirSlow;
                player.Inertia.X = Math.Min(player.Inertia.X, 0);
            }
            
            // Gravity
            player.Inertia.Y += Const.Gravity;
        }

        public void Swing(Player player, Level level)
        {
            Vector2i playerCOM = player.GetCOM();
            int x = player.HookPoint.X, y = player.HookPoint.Y;
            player.SetState(PlayerState.Swinging);

            MoveX(Inertia.X/Const.InertiaPerPixel, level);
            MoveY(Inertia.Y/Const.InertiaPerPixel, level);
        }

        public static bool Collision(ref IntRect projection, Level level)
        {
            foreach (Tile tile in level.Tiles) {
                var rect = tile.Rect;
                if (projection.Intersects(rect)) {
                    projection = rect;
                    return true;
                }
            }
            
            IntRect bounds = level.Bounds;
            projection.Intersects(bounds, out IntRect overLap);
            if (!projection.Equals(overLap)) {
                projection.Left = bounds.Left + bounds.Width;
                projection.Top = bounds.Top + bounds.Height;
                projection.Width = -bounds.Width;
                projection.Height = -bounds.Height;
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
            if (Jump && player.JumpEnabled) {
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