using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudioTools.Project;

namespace Baku.KuinStudio.Project
{
    [Guid(KuinStudioPackage.ProjectNodeGuidString)]
    internal class KuinProjectNode : CommonProjectNode
    {
        public KuinProjectNode(IServiceProvider serviceProvider, ImageList imageList) : base(serviceProvider, imageList)
        {
        }

        protected override Stream ProjectIconsImageStripStream
        {
            get
            {
                return typeof(KuinProjectNode).Assembly.GetManifestResourceStream(
                    "Baku.KuinStudio.Resources.KuinColor.bmp"
                    );
            }
        }

        public override string GetProjectName() => "Kuin";

        public override Type GetProjectFactoryType() => typeof(KuinProjectFactory);

        public override string GetFormatList() => "Kuin Code Files(*.kn)|*.kn";

        internal override string IssueTrackerUrl => "https://github.com/malaybaku/KuinStudio/issues";

        public override Type GetGeneralPropertyPageType() => typeof(KuinGeneralPropertyPage);

        //以下は不要そう

        public override IProjectLauncher GetLauncher() => null;

        public override Type GetEditorFactoryType() => null;

        public override Type GetLibraryManagerType() => typeof(KuinLibraryManager);

        protected override NodeProperties CreatePropertiesObject()
            => new KuinProjectNodeProperties(this);

        protected override Guid[] GetConfigurationIndependentPropertyPages()
        {
            return new Guid[]
            {
                typeof(KuinGeneralPropertyPage).GUID
            };
        }
    }
}
