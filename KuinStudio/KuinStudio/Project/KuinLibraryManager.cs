using Microsoft.VisualStudioTools.Navigation;
using Microsoft.VisualStudioTools;
using System.Runtime.InteropServices;

namespace Baku.KuinStudio.Project
{
    /// <summary> Dummy manager to avoid NullReferenceException </summary>
    [Guid(KuinStudioPackage.LibraryManagerGuidString)]
    internal class KuinLibraryManager : LibraryManager
    {
        public KuinLibraryManager(CommonPackage package) : base(package)
        {
        }
    }
}
