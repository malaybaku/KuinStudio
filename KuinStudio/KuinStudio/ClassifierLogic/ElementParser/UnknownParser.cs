namespace Baku.KuinStudio.Parser
{
    internal class UnknownParser : IKuinElementParser
    {
        public string ContentType => KuinTokens.Unknown;

        public string FindElement(string src)
            => src.Substring(0, 1);
    }
}
