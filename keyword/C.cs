using System;
using System.Collections.Generic;


namespace nccat.BaseKeyword {
    public class C {
        static public string json { get; } = @"
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


        static public BaseKeyword word { get; } = new BaseKeyword {
            keyword = new string[] {
                "char",
                "short",
                "int",
                "long"
            },

            preprocessor = new string[] {
                "define",
                "ifdef",
                "endif"
            },

            comment = new string[] {
                "日本語テスト",
                "🎮"
            }
        };
    }
}

