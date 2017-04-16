using System.Linq;

namespace Baku.KuinStudio.Parser
{
    internal class SingleLineCommentParser : IKuinElementParser
    {
        public string FindElement(string src)
        {
            if (src[0] == ';')
            {
                return new string(src.TakeWhile(c => c != '\n').ToArray());
            }
            return "";
        }

        public string ContentType => KuinTokens.SingleLineComment;
    }
}
