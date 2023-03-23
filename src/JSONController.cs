using System.Text.Json.Nodes;
using System;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace json_editor
{
	public class JSONControler : StatusClass
	{	    
		protected JsonNode outputJSONObject 
		{
			get; set;
		}
		protected JSONControler()
		{
			this.outputJSONObject = JsonNode.Parse(LABEL_DICT_OPEN_CLOSE);
		}
		public void PerseFromFile(string json_text)
		{
			try 
			{
				//allText = inputTextStream.ReadToEnd();
				this.outputJSONObject = JsonNode.Parse(json_text);
				MessageBox.Show(JSON_CHANGE_SUCCESS);
			}
			catch(Exception e)
			{
				MessageBox.Show(JSON_CHANGE_ERROR);
			}
		}
	}

	class JsonControllerClass : JSONControler
	{
        private ObservableCollection<ItemClass> Items;
        private JsonTextClass json_text;
        private TextControllerClass text_controller;

        public JsonControllerClass(ObservableCollection<ItemClass> Items,
                JsonTextClass json_text) 
        {
            this.Items = Items;
            this.json_text = json_text;
            this.text_controller = new TextControllerClass(json_text);
        }

		// 引数のtextをjson形式にして返す
		public JsonNode text_jsonparse(string text)
		{	
			this.PerseFromFile(text);
			var JSON = outputJSONObject;
			return JSON;
		}

		// 再起処理でjsonnodeをたどりながら、リストに追加する
        // 引数
        // key: key
        //value: value
        // indent: indentの個数
        public void json_node_check(JsonNode value, JsonNode key = null, int indent = 0) 
        {
            string item_text = "";
            indent += 1;
            string bracket_indent;
            string checked_value;
            // nodeがjsonオブジェクトかリスト, valueか判定
            if (value is JsonObject) {
                bracket_indent = new string(INDENT_WORD, indent);  
                // keyの追加処理  
                if (key != null) {
                    json_list_add(bracket_indent + QUOTATION_WORD + key + QUOTATION_WORD + ": ");
                }
                json_list_add(bracket_indent + LABEL_DICT_OPEN);
                foreach (var node in value.AsObject()) {
                    json_node_check(node.Value, node.Key, indent);
                }
                json_list_add(bracket_indent + LABEL_DICT_CLOSE);
                
            } else if (value is JsonArray) {
                bracket_indent = new string(INDENT_WORD, indent);  
                // keyの追加処理  
                if (key != null) {
                    json_list_add(bracket_indent + QUOTATION_WORD + key + COLON_WORD);
                }
                json_list_add(bracket_indent + LABEL_LIST_OPEN);
                foreach (var node in value.AsArray()) {
                    json_node_check(node, indent: indent);
                }
                json_list_add(bracket_indent + LABEL_LIST_CLOSE);

            } else if (value is JsonValue) {
                // keyがないならリスト内のvalue, あるならjsonobject
                if (key != null) {
                    // {}があるところだけインデントを下げる
                    bracket_indent = new string(INDENT_WORD, indent);
                    checked_value = text_controller.check_value_text(value.ToJsonString());  
                    // valueを型変換可能か確認して、末尾が”なら文字列なのでkey+valueの組で入力               
                    if (checked_value.Substring(checked_value.Length - 1) == QUOTATION_WORD) {
                        item_text += new string (INDENT_WORD, indent) + QUOTATION_WORD 
                        + key.ToString() + COLON_WORD + checked_value;
                    } else {
                        item_text += new string (INDENT_WORD, indent) + QUOTATION_WORD 
                        + key.ToString() + COLON_WORD + value ;
                    }
                    
                } else {
                    item_text += new string (INDENT_WORD, indent);
                    checked_value = text_controller.check_value_text(value.ToJsonString());
                    // リスト内のvalueなのでそのまま入力                  
                    if (checked_value.Substring(checked_value.Length - 1) == QUOTATION_WORD) {
                        item_text +=  checked_value;
                    } else {
                        item_text += checked_value;
                    }
                    
                }
                json_list_add(item_text);
            } 
        }
        
        // jsontextをjson構文に直して、リスト変換
        // 返り値: 変換が成功したらTrue, 失敗ならFalse
        public bool json_text_to_list() 
        {
             // リスト変換したいjson_textを渡す
			var json = text_jsonparse(json_text.Json);

            int i = 0;
            // json_textの始めと最後の文字を格納するための変数
            string first_word = "";
            string end_word = "";

            // json変換でエラー発生したら、何もしない
            if (json.ToString() == LABEL_DICT_OPEN_CLOSE) {
                return false;
            }

            try {
                first_word = json_text.Json.Substring(0,1);
                end_word = json_text.Json.Substring(json_text.Json.Length - 1);
                json_list_add(first_word);
                foreach (var json_item in json.AsObject()) {
                    json_node_check(json_item.Value, json_item.Key); 
                    i += 1;
                }
                if (i != 0) {
                    json_list_add(end_word);
                }
            } catch {
                return false;
            }
            return true;
        }

        // json構文にあうようにリストからテキスト変換
        public void list_to_text()
        {            
            // 最初にjson_textを初期化
            json_text.delete();
            bool open_label;
            bool close_label;
            string text;
            string before_text;

            for (int i = 0; i < Items.Count; i += 1) {
                text = Items[i].item;
                // 正規表現を使って{, }, [, ]を特定する
                open_label = Regex.IsMatch(text, OPEN_PATTERN);
                close_label = Regex.IsMatch(text, CLOSE_PATTERN);
                
                // 一番最初は "{, [" なのでそのままテキスト変換
                if (i == 0) {
                    json_text.Json = text;
                    continue;
                }

                // 前回のテキストを格納
                before_text = Items[i - 1].item;

                // 要素がopenかcloseによって処理を変える
                if (open_label) {
                    // 要素がopen_labelかつ、前の要素がclose_labelかvalueならコンマをつける
                    if (Regex.IsMatch(before_text, CLOSE_PATTERN) || 
                            Regex.IsMatch(before_text.Substring(before_text.Length - 1), VALUE_PATTERN)){
                        json_text.Json = COMMA_WORD + text;   
                    } else {
                        json_text.Json = NEWLINE_WORD + text;
                    }
                } else if(close_label) {
                    json_text.Json = NEWLINE_WORD + text;
                } else {
                    // 前の要素がopen_labelなら改行して入力
                    if (Regex.IsMatch(before_text, OPEN_PATTERN)) {
                        json_text.Json = NEWLINE_WORD + text;
                    } else {
                        json_text.Json = COMMA_WORD + text;
                    }
                }
            }
        }

        // 返り値: 作成したリスト(getter)
        public ObservableCollection<ItemClass> get_json_list()
        {
            return Items;
        }
        
        // 与えられたtextをリストに追加する
        // 引数: 文字列
        public void json_list_add(string text)
        {
            Items.Add(new ItemClass {item = text});
        }

        // 移動先をチェックする
        // 移動先が{},[]内に入ってしまう場合は、飛ばした場所に移動させる
        // 引数
        // index_list: TextControllerClassのget_indent_elementで作成したindex_list
        // item_index: 移動対象のindex
        // select_item: 選択されているアイテム
        // is_up: 上下どちらに移動するか
        // 返り値: 移動するindex
        public int dest_check(List<int> index_list, int item_index, 
                              string select_item, bool is_up)
        {
            // upとdown方向の定数
            const int UP_DIR = 0;
            const int UP_INDEX_DIR = - 1;
            const int DOWN_DIR = - 1;
            const int DOWN_INDEX_DIR = 1;
            int move_dir = 0;
            int index_dir = 0;
            int select_item_indent = text_controller.count_char(select_item, INDENT_WORD);
            int dest_item_indent;
            int next_dest_item_indent;
            int move_index;
            string skip_word = "";
            string dest_item = "";
            string dest_item_end = "";
            string next_dest_item = "";
            string next_dest_item_end = "";
            bool is_skip_word = false;
            bool is_skip = false;
            bool is_value = false;

            // is_upなら上, !is_upなら下. 引数に応じて変数を設定する
            if (is_up) {
                move_dir = UP_DIR;
                index_dir = UP_INDEX_DIR;
                skip_word = UP_SKIP_WORD_PETTERN;
            } else {
                move_dir = DOWN_DIR;
                index_dir = DOWN_INDEX_DIR;
                skip_word = DOWN_SKIP_WORD_PETTERN;
            }

            List<int> move_index_list = new List<int>();
            /*
                index_listの要素: 0+2*nがlabel_open, 1+2*nがcloseになっている
            */
            for (int i = 0; i < index_list.Count - 1; i += 2){
                for (int j = index_list[i] - 1; j < index_list[i + 1]; j += 1){
                    move_index_list.Add(j + 1);
                }     
            }
            

            // 深さ同じ場所で一つ上 or 下のindexを取得する
            move_index = move_index_list[move_index_list.IndexOf(item_index) + index_dir];
            
            while (true) {
                // 移動先の文字列とインデントの数,末尾を取得
                dest_item = Items[move_index + move_dir].item;
                dest_item_indent = text_controller.count_char(dest_item, INDENT_WORD);
                dest_item_end = dest_item.Substring(dest_item.Length - 1);
                
                // 引数に応じて設定した正規表現を使い、当てはまるならそこには移動しない
                is_skip_word = Regex.IsMatch(dest_item_end, skip_word);

                // move_index が末尾か一番最初なら移動して終了
                if (move_index_list[move_index_list.IndexOf(move_index) + index_dir] == move_index_list[move_index_list.Count - 1] || 
                        move_index_list[move_index_list.IndexOf(move_index) + index_dir] == move_index_list[0]) {
                    break;
                }
                
                // is_upなら上, !is_upなら下.
                if (is_up){
                    next_dest_item = Items[move_index + index_dir].item;
                    next_dest_item_indent = text_controller.count_char(next_dest_item, INDENT_WORD);
                    next_dest_item_end = next_dest_item.Substring(next_dest_item.Length - 1);
                } else {
                    next_dest_item = Items[move_index].item;
                    next_dest_item_indent = text_controller.count_char(next_dest_item, INDENT_WORD);
                    next_dest_item_end = next_dest_item.Substring(next_dest_item.Length - 1);
                }
                
                is_value = !(Regex.IsMatch(select_item, COLON_WORD));

                // 移動先のインデントを選択アイテムのインデントと比較してスキップするかどうか
                
                // 選択アイテムのインデントと移動先のインデントの差が-1, スキップワードではないなら、そこに移動する
                if (select_item_indent - 1 == dest_item_indent && 
                        !is_skip_word){
                    is_skip = false;
                // インデントが同じ, スキップワードではない, 移動する次のアイテムが自分のインデントより大きくない場合移動する
                } else if (select_item_indent == dest_item_indent && 
                        !is_skip_word) {
                    is_skip = false;
                    if (select_item_indent < next_dest_item_indent) {
                        is_skip = true;
                    }
                } else {
                    is_skip = true;
                }

                // リスト内のvalue移動させるときの処理
                if (is_value) {
                    // }{ が連続して出てくるときにその間に移動させる
                    if (Regex.IsMatch(dest_item_end, UP_VALUE_SKIP_WORD_PETTERN) &&
                            Regex.IsMatch(next_dest_item_end, DOWN_VALUE_SKIP_WORD_PETTERN) && 
                            select_item_indent == dest_item_indent && is_up){
                        is_skip = false;
                    // }{ が連続して出てくるときにその間に移動させる
                    } else if (Regex.IsMatch(dest_item_end, DOWN_VALUE_SKIP_WORD_PETTERN) &&
                            Regex.IsMatch(next_dest_item_end, UP_VALUE_SKIP_WORD_PETTERN) && 
                            select_item_indent == dest_item_indent && 
                            !is_up){
                        is_skip = false;
                    }else {
                        is_skip = true;
                    }
                }

                // リスト外の要素を参照しないように0番目を除外
                if (is_skip && 
                        move_index_list.IndexOf(move_index).ToString() != "0") {
                    move_index = move_index_list[move_index_list.IndexOf(move_index) + index_dir];   
                    continue;
                }
                break;
            }
            return move_index;
        }

	}
}



