using System;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;

namespace json_editor
{
    public partial class GUIControllerClass : Window
    {
        // text_checkで使われるvalueの入力値の状態
        public bool value_check { get; set; }
        // text_checkで使われるkeyの入力値の状態
        public bool key_check { get; set; }
        // json_list_manageで使われる現在の状態がリストかどうか
        public bool is_json_list { get; set; }
        // filenameを格納
        public string file_name { get; set; }
        // fileのpath
        public string file_path { get; set; }
        JsonTextClass json_text;
        ObservableCollection<ItemClass> Items;
        TextControllerClass text_controller;
        StatusClass status;
        JsonControllerClass json_controller;

        // 拡張子を入れたファイル名の最小サイズ .json + 1文字
        const int FILE_MIN_LENGTH = 5;

        // dupli_key_checkに渡すモード, 追加処理なのでadd
        const string ADD_MODE = "add";
        // dupli_key_checkに渡すモード, 編集処理なのでedit
        const string EDIT_MODE = "edit";

        // 以下 メッセージ

        // UpRow_Clickで何も選択されていないか、先頭を移動させようとしたときのエラーメッセージ        
        const string UPROW_ERROR1 = "選択されていないか、先頭は移動できません";
        // UpRow_Clickで上限より上に移動させようとしたとき
        const string UPROW_ERROR2 = "これ以上、上には移動できません";

        // DawnRow_Clickで何も選択されていないか、末尾を移動させようとき
        const string DOWNROW_ERROR1 = "選択されていないか、末尾は移動できません";
        // DownRow_Clickで下限より下に移動させようとしたとき
        const string DOWNROW_ERROR2 = "これ以上、下には移動できません";
        // delete_itemで何も選択されていないか、先頭と末尾を削除しようとしたとき
        const string DELETE_ERROR = "選択されていないか、先頭と末尾は削除できません";
        
        // delete_close_labelで}で閉じられていなかったとき
        const string DELETE_CLOSE_LABEL_ERROR = "\"}\"で閉じられていません。\n削除するには\"}\"で閉じる必要があります";
        
        // 以下ファイル選択メッセージ
        // load_json_fileで使用するメッセージ
        const string LOAD_DESCRIPTION = "開きたいjsonを選択してください";
        // キャンセル時のメッセージ
        const string CANCEL_MESSAGE = "キャンセルしました";
        // 拡張子が.jsonではないときのエラー
        const string EXTENSION_ERROR = ".jsonで終了していません。\n.jsonを追加してください。";
        // filenameが入力されていないエラー
        const string FILE_NAME_ERROR = "ファイル名が入力されていないか\n.jsonをつけ忘れている可能性があります。";
        // file_pathが指定されていない場合
        const string FILE_PATH_ERROR = "file_pathが指定されていないため、保存場所を指定してください";
        // リスト変更ボタンのテキストを記述に変更にする
        const string CHANGE_EDIT = "記述に変更";
        // 記述に変更ボタンのテキストをリストに変更にする
        const string CHANGE_LIST = "リストに変更";
        // ポップアップウィンドウでなにも入力されていない or キャンセル
        const string EDIT_ITEM_ERROR = "なにも入力されていないか、キャンセルされました";

        
        // コンストラクタ
        public GUIControllerClass()
        {
            InitializeComponent();
            this.key_check = false;
            this.value_check = false;
            this.is_json_list = false;
            this.file_name = "";
            this.file_path = "";
            json_text = new JsonTextClass();
            status = new StatusClass();
            Items = new ObservableCollection<ItemClass>();
            text_controller = new TextControllerClass(json_text);
            json_controller = new JsonControllerClass(Items, json_text);
            json_list.Visibility = Visibility.Collapsed;
            change_color();
        }
        
        // 入力されているテキストの確認
        public void text_check()
        {
            // keyが空白か、前回入力されたボタンがkeyのときはkeyを入力できないように
            if (key_text.Text != status.SPACE_WORD) {
                key_check = true;
            } else {
                key_check = false;
            }
            
            // valueが空白で前回がkeyではないときは入力できないように
            if (value_text.Text != status.SPACE_WORD) {
                value_check = true;
            } else {
                value_check = false;
            }
        }

        // ボタンの色を更新する
        // ifにonになる処理、elseにoffになる処理を書く
        public void change_color()
        {
            // 色指定 on：白、off：赤
            Brush on = Brushes.Silver;
            Brush off = Brushes.Firebrick;
            
            // 前回押されたボタンとどのかっこの最内側にいるかどうかで、処理を変える
            // keyボタン
            if (text_controller.before_type != status.KEY && 
                    !text_controller.is_label_list && 
                    text_controller.is_label_dict && 
                    key_check) {
                key_button.Background = on;
            } else {
                key_button.Background = off;
            }
            
            // valueボタン
            if ((text_controller.before_type == status.KEY || 
                    text_controller.is_label_list) && 
                    value_check) {
                value_button.Background = on;
            } else {
                value_button.Background = off;
            }
            // {ボタン
            if (text_controller.is_label_list || 
                    text_controller.before_text == status.SPACE_WORD ||
                    text_controller.before_type == status.KEY) {
                label_dict_open.Background = on;
            } else {
                label_dict_open.Background = off;
            }
            // }ボタン
            if (text_controller.is_label_dict && 
                    text_controller.before_type != status.KEY) {
                label_dict_close.Background = on;
            } else {
                label_dict_close.Background = off;
            }
            // [ボタン
            if (text_controller.is_label_list ||
                    text_controller.before_type == status.KEY) {
                label_list_open.Background = on;
            } else {
                label_list_open.Background = off;
            }
            // ]ボタン
            if (text_controller.is_label_list) {
                label_list_close.Background = on;
            } else {
                label_list_close.Background = off;
            }
        }

        // プレビュー画面の更新
        public void edit_text_reload() 
        {
            edit_text.Text = json_text.Json;
            edit_text.ScrollToEnd();
        }

        // 選択アイテムを入れ替え
        // 入れ替えボタンのイベントハンドラから呼び出す
        // 引数
        // is_up: 上に移動か下に移動か
        public void change_element(bool is_up)
        {
            int item_index = json_list.SelectedIndex;
            ItemClass selectitem;
            int move_index = 0;
            bool is_label = false;
            string selectitem_end = "";
            string select_text = "";

            // 選択されていないか、末尾ならなにもしない
            // 何も選択していない場合か先頭ならなにもしない
            if (is_up && 
                    (json_list.SelectedItem == null || 
                    item_index == 0)) {
                MessageBox.Show(UPROW_ERROR1);
                return ;
            } else if (!is_up && 
                    (json_list.SelectedItem == null || 
                    item_index == json_list.Items.Count - 1)) {
                MessageBox.Show(DOWNROW_ERROR1);
                return ;
            }

            // 選択されているアイテムを取得
            selectitem = (ItemClass)json_list.SelectedItem;
            selectitem_end = selectitem.item.Substring(selectitem.item.Length - 1);
            
            // 選択しているものがかっこかどうか
            is_label = Regex.IsMatch(selectitem_end, status.LABEL_PATTERN);
            // {}[]を選択している場合は、なにもしない
            if (is_label) {
                return;
            }
            // 選択したアイテムと同じインデントを持つindexをリストで取得
            select_text = selectitem.item;
            List<int> index_list = text_controller.get_indent_element(select_text, Items, status.INDENT_WORD);
            
            // 選択されているアイテムと同じインデントを持つ要素の下に移動
            // 選択されているアイテムが一番下の要素にいるときはなにもしない
            if (index_list[0] + 1 == item_index && 
                    is_up) {
                MessageBox.Show(UPROW_ERROR2);
                return;
            } else if (index_list[index_list.Count - 1] - 1 == item_index && 
                    !is_up) {
                MessageBox.Show(DOWNROW_ERROR2);
                return;
            }
            // 選択されているアイテムを取り出してから、リストから削除
            Items.Remove(selectitem);
            
            // 移動させる要素番号を取得
            move_index = json_controller.dest_check(index_list, item_index, select_text, is_up);
            
            Items.Insert(move_index, selectitem);

            //入れ替えたものをjsontextに反映
            json_controller.list_to_text();
            // 下に移動させたものをそのまま選択状態にする
            json_list.SelectedIndex = move_index;
            
            edit_text_reload();
        }

        // 同じkeyを入力させない処理
        // json_textを改行で分割して、それぞれのインデントを計算
        // 同じ{}にあるkey(":より前)がcheck_textと同じならエラー表示 
        // 引数
        // check_text: 重複確認をする文字列. インデントが同じ数ある他のkeyに重複文字列があるかどうか
        // mode: edit, add 編集するときと追加で処理を変える
        public bool dupli_key_check(string check_text, string mode, int check_text_index = 0)
        {
            bool is_match = false;
            int text_indent = 0;
            List<int> dict_index_list = new List<int>();
            string item = "";
            string text = "";
            string text_key = "";
            int start_index = 0;
            int end_index = 0;
            int check_text_indent = 0;

            // 改行で区切って要素を取り出す
            string[] json_text_list = json_text.Json.Split("\n");
            for (int i = 0; i < json_text_list.Length; i += 1) {
                item = json_text_list[i];
                text_indent = text_controller.count_char(item, status.INDENT_WORD);
                // { を検索
                if (mode == "add") {
                    if (text_indent == text_controller.indent_num - 1 &&
                            Regex.IsMatch(item, status.LABEL_DICT_OPEN)) {
                        dict_index_list.Add(i);
                    }
                } else if (mode == "edit"){
                    check_text_indent = text_controller.count_char(Items[check_text_index].item, status.INDENT_WORD);
                    if (text_indent == check_text_indent - 1 &&
                            Regex.IsMatch(item, status.DICT_PATTERN)) {
                        dict_index_list.Add(i);
                    }
                } 
            }
            
            if (mode == "add") {
                // 挿入の場合は} は気にしなくてよい
                // 同じインデントかつ直近の{だけ参照
                start_index = dict_index_list[dict_index_list.Count - 1];
                end_index = json_text_list.Length;
            } else if (mode == "edit") {
                // 同じインデントで直近の{と}を探す
                int dict_open_index = 0;
                int dict_close_index = 1000;
                for (int i = 0; i < dict_index_list.Count; i += 1) {
                    if (dict_index_list[i] <= check_text_index && 
                            dict_open_index <= dict_index_list[i]) {
                        dict_open_index = dict_index_list[i];
                    }
                    if (dict_index_list[i] >= check_text_index &&
                            dict_close_index >= dict_index_list[i]) {
                        dict_close_index = dict_index_list[i];
                    }
                }
                start_index = dict_open_index;
                end_index = dict_close_index;
            }
            
            for (int i = start_index; i < end_index; i += 1) {
                text = json_text_list[i];
                text_indent = text_controller.count_char(text, status.INDENT_WORD);

                // インデントが同じで要素がかっこではないときに、要素のkeyを取り出す
                if (text_indent == text_controller.indent_num &&
                        !Regex.IsMatch(text, status.LABEL_PATTERN) && 
                        mode == "add") {
                    // keyは"key"と”で区切られているので[1]がkeyの文字列となる
                    text_key = text.Split(status.QUOTATION_WORD)[1];
                    // keyが入力したいkeyと同じならダメ
                    if (text_key == check_text) {
                        is_match = true;
                    }
                } else if (text_indent == check_text_indent &&
                        !Regex.IsMatch(text, status.LABEL_PATTERN) && 
                        mode == "edit") {
                    // keyは"key"と”で区切られているので[1]がkeyの文字列となる
                    text_key = text.Split(status.QUOTATION_WORD)[1];
                    // keyが入力したいkeyと同じならダメ
                    if (text_key == check_text) {
                        is_match = true;
                    }
                }
            }

            return is_match;
        }

        // 入力させるウィンドウの表示
        // 引数
        // type: ウィンドウのタイトル
        public string edit_window(string type, string before_text)
        {  
            string message_text = type + "を入力してください";
            string input_text;
            input_text = Interaction.InputBox(type + "を入力して下さい。\n 編集前のテキスト: " + before_text, " ");
            
            return input_text;
        }


        // パスを含めたfile名から、パス名だけを取得する
        // 引数
        // file_text: file名(パス含めた)を
        public void get_file_path(string file_text) 
        {
            file_path = "";
            string[] split_file_name = file_text.Split(status.SPLIT_PATH_WORD);
            for (int i = 0; i < split_file_name.Length - 1; i += 1) {
                file_path += split_file_name[i] + status.SPLIT_PATH_WORD;
            }
        }

        // パスを含めたfile名から、file名だけ取得する
        //引数
        // file_text: file名(パスを含めた)
        public void get_file_name(string file_text)
        {
            string[] split_file_name = file_text.Split(status.SPLIT_PATH_WORD);
            file_name = split_file_name[split_file_name.Length - 1];
        }

        /*
            以下イベントハンドラ
        */

        // textの入力判定
        // テキストが打ち込まれる毎に呼び出される
        public void changedtext(object sender, TextChangedEventArgs args)
        {
            text_check();
            change_color();
        }

        // クラス変数のファイル名を更新
        // filenameの入力
        public void changed_file_name(object sender, TextChangedEventArgs args)
        {
            file_name = file_name_text.Text;
        }

        
        // keyボタンが押された時の処理
        // if内に入力する際の処理
        public void key_click(object sender, RoutedEventArgs e)
        {
            text_check();
            // 入力条件
            if (key_check && 
                    text_controller.before_type != status.KEY && 
                    !text_controller.is_label_list && 
                    text_controller.is_label_dict) {
                // 入力したkeyがjson_text内に存在するかどうか. 存在したら、メッセージを出して、入力はしない
                if (dupli_key_check(key_text.Text, ADD_MODE)) {
                    MessageBox.Show(status.SAME_KEY_ERROR);
                    return;
                }
                text_controller.edit_json(key_text.Text, status.KEY);
                edit_text_reload();
            }
            change_color();
        }

        // valueボタンが押された時の処理
        // if内に入力する際の処理
        public void value_click(object sender, RoutedEventArgs e)
        {
            text_check();
            // 入力条件
            if ((value_check && 
                    text_controller.before_type == status.KEY) || 
                    text_controller.is_label_list){
                // 型を反映させたvalueを書き込む
                string text = text_controller.check_value_text(value_text.Text);
                text_controller.edit_json(text, status.VALUE);
                edit_text_reload();
            }
            change_color();
        }

        // {ボタンが押された時の処理
        // if内に入力する際の処理
        public void label_dict_open_click(object sender, RoutedEventArgs e)
        {
            text_check();
            // 入力条件
            if (text_controller.is_label_list || 
                    text_controller.before_text == status.SPACE_WORD || 
                    text_controller.before_type == status.KEY) {
                text_controller.edit_json( status.LABEL_DICT_OPEN, status.LABEL_DICT);
                edit_text_reload();
            }
            change_color();
        }

        // }ボタンが押された時の処理
        // if内に入力する際の処理
        public void label_dict_close_click(object sender, RoutedEventArgs e)
        {
            text_check();
            // 入力条件
            if (text_controller.get_label() == status.LABEL_DICT_OPEN 
                    && text_controller.before_type != status.KEY) {
                text_controller.edit_json(status.LABEL_DICT_CLOSE, status.LABEL_DICT);
                edit_text_reload();
            }
            change_color();
        }

        // [ボタンが押された時の処理
        // if内に入力する際の処理
        public void label_list_open_click(object sender, RoutedEventArgs e)
        {
            text_check();
            // 入力条件
            if(text_controller.before_type == status.KEY ||
                    text_controller.is_label_list) {
                text_controller.edit_json(status.LABEL_LIST_OPEN, status.LABEL_LIST);
                edit_text_reload();
            }
            change_color();
            
        }

        // ]ボタンが押された時の処理
        // if内に入力する際の処理
        public void label_list_close_click(object sender, RoutedEventArgs e)
        {
            text_check();
            // 入力条件
            if (text_controller.get_label() == status.LABEL_LIST_OPEN) {
                text_controller.edit_json(status.LABEL_LIST_CLOSE, status.LABEL_LIST);
                edit_text_reload();
            }
            change_color();
        }  
        

        // 選択した項目を上に移動する
        public void UpRow_Click(object sender, RoutedEventArgs e)
        {
            const bool is_up = true;
            change_element(is_up);
        }

        // 選択した項目を下に移動
        public void DownRow_Click(object sender, RoutedEventArgs e)
        {
            
            const bool is_up = false;
            change_element(is_up);
        }


        // 変換ボタンを押したときの処理
        // 編集画面をリストにするか、テキストにするか
        public void json_list_manage(object sender, RoutedEventArgs e)
        {
            // リストとjson_controllerを初期化する
            json_controller = new JsonControllerClass(Items, json_text);
            Items = new ObservableCollection<ItemClass>();
            var on = Visibility.Visible;
            var off = Visibility.Collapsed;
            // json_textに入力されている or 現在表示されている画面がリストではないとき
            if ((!is_json_list && 
                    json_text.Json != status.SPACE_WORD) ||
                    !is_json_list) {
                bool is_text_to_list = json_controller.json_text_to_list();
                // リスト変換が失敗したらなにもしない
                if (!is_text_to_list) { 
                    return;
                }
                // jsonリストを作成する
                Items = json_controller.get_json_list();
                // 作ったリストを受け取る
                json_list.ItemsSource = Items;

                // テキストボックスを見えなくして、リストを表示する
                edit_text.Visibility = off;
                delete_up_to_label.Visibility = off;
                json_list.Visibility = on;
                itemdel_button.Visibility = on;
                itemedit_button.Visibility = on;
                updownButton.Visibility = on;
                is_json_list = true;
                change_button.Content = CHANGE_EDIT;
            } else {
                // リストを見えなくして、テキストボックスを表示する
                edit_text.Visibility = on;
                delete_up_to_label.Visibility = on;
                json_list.Visibility = off;
                itemdel_button.Visibility = off;
                itemedit_button.Visibility = off;
                updownButton.Visibility = off;
                is_json_list = false;
                change_button.Content = CHANGE_LIST;
            }
        }

        // deleteが押されたら、全て初期化する
        public void delete_click(object sender, RoutedEventArgs e)
        {
            // json_listの状態は初期化したくないため残す
            bool is_json_list_bak = is_json_list;
            // パラメータの初期化
            json_text.delete();
            Items = new ObservableCollection<ItemClass>();
            text_controller = new TextControllerClass(json_text);
            json_controller = new JsonControllerClass(Items, json_text);
            edit_text.Text = "";
            is_json_list = is_json_list;
            change_color();
        }

        // 選択アイテムの削除
        public void delete_item(object sender, RoutedEventArgs e)
        {
            int item_index = json_list.SelectedIndex;
            bool is_label;
            string selectitem_end = "";
            string[] split_item;
            bool is_key;

            // 選択されていないか、先頭,末尾ならなにもしない
            if (json_list.SelectedItem == null || 
                    item_index == json_list.Items.Count - 1 ||
                    item_index == 0) {
                MessageBox.Show(DELETE_ERROR);
                return ;
            }
            
            // 選択されているアイテムを取得
            ItemClass selectitem = (ItemClass)json_list.SelectedItem;
            selectitem_end = selectitem.item.Substring(selectitem.item.Length - 1);
            
            // 選択しているものがかっこかどうか
            is_label = Regex.IsMatch(selectitem_end, status.LABEL_PATTERN);
            split_item = selectitem.item.Split(status.QUOTATION_WORD);
            is_key = Regex.IsMatch(selectitem.item, status.COLON_WORD);
            
            // {}[]を選択している場合は、なにもしない
            if (is_label || 
                    (split_item.Length == 3 && is_key)) {
                return;
            }

            // 選択されているアイテムを取り出してから、リストから削除
            Items.Remove(selectitem);

            //入れ替えたものをjsontextに反映
            json_controller.list_to_text();
            edit_text_reload();

        }
        
        // 選択したアイテムの編集
        public void edit_item(object sender, RoutedEventArgs e) 
        {
            int item_index = json_list.SelectedIndex;
            bool is_label;
            string selectitem_end = "";
            string key = "";
            string check_value = "";
            string value = "";
            bool is_key;
            string[] split_item;
            
            // 選択されていないか、先頭,末尾ならなにもしない
            if (json_list.SelectedItem == null || 
                    item_index == json_list.Items.Count - 1 ||
                    item_index == 0) {
                MessageBox.Show(DELETE_ERROR);
                return;
            }
            
            // 選択されているアイテムを取得
            ItemClass selectitem = (ItemClass)json_list.SelectedItem;
            selectitem_end = selectitem.item.Substring(selectitem.item.Length - 1);
            
            // 選択しているものがかっこかどうか
            is_label = Regex.IsMatch(selectitem_end, status.LABEL_PATTERN);
            // {}[]を選択している場合は、なにもしない
            if (is_label) {
                return;
            }

            // 選択したものがkeyなら":" が含まれる. valueなら含まれない
            is_key = Regex.IsMatch(selectitem.item, status.COLON_WORD);
            split_item = selectitem.item.Split(status.QUOTATION_WORD);
            
            if (is_key) {
                key = edit_window("key", split_item[1]);
                if (key == status.SPACE_WORD) {
                        MessageBox.Show(EDIT_ITEM_ERROR);
                        return ;
                }
                // 入力したkeyがjson_text内に存在するかどうか. 存在したら、メッセージを出して、入力はしない
                if (dupli_key_check(key, EDIT_MODE, item_index)) {
                    MessageBox.Show(status.SAME_KEY_ERROR);
                    return;
                }
                // key, valueが取得できたitemは長さが5になる。
                // 5未満のものは、valueをかっことしたkeyだけなので、その場合はkeyのみ編集
                if (split_item.Length == 5) {
                    value = edit_window("value", split_item[split_item.Length - 2]);
                    if (value == status.SPACE_WORD) {
                        MessageBox.Show(EDIT_ITEM_ERROR);
                        return ;
                    }
                    check_value = text_controller.check_value_text(value);
                    // valueを型変換可能か確認して、末尾が”なら文字列なのでkey+valueの組で入力               
                    if (check_value.Substring(check_value.Length - 1) != status.QUOTATION_WORD) {
                        value = check_value;
                            selectitem.item = split_item[0] 
                                        + status.QUOTATION_WORD 
                                        + key 
                                        + status.COLON_WORD
                                        + value;
                    } else {
                            selectitem.item = split_item[0] 
                                        + status.QUOTATION_WORD 
                                        + key 
                                        + status.COLON_WORD
                                        + status.QUOTATION_WORD
                                        + value
                                        + status.QUOTATION_WORD;
                    }
                    MessageBox.Show(selectitem.item);
                } else {
                    selectitem.item = split_item[0]
                                        + status.QUOTATION_WORD 
                                        + key 
                                        + status.COLON_WORD;
                }
            } else {
                // リスト内valueには文字列のほかにint, booleanが存在
                if (split_item.Length > 1) {
                    value = edit_window("value", split_item[1]);
                } else {
                    value = edit_window("value", split_item[0]);
                }
                if (value == status.SPACE_WORD) {
                        MessageBox.Show(EDIT_ITEM_ERROR);
                        return ;
                }
                check_value = text_controller.check_value_text(value);
                int indent = text_controller.count_char(split_item[0], status.INDENT_WORD);
                // valueを型変換可能か確認して、末尾が”なら文字列なのでkey+valueの組で入力               
                if (check_value.Substring(check_value.Length - 1) != status.QUOTATION_WORD) {
                    value = check_value;
                    selectitem.item = new string (status.INDENT_WORD, indent)
                                        + value;
                } else{
                    selectitem.item = new string (status.INDENT_WORD, indent)
                                    + status.QUOTATION_WORD
                                    + value
                                    + status.QUOTATION_WORD;
                }
            }
            // リストから削除して、新しく挿入する
            Items.Remove(selectitem);
            Items.Insert(item_index, selectitem);

            // 更新
            json_controller.list_to_text();
            edit_text_reload();
            
        }

        // json_textが "}"で閉じられている場合のみ処理を行う
        // "{" まで文字を削除する
        public void delete_close_label(object sender, RoutedEventArgs e)
        {
            // json_textが{か空白のとき、なにもしない(消すものがないから)
            if (json_text.Json == status.LABEL_DICT_OPEN || 
                    json_text.Json == status.SPACE_WORD){
                return;
            }
            string json_text_save;
            
            // 現在のjson_textの最後の文字が "{"の場合は、その文字を消してlabel_listを更新する
            string json_text_end = json_text.Json.Substring(json_text.Json.Length - 1);
            if (json_text_end == status.LABEL_DICT_OPEN) {
                // json_textの末尾を削除して更新する
                json_text_save = json_text.Json.Substring(0, json_text.Json.Length - 1);
                json_text.delete();
                json_text.Json = json_text_save;
                text_controller.manage_label(status.LABEL_DICT_CLOSE);
            }

            string[] alist;
            string delete_item;
            Match match_item;
            int last_indent = 0;
            match_item = Regex.Match(json_text.Json, status.LABEL_DICT_OPEN);
            alist = Regex.Split(json_text.Json, status.LABEL_DICT_OPEN);
            json_text.delete();
            delete_item = alist[alist.Length - 1];
            string end_word;

            // 消すアイテムの中に}, ]がいくつ入っているか
            var match_delete_labels = Regex.Matches(delete_item, status.CLOSE_PATTERN).Reverse();
            
            // 消すラベルをlabel_listに追加する
            foreach  (var delete_label in match_delete_labels) {
                if (delete_label.ToString() == status.LABEL_DICT_CLOSE) {
                    // label_listに{を追加する
                    text_controller.manage_label(status.LABEL_DICT_OPEN);
                } else if (delete_label.ToString() == status.LABEL_LIST_CLOSE) {
                    text_controller.manage_label(status.LABEL_LIST_OPEN);
                    // label_listに[を追加する
                }
            }
            
            // 消すアイテムが空白なら２回消すための処理
            if (delete_item == status.SPACE_WORD) {
                alist = alist.SkipLast(1).ToArray();
                delete_item = alist[alist.Length - 1];
            }

            // "}" から "{" までを消す
            if (Regex.IsMatch(alist[alist.Length - 1], status.SEARCH_LABEL_LIST_OPEN_PETTERN)) {
                // label_listから[を消す
                text_controller.manage_label(status.LABEL_LIST_CLOSE);
            }

            alist = alist.SkipLast(1).ToArray();
            
            foreach (var item in alist) {
                json_text.Json = item;
                json_text.Json = match_item.ToString();
            }

            // 消した後の一番最後の行の深さを取得
            var end_list = json_text.Json.Split(status.NEWLINE_WORD);
            end_word = end_list[end_list.Length - 1];
            last_indent = text_controller.count_char(end_word, status.INDENT_WORD);

            // 削除したものを更新する
            text_controller.indent_num = last_indent + 1;
            text_controller.before_text = status.LABEL_DICT_OPEN;
            text_controller.before_type = status.LABEL_DICT;
            change_color();
            edit_text_reload();
            
        }

        // 作成したjsonをファイルに保存する(保存ダイアログを出さない)
        public void new_save_json(object sender, RoutedEventArgs e)
        {
            // file名の長さチェック
            if (file_name.Length < FILE_MIN_LENGTH) {
                MessageBox.Show(FILE_NAME_ERROR);
                return;
            }
            // 最後が.jsonになっているかどうか
            bool is_json_extension = file_name.Length < FILE_MIN_LENGTH || 
                    file_name.Substring(file_name.Length - FILE_MIN_LENGTH) == status.JSON_EXT ;
            if (!is_json_extension) {
                MessageBox.Show(EXTENSION_ERROR);
                return;
            }

            // メッセージを出すためのテキスト
            string message_text = "";

            // SaveFileDialogを表示
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            // タイトル
            saveFileDialog.Title = "jsonファイルを保存"; 
            saveFileDialog.FileName = @file_name;
            
            
            DialogResult result = saveFileDialog.ShowDialog();
            // 保存できたかどうか
            if (result == System.Windows.Forms.DialogResult.OK) {
                File.WriteAllText(saveFileDialog.FileName, json_text.Json);
                message_text = saveFileDialog.FileName + "に保存しました。" ;
            } else {
                message_text = CANCEL_MESSAGE;
            }

            get_file_name(saveFileDialog.FileName);
            get_file_path(saveFileDialog.FileName);
            
            MessageBox.Show(message_text);
        }

        // 作成したjsonをファイルに保存する(保存ダイアログを出す)
        public void save_json(object sender, RoutedEventArgs e)
        {
            // file名の長さチェック
            if (file_name.Length < FILE_MIN_LENGTH) {
                MessageBox.Show(FILE_NAME_ERROR);
                return;
            }
            
            // 最後が.jsonになっているかどうか
            bool is_json_extension = file_name.Substring(file_name.Length - FILE_MIN_LENGTH) == status.JSON_EXT;
            if (!is_json_extension) {
                MessageBox.Show(EXTENSION_ERROR);
                return;
            }
            
            // 新規保存やファイルを開くをしていないのに、上書き保存をしたときは新規に保存させる
            if (file_name != status.SPACE_WORD && 
                    file_path != status.SPACE_WORD) {
                File.WriteAllText(file_path + file_name, json_text.Json);
            } else {
                MessageBox.Show(FILE_PATH_ERROR); 
                new_save_json(sender, e);
            }
        }

        // json_fileを読み込んでプレビューに表示する
        public void open_json(object sender, RoutedEventArgs e)
        {
            // メッセージを出すためのテキスト
            string message_text = "";
            OpenFileDialog loadFileDialog = new OpenFileDialog();
            
            // タイトル
            loadFileDialog.Title = LOAD_DESCRIPTION;
            // デフォルトフォルダ
            loadFileDialog.InitialDirectory = @status.JSON_EXT;
            DialogResult result = loadFileDialog.ShowDialog();
            // 開けたかどうか
            if (result == System.Windows.Forms.DialogResult.OK) {
                message_text = loadFileDialog.FileName + "を開きました";
            } else {
                message_text = CANCEL_MESSAGE;
                MessageBox.Show(message_text);
                return;
            }
            MessageBox.Show(message_text);
            
            get_file_name(loadFileDialog.FileName);
            get_file_path(loadFileDialog.FileName);
            
            // file_nameの更新
            file_name_text.Text = file_name;

            StreamReader sr = new StreamReader(loadFileDialog.FileName,
                System.Text.Encoding.GetEncoding("UTF-8"));
            
            json_text.delete();
            string open_text = sr.ReadToEnd();
            json_text.Json = open_text;
            edit_text_reload();
            sr.Close();
            
            // 最後に入力されたテキストの更新
            text_controller.before_text = open_text.Substring(open_text.Length - 1);
            change_color();
        }
    }
}
