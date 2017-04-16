namespace Baku.KuinStudio
{
    public static class KuinTokens
    {
        public static readonly string End = "End";
        public static readonly string Empty = "Empty";

        public static readonly string SingleLineComment = "SingleLineComment";
        public static readonly string MultiLineComment = "MultiLineComment";

        public static readonly string Reserved = "Reserved";
        public static readonly string Operator = "Operator";

        public static readonly string VarName = "VarName";
        public static readonly string FuncName = "FuncName";

        public static readonly string StringLiteral = "String";
        public static readonly string NumberLiteral = "Number";

        //よく分からない場合でもパースを進まなければならないので、1文字の不明トークンとして扱うことを許可
        public static readonly string Unknown = "Unknown";
    }
}
