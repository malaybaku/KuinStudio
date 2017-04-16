using System.Text.RegularExpressions;

namespace Baku.KuinStudio.Parser
{
    internal class NumberLiteralParser : IKuinElementParser
    {
        public string ContentType => KuinTokens.NumberLiteral;

        public string FindElement(string src)
        {
            //とりあえず行をまたぐの禁止！(パース時の数え方がめんどくさすぎる)
            string target = src.Split('\n')[0];

            //10進数以外
            var m16 = Regex.Match(target, "^16#[0-9A-F]+");
            if(m16.Success)
            {
                return target.Substring(0, m16.Length);
            }

            var m8 = Regex.Match(target, "^8#[0-7]+");
            if (m8.Success)
            {
                return target.Substring(0, m8.Length);
            }

            var m2 = Regex.Match(target, "^2#[0-1]+");
            if (m2.Success)
            {
                return target.Substring(0, m2.Length);
            }

            //10進数(一番よくあるパターン)
            var m10 = Regex.Match(target, @"^[0-9]*\.?[0-9]*(e[\+-][0-9]+)?");
            if (m10.Success)
            {
                string result = target.Substring(0, m10.Length);
                if (result.Length > 0 && result != ".")
                {
                    return result;
                }
            }

            return "";
        }
    }
}
