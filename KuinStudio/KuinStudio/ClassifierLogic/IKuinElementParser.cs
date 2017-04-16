namespace Baku.KuinStudio.Parser
{
    internal interface IKuinElementParser
    {
        //先頭からトークンを取り出せたら取り出し、ダメならnullを返す。
        string FindElement(string src);

        //このパーサーが取り出せるトークンの名称
        string ContentType { get; }
    }

    internal static class IKuinElementParserExtensions
    {
        public static KuinParseResult ParseHead(this IKuinElementParser parser, string src)
        {
            string res = parser.FindElement(src);
            return string.IsNullOrEmpty(res) ? null : new KuinParseResult(parser.ContentType, res);
        }
    }

    internal class KuinParseResult
    {
        public KuinParseResult(string tokenType, string content)
        {
            TokenType = tokenType;
            Content = content;
        }

        public string TokenType { get; }
        public string Content { get; }
    }
}
