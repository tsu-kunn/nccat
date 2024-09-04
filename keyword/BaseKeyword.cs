using System;
using System.Collections.Generic;


namespace nccat.BaseKeyword {
    public class baseKeyword
    {
        private string[]? _keyword;
        public string[]? keyword { get; set; }

        private string[]? _preprocessor;
        public string[]? preprocessor { get; set; }

        private string[]? _comment;
        public string[]? comment { get; set; }
    }
}
