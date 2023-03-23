# モジュール仕様書 - JsonControllerClass

- [モジュール仕様書 - JsonControllerClass](#モジュール仕様書---jsoncontrollerclass)
  - [クラス説明](#クラス説明)
  - [クラス変数](#クラス変数)
  - [定数](#定数)
    - [UP\_DIR詳細](#up_dir詳細)
    - [DOWN\_DIR詳細](#down_dir詳細)
  - [メソッド](#メソッド)

## クラス説明

テキストからJson構文に沿うように分割処理, リストとテキスト変換処理を行うクラス </br> StatusClassを継承

## クラス変数

| クラス変数名 | 機能 |
| :--: | :-- |
| outputJSONObject | jsonObjectを格納 |

## 定数

| 定数名 | 値 | 説明 |
| :--: | :--: | :-- |
| UP_DIR | 0 | 入れ替え先のリストの要素を取得する際に使う </br> 詳細は[UP_DIR](#up_dir詳細) |
| UP_INDEX_DIR | -1 | 上に入れ替える際に使用 </br> 上に入れ替える時はリストのindexを`-1`する |
| DOWN_DIR | -1 | 入れ替え先のリストの要素を取得する際に使う </br> 詳細は[DOWN_DIR](#down_dir詳細) |
| DOWN_INDEX_DIR | 1 | 下に入れ替える際に使用 </br> 下に入れ替える時はリストのindexを`+1`する |

### UP_DIR詳細

要素2を要素4の上に移動する際は、4の上のindex(index: 2)を取得してから、要素2を消して、取得したindex:2に<font color="Red"> +0 </font> したindex:2 に要素2を挿入するという処理をしているため、UP_DIRは<font color="Red"> +0 </font>

    移動前              削除                    移動後
    index, 要素         index,    要素          index,  要素
    0       1           0           1           0       1
    1       2           1           3           1       3
    2       3           2           4           2       2
    3       4           3           5           3       4
    4       5                                   4       5

### DOWN_DIR詳細

要素2を要素4の下に移動する際は、4の下のindex(index: 4)を取得してから、要素2を消して、取得したindex:4に <font color="Red"> -1 </font> したindex:3に要素2を挿入するという処理をしているため、DOWN_DIRは<font color="Red"> -1 </font> 

    移動前              削除                    移動後
    index, 要素         index,    要素          index,  要素
    0       1           0           1           0       1
    1       2           1           3           1       3
    2       3           2           4           2       4
    3       4           3           5           3       2
    4       5                                   4       5

## メソッド

| メソッド名 | 引数 | 返り値 | 機能 |
| :--: | :--: | :--: | :-- |
| json_list_add | text |  | 与えられた文字列をリストに追加する |
| json_node_check | value, key, indent |  | 再起処理でjsonnodeをたどりながら、keyとvalueリストに追加する |
| json_text_to_list |  | true or false | json_textをjson構文に直して、リスト変換する |
| list_to_text |  |  | jsonリストからjson構文に合うようにjson_textへ変換 |
| get_json_list |  | Items | json_text_to_listで作成したリストを返す |
| dest_check | index_list, item_index, select_item, is_up | move_index | リスト入れ替え時の移動先決定を行う.|

