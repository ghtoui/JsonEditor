# モジュール仕様書 - JsonTextClass

- [モジュール仕様書 - JsonTextClass](#モジュール仕様書---jsontextclass)
  - [クラス説明](#クラス説明)
  - [クラス変数](#クラス変数)
    - [json\_text setter getter詳細](#json_text-setter-getter詳細)
  - [メソッド](#メソッド)

## クラス説明

プレビューに表示するためのtextを格納するクラス

## クラス変数

| クラス変数名 | 機能 |
| :--: | :-- |
| json_text | setterを使用して、json_textに入力したテキストを**追加** </br> 詳細は[json_text](#json_text-setter-getter詳細) |

### json_text setter getter詳細

json_textを編集する場合: json_text.Json = "追加する文字列"

右辺を代入するのではなく、**スタックさせる**というとこが少し変わっている点

内容を取得する場合: json_text.Json

## メソッド

| メソッド名 | 引数 | 返り値 | 機能 |
| :--: | :--: | :--: | :-- |
| delete | | | json_textを削除する |

