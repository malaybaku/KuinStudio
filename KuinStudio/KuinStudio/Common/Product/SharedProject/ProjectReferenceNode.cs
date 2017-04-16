// Visual Studio Shared Project
// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.Project {
    internal class ProjectReferenceNode : ReferenceNode {
        #region fields
        /// <summary>
        /// The name of the assembly this refernce represents
        /// </summary>
        private Guid referencedProjectGuid;

        private string referencedProjectName = String.Empty;

        private string referencedProjectRelativePath = String.Empty;

        private string referencedProjectFullPath = String.Empty;

        private BuildDependency buildDependency;

        /// <summary>
        /// This is a reference to the automation object for the referenced project.
        /// </summary>
        private EnvDTE.Project referencedProject;

        /// <summary>
        /// This state is controlled by the solution events.
        /// The state is set to false by OnBeforeUnloadProject.
        /// The state is set to true by OnBeforeCloseProject event.
        /// </summary>
        private bool canRemoveReference = true;

        /// <summary>
        /// Possibility for solution listener to update the state on the dangling reference.
        /// It will be set in OnBeforeUnloadProject then the nopde is invalidated then it is reset to false.
        /// </summary>
        private bool isNodeValid;

        #endregion

        #region properties

        public override string Url {
            get {
                return this.referencedProjectFullPath;
            }
        }

        public override string Caption {
            get {
                return this.referencedProjectName;
            }
        }

        internal Guid ReferencedProjectGuid {
            get {
                return this.referencedProjectGuid;
            }
        }

        internal string ReferencedProjectIdentity {
            get {
                var sln = (IVsSolution)ProjectMgr.GetService(typeof(SVsSolution));
                var guid = ReferencedProjectGuid;
                IVsHierarchy hier;
                string projRef;
                if (ErrorHandler.Succeeded(sln.GetProjectOfGuid(ref guid, out hier)) &&
                    ErrorHandler.Succeeded(sln.GetProjrefOfProject(hier, out projRef))) {
                    return projRef;
                }
                return null;
            }
        }

        /// <summary>
        /// Possiblity to shortcut and set the dangling project reference icon.
        /// It is ussually manipulated by solution listsneres who handle reference updates.
        /// </summary>
        internal protected bool IsNodeValid {
            get {
                return this.isNodeValid;
            }
            set {
                this.isNodeValid = value;
            }
        }

        /// <summary>
        /// Controls the state whether this reference can be removed or not. Think of the project unload scenario where the project reference should not be deleted.
        /// </summary>
        internal bool CanRemoveReference {
            get {
                return this.canRemoveReference;
            }
            set {
                this.canRemoveReference = value;
            }
        }

        internal string ReferencedProjectName {
            get { return this.referencedProjectName; }
        }

        /// <summary>
        /// Gets the automation object for the referenced project.
        /// </summary>
        internal EnvDTE.Project ReferencedProjectObject {
            get {
                // If the referenced project is null then re-read.
                if (this.referencedProject == null) {

                    // Search for the project in the collection of the projects in the
                    // current solution.
                    var dte = (EnvDTE.DTE)ProjectMgr.GetService(typeof(EnvDTE.DTE));
                    if (null == dte || null == dte.Solution) {
                        return null;
                    }
                    var unmodeled = new Guid(EnvDTE.Constants.vsProjectKindUnmodeled);
                    referencedProject = dte.Solution.Projects
                        .Cast<EnvDTE.Project>()
                        .Where(prj => !Utilities.GuidEquals(unmodeled, prj.Kind))
                        .FirstOrDefault(prj => CommonUtils.IsSamePath(referencedProjectFullPath, prj.FullName));
                }

                return this.referencedProject;
            }
            set {
                this.referencedProject = value;
            }
        }

        private static string GetFilenameFromOutput(IVsOutput2 output) {
            object propVal;
            int hr;
            try {
                hr = output.get_Property("OUTPUTLOC", out propVal);
            } catch (Exception ex) {
                hr = Marshal.GetHRForException(ex);
                propVal = null;
            }
            var path = propVal as string;
            if (ErrorHandler.Succeeded(hr) && !string.IsNullOrEmpty(path)) {
                return path;
            }

            ErrorHandler.ThrowOnFailure(output.get_DeploySourceURL(out path));
            return new Uri(path).LocalPath;
        }

        private static IEnumerable<string> EnumerateOutputs(IVsProjectCfg2 config, string canonicalName) {
            var actual = new uint[1];
            ErrorHandler.ThrowOnFailure(config.get_OutputGroups(0, null, actual));
            var groups = new IVsOutputGroup[actual[0]];
            ErrorHandler.ThrowOnFailure(config.get_OutputGroups((uint)groups.Length, groups, actual));

            var group = groups.FirstOrDefault(g => {
                string name;
                ErrorHandler.ThrowOnFailure(g.get_CanonicalName(out name));
                return canonicalName.Equals(name, StringComparison.OrdinalIgnoreCase);
            });
            if (group == null) {
                return Enumerable.Empty<string>();
            }

            string keyName;
            if (!ErrorHandler.Succeeded(group.get_KeyOutput(out keyName))) {
                keyName = null;
            }

            try {
                ErrorHandler.ThrowOnFailure(group.get_Outputs(0, null, actual));
            } catch (NotImplementedException) {
                if (CommonUtils.IsValidPath(keyName)) {
                    return Enumerable.Repeat(keyName, 1);
                }
                throw;
            }
            var outputs = new IVsOutput2[actual[0]];
            ErrorHandler.ThrowOnFailure(group.get_Outputs((uint)outputs.Length, outputs, actual));

            string keyResult = null;
            var results = new List<string>();

            foreach (var o in outputs) {
                string name;
                if (keyName != null &&
                    ErrorHandler.Succeeded(o.get_CanonicalName(out name)) &&
                    keyName.Equals(name, StringComparison.OrdinalIgnoreCase)
                ) {
                    keyResult = GetFilenameFromOutput(o);
                } else {
                    results.Add(GetFilenameFromOutput(o));
                }
            }

            if (keyResult != null) {
                results.Insert(0, keyResult);
            }
            return results;
        }

        internal virtual IEnumerable<string> ReferencedProjectBuildOutputs {
            get {
                var hier = VsShellUtilities.GetHierarchy(ProjectMgr.Site, ReferencedProjectGuid);
                var bldMgr = (IVsSolutionBuildManager)ProjectMgr.GetService(typeof(SVsSolutionBuildManager));
                var activeCfgArray = new IVsProjectCfg[1];
                ErrorHandler.ThrowOnFailure(bldMgr.FindActiveProjectCfg(IntPtr.Zero, IntPtr.Zero, hier, activeCfgArray));
                var activeCfg = activeCfgArray[0] as IVsProjectCfg2;
                if (activeCfg == null) {
                    throw new InvalidOperationException("cannot get active configuration");
                }

                return EnumerateOutputs(activeCfg, "Built");
            }
        }

        /// <summary>
        /// Gets the full path to the assembly generated by this project.
        /// </summary>
        internal virtual string ReferencedProjectOutputPath {
            get {
                return ReferencedProjectBuildOutputs.FirstOrDefault();
            }
        }

        internal string AssemblyName {
            get {
                // Now get the name of the assembly from the project.
                // Some project system throw if the property does not exist. We expect an ArgumentException.
                EnvDTE.Property assemblyNameProperty = null;
                if (ReferencedProjectObject != null &&
                    !(ReferencedProjectObject is Automation.OAProject)) // our own projects don't have assembly names
                {
                    try {
                        assemblyNameProperty = this.ReferencedProjectObject.Properties.Item(ProjectFileConstants.AssemblyName);
                    } catch (ArgumentException) {
                    }
                    if (assemblyNameProperty != null) {
                        return assemblyNameProperty.Value.ToString();
                    }
                }
                return null;
            }
        }

        private Automation.OAProjectReference projectReference;
        internal override object Object {
            get {
                if (null == projectReference) {
                    projectReference = new Automation.OAProjectReference(this);
                }
                return projectReference;
            }
        }
        #endregion

        #region ctors
        /// <summary>
        /// Constructor for the ReferenceNode. It is called when the project is reloaded, when the project element representing the refernce exists. 
        /// </summary>
        public ProjectReferenceNode(ProjectNode root, ProjectElement element)
            : base(root, element) {
            this.referencedProjectRelativePath = this.ItemNode.GetMetadata(ProjectFileConstants.Include);
            Debug.Assert(!String.IsNullOrEmpty(this.referencedProjectRelativePath), "Could not retrieve referenced project path form project file");

            string guidString = this.ItemNode.GetMetadata(ProjectFileConstants.Project);

            // Continue even if project setttings cannot be read.
            try {
                this.referencedProjectGuid = new Guid(guidString);

                this.buildDependency = new BuildDependency(this.ProjectMgr, this.referencedProjectGuid);
                this.ProjectMgr.AddBuildDependency(this.buildDependency);
            } finally {
                Debug.Assert(this.referencedProjectGuid != Guid.Empty, "Could not retrive referenced project guidproject file");

                this.referencedProjectName = this.ItemNode.GetMetadata(ProjectFileConstants.Name);

                Debug.Assert(!String.IsNullOrEmpty(this.referencedProjectName), "Could not retrive referenced project name form project file");
            }

            // TODO: Maybe referenced projects should be relative to ProjectDir?
            this.referencedProjectFullPath = CommonUtils.GetAbsoluteFilePath(this.ProjectMgr.ProjectHome, this.referencedProjectRelativePath);
        }

        /// <summary>
        /// constructor for the ProjectReferenceNode
        /// </summary>
        public ProjectReferenceNode(ProjectNode root, string referencedProjectName, string projectPath, string projectReference)
            : base(root) {
            Debug.Assert(root != null && !String.IsNullOrEmpty(referencedProjectName) && !String.IsNullOrEmpty(projectReference)
                && !String.IsNullOrEmpty(projectPath), "Can not add a reference because the input for adding one is invalid.");

            if (projectReference == null) {
                throw new ArgumentNullException("projectReference");
            }

            this.referencedProjectName = referencedProjectName;

            int indexOfSeparator = projectReference.IndexOf('|');


            string fileName = String.Empty;

            // Unfortunately we cannot use the path part of the projectReference string since it is not resolving correctly relative pathes.
            if (indexOfSeparator != -1) {
                string projectGuid = projectReference.Substring(0, indexOfSeparator);
                this.referencedProjectGuid = new Guid(projectGuid);
                if (indexOfSeparator + 1 < projectReference.Length) {
                    string remaining = projectReference.Substring(indexOfSeparator + 1);
                    indexOfSeparator = remaining.IndexOf('|');

                    if (indexOfSeparator == -1) {
                        fileName = remaining;
                    } else {
                        fileName = remaining.Substring(0, indexOfSeparator);
                    }
                }
            }

            Debug.Assert(!String.IsNullOrEmpty(fileName), "Can not add a project reference because the input for adding one is invalid.");

            string justTheFileName = Path.GetFileName(fileName);
            this.referencedProjectFullPath = CommonUtils.GetAbsoluteFilePath(projectPath, justTheFileName);
            // TODO: Maybe referenced projects should be relative to ProjectDir?
            this.referencedProjectRelativePath = CommonUtils.GetRelativeFilePath(this.ProjectMgr.ProjectHome, this.referencedProjectFullPath);

            this.buildDependency = new BuildDependency(this.ProjectMgr, this.referencedProjectGuid);

        }
        #endregion

        #region methods
        protected override NodeProperties CreatePropertiesObject() {
            return new ProjectReferencesProperties(this);
        }

        /// <summary>
        /// The node is added to the hierarchy and then updates the build dependency list.
        /// </summary>
        public override void AddReference() {
            if (this.ProjectMgr == null) {
                return;
            }
            base.AddReference();
            this.ProjectMgr.AddBuildDependency(this.buildDependency);
            return;
        }

        /// <summary>
        /// Overridden method. The method updates the build dependency list before removing the node from the hierarchy.
        /// </summary>
        public override bool Remove(bool removeFromStorage) {
            if (this.ProjectMgr == null || !this.CanRemoveReference) {
                return false;
            }
            this.ProjectMgr.RemoveBuildDependency(this.buildDependency);
            return base.Remove(removeFromStorage);
        }

        /// <summary>
        /// Links a reference node to the project file.
        /// </summary>
        protected override void BindReferenceData() {
            Debug.Assert(!String.IsNullOrEmpty(this.referencedProjectName), "The referencedProjectName field has not been initialized");
            Debug.Assert(this.referencedProjectGuid != Guid.Empty, "The referencedProjectName field has not been initialized");

            this.ItemNode = new MsBuildProjectElement(this.ProjectMgr, this.referencedProjectRelativePath, ProjectFileConstants.ProjectReference);

            this.ItemNode.SetMetadata(ProjectFileConstants.Name, this.referencedProjectName);
            this.ItemNode.SetMetadata(ProjectFileConstants.Project, this.referencedProjectGuid.ToString("B"));
            this.ItemNode.SetMetadata(ProjectFileConstants.Private, true.ToString());
        }

        /// <summary>
        /// Defines whether this node is valid node for painting the refererence icon.
        /// </summary>
        /// <returns></returns>
        protected override bool CanShowDefaultIcon() {
            if (this.referencedProjectGuid == Guid.Empty || this.ProjectMgr == null || this.ProjectMgr.IsClosed || this.isNodeValid) {
                return false;
            }

            IVsHierarchy hierarchy = null;

            hierarchy = VsShellUtilities.GetHierarchy(this.ProjectMgr.Site, this.referencedProjectGuid);

            if (hierarchy == null) {
                return false;
            }

            //If the Project is unloaded return false
            if (this.ReferencedProjectObject == null) {
                return false;
            }

            return File.Exists(this.referencedProjectFullPath);
        }

        /// <summary>
        /// Checks if a project reference can be added to the hierarchy. It calls base to see if the reference is not already there, then checks for circular references.
        /// </summary>
        /// <param name="errorHandler">The error handler delegate to return</param>
        /// <returns></returns>
        protected override bool CanAddReference(out CannotAddReferenceErrorMessage errorHandler) {
            // When this method is called this refererence has not yet been added to the hierarchy, only instantiated.
            if (!base.CanAddReference(out errorHandler)) {
                return false;
            }

            errorHandler = null;
            if (this.IsThisProjectReferenceInCycle()) {
                errorHandler = new CannotAddReferenceErrorMessage(ShowCircularReferenceErrorMessage);
                return false;
            }

            return true;
        }

        private bool IsThisProjectReferenceInCycle() {
            return IsReferenceInCycle(this.referencedProjectGuid);
        }

        private void ShowCircularReferenceErrorMessage() {
            string message = SR.GetString(SR.ProjectContainsCircularReferences, this.referencedProjectName);
            string title = string.Empty;
            OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
            OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
            OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
            VsShellUtilities.ShowMessageBox(this.ProjectMgr.Site, title, message, icon, buttons, defaultButton);
        }

        /// <summary>
        /// Recursively search if this project reference guid is in cycle.
        /// </summary>
        private bool IsReferenceInCycle(Guid projectGuid) {
            // TODO: This has got to be wrong, it doesn't work w/ other project types.
            IVsHierarchy hierarchy = VsShellUtilities.GetHierarchy(this.ProjectMgr.Site, projectGuid);

            IReferenceContainerProvider provider = hierarchy.GetProject().GetCommonProject() as IReferenceContainerProvider;
            if (provider != null) {
                IReferenceContainer referenceContainer = provider.GetReferenceContainer();

                Utilities.CheckNotNull(referenceContainer, "Could not found the References virtual node");

                foreach (ReferenceNode refNode in referenceContainer.EnumReferences()) {
                    ProjectReferenceNode projRefNode = refNode as ProjectReferenceNode;
                    if (projRefNode != null) {
                        if (projRefNode.ReferencedProjectGuid == this.ProjectMgr.ProjectIDGuid) {
                            return true;
                        }

                        if (this.IsReferenceInCycle(projRefNode.ReferencedProjectGuid)) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion
    }

}
