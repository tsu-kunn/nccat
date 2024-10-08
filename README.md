# nccat
Linux環境でC#と.NET8の勉強を兼ねて欲しいツールを作成。 \
超簡易 cat に行番号とシンタックスハイライトを追加したものを目指す。\
その他は使ってみて不満だったところを修正していく。

[bat](https://github.com/sharkdp/bat) で十二分に満たせてるけど、自分の要望には過剰なのでシンプルなものを作りたくなった。

## やりたいこと
- C#と.NET8の勉強
- 指定されたテキストファイルを行番号付きで出力
- 特定のファイルはシンタックスハイライトで出力
  - C/C++/C#/sh/markdown の対応を目標
- パイプライン対応

## 調べること
- [ ] シンタックスハイライトのやり方
  - 正規表現を駆使して対応？
    - JavaScriptとか他のものを含めて色々調べた結果、やはり正規表現を使ってる
    - .NETの `Console.ForegroundColor` を使えばコンソール出力の文字色を変えられる
      - [使い方](https://learn.microsoft.com/ja-jp/dotnet/api/system.console.foregroundcolor?view=net-8.0)
      - [c#コンソールアプリケーションで標準エラー出力に色を付ける方法](https://qiita.com/rougemeilland/items/9f272db7e0252c2f48d3) より、ANSIエスケープコードを使用するのがよさそう。
- [ ] パイプラインの対応方法
  - 入力と出力の対応方法
    - 標準入力からかの確認は [Console.IsInputRedirected](https://learn.microsoft.com/en-us/dotnet/api/system.console.isinputredirected?view=netframework-4.8#System_Console_IsInputRedirected) でできる？
    - 標準入力の取得は `Console.In` でできそう
  - batはファイルの指定がない場合はパイプライン処理としている
    - `$ batcat` と実行すると入力待ちとなるので、自分のやりたいこととはちょっと違う結果になる
- [ ] 同じ処理の共通化
  - 同じ処理がちらほら出てきたので共通化したい！
