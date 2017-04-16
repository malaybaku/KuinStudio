namespace Baku.KuinStudio.Parser
{
    internal class MultiLineCommentParser : IKuinElementParser
    {
        public string ContentType => KuinTokens.MultiLineComment;

        public string FindElement(string src)
        {
            if (src[0] != '{')
            {
                return "";
            }

            //"{"と"}"の数がそろえばOK
            int count = 1;
            for (int i = 1; i < src.Length; i++)
            {
                count =
                    (src[i] == '{') ? (count + 1) :
                    (src[i] == '}') ? (count - 1) :
                    count;

                //編集途中で起きやすそうな、末尾(ラストの改行文字の手前)まで
                if (count == 0 || (i == src.Length - 2))
                {
                    return src.Substring(0, i + 1);
                }
            }

            //来ないはずだけど一応
            return src.Substring(0, src.Length - 2);
        }
    }
}
