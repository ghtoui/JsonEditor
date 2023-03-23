# モジュール仕様書 - TextControllerClass

- [モジュール仕様書 - TextControllerClass](#モジュール仕様書---textcontrollerclass)
  - [クラス説明](#クラス説明)
  - [クラス変数](#クラス変数)
  - [定数](#定数)
  - [メソッド](#メソッド)
    - [check\_value詳細](#check_value詳細)
    - [get\_indent\_element詳細](#get_indent_element詳細)

## クラス説明

テキスト変換処理, 次に入力するラベルやコンマの処理を行うクラス </br> StatusClassを継承

## クラス変数

| クラス変数名 | 機能 |
| :--: | :-- |
| is_label_dict | {}の最内側にいるならTrue, いないならFalse |
| is_label_list | []の最内側にいるならTrue, いないならFalse |
| indent_num | インデントの個数を格納する </br> 入力したテキストにインデントを追加する際に必要 |
| before_type | 前回入力されたボタンの種類を格納する </br> 詳細はソースコードのコメントに記載|
| before_text | 前回入力されたテキストの種類を格納する </br> 詳細はソースコードのコメントに記載 |
| label_list | {, [ が入力順に格納されるリスト。このリストにより次に閉じるのが}, ] のどっちなのかを管理する |

## 定数

| 定数名 | 値 | 説明 |
| :--: | :--: | :-- |
| is_up | true | 上に移動ボタンが押された時 |
| is_up | false | 下に移動ボタンが押された時 |

## メソッド

| メソッド名 | 引数 | 返り値 | 機能 |
| :--: | :--: | :--: | :-- |
| check_value_text | text | 型変換した文字列 | 文字列が型変換できるかチェック.詳細は[check_value](#check_value詳細) |
| edit_json | text, type |  | 押されたボタン(type)に合わせて整形されたテキストをjson_textに追加 |
| manage_label | text |  | {, [ が入力されたら、リストに追加 |
| add_text | type, text | add_text | 入力されたテキスト情報をjson構造に整形 |
| calc_indent |  | indent | プレビューに追加するテキストに必要なインデントの数を返す |
| calc_comma | type, text | comma | プレビューに追加するテキストに必要なコンマと改行を返す |
| countchar | text, search_word | counter | 検索文字の出現回数を返す |
| get_indent_element | text, target_Items, search_char | same_indentlist | 検索文字が検索文字列と同じ数ある要素のindexをリストにして返す. 詳細は[get_indent_element](#get_indent_element詳細) |
| get_label |  | last_label | labelリストの最後の要素を返す |
| move_index | | | リスト入れ替え時の移動先決定を行う.詳細は|

### check_value詳細

| 型変換前 | 型変換後 |
|:--:|:--:|
| "aiueo" | "aiueo" |
| ""aiueo"" | "aiueo" |
| "true" | true |
| ""true"" | "true" |
| "false" | false |
| ""false"" | "false" |
| "1" | 1 |
| ""1"" | "1" |
| "null" | null |

### get_indent_element詳細

    検索対象リスト
    index   要素
    0       \t hello
    1       \t\t konnitiha
    2       \t good
    3       \t\t iine
    4       \t sorry
    5       \t\t gomen

    検索文字列: \t hello
    検索文字: \t

    返り値: [0,2,4]

