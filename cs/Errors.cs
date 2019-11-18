using System;
using SDL2;

namespace shootingame
{
    class Errors
    {
        public static string msg = "";
        public static void Check(int val, string msg = "")
        {
            if (val == 0)
                return;
            
            if (msg != "")
                Errors.msg = msg;
            Print();
        }

        public static void CheckNull(IntPtr ptr, string msg = "")
        {
            if (ptr != null)
                return;

            if (msg != "")
                Errors.msg = msg;
            Print();
        }

        private static void Print()
        {
            if (msg == "")
                msg = "Error";
            
            throw new Exception($"{msg}: {SDL.SDL_GetError()}");
        }
    }
}