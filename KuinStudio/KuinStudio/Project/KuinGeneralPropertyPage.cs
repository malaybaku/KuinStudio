using Microsoft.VisualStudioTools.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Baku.KuinStudio.Project
{
    [Guid(KuinStudioProjectPackage.KuinGeneralPropertyPageGuid)]
    public class KuinGeneralPropertyPage : CommonPropertyPage
    {
        private readonly KuinGeneralPropertyPageControl _control;

        public KuinGeneralPropertyPage()
        {
            _control = new KuinGeneralPropertyPageControl(this);
        }

        internal KuinProjectNode KuinProject => (KuinProjectNode)Project;

        public override Control Control => _control;

        public override string Name => KuinProjectResources.General;

        public override void Apply()
        {
            Project.SetProjectProperty(KuinProjectResources.StartupFilePropertyName, _control.StartupFile);
            Project.SetProjectProperty(KuinProjectResources.OutputFilePropertyName, _control.OutputFile);
            Project.SetProjectProperty(KuinProjectResources.IconFilePropertyName, _control.IconFileName);
            Project.SetProjectProperty(KuinProjectResources.WorkingDirectoryPropertyName, _control.WorkingDirectory);
            Project.SetProjectProperty(KuinProjectResources.UseReleaseBuildPropertyName, _control.UseReleaseBuild);
            Project.SetProjectProperty(KuinProjectResources.ApplicationTypePropertyName, _control.ApplicationType);
            Project.SetProjectProperty(KuinProjectResources.CustomSystemDirectoryPropertyName, _control.CustomSystemDirectory);

            IsDirty = false;
            LoadSettings();
        }

        public override void LoadSettings()
        {
            Loading = true;
            try
            {
                _control.LoadSettings();
            }
            finally
            {
                Loading = false;
            }
        }

    }
}
