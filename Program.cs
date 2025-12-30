using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using MtoLib;
using nccat.BaseKeyword;

namespace nccat
{
    // ... (HighlightRule and MatchResult classes remain the same)
    class HighlightRule
    {
        public Regex Pattern { get; }
        public ConsoleColor Color { get; }
        public int Priority { get; }

        public HighlightRule(string pattern, ConsoleColor color, int priority)
        {
            Pattern = new Regex(pattern, RegexOptions.Compiled);
            Color = color;
            Priority = priority;
        }
    }

    class MatchResult : IComparable<MatchResult>
    {
        public int Index { get; }
        public int Length { get; }
        public ConsoleColor Color { get; }
        public int Priority { get; }

        public MatchResult(int index, int length, ConsoleColor color, int priority)
        {
            Index = index;
            Length = length;
            Color = color;
            Priority = priority;
        }

        public int CompareTo(MatchResult? other)
        {
            if (other == null) return 1;
            int indexComparison = Index.CompareTo(other.Index);
            if (indexComparison != 0) return indexComparison;
            return Priority.CompareTo(other.Priority);
        }
    }

    class Program
    {
        // 通常のテキスト（ブロックコメント以外）をハイライトするメソッド
        private static string HighlightText(string text, List<HighlightRule> rules)
        {
            // (This method remains the same as before)
            if (string.IsNullOrEmpty(text) || !rules.Any()) return text;

            var allMatches = new List<MatchResult>();
            foreach (var rule in rules)
            {
                foreach (Match match in rule.Pattern.Matches(text))
                {
                    if (match.Success && match.Length > 0)
                        allMatches.Add(new MatchResult(match.Index, match.Length, rule.Color, rule.Priority));
                }
            }
            allMatches.Sort();

            var finalMatches = new List<MatchResult>();
            int lastMatchEnd = -1;
            foreach (var match in allMatches)
            {
                if (match.Index >= lastMatchEnd)
                {
                    finalMatches.Add(match);
                    lastMatchEnd = match.Index + match.Length;
                }
            }

            var sb = new StringBuilder();
            int currentPos = 0;
            foreach (var match in finalMatches)
            {
                if (match.Index > currentPos) sb.Append(text.Substring(currentPos, match.Index - currentPos));
                sb.Append(ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode(match.Color));
                sb.Append(text.Substring(match.Index, match.Length));
                sb.Append(ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode((ConsoleColor)(-1)));
                currentPos = match.Index + match.Length;
            }
            if (currentPos < text.Length) sb.Append(text.Substring(currentPos));

            return sb.ToString();
        }

        // ファイルパスに応じてハイライトルールを取得するメソッド
        private static List<HighlightRule> GetRulesForFile(string? path)
        {
            var rules = new List<HighlightRule>();
            string? json = null;

            if (!string.IsNullOrEmpty(path))
            {
                string extension = Path.GetExtension(path).ToLower();
                switch (extension)
                {
                    case ".c":
                    case ".h":
                        json = C.json; break;
                    case ".cpp":
                    case ".hpp":
                    case ".cc":
                        json = Cpp.json; break;
                    case ".cs":
                        json = Csharp.json; break;
                    case ".rs":
                        json = Rust.json; break;
                    case ".js":
                        json = JavaScript.json; break;
                }
            }
            
            if (string.IsNullOrEmpty(json)) return rules;

            BaseKeyword.BaseKeyword? syntaxRules = Json.JsonToBaseKeyword(json);
            if (syntaxRules == null) return rules;

            // --- ルールリストの構築 ---
            // 文字列 "" のルールはメインのステートマシンで処理するため、ここでは定義しない
            rules.Add(new HighlightRule(@"(//.*)", ConsoleColor.Green, 1));
            
            if (syntaxRules.specific != null && syntaxRules.specific.Any())
            {
                var pattern = @"\b(" + string.Join("|", syntaxRules.specific.OrderByDescending(s => s.Length).Select(Regex.Escape)) + @")\b";
                rules.Add(new HighlightRule(pattern, ConsoleColor.Yellow, 2));
            }
            if (syntaxRules.preprocessor != null && syntaxRules.preprocessor.Any())
            {
                var pattern = "(" + string.Join("|", syntaxRules.preprocessor.OrderByDescending(s => s.Length).Select(Regex.Escape)) + ")";
                rules.Add(new HighlightRule(pattern, ConsoleColor.Magenta, 3));
            }
            if (syntaxRules.keyword != null && syntaxRules.keyword.Any())
            {
                var pattern = @"\b(" + string.Join("|", syntaxRules.keyword.OrderByDescending(s => s.Length).Select(Regex.Escape)) + @")\b";
                rules.Add(new HighlightRule(pattern, ConsoleColor.Cyan, 4));
            }

            return rules;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0 && !Console.IsInputRedirected)
            {
                Console.WriteLine("Usage: nccat <file_path>");
                Console.WriteLine("Or pipe content to it: cat <file_path> | nccat");
                return;
            }

            string? filePath = args.Length > 0 ? args[0] : null;
            List<HighlightRule> rules = GetRulesForFile(filePath);

            Action<TextReader> processContent = (reader) =>
            {
                int lineNum = 1;
                string? line;
                bool inBlockComment = false;
                var blockCommentColor = ConsoleColor.Green;
                var stringColor = ConsoleColor.DarkYellow;
                var resetColor = ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode((ConsoleColor)(-1));

                while ((line = reader.ReadLine()) != null)
                {
                    var sb = new StringBuilder();
                    int currentPos = 0;
                    sb.AppendFormat("{0, -5}: ", lineNum++);

                    while (currentPos < line.Length)
                    {
                        if (inBlockComment)
                        {
                            int endIndex = line.IndexOf("*/", currentPos);
                            if (endIndex == -1)
                            {
                                sb.Append(ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode(blockCommentColor));
                                sb.Append(line.Substring(currentPos));
                                sb.Append(resetColor);
                                currentPos = line.Length;
                            }
                            else
                            {
                                int length = (endIndex + 2) - currentPos;
                                sb.Append(ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode(blockCommentColor));
                                sb.Append(line.Substring(currentPos, length));
                                sb.Append(resetColor);
                                currentPos += length;
                                inBlockComment = false;
                            }
                        }
                        else
                        {
                            int commentIndex = line.IndexOf("/*", currentPos);
                            int quoteIndex = line.IndexOf('"', currentPos);

                            if (quoteIndex != -1 && (quoteIndex < commentIndex || commentIndex == -1))
                            {
                                // 文字列がコメントより先に見つかった
                                sb.Append(HighlightText(line.Substring(currentPos, quoteIndex - currentPos), rules));
                                
                                int endQuoteIndex = quoteIndex + 1;
                                while (endQuoteIndex < line.Length)
                                {
                                    if (line[endQuoteIndex] == '"' && line[endQuoteIndex - 1] != '\\') break;
                                    endQuoteIndex++;
                                }

                                if (endQuoteIndex < line.Length)
                                {
                                    int length = (endQuoteIndex + 1) - quoteIndex;
                                    sb.Append(ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode(stringColor));
                                    sb.Append(line.Substring(quoteIndex, length));
                                    sb.Append(resetColor);
                                    currentPos = endQuoteIndex + 1;
                                }
                                else
                                {
                                    sb.Append(ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode(stringColor));
                                    sb.Append(line.Substring(quoteIndex));
                                    sb.Append(resetColor);
                                    currentPos = line.Length;
                                }
                            }
                            else if (commentIndex != -1)
                            {
                                // コメントが文字列より先に見つかった
                                sb.Append(HighlightText(line.Substring(currentPos, commentIndex - currentPos), rules));
                                currentPos = commentIndex;
                                inBlockComment = true;
                            }
                            else
                            {
                                // コメントも文字列も見つからなかった
                                sb.Append(HighlightText(line.Substring(currentPos), rules));
                                currentPos = line.Length;
                            }
                        }
                    }
                    Console.WriteLine(sb.ToString());
                }
            };

            if (Console.IsInputRedirected)
            {
                processContent(Console.In);
            }
            else
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Error: File not found at '{filePath}'");
                    return;
                }
                using (StreamReader sr = new StreamReader(filePath!))
                {
                    processContent(sr);
                }
            }
        }
    }
}
