using System;
using System.Collections.Generic;
using System.Text.Json;


namespace nccat.BaseKeyword {
    public class BaseKeyword
    {
        public string[]? keyword { get; set; }
        public string[]? preprocessor { get; set; }
        public string[]? comment { get; set; }
    }

    public class Json
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

        public static string? KeywordToJson(BaseKeyword keyword)
        {
            if (keyword != null)
            {
                try {
                    var jsonText = JsonSerializer.Serialize(keyword, GetOption());
                    return jsonText;
                }
                catch (JsonException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }


        public static BaseKeyword? JsonToBaseKeyword(string json)
        {
            if (!String.IsNullOrEmpty(json))
            {
                try {
                    BaseKeyword? bkey = JsonSerializer.Deserialize<BaseKeyword>(json, GetOption());
                    return bkey;
                }
                catch (JsonException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }
    }
}
