//using Microsoft.VisualStudio;
//using Microsoft.VisualStudio.TextManager.Interop;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace Baku.KuinStudio
//{
//    [Guid(KuinStudioPackage.LanguageInfoGuidString)]
//    internal sealed class KuinLanguageInfo : IVsLanguageInfo, IVsLanguageDebugInfo
//    {
//        private readonly IServiceProvider _serviceProvider;

//        public KuinLanguageInfo(IServiceProvider serviceProvider)
//        {
//            _serviceProvider = serviceProvider;
//        }

//        public int GetCodeWindowManager(IVsCodeWindow pCodeWin, out IVsCodeWindowManager ppCodeWinMgr)
//        {
//            ppCodeWinMgr = null;
//            return VSConstants.E_FAIL;
//        }

//        public int GetFileExtensions(out string pbstrExtensions)
//        {
//            // This is the same extension the language service was
//            // registered as supporting.
//            pbstrExtensions = KuinFileSpecification.KuinFileExtension;
//            return VSConstants.S_OK;
//        }


//        public int GetLanguageName(out string bstrName)
//        {
//            // This is the same name the language service was registered with.
//            bstrName = KuinFileSpecification.KuinContentType;
//            return VSConstants.S_OK;
//        }

//        /// <summary>
//        /// GetColorizer is not implemented because we implement colorization using the new managed APIs.
//        /// </summary>
//        public int GetColorizer(IVsTextLines pBuffer, out IVsColorizer ppColorizer)
//        {
//            ppColorizer = null;
//            return VSConstants.E_FAIL;
//        }

//        public IServiceProvider ServiceProvider => _serviceProvider;

//        #region IVsLanguageDebugInfo Members

//        public int GetLanguageID(IVsTextBuffer pBuffer, int iLine, int iCol, out Guid pguidLanguageID)
//        {
//            pguidLanguageID = DebuggerConstants.guidLanguagePython;
//            return VSConstants.S_OK;
//        }

//        public int GetLocationOfName(string pszName, out string pbstrMkDoc, TextSpan[] pspanLocation)
//        {
//            pbstrMkDoc = null;
//            return VSConstants.E_FAIL;
//        }

//        public int GetNameOfLocation(IVsTextBuffer pBuffer, int iLine, int iCol, out string pbstrName, out int piLineOffset)
//        {
//            var model = _serviceProvider.GetService(typeof(SComponentModel)) as IComponentModel;
//            var service = model.GetService<IVsEditorAdaptersFactoryService>();
//            var entryService = model.GetService<AnalysisEntryService>();
//            var buffer = service.GetDataBuffer(pBuffer);
//            AnalysisEntry entry;
//            if (entryService != null && entryService.TryGetAnalysisEntry(buffer, out entry))
//            {
//                var location = entry.Analyzer.WaitForRequest(entry.Analyzer.GetNameOfLocationAsync(entry, buffer, iLine, iCol), "PythonLanguageInfo.GetNameOfLocation");
//                if (location != null)
//                {
//                    pbstrName = location.name;
//                    piLineOffset = location.lineOffset;

//                    return VSConstants.S_OK;
//                }
//            }

//            pbstrName = null;
//            piLineOffset = 0;
//            return VSConstants.E_FAIL;
//        }

//        /// <summary>
//        /// Called by debugger to get the list of expressions for the Autos debugger tool window.
//        /// </summary>
//        /// <remarks>
//        /// MSDN docs specify that <paramref name="iLine"/> and <paramref name="iCol"/> specify the beginning of the span,
//        /// but they actually specify the end of it (going <paramref name="cLines"/> lines back).
//        /// </remarks>
//        public int GetProximityExpressions(IVsTextBuffer pBuffer, int iLine, int iCol, int cLines, out IVsEnumBSTR ppEnum)
//        {
//            var model = _serviceProvider.GetService(typeof(SComponentModel)) as IComponentModel;
//            var service = model.GetService<IVsEditorAdaptersFactoryService>();
//            var entryService = model.GetService<AnalysisEntryService>();
//            var buffer = service.GetDataBuffer(pBuffer);
//            AnalysisEntry entry;
//            if (entryService != null && entryService.TryGetAnalysisEntry(buffer, out entry))
//            {
//                var names = entry.Analyzer.WaitForRequest(entry.Analyzer.GetProximityExpressionsAsync(entry, buffer, iLine, iCol, cLines), "PythonLanguageInfo.GetProximityExpressions");
//                ppEnum = new EnumBSTR(names);
//            }
//            else
//            {
//                ppEnum = new EnumBSTR(Enumerable.Empty<string>());
//            }
//            return VSConstants.S_OK;
//        }

//        public int IsMappedLocation(IVsTextBuffer pBuffer, int iLine, int iCol)
//        {
//            return VSConstants.E_FAIL;
//        }

//        public int ResolveName(string pszName, uint dwFlags, out IVsEnumDebugName ppNames)
//        {
//            /*if((((RESOLVENAMEFLAGS)dwFlags) & RESOLVENAMEFLAGS.RNF_BREAKPOINT) != 0) {
//                // TODO: This should go through the project/analysis and see if we can
//                // resolve the names...
//            }*/
//            ppNames = null;
//            return VSConstants.E_FAIL;
//        }

//        public int ValidateBreakpointLocation(IVsTextBuffer pBuffer, int iLine, int iCol, TextSpan[] pCodeSpan)
//        {
//            // per the docs, even if we don't indend to validate, we need to set the span info:
//            // http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.textmanager.interop.ivslanguagedebuginfo.validatebreakpointlocation.aspx
//            // 
//            // Caution
//            // Even if you do not intend to support the ValidateBreakpointLocation method but your 
//            // language does support breakpoints, you must implement this method and return a span 
//            // that contains the specified line and column; otherwise, breakpoints cannot be set 
//            // anywhere except line 1. You can return E_NOTIMPL to indicate that you do not otherwise 
//            // support this method bu6t the span must always be set. The example shows how this can be done.

//            // http://pytools.codeplex.com/workitem/787
//            // We were previously returning S_OK here indicating to VS that we have in fact validated
//            // the breakpoint.  Validating breakpoints actually interacts and effectively disables
//            // the "Highlight entire source line for breakpoints and current statement" option as instead
//            // VS highlights the validated region.  So we return E_NOTIMPL here to indicate that we have 
//            // not validated the breakpoint, and then VS will happily respect the option when we're in 
//            // design mode.
//            pCodeSpan[0].iStartLine = iLine;
//            pCodeSpan[0].iEndLine = iLine;
//            return VSConstants.E_NOTIMPL;
//        }

//        #endregion
//    }

//}
