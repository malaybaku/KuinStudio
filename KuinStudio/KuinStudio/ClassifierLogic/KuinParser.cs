using System;
using System.Collections.Generic;
using Baku.KuinStudio.Parser;
using System.Linq;

namespace Baku.KuinStudio
{
    /// <summary>
    /// Kuinコードのデータ種類(予約語、変数名、組み込みグローバル関数、文字列リテラル、数値リテラル、コメント)を
    /// 識別してハイライティングの内容を決めてくれるすごいやつだよ
    /// </summary>
    internal class KuinParser
    {
        public KuinParser()
        {

        }


        /// <summary>
        /// もっともシンプルなパターン、ソースファイルを最初から最後まで読み込んだときのパースを行う。
        /// </summary>
        /// <param name="fullSource"></param>
        /// <returns></returns>
        public IEnumerable<ParsedKuinToken> Parse(string fullSource)
        {
            int location = 0;
            int line = 0;
            int col = 0;

            //改行整備 + パーサ動作都合で末尾に必ず改行つける
            string source = fullSource.Replace("\r\n", "\n");
            source = source.EndsWith("\n") ? source : source + "\n";

            var result = new List<ParsedKuinToken>();
            //ひたすら先頭から千切って結果のリストに叩き込む
            while (source != "")
            {
                //要素パーサーのラストにunknown parserがあるので必ずどれかには引っかかる
                var token = _elemParser
                    .Select(parser => parser.ParseHead(source))
                    .First(item => item != null);

                result.Add(new ParsedKuinToken(token.TokenType, token.Content, location, line, col));

                location += token.Content.Length;

                //Endとか複数行コメントがある場合は行数も増やしていかないと対応できない
                line += token.Content.Count(c => c == '\n');

                //colは改行計算必要な点に注意
                if (token.TokenType == KuinTokens.End)
                {
                    col = 0;
                }
                else if (token.TokenType == KuinTokens.MultiLineComment)
                {
                    var comments = token.Content.Split('\n');
                    col = comments[comments.Length - 1].Length;
                }
                else
                {
                    col += token.Content.Length;
                }

                //取り出した分を削って次へ行きましょう。
                source = source.Substring(token.Content.Length);
            }

            return result;
        }

        private readonly IKuinElementParser[] _elemParser = new IKuinElementParser[]
        {
            //順番に注意、例えば予約語判定は関数名称より前だし、変数名判定は関数名称より後じゃないと困る
            new EndParser(),
            new EmptyParser(),
            new SingleLineCommentParser(),
            new MultiLineCommentParser(),
            new OperatorParser(),
            new ReservedParser(),
            new StringLiteralParser(),
            new NumberLiteralParser(),
            new FuncNameParser(),
            new VarNameParser(),
            new UnknownParser()
        };

    }

    /// <summary>
    /// ハイライトが必要なKuinコードの位置を表します。
    /// </summary>
    internal class ParsedKuinToken
    {
        public ParsedKuinToken(string tokenType, string content, int location, int lineNumber, int colNumber)
        {
            TokenType = tokenType;
            Content = content;

            Location = location;
            LineNumber = lineNumber;
            ColNumber = colNumber;
        }

        /// <summary>get token type by string</summary>
        public string TokenType { get; }

        /// <summary>get actual source code content</summary>
        public string Content { get; }

        /// <summary>
        /// Location in full source, but returns -1 if not calculated.
        /// </summary>
        public int Location { get; }

        /// <summary>
        /// Line number in full source
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Column number in full source
        /// </summary>
        public int ColNumber { get; }
    }
}