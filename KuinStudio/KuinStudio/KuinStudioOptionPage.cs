using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Baku.KuinStudio
{
    [ComVisible(true)]
    public class KuinStudioOptionPage : DialogPage
    {
        private string _kuinCompilerPath = "kuincl.exe";
        [Category("Environment")]
        [DisplayName("CompilerPath")]
        [Description("Compiler full path of Kuin (kuincl.exe) .")]
        public string KuinCompilerPath
        {
            get { return _kuinCompilerPath; }
            set
            {
                _kuinCompilerPath = value;
                KuinStudioOptionSettings.Instance.KuinCompilerPath = _kuinCompilerPath;
            }
        }

    }
}
