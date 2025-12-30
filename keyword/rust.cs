using System;
using System.Collections.Generic;

namespace nccat.BaseKeyword {
    public class Rust {
        static public string json { get; } = @"
        {
            ""keyword"": [
                ""as"", ""async"", ""await"", ""break"", ""const"", ""continue"", ""crate"", ""dyn"", ""else"", 
                ""enum"", ""extern"", ""false"", ""fn"", ""for"", ""if"", ""impl"", ""in"", ""let"", ""loop"", 
                ""match"", ""mod"", ""move"", ""mut"", ""pub"", ""ref"", ""return"", ""Self"", ""self"", 
                ""static"", ""struct"", ""super"", ""trait"", ""true"", ""type"", ""union"", ""unsafe"", 
                ""use"", ""where"", ""while"",
                ""abstract"", ""become"", ""box"", ""do"", ""final"", ""macro"", ""override"", ""priv"", 
                ""try"", ""typeof"", ""unsized"", ""virtual"", ""yield""
            ],
            ""preprocessor"": [
                ""#"", ""#!"", ""#[""
            ],
            ""specific"": [
                ""i8"", ""i16"", ""i32"", ""i64"", ""i128"", ""isize"",
                ""u8"", ""u16"", ""u32"", ""u64"", ""u128"", ""usize"",
                ""f32"", ""f64"", ""char"", ""bool"", ""str"", ""String"",
                ""Option"", ""Result"", ""Vec"",
                ""println!"", ""eprintln!"", ""print!"", ""eprint!"", ""vec!"", ""format!"",
                ""assert!"", ""assert_eq!"", ""assert_ne!"",
                ""panic!"", ""unreachable!"", ""todo!"", ""dbg!""
            ],
            ""comment"": [
                ""//""
            ]
        }";
    }
}
