using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Baku.KuinStudio
{
    // note: some colors are from Visual Studio dark theme
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = KuinHighlights.Base)]
    [Name(KuinHighlights.Base)]
    internal sealed class KuinBaseFormat : ClassificationFormatDefinition
    {
        public KuinBaseFormat()
        {
            //no change
            //ForegroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = KuinHighlights.Reserved)]
    [Name(KuinHighlights.Reserved)]
    internal class KuinReservedKeywordFormat : ClassificationFormatDefinition
    {
        public KuinReservedKeywordFormat()
        {
            ForegroundColor = Color.FromArgb(0xFF, 0x56, 0x9C, 0xD6);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = KuinHighlights.Operator)]
    [Name(KuinHighlights.Operator)]
    internal class KuinOperatorFormat : ClassificationFormatDefinition
    {
        public KuinOperatorFormat()
        {
            //no change
            //ForegroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = KuinHighlights.SingleLineComment)]
    [Name(KuinHighlights.SingleLineComment)]
    internal class KuinSingleLineCommentFormat : ClassificationFormatDefinition
    {
        public KuinSingleLineCommentFormat()
        {
            ForegroundColor = Color.FromArgb(0xFF, 0x57, 0xA6, 0x4A);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = KuinHighlights.MultiLineComment)]
    [Name(KuinHighlights.MultiLineComment)]
    internal class KuinMultiLineCommentFormat : ClassificationFormatDefinition
    {
        public KuinMultiLineCommentFormat()
        {
            ForegroundColor = Color.FromArgb(0xFF, 0x57, 0xA6, 0x4A);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = KuinHighlights.VarName)]
    [Name(KuinHighlights.VarName)]
    internal class KuinVarNameFormat : ClassificationFormatDefinition
    {
        public KuinVarNameFormat()
        {
            //do nothing
            //ForegroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = KuinHighlights.FuncName)]
    [Name(KuinHighlights.FuncName)]
    internal class KuinFuncNameFormat : ClassificationFormatDefinition
    {
        public KuinFuncNameFormat()
        {
            ForegroundColor = Color.FromArgb(0xFF, 0xEE, 0x38, 0xFF);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = KuinHighlights.String)]
    [Name(KuinHighlights.String)]
    internal class KuinStringLiteralFormat : ClassificationFormatDefinition
    {
        public KuinStringLiteralFormat()
        {
            ForegroundColor = Color.FromArgb(0xff, 0xD6, 0x9D, 0x85);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = KuinHighlights.Number)]
    [Name(KuinHighlights.Number)]
    internal class KuinNumberLiteralFormat : ClassificationFormatDefinition
    {
        public KuinNumberLiteralFormat()
        {
            ForegroundColor = Color.FromArgb(0xFF, 0xB5, 0xCE, 0xA8);
        }
    }

}
