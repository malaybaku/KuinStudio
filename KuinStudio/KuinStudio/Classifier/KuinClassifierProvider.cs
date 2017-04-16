using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Baku.KuinStudio
{
    [Export(typeof(IClassifierProvider))]
    [ContentType(KuinFileSpecification.KuinContentType)]
    internal class KuinClassifierProvider : IClassifierProvider
    {
        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(
                () => new KuinClassifier(classificationRegistry)
                );
        }

#pragma warning disable 649

        [Import]
        private IClassificationTypeRegistryService classificationRegistry;

#pragma warning restore 649
    }

    /// <summary>
    /// file type and extension definition.
    /// </summary>
    internal class KuinSourceContentTypeDefinitions
    {
#pragma warning disable 649

        [Export]
        [Name(KuinFileSpecification.KuinContentType)]
        [BaseDefinition(KuinFileSpecification.KuinFileBaseDefinition)]
        internal static ContentTypeDefinition kuinContentTypeDefinition;

        [Export]
        [FileExtension(KuinFileSpecification.KuinFileExtension)]
        [ContentType(KuinFileSpecification.KuinContentType)]
        internal static FileExtensionToContentTypeDefinition kuinFileExtensionDefinition;

#pragma warning restore 649
    }
}
