namespace Baku.KuinStudio.Parser
{
    internal class EndParser : IKuinElementParser
    {
        public string ContentType => KuinTokens.End;

        public string FindElement(string src)
            => (src[0] == '\n') ? src.Substring(0, 1) : "";
    }
}
