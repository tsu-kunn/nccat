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
    // ハイライトのルール（パターン、色、優先順位）を保持するクラス
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

    // マッチした結果を保持するクラス
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
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var allMatches = new List<MatchResult>();
            foreach (var rule in rules)
            {
                foreach (Match match in rule.Pattern.Matches(text))
                {
                    if (match.Success && match.Length > 0)
                    {
                        allMatches.Add(new MatchResult(match.Index, match.Length, rule.Color, rule.Priority));
                    }
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
                if (match.Index > currentPos)
                {
                    sb.Append(text.Substring(currentPos, match.Index - currentPos));
                }
                sb.Append(ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode(match.Color));
                sb.Append(text.Substring(match.Index, match.Length));
                sb.Append(ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode((ConsoleColor)(-1)));
                currentPos = match.Index + match.Length;
            }

            if (currentPos < text.Length)
            {
                sb.Append(text.Substring(currentPos));
            }

            return sb.ToString();
        }

        static void Main(string[] args)
        {
            if (args.Length == 0 && !Console.IsInputRedirected)
            {
                Console.WriteLine("Usage: nccat <file_path>");
                Console.WriteLine("Or pipe content to it: cat <file_path> | nccat");
                return;
            }

            // --- ハイライトルールの定義 ---
            BaseKeyword.BaseKeyword? syntaxRules = Json.JsonToBaseKeyword(C.json);
            if (syntaxRules == null) { /* ... error handling ... */ return; }

            var rules = new List<HighlightRule>();
            rules.Add(new HighlightRule(@"(\""[^\n]*?\"")", ConsoleColor.DarkYellow, 1)); // 1: 文字列
            rules.Add(new HighlightRule(@"(//.*)", ConsoleColor.Green, 1));             // 1: 単一行コメント
            if (syntaxRules.specific != null) { /* ... add rule ... */ }
            if (syntaxRules.preprocessor != null) { /* ... add rule ... */ }
            if (syntaxRules.keyword != null && syntaxRules.keyword.Any())
            {
                var pattern = @"\b(" + string.Join("|", syntaxRules.keyword.OrderByDescending(s => s.Length).Select(Regex.Escape)) + @")\b";
                rules.Add(new HighlightRule(pattern, ConsoleColor.Cyan, 4));
            }
            if (syntaxRules.preprocessor != null && syntaxRules.preprocessor.Any())
            {
                var pattern = "(" + string.Join("|", syntaxRules.preprocessor.OrderByDescending(s => s.Length).Select(Regex.Escape)) + ")";
                rules.Add(new HighlightRule(pattern, ConsoleColor.Magenta, 3));
            }
             if (syntaxRules.specific != null && syntaxRules.specific.Any())
            {
                var pattern = @"\b(" + string.Join("|", syntaxRules.specific.OrderByDescending(s => s.Length).Select(Regex.Escape)) + @")\b";
                rules.Add(new HighlightRule(pattern, ConsoleColor.Yellow, 2));
            }


            // --- メイン処理 ---
            Action<TextReader> processContent = (reader) =>
            {
                int lineNum = 1;
                string? line;
                bool inBlockComment = false;
                var blockCommentColor = ConsoleColor.Green;
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
                            int startIndex = line.IndexOf("/*", currentPos);
                            if (startIndex == -1)
                            {
                                sb.Append(HighlightText(line.Substring(currentPos), rules));
                                currentPos = line.Length;
                            }
                            else
                            {
                                sb.Append(HighlightText(line.Substring(currentPos, startIndex - currentPos), rules));
                                currentPos = startIndex;
                                inBlockComment = true;
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
                string path = args[0];
                if (!File.Exists(path))
                {
                    Console.WriteLine($"Error: File not found at '{path}'");
                    return;
                }
                using (StreamReader sr = new StreamReader(path))
                {
                    processContent(sr);
                }
            }
        }
    }
}

