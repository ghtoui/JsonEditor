# モジュール仕様書 - GUIControllerClass

- [モジュール仕様書 - GUIControllerClass](#モジュール仕様書---guicontrollerclass)
  - [クラス説明](#クラス説明)
  - [クラス変数](#クラス変数)
  - [定数](#定数)
  - [メソッド](#メソッド)
    - [dupli\_key\_check詳細](#dupli_key_check詳細)

## クラス説明

XAMLからのイベント処理, XAMLのボタンの配色変更処理を行うクラス

## クラス変数

| クラス変数名 | 機能 |
| :--: | :-- |
| value_check | valueを入力していいかどうか </br> valueになにも入力されていないならFalse, されているならTrue |
| key_check | keyを入力していいかどうか </br> keyになにも入力されていないならFalse, されているならTrue |
| is_json_list | 今表示されているのが、プレビューならFalse, リストならTrue |
| file_name | 保存するファイル名を格納する </br> 保存や新規保存, ファイル読み込みの時に更新, 使用 </br> file_pathと結合して使用 |
| file_path | 保存するファイルパスを格納する </br> 保存や新規保存, ファイル読み込みの時に更新, 使用 </br> file_nameと結合して使用 |

## 定数

| 定数名 | 値 | 説明 |
| :--: | :--: | :-- |
| is_up | true | 上に移動ボタンが押された時 |
| is_up | false | 下に移動ボタンが押された時 |

## メソッド

| メソッド名 | 引数 | 返り値 | 機能 |
| :--: | :--: | :--: | :-- |
| text_check |  |  | key, valueのテキストボックスに入力されている敵テキストが空白ではないか確認。</br> keyが連続で入力できないように制御 |
| change_color |  |  | ボタンの色変更 |
| edit_text_reload |  |  | プレビュー画面の更新 |
| change_element | is_up |  | 選択アイテムを入れ替える。入れ替えボタンのイベントハンドラから呼び出し |
| dupli_key_check | check_text, mode, check_text_index | 重複なしならfalse</br>重複ありならtrue | keyを重複して入力しないようにチェックする </br> 引数modeを使って編集と追加の処理を分ける 詳細は[dupli_key_check](#dupli_key_check詳細) |
| edit_window | type, before_text | input_text | 入力可能なポップアップウィンドウを表示する。 </br> 入力した文字が返り値となる。キャンセルした場合は空白文字が返り値となる |

### dupli_key_check詳細

![追加の場合](/Spec/spec_pic/add_depli_picture.png)

編集の場合の処理
![編集の場合](/Spec/spec_pic/edit_depli_picture.png)

