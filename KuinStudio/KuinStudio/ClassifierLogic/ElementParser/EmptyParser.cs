using System.Text;

namespace Baku.KuinStudio.Parser
{
    internal class EmptyParser : IKuinElementParser
    {
        public string ContentType => KuinTokens.Empty;

        public string FindElement(string src)
        {
            //Regex使わんでもいいレベル
            var res = new StringBuilder();

            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == ' ' || src[i] == '\t')
                {
                    res.Append(src[i]);
                }
                else
                {
                    break;
                }
            }

            return res.ToString();
        }
    }
}
