namespace json_editor {
    class JsonTextClass {
        // json_textを編集する場合は
        // json_text.Json = ”追加する文字列” 
        // 右辺を代入するのではなく、蓄積させるというとこが少し変わっている点
        
        // 内容を取得する場合は
        // json_text.Json
        
        private string json_text = "";
        public string Json 
        {
            set {
                this.json_text += value;
            }
            get {
                return this.json_text;
            } 
        }
        // json_textを初期化する
        public void delete()
        {
            this.json_text = "";    
        }
    }
}