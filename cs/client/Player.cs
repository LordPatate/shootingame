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
        public int ID;
        public string Name;
        public int Score;
        public int Deaths;
        public IntRect Rect;
        public PlayerState State;
        public uint Frame;
        public bool Direction;
        public Vector2i HookPoint;
        public bool Hooked;
        public bool JumpEnabled;
        public Vector2i Inertia;
        public uint GunCoolDown;
        public Vector2i ShotPoint;
        public bool Shot;
        public int ShotCount;
        public bool HasRespawned;

        public void FromLightPlayer(LightPlayer player) {
            ID = player.ID;
            Rect.Left = player.Pos.X;
            Rect.Top = player.Pos.Y;
            State = player.State;
            Frame = player.Frame;
            Direction = player.Direction;
            HookPoint = new Vector2i() {
                X = player.HookPoint.X,
                Y = player.HookPoint.Y
            };
            Hooked = player.Hooked;
        }
        public Player(LightPlayer player) {
            MakeRect();
            FromLightPlayer(player);
        }
        public Player(int id, string name)
        {
            MakeRect();
            
            ID = id;
            Name = name;
            Score = 0;
            Deaths = 0;
            Inertia =  new Vector2i();
            HookPoint = new Vector2i();
            Hooked = false;
            JumpEnabled = true;
            GunCoolDown = 0;
            ShotPoint = new Vector2i();
            Shot = false;
            ShotCount = 0;
            HasRespawned = false;

            Texture = new Texture(Program.ResourceDir + Const.PlayerSpriteSheet);

            TextureArea = new IntRect(0, 0, width: Const.PlayerSpriteWidth, height: Const.PlayerSpriteHeight);
        }

        public void Draw()
        {
            PlayerAnimations.SetTextureArea(this);
            
            Func<int, int> scale = (x) => x * Const.PlayerScalePercent / 100;
            int width = scale(Const.PlayerSpriteWidth), height = scale(Const.PlayerSpriteHeight);
            IntRect dst = Geometry.AdaptRect(new IntRect(
                left: Rect.Left + Rect.Width/2 - width/2,
                top: Rect.Top + Rect.Height/2 - height/2,
                width: width, height: height
            ));

            Screen.GameScene.Draw(Drawing.SpriteOf(Texture, dst, TextureArea));

            if (Hooked) {
                VertexArray line = new VertexArray(PrimitiveType.Lines);
                line.Append(new Vertex(
                    (Vector2f)Geometry.AdaptPoint(GetCOM()),
                    new Color(red: 255, 0, 0)
                ));
                line.Append(new Vertex(
                    (Vector2f)Geometry.AdaptPoint(HookPoint),
                    new Color(red: 255, 0, 0)
                ));
                Screen.GameScene.Draw(line);
            }
        }

        public void Update(Level level)
        {
            ++Frame;

            if (Controls.RightClick) {
                if (!Hooked) {
                    Vector2i hookPoint = HitScan(level, false);
                    if (Geometry.Dist(GetCOM(), hookPoint) <= Const.HookMaxRange) {
                        Hooked = true;
                        HookPoint = hookPoint;
                        Sounds.PlayIrregularFor("hook", Const.HookSoundStep);
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

            Shot = false;
            if (GunCoolDown > 0) --GunCoolDown;
            if (GunCoolDown == 0 && Controls.LeftClick) {
                ShotPoint = HitScan(level, true);
                Shot = true;
                ShotCount += 1;
                GunCoolDown = Const.GunCoolDown;
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
        
        public void MakeRect()
        {
            Func<int, int> scale = (x) => x * Const.PlayerScalePercent / 100;
            Rect = new IntRect(0, 0, width: scale(Const.NormalStateDimX), height: scale(Const.NormalStateDimY));
        }

        private Vector2i HitScan(Level level, bool hitPlayers)
        {
            Vector2i hit = new Vector2i();
            
            Vector2i playerCOM = Geometry.AdaptPoint(GetCOM());
            Vector2i proj = Geometry.PointShade(Game.Bounds, playerCOM, Controls.MousePos.X, Controls.MousePos.Y);
            double minDist = Double.PositiveInfinity;
            
            Action<Vector2i> minimize = (point) => {
                double dist = Geometry.Dist(playerCOM, point);
                if (dist < minDist) {
                    minDist = dist;
                    hit = point;
                }
            };
            minimize(proj);

            foreach (var tile in level.Tiles)
            {
                var rect = Geometry.AdaptRect(tile.Rect);
                if (Geometry.IntersectLine(rect, playerCOM, proj)) {
                    var point = Geometry.HitPoint(rect, playerCOM, proj);
                    minimize(point);
                }
            }
            if (hitPlayers) {
                foreach (var lightPlayer in Game.Players)
                {
                    if (lightPlayer == null || lightPlayer.ID == ID)
                        continue;
                    var player = new Player(lightPlayer);
                    var rect = Geometry.AdaptRect(player.Rect);
                    if (Geometry.IntersectLine(rect, playerCOM, proj)) {
                        var point = Geometry.HitPoint(rect, playerCOM, proj);
                        minimize(point);
                    }
                }
            }
            
            hit.X = (hit.X - Game.Bounds.Left)*level.Bounds.Width/Game.Bounds.Width;
            hit.Y = (hit.Y - Game.Bounds.Top)*level.Bounds.Height/Game.Bounds.Height;
            return hit;
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
