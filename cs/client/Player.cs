using System;
using SFML.Graphics;
using SFML.System;

namespace shootingame
{
    public enum PlayerState
    {
        Idle,
        Running,
        Jumping,
        Falling,
        WallSliding,
        WallJumping,
        Swinging
    }

    public class Player
    {
        public Texture Texture;
        public IntRect TextureArea;
        public uint ID;
        public IntRect Rect;
        public PlayerState State;
        public uint Frame;
        public bool Direction;
        public Vector2i HookPoint;
        public bool Hooked;
        public bool JumpEnabled;
        public Vector2i Inertia;

        public void FromLightPlayer(LightPlayer player) {
            ID = player.ID;
            Rect = new IntRect() {
                Left = player.Rect.Left,
                Top = player.Rect.Top,
                Width = player.Rect.Width,
                Height = player.Rect.Height
            };
            State = player.State;
            Direction = player.Direction;
            HookPoint = new Vector2i() {
                X = player.HookPoint.X,
                Y = player.HookPoint.Y
            };
            Hooked = player.Hooked;
        }
        public Player(LightPlayer player) {
            FromLightPlayer(player);
        }
        public Player(uint id)
        {
            ID = id;
            Func<int, int> scale = (x) => x * Const.PlayerScalePercent / 100;

            Rect = new IntRect(0, 0, width: scale(Const.NormalStateDimX), height: scale(Const.NormalStateDimY));
            Inertia =  new Vector2i();
            HookPoint = new Vector2i();
            Hooked = false;
            JumpEnabled = true;

            Texture = new Texture(Const.PlayerSpriteSheet);

            TextureArea = new IntRect(0, 0, width: Const.PlayerSpriteWidth, height: Const.PlayerSpriteHeight);
        }
        
        public void Destroy()
        {
        }

        public void Draw()
        {
            PlayerAnimations.SetTextureArea(this);
            
            Func<int, int> scale = (x) => x * Const.PlayerScalePercent / 100;
            int width = scale(Const.PlayerSpriteWidth), height = scale(Const.PlayerSpriteHeight);
            IntRect dst = new IntRect(
                left: Game.Bounds.Left + Rect.Left + Rect.Width/2 - width/2,
                top: Game.Bounds.Top + Rect.Top + Rect.Height/2 - height/2,
                width: width, height: height
            );

            Screen.GameScene.Draw(Drawing.SpriteOf(Texture, dst, TextureArea));

            if (Hooked) {
                VertexArray line = new VertexArray(PrimitiveType.Lines);
                line.Append(new Vertex((Vector2f)GetCOM(), new Color(red: 255, 0, 0)));
                line.Append(new Vertex((Vector2f)HookPoint, new Color(red: 255, 0, 0)));
                Screen.GameScene.Draw(line);
            }
        }

        public void Update(Level level)
        {
            Controls.Update();

            if (Controls.RightClick) {
                if (!Hooked) {
                    if (Hooked = TryHook(level, out Vector2i hookPoint)) {
                        HookPoint = hookPoint;
                    }
                } else {
                    if (Geometry.Dist(GetCOM(), HookPoint) <= Const.HookMaxRange) {
                        Controls.Swing(this, level);
                        return;
                    }
                    Hooked = false;
                }
            } else {
                Hooked = false;
            }

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

        public Vector2i GetCOM()
        {
            return new Vector2i(
                x: Rect.Left + Rect.Width/2,
                y: Rect.Top + Rect.Height/2 
            );
        }

        public bool TryHook(Level level, out Vector2i hookPoint)
        {
            bool ok = false;
            Vector2i tmp = new Vector2i();
            
            Vector2i playerCOM = GetCOM();
            Vector2i proj = Geometry.PointShade(level.Bounds, playerCOM, Controls.MousePos.X, Controls.MousePos.Y);
            double minDist = Double.PositiveInfinity;
            
            Action<Vector2i> minimize = (point) => {
                double dist = Geometry.Dist(playerCOM, point);
                if (dist < minDist && dist <= Const.HookMaxRange) {
                    minDist = dist;
                    tmp = point;
                    ok = true;
                }
            };
            minimize(proj);

            foreach (var tile in level.Tiles)
            {
                if (Geometry.IntersectLine(tile.Rect, playerCOM, proj)) {
                    var point = Geometry.HitPoint(tile.Rect, playerCOM, proj);
                    minimize(point);
                }
            }
            
            hookPoint = tmp;
            return ok;
        }

        public void MoveX(int delta, Level level)
        {
            IntRect projection = new IntRect(
                left: Rect.Left + delta,
                top: Rect.Top, width: Rect.Width, height: Rect.Height
            );

            if (Controls.Collision(ref projection, level))
            {
                Inertia.X = 0;
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

            if (Controls.Collision(ref projection, level))
            {
                Inertia.Y = 0;
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