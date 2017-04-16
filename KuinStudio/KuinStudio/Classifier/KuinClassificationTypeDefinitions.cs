using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Baku.KuinStudio
{
    internal static class KuinEditorClassifierClassificationDefinition
    {
#pragma warning disable 414

        [Export]
        [Name(KuinHighlights.Base)]
        private static ClassificationTypeDefinition kuinBase = null;

        [Export]
        [Name(KuinHighlights.Reserved)]
        [BaseDefinition(KuinHighlights.Base)]
        private static ClassificationTypeDefinition kuinReserved = null;

        [Export]
        [Name(KuinHighlights.Operator)]
        [BaseDefinition(KuinHighlights.Base)]
        private static ClassificationTypeDefinition kuinOperator = null;

        [Export]
        [Name(KuinHighlights.String)]
        [BaseDefinition(KuinHighlights.Base)]
        private static ClassificationTypeDefinition kuinString = null;

        [Export]
        [Name(KuinHighlights.Number)]
        [BaseDefinition(KuinHighlights.Base)]
        private static ClassificationTypeDefinition kuinNumber = null;

        [Export]
        [Name(KuinHighlights.VarName)]
        [BaseDefinition(KuinHighlights.Base)]
        private static ClassificationTypeDefinition kuinVarName = null;

        [Export]
        [Name(KuinHighlights.FuncName)]
        [BaseDefinition(KuinHighlights.Base)]
        private static ClassificationTypeDefinition kuinFuncName = null;

        [Export]
        [Name(KuinHighlights.SingleLineComment)]
        [BaseDefinition(KuinHighlights.Base)]
        private static ClassificationTypeDefinition kuinSingleLineComment = null;

        [Export]
        [Name(KuinHighlights.MultiLineComment)]
        [BaseDefinition(KuinHighlights.Base)]
        private static ClassificationTypeDefinition kuinMultiLineComment = null;

#pragma warning restore 414
    }
}
