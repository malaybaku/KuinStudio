using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudioTools;
using Microsoft.VisualStudioTools.Project;

using Baku.KuinStudio.Project;

//cf: https://github.com/Microsoft/PTVS/blob/master/Python/Product/PythonTools/PythonTools/Commands/StartScriptCommand.cs

namespace Baku.KuinStudio
{
    internal abstract class KuinCommandBase : Command
    {
        public KuinCommandBase(IServiceProvider provider)
        {
            _provider = provider;
        }
        private readonly IServiceProvider _provider;

        public override EventHandler BeforeQueryStatus => OnQueryDisplayStatus;
        private void OnQueryDisplayStatus(object sender, EventArgs e)
        {
            var menuCommand = (sender as OleMenuCommand);
            if (menuCommand == null)
            {
                return;
            }

            if (GetKuinProjectProperties() != null)
            {
                menuCommand.Visible = true;
                menuCommand.Enabled = true;
            }
            else
            {
                menuCommand.Visible = false;
                menuCommand.Enabled = false;
            }
        }

        private static string KuinclPath
            => KuinStudioOptionSettings.Instance.KuinCompilerPath;

        private IVsOutputWindowPane GetOutputWindowPane()
        {
            Guid targetGuid = VSConstants.GUID_BuildOutputWindowPane;

            IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            outWindow.GetPane(ref targetGuid, out IVsOutputWindowPane outPane);
            return outPane;
        }

        /// <summary>
        /// ソリューションにあるアクティブなKuin Projectのプロパティを取得する。
        /// WARN: 開いているドキュメントの所属プロジェクトとか、スタートアッププロジェクトは参照していない
        /// 複数のKuin Projectがあるslnだとうまく動かないような。
        /// </summary>
        /// <returns></returns>
        protected KuinProjectNodeProperties GetKuinProjectProperties()
        {
            var sln = (IVsSolution)_provider.GetService(typeof(SVsSolution));
            try
            {
                IEnumerable projects =
                    ((EnvDTE.DTE)_provider.GetService(typeof(EnvDTE.DTE)))
                    .ActiveSolutionProjects as IEnumerable;

                var project = projects?.OfType<EnvDTE.Project>()
                    .Select(p => p.GetCommonProject() as KuinProjectNode)
                    .FirstOrDefault(p => p != null);

                return (project?.NodeProperties as KuinProjectNodeProperties);
                    
            }
            catch (COMException)
            {
                return null;
            }

        }


        protected bool DoBuild(KuinProjectNodeProperties kuinProperties, bool isReleaseMode)
        {
            IVsOutputWindowPane outPane = GetOutputWindowPane();
            string commandLineArgs = kuinProperties?.CreateCommandLineArg(isReleaseMode);

            if (!Utilities.SaveDirtyFiles() || 
                kuinProperties == null || 
                outPane == null || 
                string.IsNullOrEmpty(commandLineArgs))
            {
                return false;
            }

            if (!File.Exists(KuinclPath))
            {
                MessageBox.Show(
                    "Kuin compiler (kuincl.exe) path was not found: please set in 'Tool > Options > KuinStudio > Compiler Path'", 
                    "Kuin Studio", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Exclamation);
                return false;
            }

            //ビルド
            using (var compileProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.Unicode,
                    WorkingDirectory = kuinProperties.ActualWorkingDirectory,
                    FileName = KuinclPath,
                    Arguments = commandLineArgs,
                }
            })
            {
                compileProcess.Start();
                compileProcess.WaitForExit();
                string res = string.Join("\n",
                    "Start " + (isReleaseMode ? "Release" : "Debug") + " Build",
                    "Command: " + KuinclPath + " " + commandLineArgs,
                    compileProcess.StandardOutput.ReadToEnd());
                outPane.Clear();
                outPane.OutputString(res);
                outPane.Activate();

                //kunncl output specification
                //0: success
                //non-0 (1): failure
                return compileProcess.ExitCode == 0;
            }
        }

        protected void RunProgram(KuinProjectNodeProperties kuinProperties)
        {
            string target = kuinProperties.BuildResultExePath;
            if (!File.Exists(target))
            {
                return;
            }

            //cui -> do program and show "press any key to continue...", so user can check the result
            //gui/web -> just do it!

            if (kuinProperties.ApplicationType == KuinProjectNodeProperties.AppTypeCui)
            {
                new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = Environment.GetEnvironmentVariable("ComSpec"),
                        Arguments = $"/c \"{target}\" & pause",
                        WorkingDirectory = kuinProperties.ActualWorkingDirectory,
                    }
                }.Start();
            }
            else
            {
                new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = target,
                        WorkingDirectory = kuinProperties.ActualWorkingDirectory,
                    }
                }.Start();
            }
        }
    }



    internal class DebugBuildCommand : KuinCommandBase
    {
        public DebugBuildCommand(IServiceProvider provider) : base(provider)
        {
        }

        public override int CommandId => KuinStudioPackage.DebugBuildCommandId;

        public override void DoCommand(object sender, EventArgs args)
        {
            var properties = GetKuinProjectProperties();
            if (properties != null)
            {
                DoBuild(properties, false);
            }
        }
    }

    internal class ReleaseBuildCommand : KuinCommandBase
    {
        public ReleaseBuildCommand(IServiceProvider provider) : base(provider)
        {
        }

        public override int CommandId => KuinStudioPackage.ReleaseBuildCommandId;

        public override void DoCommand(object sender, EventArgs args)
        {
            var properties = GetKuinProjectProperties();
            if (properties != null)
            {
                DoBuild(properties, true);
            }
        }
    }

    internal class DebugBuildAndRunCommand : KuinCommandBase
    {
        public DebugBuildAndRunCommand(IServiceProvider provider) : base(provider)
        {
        }

        public override int CommandId => KuinStudioPackage.DebugBuildAndRunCommandId;

        public override void DoCommand(object sender, EventArgs args)
        {
            var properties = GetKuinProjectProperties();
            if (properties != null)
            {
                if (DoBuild(properties, false))
                {
                    RunProgram(properties);
                }
            }
        }
    }

    internal class ReleaseBuildAndRunCommand : KuinCommandBase
    {
        public ReleaseBuildAndRunCommand(IServiceProvider provider) : base(provider)
        {

        }

        public override int CommandId => KuinStudioPackage.ReleaseBuildAndRunCommandId;

        public override void DoCommand(object sender, EventArgs args)
        {
            var properties = GetKuinProjectProperties();
            if (properties != null)
            {
                if (DoBuild(properties, true))
                {
                    RunProgram(properties);
                }
            }

        }
    }

}
