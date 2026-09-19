# OpenTween 個人改修版

[OpenTween](https://github.com/opentween/OpenTween)を日常利用するために改修した、非公式の個人フォークです。本家の公式版・公認後継ではありません。
Windows向けのタブ型Twitter/Xクライアントを基に、検索復旧と表示・操作の改善を追加しています。

本家の保守窓口に、このフォーク固有の不具合を報告しないでください。継続的な保守・サポート・Xへの接続動作は保証していません。

## 追加・変更した機能

| 機能 | 内容 |
| --- | --- |
| PublicSearch | Cookie使用時、非表示WebView2でXの検索応答を取得。設定件数まで追加取得 |
| 検知順 | PublicSearchタブで、後から見つかった古い投稿も上に表示可能 |
| 反応数 | 一覧のいいね数、投稿詳細のいいね・リポスト数を表示 |
| タブ別表示設定 | 反応数を「全体設定に従う／表示する／表示しない」で切り替え |
| 一括フォント | 投稿表示・入力のフォントをまとめて設定。解除すると個別設定に戻る |
| 全件既読 | タブ右クリックに「このタブを全て既読」を追加 |

操作・仕様・制限は [FORK_CHANGES.md](FORK_CHANGES.md) に記載しています。本家の履歴は [CHANGELOG.txt](CHANGELOG.txt) を参照してください。

## 動作環境

- Windows 10以降、.NET Framework 4.8
- 検索機能には Microsoft Edge WebView2 Runtime
- Twitter/XのCookie認証を使う機能には、ご自身のアカウントの有効なログインCookie

本家に組み込まれたTwitter APIキーは凍結されています。[本家READMEの保存版](docs/UPSTREAM_README.md)の注意事項も参照してください。
このフォークはWeb版Xの画面と通信形式に依存するため、Xの変更・ログイン状態・レート制限などで検索が失敗することがあります。
初回公開はソースコードのみで、このフォークのexe配布はありません。本家の配布exeにはここに記載した改修は含まれません。

## ビルドとテスト

Visual Studio 2022 17.4以降の「.NET デスクトップ開発」、.NET Framework 4.8開発ツール、Gitを用意してください。
開発時には Visual Studio 2026 / MSBuild 18 と .NET SDK 8 を使用しています。Visual Studio 2022でのこの改修版の再検証は未実施です。

1. このフォークの「Code」に表示されるURLからクローンします。
2. `OpenTween.sln` をVisual Studioで開き、NuGetパッケージを復元します。
3. `OpenTween` をスタートアッププロジェクトにし、Release / Any CPUでビルドします。
4. `OpenTween/bin/Release/net48/OpenTween.exe` を起動します。

Developer PowerShell for Visual Studioからも実行できます。

```powershell
msbuild OpenTween/OpenTween.csproj /restore /t:Build /p:Configuration=Release /p:Platform=AnyCPU
msbuild OpenTween.Tests/OpenTween.Tests.csproj /restore /t:Build /p:Configuration=Debug /p:Platform=AnyCPU
dotnet test OpenTween.Tests/OpenTween.Tests.csproj -c Debug --no-build
```

WebView2ローダーとの整合のため、現状のAnyCPU構成は32ビット優先です。実行時はexeだけでなく、ビルド出力のDLLと`runtimes`なども必要です。
WebP関連テストはWindows側のコーデック環境によって失敗することがあります。テスト成功は実際のX通信の成功を保証しません。

## 設定と認証情報

設定は通常、実行ファイルと同じフォルダーの`Setting*.xml`に保存されます。既存環境を更新する際は設定をバックアップし、実行フォルダーを削除せず、設定以外の成果物だけを上書きしてください。

Cookieはログインに使える秘密情報です。Cookie、設定XML、HAR、実通信JSON、WebView2プロファイルをGitHubに添付しないでください。
検索用WebView2は`%LOCALAPPDATA%/OpenTweenSearchTimelineWebView2`にプロファイルを保持します。
不具合報告でログを共有する場合も、認証情報・個人情報が含まれていないか確認してください。

## 開発・貢献について

改修にはOpenAI Codexを使用しています。利用者が仕様を決め、手元での動作を確認しながら進めています。すべての環境・操作を検証したものではありません。
機能追加や修正は目的を絞り、変更理由・確認した動作・残る制限を記載してください。開発上の注意事項は [AGENTS.md](AGENTS.md) にあります。

GitHub Actionsは手動起動のみです。手動でパッケージworkflowを起動するとexeを含むArtifactが作成されるため、ソース公開のみの間は起動しないでください。GitHub Releasesへの自動配布は設定していません。

## 本家とライセンス

本家: [opentween/OpenTween](https://github.com/opentween/OpenTween)。基点は `8a07d2688c959b26b1eccecc5fb6db001338df6f` です。
OpenTweenは2011年時点のGPL版Tweenから派生した別プロジェクトで、現在のTweenのソースコードではありません。

Copyright 2011 OpenTween contributors. 既存ソースの著作権表記は保持しています。
ソースコードはGPLv3の下で利用できます。詳細は [LICENSE.ja](LICENSE.ja)、[LICENSE](LICENSE)、同梱ライセンスを参照してください。
ロゴなどの画像リソースは本家のCC BY-SA 2.1 JPの案内に従います。[OpenTween-icons](https://github.com/opentween/OpenTween-icons)も参照してください。
