namespace json_editor {
    public class StatusClass{

        // 他クラスで多用される定数の宣言

        // readonlyはconstと同じようなもの
        public readonly string KEY = "key";
        public readonly string VALUE = "value";
        public readonly string LABEL_DICT_OPEN = "{";
        public readonly string LABEL_DICT_CLOSE = "}";
        public readonly string LABEL_DICT_OPEN_CLOSE = "{}";
        public readonly string LABEL_DICT = "label_dict";
        public readonly string LABEL_LIST_OPEN = "[";
        public readonly string LABEL_LIST_CLOSE = "]";
        public readonly string LABEL_LIST = "label_list";
        public readonly string QUOTATION_WORD = "\"";
        public readonly string COLON_WORD = "\": ";
        public readonly string NULL_WORD = "null";
        public readonly string FALSE_WORD = "false";
        public readonly string TRUE_WORD = "true"; 
        public readonly string NEWLINE_WORD = "\n";  
        public readonly string COMMA_WORD = ",\n";
        public readonly string SPACE_WORD = "";
        public readonly string SPLIT_PATH_WORD = "\\";
        public readonly string JSON_EXT = ".json";
        
        
        public readonly char INDENT_WORD = '\t';
        
        // 正規表現
        // {, [ かどうか(開きかっこ)
        public readonly string OPEN_PATTERN = "[{[]";
        // }, ] かどうか(閉じかっこ) 
        public readonly string CLOSE_PATTERN = @"[}\]]";
        // ", e, l, 数字　かどうか
        // 前回打ち込まれたテキストがvalueかどうか判断する
        public readonly string VALUE_PATTERN = "[\"el0-9]"; 
        // {}, []の末尾にきたら移動させないskip_wordの指定
        public readonly string UP_SKIP_WORD_PETTERN = @"[{[\]]";
        // {, [, 半角空白の末尾にきたら移動させないskip_wordの指定
        public readonly string DOWN_SKIP_WORD_PETTERN = @"[ }[]";
        // [ を検索する。
        public readonly string SEARCH_LABEL_LIST_OPEN_PETTERN = @"\[";
        // labelpettern
        public readonly string LABEL_PATTERN = @"[{}[\]]";
        // dict_pettern 
        public readonly string DICT_PATTERN = @"[{}]";
        
        // value入れ替え時のskip_word
        public readonly string UP_VALUE_SKIP_WORD_PETTERN = @"[{]";
        public readonly string DOWN_VALUE_SKIP_WORD_PETTERN = @"[}]";

        // メッセージ用
        public readonly string JSON_CHANGE_SUCCESS = "json変換成功";
        public readonly string JSON_CHANGE_ERROR = "jsonに変換できませんでした。\nもう一度書き直してください";
        public readonly string SAME_KEY_ERROR = "入力しようとしたkeyは既に存在しています.\nkeyを変更してください";
    }   
}