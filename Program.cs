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

            if (Console.IsInputRedirected)
            {
                Console.WriteLine("リダイレクトからの入力");
                
                TextReader textReader = Console.In;
                string? line;

                while ((line = textReader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
            else
            {
                Console.WriteLine("通常の入力");
            }
        }
    }
}

