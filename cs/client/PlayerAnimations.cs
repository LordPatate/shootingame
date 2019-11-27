using System;
using SFML.Graphics;
using SFML.System;

namespace shootingame
{
    public class PlayerAnimations
    {
        private static readonly Vector2i[] idleCoords = {
            new Vector2i(0, 0), new Vector2i(0, 0),
            new Vector2i(0, 1), new Vector2i(0, 1),
            new Vector2i(0, 2), new Vector2i(0, 2),
            new Vector2i(0, 3), new Vector2i(0, 3)
        };
        private static readonly Vector2i[] runningCoords = {
            new Vector2i(1, 1), new Vector2i(1, 2),
            new Vector2i(1, 3), new Vector2i(1, 4),
            new Vector2i(1, 5), new Vector2i(1, 6)
        };
        private static readonly Vector2i[] jumpingCoords = {
            new Vector2i(2, 0), new Vector2i(2, 1),
            new Vector2i(2, 2), new Vector2i(2, 2),
            new Vector2i(2, 3), new Vector2i(2, 4)
        };
        private static readonly Vector2i[] fallingCoords = {
            new Vector2i(3, 1), new Vector2i(3, 2)
        };
        private static readonly Vector2i[] wallSlidingCoords = {
            new Vector2i(11, 2), new Vector2i(11, 3)
        };
        private static readonly Vector2i[] wallJumpingCoords = {
            new Vector2i(11, 0), new Vector2i(11, 1)
        };
        private static readonly Vector2i[] swingingCoords = {
            new Vector2i(11, 0), new Vector2i(11, 1)
        };

        private static readonly Vector2i[][] spriteCoord = {
            idleCoords, runningCoords, jumpingCoords, fallingCoords, wallSlidingCoords, wallJumpingCoords, swingingCoords
        };

        public static void SetTextureArea(Player player)
        {
            var spriteCoordArray = spriteCoord[(int)player.State];
            player.Frame %= (uint)(spriteCoordArray.Length * Const.StepsPerFrame);
            Vector2i coord = spriteCoordArray[player.Frame / Const.StepsPerFrame];

            player.TextureArea = new IntRect(
                left: coord.Y * Const.PlayerSpriteWidth,
                top: coord.X * Const.PlayerSpriteHeight,
                width: Const.PlayerSpriteWidth,
                height: Const.PlayerSpriteHeight
            );
            if (player.Direction == Const.Left) {
                player.TextureArea.Left += Const.PlayerSpriteWidth;
                player.TextureArea.Width *= -1;
            }

            ++player.Frame;
        }
    }
}