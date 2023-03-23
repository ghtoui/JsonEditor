# モジュール仕様書 - StatusClass

- [モジュール仕様書 - StatusClass](#モジュール仕様書---statusclass)
  - [クラス説明](#クラス説明)
  - [クラス変数](#クラス変数)
    - [VALUE\_PETTERN詳細](#value_pettern詳細)
  - [定数](#定数)

## クラス説明

他クラス多用される定数の宣言

## クラス変数

ただの定数宣言は、ソースコード
重要な奴だけ記載
| クラス変数名 | 機能 |
| :--: | :-- |
| COLON_WORD | keyの次に入力するもの. |
| JSON_EXT | jsonファイルの拡張子 **.json** の宣言 |
| INDENT_WORD | stringではなくchar型の **\t** |
| OPEN_PETTERN | {, [ のどちらかに当てはまるかを正規表現でチェックするため |
| CLOSE_PETTERN | }, ] のどちらかに当てはまるかを正規表現でチェックするため |
| VALUE_PATTERN | ", e, l, 数字のどれかに当てはまるか正規表現でチェック.詳細は[value_pattern](#value_pettern詳細) </br> valueをかどうかの判定に使用 |

### VALUE_PETTERN詳細

文字列の末尾だけを参照する
| 末尾の文字 | 元の文字列 |
| :--: | :--: |
| " | "文字列<font color="red">" </font> |
| e | tru<font color="red">e </font>, fals<font color="red">e </font> |
| l | nul<font color="red">l </font> |

## 定数

クラス変数が定数[クラス変数](#クラス変数)

