# 構成仕様書

- [構成仕様書](#構成仕様書)
  - [クラス一覧](#クラス一覧)

## クラス一覧

| クラス名 | 機能 |
| :--: | :-- |
| StatusClass | 他クラス多用される定数の宣言 |
| ItemClass | リストを作成する際に必要なクラス </br> リストの要素を格納 |
| JsonTextClass | プレビューに表示するためのtextを格納するクラス |
| TextControllerClass | テキスト変換処理, 次に入力するラベルやコンマの処理を行うクラス </br> StatusClassを継承 |
| JsonControllerClass | テキストからJson構文に沿うように分割処理, リストとテキスト変換処理を行うクラス </br> StatusClassを継承 |
| GUIControllerClass | XAMLからのイベント処理, XAMLのボタンの配色変更処理を行うクラス |

