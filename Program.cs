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
        private static JsonSerializerOptions GetOption()
        {
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
                WriteIndented = true
            };
        
            return options;
        }

        public static BaseKeyword.baseKeyword? JsonTobaseKeyword(string json)
        {
            if (!String.IsNullOrEmpty(json))
            {
                try {
                    BaseKeyword.baseKeyword? bkey = JsonSerializer.Deserialize<BaseKeyword.baseKeyword>(json, Program.GetOption());
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
                BaseKeyword.baseKeyword keyC = new BaseKeyword.baseKeyword {
                    keyword = new string[] {"char", "short", "int", "long"},
                    preprocessor = new string[] {"define", "ifdef", "endif"},
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
                    ""preprocessor"": [
                        ""define"",
                        ""ifdef"",
                        ""endif""
                    ],
                    ""comment"": [
                        ""日本語テスト"",
                        ""\uD83C\uDFAE""
                    ]
                }";

                //BaseKeyword.C cword = new BaseKeyword.C();
                //if (cword.word == null) Console.WriteLine("cword is null");
                if (BaseKeyword.C.word == null) Console.WriteLine("cwod is null");

                BaseKeyword.baseKeyword? keyCpp = JsonTobaseKeyword(BaseKeyword.C.word == null ? baseText : BaseKeyword.C.word);

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

