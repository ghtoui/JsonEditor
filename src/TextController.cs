using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using MessageBox = System.Windows.MessageBox;

namespace json_editor {
    class TextControllerClass : StatusClass{
        
        // {}の内部にいるかどうか
        public bool is_label_dict { get; set; }
        // []の内部にいるかどうか
        public bool is_label_list { get; set; }
        // インデント計算に使う
        public int indent_num { get; set; }
        // 前回入力されたボタンの種類
        // key, value, dict_open, dict_close, list_open, list_closeが格納される
        public string before_type { get; set; }
        // 前回入力されたテキスト
        // keyとvalueは打ち込まれたもの、{, }, [, ]が格納される
        public string before_text { get; set; }
        // manage_label, get_label で使われる {, [ の入力順を管理するリスト
        public List<string> label_list;
        protected JsonTextClass json_text;
        public TextControllerClass(JsonTextClass json_text)
        {
            this.indent_num = 0;
            this.is_label_dict = false;
            this.is_label_list = false;
            this.before_type = "";
            this.before_text = "";
            this.label_list = new List<string>(){""};
            this.json_text = json_text;
        }

        // 与えられた文字列が型変換できるか判断する
        // 引数
        // text: 文字列
        public string check_value_text(string text)
        {
            int i;
            bool isbool;
            string checked_text = "";
            
            // int判定
            // 文字列をintに変換できるかどうか
            if (int.TryParse(text, out i)) {
                checked_text = i.ToString();
            } else if (text.Length > 2 && int.TryParse(text.Substring(1, text.Length - 2), out i)) {
                checked_text = text;
            }
            
            // true, false 判定
            // 文字列をbooleanに変換できるかどうか
            if (bool.TryParse(text, out isbool)) {
                if (isbool) {
                    checked_text = TRUE_WORD;
                } else if (!isbool) {
                    checked_text = FALSE_WORD;
                }
            }
            // null判定
            if (text == NULL_WORD) {
                checked_text = NULL_WORD;
            }
            
            if (checked_text == "") {
                checked_text = QUOTATION_WORD + text + QUOTATION_WORD;
            }

            // 最初に"が2つ重なっている場合、1つに変換する
            if (checked_text.Length > 2 && 
                    checked_text.Substring(0,2) == QUOTATION_WORD + QUOTATION_WORD) {
                checked_text = checked_text.Substring(1, checked_text.Length - 2);
            }
            
            return checked_text;
        }
    
        // jsonにテキストを入力する準備
        // 引数
        // text: json_textに追加する文字列
        // type: 押したボタンの種類 
        public void edit_json(string text, string type)
        {
            manage_label(text);
            // 押されたボタンによって処理を変える
            if (type == KEY) {
                json_text.Json = add_text(type, QUOTATION_WORD + text + COLON_WORD);
            } else if (type == VALUE) {
                json_text.Json = add_text(type, text);
            } else if (type == LABEL_DICT) {
                if (text == LABEL_DICT_OPEN) {
                    json_text.Json = add_text(type, text);
                    indent_num += 1;
                } else {
                    indent_num -= 1;
                    json_text.Json = add_text(type, text);
                }
            } else if (type == LABEL_LIST) {
                if (text == LABEL_LIST_OPEN) {
                    json_text.Json = add_text(type, text);
                    indent_num += 1;
                } else {
                    indent_num -= 1;
                    json_text.Json = add_text(type, text);
                }
            }
        }

        // {}, []の管理
        // 補足：{, [の場合のみ処理を行う
        // 引数
        // text: json_textに追加する文字列
        public void manage_label(string text)
        {
            int end_index = label_list.Count - 1;
            // {, [ の対を検知したらlistの中身を消す
            if ((label_list[end_index] == LABEL_DICT_OPEN && text == LABEL_DICT_CLOSE) ||
                    label_list[end_index] == LABEL_LIST_OPEN && 
                    text == LABEL_LIST_CLOSE) {
                label_list.RemoveAt(end_index);
            } else if (text == LABEL_DICT_OPEN || 
                    text == LABEL_LIST_OPEN) {
                label_list.Add(text);
            }
            
            // {}, []に入っているかどうか
            if (get_label() == LABEL_DICT_OPEN) {
                is_label_dict = true;
            } else {
                is_label_dict = false;
            }
            if (get_label() == LABEL_LIST_OPEN) {
                is_label_list = true;
            } else {
                is_label_list = false;
            }
        }

        // 入力したテキスト情報をjson構造に整形する
        // valueがkeyの次ではなく[ の次に入力されるときだけ、valueにインデントをつける必要がある
        // []のなかにはvalueだけが複数入力する場合があるため、valueが連続で打ち込まれるときもインデントをつける
        // 引数
        // type: 押したボタンの種類
        // text: 追加文字列
        // 返り値: 編集した追加文字列
        public string add_text(string type, string text)
        {
            string add_text;
            // keyの次のvalueにはインデントをつけたくない
            if (type != VALUE || 
                    (type == VALUE && is_label_list)) {
                add_text = calc_comma(type, text) + calc_indent() + text;
            } else {
                add_text = calc_comma(type, text) + text;
            }
            
            return add_text;
        }

        // 返り値: インデントの数
        public string calc_indent()
        {
            string indent = "";
            for(int i = 0; i < indent_num; i += 1) {
                indent += INDENT_WORD;
            }
            return indent;
        }

        // コンマと改行をいれるかどうか
        // 引数
        // type: 押したボタンの種類
        // text: 追加文字列
        // 返り値: 挿入するコンマ
        public string calc_comma(string type, string text)
        {
            string comma = "";

            /*
                前回入力された文字列と, 前回押されたボタン, 
                入力された文字列, 押されたボタンによって処理を変える
            */
            if ((before_type == VALUE && type == KEY) || 
                    (is_label_dict && type == KEY && before_text != LABEL_DICT_OPEN) ||      
                    (before_text == LABEL_DICT_CLOSE && text == LABEL_DICT_OPEN) ||
                    (is_label_list && type == VALUE && before_type == VALUE) ||
                    (!is_label_list && text == LABEL_DICT_OPEN && before_type == VALUE) ||
                    (is_label_list && type == VALUE && before_text == LABEL_DICT_CLOSE)) {
                comma = COMMA_WORD;
            // 改行がいらないのは、keyの次のvalueと一番初めに打ち込んだテキストだけ
            } else if (before_type == SPACE_WORD ||
                    (type == VALUE && before_type == KEY)) {
                comma = "";
            } else {
                comma = NEWLINE_WORD;
            }

            before_type = type;
            before_text = text;
            
            return comma;
        }

        // 文字の出現回数をカウント
        // 引数
        // text: 検索対象文字列
        // search_word: 検索文字
        // 返り値
        // counter: 検索対象文字列に検索文字が入っている個数
        public int count_char(string text, char search_word)
        {
            int counter =  text.Length - text.Replace(search_word.ToString(), "").Length;
            return counter;
        }

        // 検索対象リストの中に検索文字列にある検索文字の数と
        // 検索文字が同じ数ある要素のindexをリストで返す
        // 引数 
        // text: 検索テキスト
        // target_items: 検索するリスト
        // search_char: 検索文字
        // 返り値
        // 検索文字が同じ数あるindexリストで返す
        public List<int> get_indent_element(string text, ObservableCollection<ItemClass> target_Items,
                char search_char)
        {
            bool is_label;
            int text_indent = count_char(text, search_char);
            int purp_indent = text_indent - 1;
            int element_indent;
            string element_end_word;
            
            List<int> same_indentlist = new List<int>();
            int i = 0;
            foreach (var element in target_Items) {

                element_indent = count_char(element.item, search_char);
                element_end_word = element.item.Substring(element.item.Length - 1);
                
                // ラベルかどうか
                is_label = Regex.IsMatch(element_end_word, LABEL_PATTERN);

                // 選択要素のインデントと移動対象のインデントが同じかどうか
                if (element_indent == purp_indent && 
                        is_label) {
                    same_indentlist.Add(i);
                }
                i += 1;
            }
            
            return same_indentlist;
        }
        

        // 返り値: label_listの末尾
        public string get_label()
        {
            string last_label = this.label_list[label_list.Count - 1];
            return last_label;
        }
    }
}