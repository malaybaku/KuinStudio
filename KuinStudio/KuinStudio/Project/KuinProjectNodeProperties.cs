using Microsoft.VisualStudioTools;
using Microsoft.VisualStudioTools.Infrastructure;
using Microsoft.VisualStudioTools.Project;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Baku.KuinStudio.Project
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid(KuinStudioPackage.ProjectNodePropertiesGuidString)]
    public class KuinProjectNodeProperties : CommonProjectNodeProperties
    {
        internal KuinProjectNodeProperties(ProjectNode node) : base(node)
        {
            _node = node;
        }
        private readonly ProjectNode _node;

        public string CreateCommandLineArg(bool isReleaseBuild)
        {
            var result = new StringBuilder();

            result.Append($"-i \"{StartupFilePath}\"");

            if (!string.IsNullOrEmpty(OutputFilePath))
            {
                result.Append($" -o \"{OutputFilePath}\"");
            }

            if (!string.IsNullOrEmpty(CustomSystemDirectory))
            {
                result.Append($" -s \"{CustomSystemDirectory}\"");
            }

            if (!string.IsNullOrEmpty(IconFilePath))
            {
                result.Append($" -c \"{IconFilePath}\"");
            }

            result.Append(" -e");
            result.Append(
                (ApplicationType == AppTypeGui ? " wnd" :
                 ApplicationType == AppTypeCui ? " cui" : 
                 ApplicationType == AppTypeWeb ? " web" :
                 " wnd"
                 ));

            if (isReleaseBuild) //(UseReleaseBuild == "True")
            {
                result.Append(" -r");
            }

            return result.ToString();
        }

        [Browsable(false)]
        public bool UseDefaultOutExe => string.IsNullOrEmpty(OutputFilePath);
        [Browsable(false)]
        public string DefaultOutExePath => File.Exists(StartupFilePath) ?
            Path.Combine(Path.GetDirectoryName(StartupFilePath), "out.exe") :
            "";

        [Browsable(false)]
        public string BuildResultExePath =>
            UseDefaultOutExe ? DefaultOutExePath : OutputFilePath;

        [Browsable(false)]
        public string ActualWorkingDirectory
        {
            get
            {
                try
                {
                    Path.GetFullPath(WorkingDirectory);
                    if (Path.IsPathRooted(WorkingDirectory))
                    {
                        return WorkingDirectory;
                    }
                    else if (Directory.Exists(Path.Combine(_node.FullPathToChildren, WorkingDirectory)))
                    {
                        return Path.Combine(_node.FullPathToChildren, WorkingDirectory);
                    }
                }
                catch(Exception)
                {
                }
                return _node.FullPathToChildren;
            }
        }

        [Browsable(false)]
        public string StartupFilePath =>
            string.IsNullOrEmpty(StartupFile) ? "" :
            Path.IsPathRooted(StartupFile) ? StartupFile :
            Path.Combine(_node.FullPathToChildren, StartupFile);

        [Browsable(false)]
        public string OutputFilePath => 
            string.IsNullOrEmpty(OutputFile) ? "" :
            Path.IsPathRooted(OutputFile) ? OutputFile :
            Path.Combine(_node.FullPathToChildren, OutputFile);

        [Browsable(false)]
        public string IconFilePath => 
            string.IsNullOrEmpty(IconFileName) ? "" :
            Path.IsPathRooted(IconFileName) ? IconFileName :
            Path.Combine(_node.FullPathToChildren, IconFileName);

        [Browsable(false)]
        public string ApplicationType
        {
            get
            {
                return Node.Site.GetUIThread().Invoke(() => {
                    return Node.ProjectMgr.GetProjectProperty(
                        KuinProjectResources.ApplicationTypePropertyName);
                });
            }
            set
            {
                Node.Site.GetUIThread().Invoke(() => {
                    this.Node.ProjectMgr.SetProjectProperty(
                        KuinProjectResources.ApplicationTypePropertyName, value);
                });
            }
        }

        /// <summary>
        /// NOTE: we do NOT use OutputFileName (but the reason is just wanted to use my own impl)
        /// </summary>
        [Browsable(false)]
        public string OutputFile
        {
            get
            {
                return Node.Site.GetUIThread().Invoke(() => {
                    try
                    {
                        var res = Node.ProjectMgr.GetProjectProperty(
                            KuinProjectResources.OutputFilePropertyName);
                        //if (res != null && !Path.IsPathRooted(res))
                        //{
                        //    res = CommonUtils.GetAbsoluteFilePath(Node.ProjectMgr.ProjectHome, res);
                        //}
                        return res;
                    }
                    catch (Exception ex) when (!ex.IsCriticalException())
                    {
                        Debug.Fail(ex.ToString());
                        return "(unknown)";
                    }
                });
            }
            set
            {
                Node.Site.GetUIThread().Invoke(() => {
                    this.Node.ProjectMgr.SetProjectProperty(
                        KuinProjectResources.OutputFilePropertyName,
                        CommonUtils.GetRelativeFilePath(
                            Node.ProjectMgr.ProjectHome,
                            Path.Combine(Node.ProjectMgr.ProjectHome, value)
                        )
                    );
                });
            }
        }

        [Browsable(false)]
        public string IconFileName
        {
            get
            {
                return Node.Site.GetUIThread().Invoke(() => {
                    try
                    {
                        return Node.ProjectMgr.GetProjectProperty(
                            KuinProjectResources.IconFilePropertyName);
                    }
                    catch (Exception ex) when (!ex.IsCriticalException())
                    {
                        Debug.Fail(ex.ToString());
                        return "(unknown)";
                    }
                });
            }
            set
            {
                Node.Site.GetUIThread().Invoke(() => {
                    this.Node.ProjectMgr.SetProjectProperty(KuinProjectResources.IconFilePropertyName, value);
                });
            }
        }

        [Browsable(false)]
        public string CustomSystemDirectory
        {
            get
            {
                return Node.Site.GetUIThread().Invoke(() => {
                    try
                    {
                        return Node.ProjectMgr.GetProjectProperty(
                            KuinProjectResources.CustomSystemDirectoryPropertyName);
                    }
                    catch (Exception ex) when (!ex.IsCriticalException())
                    {
                        Debug.Fail(ex.ToString());
                        return "(unknown)";
                    }
                });
            }
            set
            {
                Node.Site.GetUIThread().Invoke(() => {
                    this.Node.ProjectMgr.SetProjectProperty(
                        KuinProjectResources.CustomSystemDirectoryPropertyName, value);
                });
            }
        }

        //currently not used
        [Browsable(false)]
        public string UseReleaseBuild
        {
            get
            {
                return Node.Site.GetUIThread().Invoke(() => {
                    return Node.ProjectMgr.GetProjectProperty(
                        KuinProjectResources.UseReleaseBuildPropertyName);
                });
            }
            set
            {
                Node.Site.GetUIThread().Invoke(() => {
                    this.Node.ProjectMgr.SetProjectProperty(
                        KuinProjectResources.UseReleaseBuildPropertyName, value);
                });
            }
        }

        [Browsable(false)]
        public override VSLangProj.prjOutputType OutputType
            => VSLangProj.prjOutputType.prjOutputTypeExe;

        public static readonly string AppTypeCui = "Cui";
        public static readonly string AppTypeGui = "Gui";
        public static readonly string AppTypeWeb = "Web";
    }
}
