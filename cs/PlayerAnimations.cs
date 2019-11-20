using System;
using SDL2;

namespace shootingame
{
    class PlayerAnimations
    {
        private static readonly SDL.SDL_Point[] idleCoords = {
            SDLFactory.MakePoint(0, 0), SDLFactory.MakePoint(0, 0),
            SDLFactory.MakePoint(0, 1), SDLFactory.MakePoint(0, 1),
            SDLFactory.MakePoint(0, 2), SDLFactory.MakePoint(0, 2),
            SDLFactory.MakePoint(0, 3), SDLFactory.MakePoint(0, 3)
        };
        private static readonly SDL.SDL_Point[] runningCoords = {
            SDLFactory.MakePoint(1, 1), SDLFactory.MakePoint(1, 2),
            SDLFactory.MakePoint(1, 3), SDLFactory.MakePoint(1, 4),
            SDLFactory.MakePoint(1, 5), SDLFactory.MakePoint(1, 6)
        };
        private static readonly SDL.SDL_Point[] jumpingCoords = {
            SDLFactory.MakePoint(2, 0), SDLFactory.MakePoint(2, 1),
            SDLFactory.MakePoint(2, 2), SDLFactory.MakePoint(2, 2),
            SDLFactory.MakePoint(2, 3), SDLFactory.MakePoint(2, 4)
        };
        private static readonly SDL.SDL_Point[] fallingCoords = {
            SDLFactory.MakePoint(3, 1), SDLFactory.MakePoint(3, 2)
        };
        private static readonly SDL.SDL_Point[] wallSlidingCoords = {
            SDLFactory.MakePoint(11, 2), SDLFactory.MakePoint(11, 3)
        };
        private static readonly SDL.SDL_Point[] wallJumpingCoords = {
            SDLFactory.MakePoint(11, 0), SDLFactory.MakePoint(11, 1)
        };

        private static readonly SDL.SDL_Point[][] spriteCoord = {
            idleCoords, runningCoords, jumpingCoords, fallingCoords, wallSlidingCoords, wallJumpingCoords
        };

        public static void SetTextureArea(Player player)
        {
            var spriteCoordArray = spriteCoord[(int)player.State];
            SDL.SDL_Point coord = spriteCoordArray[player.Frame / Const.StepsPerFrame];

            player.TextureArea = SDLFactory.MakeRect(
                x: coord.x * Const.PlayerSpriteWidth,
                y: coord.y * Const.PlayerSpriteHeight,
                w: Const.PlayerSpriteWidth,
                h: Const.PlayerSpriteHeight
            );

            ++player.Frame;
            player.Frame %= (uint)(spriteCoordArray.Length * Const.StepsPerFrame);
        }
    }
}