using System;
using System.Collections.Generic;

namespace pict2png {
    public class AppMain()
    {
        private void HelloWorld()
        {
            Console.WriteLine("Hello, World!");
        }

        public void Run()
        {
            this.HelloWorld();
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            AppMain app = new AppMain();
            app.Run();

            string? line;
            int lineNum = 1;

            if (Console.IsInputRedirected)
            {
                Console.WriteLine("リダイレクトからの入力");
                
                TextReader textReader = Console.In;

                while ((line = textReader.ReadLine()) != null)
                {
                    Console.WriteLine("{0, -5}: {1}", lineNum, line);
                    lineNum++;
                }
            }
            else
            {
                Console.Write(MtoLib.ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode(ConsoleColor.Red));
                Console.WriteLine("通常の入力");
                Console.Write(MtoLib.ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode((ConsoleColor)(-1)));

                string path = args[0];

                using (StreamReader sr = new StreamReader(path))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine("{0, -5}: {1}", lineNum, line);
                        lineNum++;
                    }
                }
            }
        }
    }
}

