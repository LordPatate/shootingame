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

    class Player
    {
        public SDL.SDL_Rect Rect;
        public SDL.SDL_Point Inertia;
        public bool Direction;
        public bool JumpEnabled;
        
        public void Destroy()
        {
            
        }

        public void Copy()
        {

        }

        public void Update()
        {
            
        }

        public void SetState(PlayerState state)
        {

        }
    }
}