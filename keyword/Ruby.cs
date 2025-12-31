using System;
using System.Collections.Generic;

namespace nccat.BaseKeyword {
    public class Ruby {
        static public string json { get; } = @"
        {
            ""keyword"": [
                ""BEGIN"", ""END"", ""__ENCODING__"", ""__FILE__"", ""__LINE__"", ""alias"", ""and"", 
                ""begin"", ""break"", ""case"", ""class"", ""def"", ""defined?"", ""do"", ""else"", 
                ""elsif"", ""end"", ""ensure"", ""false"", ""for"", ""if"", ""in"", ""module"", 
                ""next"", ""nil"", ""not"", ""or"", ""redo"", ""rescue"", ""retry"", ""return"", 
                ""self"", ""super"", ""then"", ""true"", ""undef"", ""unless"", ""until"", ""when"", 
                ""while"", ""yield""
            ],
            ""preprocessor"": [],
            ""specific"": [
                ""Array"", ""String"", ""Hash"", ""Integer"", ""Float"", ""Symbol"", ""Regexp"", ""File"", 
                ""Dir"", ""IO"", ""Kernel"", ""Object"", ""Class"", ""Module"", ""Exception"",
                ""puts"", ""print"", ""gets"", ""require"", ""include"", ""extend"", ""attr_reader"", 
                ""attr_writer"", ""attr_accessor"", ""new"", ""initialize"", ""each"", ""map"", ""select""
            ],
            ""comment"": [
                ""#""
            ]
        }";
    }
}
