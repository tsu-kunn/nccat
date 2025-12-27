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
            // RegexOptions.Compiledで正規表現をコンパイルし、パフォーマンスを向上させる
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

        // 並べ替えのための比較ロジック
        public int CompareTo(MatchResult? other)
        {
            if (other == null) return 1;
            int indexComparison = Index.CompareTo(other.Index);
            if (indexComparison != 0)
            {
                return indexComparison;
            }
            // インデックスが同じ場合は、優先順位が高い（数値が小さい）方を先にする
            return Priority.CompareTo(other.Priority);
        }
    }

    class Program
    {
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
            if (syntaxRules == null)
            {
                Console.WriteLine("Failed to load syntax rules.");
                return;
            }

            var rules = new List<HighlightRule>();
            // 優先順位: 数値が小さいほど高い
            // 候補が競合した場合（例: #if と if）、優先順位が高いものが勝つ
            rules.Add(new HighlightRule(@"(\""[^\n]*?\"")", ConsoleColor.DarkYellow, 1)); // 1: 文字列
            rules.Add(new HighlightRule(@"(//.*)", ConsoleColor.Green, 1));             // 1: コメント

            if (syntaxRules.specific != null && syntaxRules.specific.Any())
            {
                var pattern = @"\b(" + string.Join("|", syntaxRules.specific.OrderByDescending(s => s.Length).Select(Regex.Escape)) + @")\b";
                rules.Add(new HighlightRule(pattern, ConsoleColor.Yellow, 2));         // 2: 型名など
            }
            if (syntaxRules.preprocessor != null && syntaxRules.preprocessor.Any())
            {
                // プリプロセッサは単語境界\bを使わない
                var pattern = "(" + string.Join("|", syntaxRules.preprocessor.OrderByDescending(s => s.Length).Select(Regex.Escape)) + ")";
                rules.Add(new HighlightRule(pattern, ConsoleColor.Magenta, 3));       // 3: プリプロセッサ
            }
            if (syntaxRules.keyword != null && syntaxRules.keyword.Any())
            {
                var pattern = @"\b(" + string.Join("|", syntaxRules.keyword.OrderByDescending(s => s.Length).Select(Regex.Escape)) + @")\b";
                rules.Add(new HighlightRule(pattern, ConsoleColor.Cyan, 4));             // 4: キーワード
            }

            // --- メイン処理 ---
            Action<TextReader> processContent = (reader) =>
            {
                int lineNum = 1;
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var allMatches = new List<MatchResult>();
                    
                    // 1. すべてのルールでマッチを検索し、結果をリストに格納
                    foreach (var rule in rules)
                    {
                        foreach (Match match in rule.Pattern.Matches(line))
                        {
                            if (match.Success && match.Length > 0)
                            {
                                allMatches.Add(new MatchResult(match.Index, match.Length, rule.Color, rule.Priority));
                            }
                        }
                    }

                    // 2. マッチ結果をソート (開始位置 → 優先順位)
                    allMatches.Sort();

                    // 3. 重複するマッチを排除（優先順位が低いものを捨てる）
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

                    // 4. 行を再構築してハイライト
                    var sb = new StringBuilder();
                    int currentPos = 0;
                    foreach (var match in finalMatches)
                    {
                        // マッチしていない部分を追加
                        if (match.Index > currentPos)
                        {
                            sb.Append(line.Substring(currentPos, match.Index - currentPos));
                        }
                        
                        // マッチした部分を色付きで追加
                        sb.Append(ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode(match.Color));
                        sb.Append(line.Substring(match.Index, match.Length));
                        sb.Append(ConsoleColorExtensions.ToForeGroundColorAnsiEscapeCode((ConsoleColor)(-1))); // 色をリセット

                        currentPos = match.Index + match.Length;
                    }

                    // 最後のマッチ以降の残り部分を追加
                    if (currentPos < line.Length)
                    {
                        sb.Append(line.Substring(currentPos));
                    }

                    Console.WriteLine("{0, -5}: {1}", lineNum++, sb.ToString());
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

