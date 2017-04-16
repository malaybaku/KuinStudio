//------------------------------------------------------------------------------
// <copyright file="KuinStudioPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudioTools;
using Microsoft.VisualStudioTools.Navigation;
using Baku.KuinStudio.Project;

namespace Baku.KuinStudio
{
    [Guid(KuinStudioPackage.PackageGuidString)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(KuinStudioOptionPage), "KuinStudio", "General", 0, 0, true)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class KuinStudioPackage : CommonPackage
    {
        public const string PackageGuidString = "e1594d31-d5e0-4d2a-bffd-5ead7e8364e6";
        public const string ProjectPackageGuidString = "0BCF5957-4BBF-44DB-911A-9D6A63E96A7F";

        public const string LanguageInfoGuidString = "D65B01B2-7D7C-4020-82B8-BE10527EF4EC";
        public const string LibraryManagerGuidString = "53138EE0-2B22-4B33-85E5-A92D3ACC45E3";
        public const string ProjectFactoryGuidString = "22D6ED26-FADE-4FF6-8477-591595FDED44";
        public const string ProjectNodePropertiesGuidString = "0CD7004B-87A1-4EB0-8978-742D50A18E6C";
        public const string ProjectNodeGuidString = "D19250E1-7CD8-4C95-8F8F-37A95477235C";

        public static readonly Guid KuinCommandSetGuid = new Guid("BAC84289-9DCE-45DD-8400-5A75AE4AC632");

        public const int DebugBuildCommandId = 0x4001;
        public const int ReleaseBuildCommandId = 0x4002;
        public const int DebugBuildAndRunCommandId = 0x4003;
        public const int ReleaseBuildAndRunCommandId = 0x4004;

        #region overrides

        public override bool IsRecognizedFile(string filename)
            => (!string.IsNullOrEmpty(filename)) && filename.EndsWith(KuinFileSpecification.KuinFileExtension);

        //use dummy to avoid NullReferenceException
        public override Type GetLibraryManagerType() => typeof(KuinLibraryManager);
        internal override LibraryManager CreateLibraryManager(CommonPackage package)
            => new KuinLibraryManager(package);

        protected override void Initialize()
        {
            base.Initialize();

            RegisterCommands(new Command[]
            {
                new DebugBuildCommand(this),
                new ReleaseBuildCommand(this),
                new DebugBuildAndRunCommand(this),
                new ReleaseBuildAndRunCommand(this),
            }, KuinCommandSetGuid);
        }

        #endregion
    }
}
