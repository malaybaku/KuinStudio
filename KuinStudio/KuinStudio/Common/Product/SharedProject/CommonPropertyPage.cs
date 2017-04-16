﻿// Visual Studio Shared Project
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Build.Construction;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Base class for property pages based on a WinForm control.
    /// </summary>
    public abstract class CommonPropertyPage : IPropertyPage, IDisposable {
        private IPropertyPageSite _site;
        private bool _dirty, _loading;
        private CommonProjectNode _project;
        private bool _disposed;

        public void Dispose() {
            if (!_disposed) {
                Dispose(true);
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }

        ~CommonPropertyPage() {
            if (!_disposed) {
                Dispose(false);
                _disposed = true;
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                Project = null;
                var control = Control;
                if (control != null && !control.IsDisposed) {
                    control.Dispose();
                }
            }
        }

        public abstract Control Control {
            get;
        }

        public abstract void Apply();
        public abstract void LoadSettings();

        public abstract string Name {
            get;
        }

        internal virtual CommonProjectNode Project {
            get {
                return _project;
            }
            set {
                _project = value;
            }
        }

        internal virtual IEnumerable<CommonProjectConfig> SelectedConfigs {
            get; set;
        }

        protected void SetProjectProperty(string propertyName, string propertyValue) {
            // SetProjectProperty's implementation will check whether the value
            // has changed.
            Project.SetProjectProperty(propertyName, propertyValue);
        }

        protected string GetProjectProperty(string propertyName) {
            return Project.GetUnevaluatedProperty(propertyName);
        }

        protected void SetUserProjectProperty(string propertyName, string propertyValue) {
            // SetUserProjectProperty's implementation will check whether the value
            // has changed.
            Project.SetUserProjectProperty(propertyName, propertyValue);
        }

        protected string GetUserProjectProperty(string propertyName) {
            return Project.GetUserProjectProperty(propertyName);
        }

        protected string GetConfigUserProjectProperty(string propertyName) {
            if (SelectedConfigs == null) {
                string condition = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    ConfigProvider.configPlatformString,
                    Project.CurrentConfig.GetPropertyValue("Configuration"),
                    Project.CurrentConfig.GetPropertyValue("Platform"));

                return GetUserPropertyUnderCondition(propertyName, condition);
            } else {
                StringCollection values = new StringCollection();

                foreach (var config in SelectedConfigs) {
                    string condition = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        ConfigProvider.configPlatformString,
                        config.ConfigName,
                        config.PlatformName);

                    values.Add(GetUserPropertyUnderCondition(propertyName, condition));
                }

                switch (values.Count) {
                case 0:
                    return null;
                case 1:
                    return values[0];
                default:
                    return "<different values>";
                }
            }
        }

        protected void SetConfigUserProjectProperty(string propertyName, string propertyValue) {
            if (SelectedConfigs == null) {
                string condition = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    ConfigProvider.configPlatformString,
                    Project.CurrentConfig.GetPropertyValue("Configuration"),
                    Project.CurrentConfig.GetPropertyValue("Platform"));

                SetUserPropertyUnderCondition(propertyName, propertyValue, condition);
            } else {
                foreach (var config in SelectedConfigs) {
                    string condition = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        ConfigProvider.configPlatformString,
                        config.ConfigName,
                        config.GetConfigurationProperty("Platform", false));

                    SetUserPropertyUnderCondition(propertyName, propertyValue, condition);
                }
            }
        }

        private string GetUserPropertyUnderCondition(string propertyName, string condition) {
            string conditionTrimmed = (condition == null) ? String.Empty : condition.Trim();

            if (Project.UserBuildProject != null) {
                if (conditionTrimmed.Length == 0) {
                    return Project.UserBuildProject.GetProperty(propertyName).UnevaluatedValue;
                }

                // New OM doesn't have a convenient equivalent for setting a property with a particular property group condition. 
                // So do it ourselves.
                ProjectPropertyGroupElement matchingGroup = null;

                foreach (ProjectPropertyGroupElement group in Project.UserBuildProject.Xml.PropertyGroups) {
                    if (String.Equals(group.Condition.Trim(), conditionTrimmed, StringComparison.OrdinalIgnoreCase)) {
                        matchingGroup = group;
                        break;
                    }
                }

                if (matchingGroup != null) {
                    foreach (ProjectPropertyElement property in matchingGroup.PropertiesReversed) // If there's dupes, pick the last one so we win
                    {
                        if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase)
                            && (property.Condition == null || property.Condition.Length == 0)) {
                            return property.Value;
                        }
                    }
                }

            }

            return null;
        }

        /// <summary>
        /// Emulates the behavior of SetProperty(name, value, condition) on the old MSBuild object model.
        /// This finds a property group with the specified condition (or creates one if necessary) then sets the property in there.
        /// </summary>
        private void SetUserPropertyUnderCondition(string propertyName, string propertyValue, string condition) {
            string conditionTrimmed = (condition == null) ? String.Empty : condition.Trim();
            const string userProjectCreateProperty = "UserProject";

            if (Project.UserBuildProject == null) {
                Project.SetUserProjectProperty(userProjectCreateProperty, null);
            }

            if (conditionTrimmed.Length == 0) {
                var userProp = Project.UserBuildProject.GetProperty(userProjectCreateProperty);
                if (userProp != null) {
                    Project.UserBuildProject.RemoveProperty(userProp);
                }
                Project.UserBuildProject.SetProperty(propertyName, propertyValue);
                return;
            }

            // New OM doesn't have a convenient equivalent for setting a property with a particular property group condition. 
            // So do it ourselves.
            ProjectPropertyGroupElement newGroup = null;

            foreach (ProjectPropertyGroupElement group in Project.UserBuildProject.Xml.PropertyGroups) {
                if (String.Equals(group.Condition.Trim(), conditionTrimmed, StringComparison.OrdinalIgnoreCase)) {
                    newGroup = group;
                    break;
                }
            }

            if (newGroup == null) {
                newGroup = Project.UserBuildProject.Xml.AddPropertyGroup(); // Adds after last existing PG, else at start of project
                newGroup.Condition = condition;
            }

            foreach (ProjectPropertyElement property in newGroup.PropertiesReversed) // If there's dupes, pick the last one so we win
            {
                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase) 
                    && (property.Condition == null || property.Condition.Length == 0)) {
                    property.Value = propertyValue;
                    return;
                }
            }

            newGroup.AddProperty(propertyName, propertyValue);
        }

        public bool Loading {
            get {
                return _loading;
            }
            set {
                _loading = value;
            }
        }

        public bool IsDirty {
            get {
                return _dirty;
            }
            set {
                if (_dirty != value && !Loading) {
                    _dirty = value;
                    if (_site != null) {
                        _site.OnStatusChange((uint)(_dirty ? PropPageStatus.Dirty : PropPageStatus.Clean));
                    }
                }
            }
        }

        void IPropertyPage.Activate(IntPtr hWndParent, RECT[] pRect, int bModal) {
            var control = Control;
            Debug.Assert(control != null, "Cannot activate property page with no control");
            Debug.Assert(!control.IsDisposed, "Cannot reactivate property page");
            NativeMethods.SetParent(control.Handle, hWndParent);
        }

        int IPropertyPage.Apply() {
            // We're letting exceptions fall through, so VS can handle them
            Apply();
            return VSConstants.S_OK;
        }

        void IPropertyPage.Deactivate() {
            Dispose();
        }

        void IPropertyPage.GetPageInfo(PROPPAGEINFO[] pPageInfo) {
            Utilities.ArgumentNotNull("pPageInfo", pPageInfo);

            PROPPAGEINFO info = new PROPPAGEINFO();

            info.cb = (uint)Marshal.SizeOf(typeof(PROPPAGEINFO));
            info.dwHelpContext = 0;
            info.pszDocString = null;
            info.pszHelpFile = null;
            info.pszTitle = Name;
            info.SIZE.cx = Control.Width;
            info.SIZE.cy = Control.Height;
            pPageInfo[0] = info;
        }

        void IPropertyPage.Help(string pszHelpDir) {
        }

        int IPropertyPage.IsPageDirty() {
            return (IsDirty ? (int)VSConstants.S_OK : (int)VSConstants.S_FALSE);
        }

        void IPropertyPage.Move(RECT[] pRect) {
            Utilities.ArgumentNotNull("pRect", pRect);

            RECT r = pRect[0];

            Control.Location = new Point(r.left, r.top);
            Control.Size = new Size(r.right - r.left, r.bottom - r.top);
        }

        public virtual void SetObjects(uint count, object[] punk) {
            if (punk == null) {
                return;
            }

            if (count > 0) {
                if (punk[0] is ProjectConfig) {
                    if (_project == null) {
                        _project = (CommonProjectNode)((CommonProjectConfig)punk.First()).ProjectMgr;
                    }

                    var configs = new List<CommonProjectConfig>();

                    for (int i = 0; i < count; i++) {
                        CommonProjectConfig config = (CommonProjectConfig)punk[i];

                        configs.Add(config);
                    }

                    SelectedConfigs = configs;
                } else if (punk[0] is NodeProperties) {
                    if (_project == null) {
                        Project = (CommonProjectNode)(punk[0] as NodeProperties).HierarchyNode.ProjectMgr;
                    }
                }
            } else {
                Project = null;
            }

            if (_project != null) {
                LoadSettings();
            }
        }

        void IPropertyPage.SetPageSite(IPropertyPageSite pPageSite) {
            _site = pPageSite;
        }

        void IPropertyPage.Show(uint nCmdShow) {
            Control.Visible = true; // TODO: pass SW_SHOW* flags through      
            Control.Show();
        }

        int IPropertyPage.TranslateAccelerator(MSG[] pMsg) {
            Utilities.ArgumentNotNull("pMsg", pMsg);

            MSG msg = pMsg[0];

            if ((msg.message < NativeMethods.WM_KEYFIRST || msg.message > NativeMethods.WM_KEYLAST) && (msg.message < NativeMethods.WM_MOUSEFIRST || msg.message > NativeMethods.WM_MOUSELAST)) {
                return VSConstants.S_FALSE;
            }

            return (NativeMethods.IsDialogMessageA(Control.Handle, ref msg)) ? VSConstants.S_OK : VSConstants.S_FALSE;
        }
    }
}
