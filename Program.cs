using System;
using System.Collections.Generic;
using System.Text.Json;


namespace nccat {
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

                if (args.Length > 0) {
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

                // クラスオブジェクトをJsonに変換
                var jsonText = BaseKeyword.Json.KeywordToJson(BaseKeyword.C.word);
                Console.WriteLine(jsonText);

                // Jsonからクラスオブジェクトに変換
                BaseKeyword.BaseKeyword? keyCpp = BaseKeyword.Json.JsonToBaseKeyword(BaseKeyword.C.json);

                if (keyCpp != null) {
                    if (keyCpp.keyword != null)
                    {
                        Console.WriteLine("keyword->");
                        foreach(string s in keyCpp.keyword)
                        {
                            Console.WriteLine(s);
                        }
                    }

                    if (keyCpp.preprocessor != null)
                    {
                        Console.WriteLine("preprocessor->");
                        foreach(string s in keyCpp.preprocessor)
                        {
                            Console.WriteLine(s);
                        }
                    }

                    if (keyCpp.comment != null)
                    {
                        Console.WriteLine("comment->");
                        foreach(string s in keyCpp.comment)
                        {
                            Console.WriteLine(s);
                        }
                    }
                }
            }
        }
    }
}

