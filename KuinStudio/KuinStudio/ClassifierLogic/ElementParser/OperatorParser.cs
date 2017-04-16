using System;
using System.Collections.Generic;
using System.Linq;

namespace Baku.KuinStudio.Parser
{
    /// <summary>
    /// 演算子ひとまとめ
    /// </summary>
    public class OperatorParser : IKuinElementParser
    {
        public string ContentType => KuinTokens.Operator;

        public string FindElement(string src)
        {
            return _operatorsSorted
                .FirstOrDefault(o => src.StartsWith(o)) ?? "";
        }

        static OperatorParser()
        {
            _operatorsSorted = _operators
                .Where(o => o.Length == 3)
                .Concat(_operators.Where(o => o.Length == 2))
                .Concat(_operators.Where(o => o.Length == 1))
                .ToArray();
        }

        private static IReadOnlyList<string> _operatorsSorted;

        //NOTE: "?"と","と":"は演算子じゃないがハイライティング的にはここに入れるのが正しいハズ
        //NOTE: "[]"と"()"、とテンプレで使う"<>"はいずれも開き閉じがあるのでバラしている
        private static readonly IReadOnlyList<string> _operators = new string[]
        {
            "?",
            ",",
            ":",
            "(",
            ")",
            "[",
            "]",
            ".",
            "$",
            "$>",
            "$<",
            "^",
            "+",
            "-",
            "!",
            "#",
            "##",
            "*",
            "/",
            "%",
            "~",
            "=",
            "<>",
            "<",
            ">",
            "<=",
            ">=",
            "=&",
            "<>&",
            "=$",
            "<>$",
            "&",
            "|",
            ":$",
            "::",
            ":+",
            ":-",
            ":*",
            ":/",
            ":%",
            ":^",
            ":~"
        };
    }
}
