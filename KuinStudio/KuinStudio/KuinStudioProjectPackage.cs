using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudioTools;
using Microsoft.VisualStudioTools.Project;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Baku.KuinStudio.Project
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideProjectFactory(typeof(KuinProjectFactory), null,
        "Kuin Project File(*.knproj);*.knproj", "knproj", "knproj", @".\Nullpath",
        LanguageVsTemplate = "Kuin")]
    [ProvideObject(typeof(KuinGeneralPropertyPage))]
    [Guid(KuinStudioPackage.ProjectPackageGuidString)]
    [Export]
    public sealed class KuinStudioProjectPackage : CommonProjectPackage
    {
        public const string KuinGeneralPropertyPageGuid = "5A5D9E06-0219-48AF-8597-7B4C7EAD4064";

        public override ProjectFactory CreateProjectFactory()
            => new KuinProjectFactory(this);

        public override CommonEditorFactory CreateEditorFactory() => null;

        public override uint GetIconIdForAboutBox() => 0;

        public override uint GetIconIdForSplashScreen() => 0;

        public override string GetProductName() => "Kuin";

        public override string GetProductDescription() => "Kuin";

        public override string GetProductVersion() 
            => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        protected override void Initialize()
        {
            base.Initialize();

            var services = (IServiceContainer)this;
            services.AddService(typeof(IClipboardService), new ClipboardService(), promote: true);

            IVsShell shell = GetService(typeof(SVsShell)) as IVsShell;
            if (shell == null) return;

            Guid PackageToBeLoadedGuid = new Guid(KuinStudioPackage.PackageGuidString);
            shell.LoadPackage(ref PackageToBeLoadedGuid, out IVsPackage package);
        }
    }
}
