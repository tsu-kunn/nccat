using System;
using System.Collections.Generic;

namespace nccat.BaseKeyword {
    public class JavaScript {
        static public string json { get; } = @"
        {
            ""keyword"": [
                ""await"", ""break"", ""case"", ""catch"", ""class"", ""const"", ""continue"", ""debugger"", 
                ""default"", ""delete"", ""do"", ""else"", ""enum"", ""export"", ""extends"", ""false"", 
                ""finally"", ""for"", ""function"", ""if"", ""import"", ""in"", ""instanceof"", ""new"", 
                ""null"", ""return"", ""super"", ""switch"", ""this"", ""throw"", ""true"", ""try"", 
                ""typeof"", ""var"", ""void"", ""while"", ""with"", ""yield"",
                ""implements"", ""interface"", ""let"", ""package"", ""private"", ""protected"", ""public"", ""static""
            ],
            ""preprocessor"": [],
            ""specific"": [
                ""Array"", ""Boolean"", ""Date"", ""Error"", ""Function"", ""JSON"", ""Math"", ""Number"", 
                ""Object"", ""Promise"", ""RegExp"", ""String"", ""Symbol"", ""Map"", ""Set"", ""WeakMap"", 
                ""WeakSet"", ""console"", ""document"", ""window"", ""alert"", ""fetch"", ""setTimeout"", 
                ""setInterval"", ""parseInt"", ""parseFloat"", ""undefined"", ""NaN"", ""Infinity"",
                ""async"", ""await"", ""of""
            ],
            ""comment"": [
                ""//""
            ]
        }";
    }
}
