using System;
using System.IO;
using System.Threading.Tasks;

namespace server
{
    class Prompt
    {
        public static string ReadLine()
        {
            if (readTask is null) {
                readTask = Task.Run(Console.ReadLine);
                return null;
            }

            if (readTask.IsCompleted) {
                string line = readTask.Result;
                readTask = Task.Run(Console.ReadLine);
                return line;
            }
            
            return null;
        }

        private static Task<string> readTask;
    }
}