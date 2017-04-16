using System.Text.RegularExpressions;

namespace Baku.KuinStudio.Parser
{
    internal class StringLiteralParser : IKuinElementParser
    {
        public string ContentType => KuinTokens.StringLiteral;

        public string FindElement(string src)
        {
            //文字リテラル
            var charMatch = Regex.Match(src, @"(^'\'')|(^'.*')");
            if (charMatch.Success)
            {
                return src.Substring(0, charMatch.Length);
            }

            //文字列リテラル ※ほんとは埋め込み文字列シンタックスについて別途検討しないとまずいんです
            var strMatch = Regex.Match(src, @"^\""((\\\"")|[^\""])*[^\\]\""");
            if (strMatch.Success)
            {
                return src.Substring(0, strMatch.Length);
            }
            return "";
        }
    }
}
