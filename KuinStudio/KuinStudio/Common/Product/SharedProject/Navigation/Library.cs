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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using VSConstants = Microsoft.VisualStudio.VSConstants;

namespace Microsoft.VisualStudioTools.Navigation {

    /// <summary>
    /// Implements a simple library that tracks project symbols, objects etc.
    /// </summary>
    sealed class Library : IVsSimpleLibrary2, IDisposable {
        private Guid _guid;
        private _LIB_FLAGS2 _capabilities;
        private readonly SemaphoreSlim _searching;
        private LibraryNode _root;
        private uint _updateCount;

        private enum UpdateType { Add, Remove }
        private readonly List<KeyValuePair<UpdateType, LibraryNode>> _updates;

        public Library(Guid libraryGuid) {
            _guid = libraryGuid;
            _root = new LibraryNode(null, String.Empty, String.Empty, LibraryNodeType.Package);
            _updates = new List<KeyValuePair<UpdateType, LibraryNode>>();
            _searching = new SemaphoreSlim(1);
        }

        public void Dispose() {
            _searching.Dispose();
        }

        public _LIB_FLAGS2 LibraryCapabilities {
            get { return _capabilities; }
            set { _capabilities = value; }
        }

        private void ApplyUpdates(bool assumeLockHeld) {
            if (!assumeLockHeld) {
                if (!_searching.Wait(0)) {
                    // Didn't get the lock immediately, which means we are
                    // currently searching. Once the search is done, updates
                    // will be applied.
                    return;
                }
            }

            try {
                lock (_updates) {
                    if (_updates.Count == 0) {
                        return;
                    }

                    // re-create root node here because we may have handed out
                    // the node before and don't want to mutate it's list.
                    _root = _root.Clone();
                    _updateCount += 1;
                    foreach (var kv in _updates) {
                        switch (kv.Key) {
                            case UpdateType.Add:
                                _root.AddNode(kv.Value);
                                break;
                            case UpdateType.Remove:
                                _root.RemoveNode(kv.Value);
                                break;
                            default:
                                Debug.Fail("Unsupported update type " + kv.Key.ToString());
                                break;
                        }
                    }
                    _updates.Clear();
                }
            } finally {
                if (!assumeLockHeld) {
                    _searching.Release();
                }
            }
        }

        internal async void AddNode(LibraryNode node) {
            lock (_updates) {
                _updates.Add(new KeyValuePair<UpdateType, LibraryNode>(UpdateType.Add, node));
            }
            ApplyUpdates(false);
        }

        internal void RemoveNode(LibraryNode node) {
            lock (_updates) {
                _updates.Add(new KeyValuePair<UpdateType, LibraryNode>(UpdateType.Remove, node));
            }
            ApplyUpdates(false);
        }

        #region IVsSimpleLibrary2 Members

        public int AddBrowseContainer(VSCOMPONENTSELECTORDATA[] pcdComponent, ref uint pgrfOptions, out string pbstrComponentAdded) {
            pbstrComponentAdded = null;
            return VSConstants.E_NOTIMPL;
        }

        public int CreateNavInfo(SYMBOL_DESCRIPTION_NODE[] rgSymbolNodes, uint ulcNodes, out IVsNavInfo ppNavInfo) {
            ppNavInfo = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetBrowseContainersForHierarchy(IVsHierarchy pHierarchy, uint celt, VSBROWSECONTAINER[] rgBrowseContainers, uint[] pcActual) {
            return VSConstants.E_NOTIMPL;
        }

        public int GetGuid(out Guid pguidLib) {
            pguidLib = _guid;
            return VSConstants.S_OK;
        }

        public int GetLibFlags2(out uint pgrfFlags) {
            pgrfFlags = (uint)LibraryCapabilities;
            return VSConstants.S_OK;
        }

        public int GetList2(uint ListType, uint flags, VSOBSEARCHCRITERIA2[] pobSrch, out IVsSimpleObjectList2 ppIVsSimpleObjectList2) {
            if ((flags & (uint)_LIB_LISTFLAGS.LLF_RESOURCEVIEW) != 0) {
                ppIVsSimpleObjectList2 = null;
                return VSConstants.E_NOTIMPL;
            }
            
            ICustomSearchListProvider listProvider;
            if (pobSrch != null &&
                pobSrch.Length > 0) {
                if ((listProvider = pobSrch[0].pIVsNavInfo as ICustomSearchListProvider) != null) {
                    switch ((_LIB_LISTTYPE)ListType) {
                        case _LIB_LISTTYPE.LLT_NAMESPACES:
                            ppIVsSimpleObjectList2 = listProvider.GetSearchList();
                            break;
                        default:
                            ppIVsSimpleObjectList2 = null;
                            return VSConstants.E_FAIL;
                    }
                } else {
                    if (pobSrch[0].eSrchType == VSOBSEARCHTYPE.SO_ENTIREWORD && ListType == (uint)_LIB_LISTTYPE.LLT_MEMBERS) {
                        string srchText = pobSrch[0].szName;
                        int colonIndex;
                        if ((colonIndex = srchText.LastIndexOf(':')) != -1) {
                            string filename = srchText.Substring(0, srchText.LastIndexOf(':'));
                            foreach (ProjectLibraryNode project in _root.Children) {
                                foreach (var item in project.Children) {
                                    if (item.FullName == filename) {
                                        ppIVsSimpleObjectList2 = item.DoSearch(pobSrch[0]);
                                        if (ppIVsSimpleObjectList2 != null) {
                                            return VSConstants.S_OK;
                                        }
                                    }
                                }
                            }
                        }

                        ppIVsSimpleObjectList2 = null;
                        return VSConstants.E_FAIL;
                    } else if (pobSrch[0].eSrchType == VSOBSEARCHTYPE.SO_SUBSTRING && ListType == (uint)_LIB_LISTTYPE.LLT_NAMESPACES) {
                        var lib = new LibraryNode(null, "Search results " + pobSrch[0].szName, "Search results " + pobSrch[0].szName, LibraryNodeType.Package);
                        foreach (var item in SearchNodes(pobSrch[0], new SimpleObjectList<LibraryNode>(), _root).Children) {
                            lib.Children.Add(item);
                        }
                        ppIVsSimpleObjectList2 = lib;
                        return VSConstants.S_OK;
                    } else if ((pobSrch[0].grfOptions & (uint)_VSOBSEARCHOPTIONS.VSOBSO_LOOKINREFS) != 0
                        && ListType == (uint)_LIB_LISTTYPE.LLT_HIERARCHY) {
                        LibraryNode node = pobSrch[0].pIVsNavInfo as LibraryNode;
                        if (node != null) {
                            var refs = node.FindReferences();
                            if (refs != null) {
                                ppIVsSimpleObjectList2 = refs;
                                return VSConstants.S_OK;
                            }
                        }
                    }
                    ppIVsSimpleObjectList2 = null;
                    return VSConstants.E_FAIL;
                }
            } else {
                ppIVsSimpleObjectList2 = _root as IVsSimpleObjectList2;
            }
            return VSConstants.S_OK;
        }

        private static SimpleObjectList<LibraryNode> SearchNodes(VSOBSEARCHCRITERIA2 srch, SimpleObjectList<LibraryNode> list, LibraryNode curNode) {
            foreach (var child in curNode.Children) {
                if (child.Name.IndexOf(srch.szName, StringComparison.OrdinalIgnoreCase) != -1) {
                    list.Children.Add(child.Clone(child.Name));
                }

                SearchNodes(srch, list, child);
            }
            return list;
        }

        internal async Task VisitNodesAsync(ILibraryNodeVisitor visitor, CancellationToken ct = default(CancellationToken)) {
            await _searching.WaitAsync(ct);
            try {
                await Task.Run(() => _root.Visit(visitor, ct));
                ApplyUpdates(true);
            } finally {
                _searching.Release();
            }
        }

        public int GetSeparatorStringWithOwnership(out string pbstrSeparator) {
            pbstrSeparator = ".";
            return VSConstants.S_OK;
        }

        public int GetSupportedCategoryFields2(int Category, out uint pgrfCatField) {
            pgrfCatField = (uint)_LIB_CATEGORY2.LC_HIERARCHYTYPE | (uint)_LIB_CATEGORY2.LC_PHYSICALCONTAINERTYPE;
            return VSConstants.S_OK;
        }

        public int LoadState(IStream pIStream, LIB_PERSISTTYPE lptType) {
            return VSConstants.S_OK;
        }

        public int RemoveBrowseContainer(uint dwReserved, string pszLibName) {
            return VSConstants.E_NOTIMPL;
        }

        public int SaveState(IStream pIStream, LIB_PERSISTTYPE lptType) {
            return VSConstants.S_OK;
        }

        public int UpdateCounter(out uint pCurUpdate) {
            pCurUpdate = _updateCount;
            return VSConstants.S_OK;
        }

        public void Update() {
            _updateCount++;
            _root.Update();
        }
        #endregion
    }
}
