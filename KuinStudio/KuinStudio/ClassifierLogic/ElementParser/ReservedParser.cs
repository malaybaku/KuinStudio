using System.Collections.Generic;
using System.Linq;

namespace Baku.KuinStudio.Parser
{
    public class ReservedParser : IKuinElementParser
    {
        public string ContentType => KuinTokens.Reserved;

        public string FindElement(string src)
        {
            return _reservedSorted.FirstOrDefault(
                r => src.StartsWith(r) &&
                IsSplittingChar(src[r.Length])
            );
        }

        //予約語だと思ってる文字列の次の文字できちんと途切れていることのチェック
        private static bool IsSplittingChar(char target)
        {
            int t = (int)target;

            return !(
                ((int)'a' <= t && t <= (int)'z') ||
                ((int)'A' <= t && t <= (int)'Z') ||
                (t == '_') ||
                (t == '@')
                );
        }

        static ReservedParser()
        {
            _reservedSorted = _reserved.OrderByDescending(r => r.Length).ToArray();
        }

        private static IReadOnlyCollection<string> _reservedSorted;

        //note: ハイライトが目的なので詳細な分類しないことに注意
        private static readonly IReadOnlyCollection<string> _reserved = new string[]
        {
            "alias",
            "assert",
            "bit16",
            "bit32",
            "bit64",
            "bit8",
            "block",
            "bool",
            "break",
            "case",
            "catch",
            "char",
            "class",
            "const",
            "dbg",
            "default",
            "dict",
            "do",
            "elif",
            "else",
            "end",
            "enum",
            "false",
            "finally",
            "float",
            "for",
            "foreach",
            "func",
            "if",
            "ifdef",
            "inf",
            "int",
            "list",
            "me",
            "null",
            "queue",
            "ret",
            "rls",
            "skip",
            "stack",
            "switch",
            "throw",
            "to",
            "true",
            "try",
            "var",
            "while",
        };

    }
}
