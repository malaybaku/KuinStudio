using System;
using Microsoft.VisualStudioTools.Project;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Baku.KuinStudio.Project
{
    [Guid(KuinStudioPackage.ProjectFactoryGuidString)]
    public class KuinProjectFactory : ProjectFactory
    {
        public KuinProjectFactory(CommonProjectPackage package) : base((IServiceProvider)package)
        {
            _package = package;
        }
        private readonly CommonProjectPackage _package;

        internal override ProjectNode CreateProject()
        {
            var stream = GetType()
                .Assembly
                .GetManifestResourceStream("Baku.KuinStudio.Resources.KuinColor.bmp");

            var project = new KuinProjectNode(Site, Utilities.GetImageList(stream));

            var package = ((IServiceProvider)_package)?
                .GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider));

            project.SetSite((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)package);

            return project;
        }
    }
}
