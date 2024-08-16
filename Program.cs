using System;
using System.Collections.Generic;
using System.Text.Json;


namespace pict2png {
    public class baseKeyword
    {
        private string[]? _keyword;
        public string[]? keyword { get; set; }

        private string[]? _control;
        public string[]? control { get; set; }

        private string[]? _comment;
        public string[]? comment { get; set; }
    }

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
        private static JsonSerializerOptions GetOption()
        {
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
                WriteIndented = true
            };
        
            return options;
        }

        public static baseKeyword? JsonTobaseKeyword(string json)
        {
            if (!String.IsNullOrEmpty(json))
            {
                try {
                    baseKeyword? bkey = JsonSerializer.Deserialize<baseKeyword>(json, Program.GetOption());
                    return bkey;
                }
                catch (JsonException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }

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

                // クラスオブジェクトをJsonに変換
                baseKeyword keyC = new baseKeyword {
                    keyword = new string[] {"char", "short", "int", "long"},
                    control = new string[] {"define", "ifdef", "endif"},
                    comment = new string[] {"日本語テスト", "🎮"}
                };

                var jsonText = JsonSerializer.Serialize(keyC, GetOption());
                Console.WriteLine(jsonText);

                // Jsonからクラスオブジェクトに変換
                string baseText = @"
                {
                    ""keyword"": [
                        ""char"",
                        ""short"",
                        ""int"",
                        ""long""
                    ],
                    ""control"": [
                        ""define"",
                        ""ifdef"",
                        ""endif""
                    ],
                    ""comment"": [
                        ""日本語テスト"",
                        ""\uD83C\uDFAE""
                    ]
                }";

                baseKeyword? keyCpp = JsonTobaseKeyword(baseText);
                if (keyCpp != null) {
                    if (keyCpp.keyword != null)
                    {
                        Console.WriteLine("keyword->");
                        foreach(string s in keyCpp.keyword)
                        {
                            Console.WriteLine(s);
                        }
                    }

                    if (keyCpp.control != null)
                    {
                        Console.WriteLine("control->");
                        foreach(string s in keyCpp.control)
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

