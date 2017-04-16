using System;
using System.Windows.Forms;

namespace Baku.KuinStudio.Project
{
    public partial class KuinGeneralPropertyPageControl : UserControl
    {
        internal KuinGeneralPropertyPageControl(KuinGeneralPropertyPage page)
        {
            InitializeComponent();
            _page = page;
        }
        private readonly KuinGeneralPropertyPage _page;

        internal void LoadSettings()
        {
            StartupFile = _page.Project.GetProjectProperty(KuinProjectResources.StartupFilePropertyName);
            OutputFile = _page.Project.GetProjectProperty(KuinProjectResources.OutputFilePropertyName);
            IconFileName = _page.Project.GetProjectProperty(KuinProjectResources.IconFilePropertyName);
            WorkingDirectory = _page.Project.GetProjectProperty(KuinProjectResources.WorkingDirectoryPropertyName);
            if (string.IsNullOrWhiteSpace(WorkingDirectory))
            {
                WorkingDirectory = ".";
            }
            ApplicationType = _page.Project.GetProjectProperty(KuinProjectResources.ApplicationTypePropertyName);
            UseReleaseBuild = _page.Project.GetProjectProperty(KuinProjectResources.UseReleaseBuildPropertyName);
            CustomSystemDirectory = _page.Project.GetProjectProperty(KuinProjectResources.CustomSystemDirectoryPropertyName);
        }

        public string StartupFile
        {
            get => textBoxStartupFileName.Text;
            set => textBoxStartupFileName.Text = value;
        }

        public string OutputFile
        {
            get => textBoxOutputFileName.Text;
            set => textBoxOutputFileName.Text = value;
        }

        public string IconFileName
        {
            get => textBoxIconFileName.Text;
            set => textBoxIconFileName.Text = value;
        }

        public string WorkingDirectory
        {
            get => textBoxWorkingDirectory.Text;
            set => textBoxWorkingDirectory.Text = value;
        }
        
        public string ApplicationType
        {
            get
            {
                return radioButtonUseWndMode.Checked ? KuinProjectNodeProperties.AppTypeGui :
                    radioButtonUseCuiMode.Checked ? KuinProjectNodeProperties.AppTypeCui :
                    radioButtonUseWebMode.Checked ? KuinProjectNodeProperties.AppTypeWeb :
                    KuinProjectNodeProperties.AppTypeGui;
            }
            set
            {
                if (value != KuinProjectNodeProperties.AppTypeGui &&
                    value != KuinProjectNodeProperties.AppTypeCui &&
                    value != KuinProjectNodeProperties.AppTypeWeb)
                {
                    return;
                }

                if (value == KuinProjectNodeProperties.AppTypeGui && !radioButtonUseWndMode.Checked)
                {
                    radioButtonUseCuiMode.Checked = false;
                    radioButtonUseWebMode.Checked = false;
                    radioButtonUseWndMode.Checked = true;
                }

                if (value == KuinProjectNodeProperties.AppTypeCui && !radioButtonUseCuiMode.Checked)
                {
                    radioButtonUseWndMode.Checked = false;
                    radioButtonUseWebMode.Checked = false;
                    radioButtonUseCuiMode.Checked = true;
                }

                if (value == KuinProjectNodeProperties.AppTypeWeb && !radioButtonUseWebMode.Checked)
                {
                    radioButtonUseWndMode.Checked = false;
                    radioButtonUseCuiMode.Checked = false;
                    radioButtonUseWebMode.Checked = true;
                }
            }
        }

        public string UseReleaseBuild
        {
            get => checkBoxUseReleaseBuild.Checked ? "True" : "False";
            set
            {
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    checkBoxUseReleaseBuild.Checked = true;
                }
                else if ("False".Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    checkBoxUseReleaseBuild.Checked = false;
                }
            }
        }

        public string CustomSystemDirectory
        {
            get => textBoxCustomSystemDirectory.Text;
            set => textBoxCustomSystemDirectory.Text = value;
        }

        private void OnChanged(object sender, EventArgs e)
        {
            _page.IsDirty = true;
        }
    }
}
