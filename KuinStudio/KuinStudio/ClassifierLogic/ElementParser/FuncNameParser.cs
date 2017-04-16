using System.Text.RegularExpressions;

namespace Baku.KuinStudio.Parser
{
    internal class FuncNameParser : IKuinElementParser
    {
        public string ContentType => KuinTokens.FuncName;

        public string FindElement(string src)
        {
            //Funcと判別する最大のポイントは末尾に"("があるかどうか
            //(※"("自体は演算子だからパース結果に含めたらダメ)
            var m = Regex.Match(src, "^[a-zA-Z_@][a-zA-Z0-9_@]+");
            if (m.Success && src[m.Length] == '(')
            {
                return src.Substring(0, m.Length);
            }
            return "";
        }
    }
}
