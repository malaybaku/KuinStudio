using System.Text.RegularExpressions;

namespace Baku.KuinStudio.Parser
{
    internal class VarNameParser : IKuinElementParser
    {
        public string ContentType => KuinTokens.VarName;

        public string FindElement(string src)
        {
            //NOTE: 変数に@つくことはない?(名前空間越しでフィールドアクセスするようなややこしい処理は不許可？)
            var m = Regex.Match(src, "^[a-zA-Z_@][a-zA-Z0-9_@]+");
            if (m.Success)
            {
                return src.Substring(0, m.Length);
            }
            return "";
        }
    }
}
