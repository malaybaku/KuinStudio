using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.Text.RegularExpressions;
using System.Linq;

namespace Baku.KuinStudio
{
    internal class KuinClassifier : IClassifier
    {
        private readonly KuinParser _parser = new KuinParser();
        private readonly IClassificationTypeRegistryService _service;
        private readonly Dictionary<string, IClassificationType> _classifiers;

        internal KuinClassifier(IClassificationTypeRegistryService registry)
        {
            _service = registry;

            _classifiers = new Dictionary<string, string>()
            {
                [KuinTokens.Reserved] = KuinHighlights.Reserved,
                [KuinTokens.Operator] = KuinHighlights.Operator,
                [KuinTokens.SingleLineComment] = KuinHighlights.SingleLineComment,
                [KuinTokens.MultiLineComment] = KuinHighlights.MultiLineComment,
                [KuinTokens.StringLiteral] = KuinHighlights.String,
                [KuinTokens.NumberLiteral] = KuinHighlights.Number,
                [KuinTokens.VarName] = KuinHighlights.VarName,
                [KuinTokens.FuncName] = KuinHighlights.FuncName
            }.ToDictionary(
                p => p.Key, 
                p => registry.GetClassificationType(p.Value)
                );
        }
            

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>();

            //とりあえず単一行でのパースだけ
            var targetLine = span.Start.GetContainingLine();
            string source = targetLine.GetTextIncludingLineBreak();

            var parsed = _parser.Parse(source);
            foreach(var item in parsed)
            {
                if (!_classifiers.ContainsKey(item.TokenType))
                {
                    continue;
                }

                result.Add(new ClassificationSpan(
                    new SnapshotSpan(targetLine.Start.Add(item.ColNumber), item.Content.Length), 
                    _classifiers[item.TokenType]
                    ));
            }

            return result;
        }

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67
    }
}
