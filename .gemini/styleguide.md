# nccat コーディングスタイルガイド

このドキュメントは `nccat` プロジェクトにおけるコーディングスタイル、規約、およびベストプラクティスを定義します。

## 1. 全般

- **言語:** C# (.NET 8.0)
- **文字エンコーディング:** UTF-8
- **改行コード:** LF

## 2. 命名規則

一貫性を保つため、以下の命名規則に従ってください。

- **名前空間 (Namespace):** `camelCase` を使用します。
  ```csharp
  namespace nccat.BaseKeyword
  ```
- **クラス名 (Class):** `PascalCase` を使用します。
  ```csharp
  public class Program { ... }
  public class BaseKeyword { ... }
  ```
- **メソッド名 (Method):** `PascalCase` を使用します。
  ```csharp
  public static void Main(string[] args) { ... }
  private static string HighlightPattern(...) { ... }
  ```
- **プロパティ名 (Property):** `PascalCase` または `camelCase` を使用します。既存のコードに合わせて使い分けてください。
  ```csharp
  // PascalCase
  public string[]? keyword { get; set; }

  // camelCase
  static public string json { get; }
  ```
- **ローカル変数 (Local Variable):** `camelCase` を使用します。
  ```csharp
  string highlightedLine = line;
  int lineNum = 1;
  ```
- **プライベートフィールド (Private Field):** `_` プレフィックスを付けた `camelCase` を使用します。
  ```csharp
  private const string _ansiEscapeCodeToResetForegroundColor = "\x1b[39m";
  ```

## 3. フォーマット

- **インデント:** 4つのスペースを使用します。タブは使用しません。
- **括弧 `{}`:** `if`, `for`, `while` ステートメントや、クラス、メソッドの定義では、開始の波括弧 `{` をステートメントと同じ行に配置します。
  ```csharp
  class Program
  {
      static void Main(string[] args)
      {
          if (args.Length == 0)
          {
              // ...
          }
      }
  }
  ```
- **`using` ディレクティブ:** ファイルの先頭にまとめて記述します。`System` 名前空間を先に記述し、その後に他の名前空間をアルファベット順で記述します。

## 4. コメント

- **基本:** コードの意図が明確でない場合にのみコメントを追加します。コードが *何をしているか* ではなく、 *なぜそうしているのか* を説明します。
- **形式:**
  - 単一行コメントには `//` を使用します。
  - 公開API（publicなクラスやメソッド）には、`///` で始まるXMLドキュメントコメントを推奨します。

## 5. Git と コミットメッセージ

コミットメッセージは、変更の履歴を追いやすくするための重要な情報です。

- **形式:** [Conventional Commits](https://www.conventionalcommits.org/) の規約に準拠します。
- **言語:** コミットメッセージ全体を **日本語** で記述します。
- **構造:**
  1.  **Type:** `feat` (新機能), `fix` (バグ修正), `chore` (ビルドプロセスや補助ツールの変更), `docs` (ドキュメントのみの変更) などを指定します。
  2.  **Subject:** 変更の概要を簡潔に記述します。
  3.  **Body (任意):**
      - 変更の意図や背景を詳細に記述します。
      - `-` で始まる箇条書きを使用して、具体的な変更点をリストアップします。
  4.  **Footer (任意):** `関連問題: #123` のように関連するIssue番号を記述します。

- **例:**
  ```
  feat: 新言語のキーワードハイライト機能を追加

  - C++用のキーワード定義を keyword/Cpp.cs に追加
  - ファイル拡張子に応じてシンタックス定義を切り替えるロジックを実装
  - 関連問題: #12
  ```
