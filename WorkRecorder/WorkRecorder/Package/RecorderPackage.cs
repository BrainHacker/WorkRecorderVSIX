//using System;
////using System.Diagnostics;
////using System.Globalization;
//using System.Runtime.InteropServices;
//using System.ComponentModel.Design;
////using Microsoft.Win32;
////using Microsoft.VisualStudio;
//using Microsoft.VisualStudio.Shell.Interop;
////using Microsoft.VisualStudio.OLE.Interop;
//using Microsoft.VisualStudio.Shell;

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace Community.WorkRecorder.VsPackage
{
    /// <summary>
    /// This is the class that implements the Work Recorder package exposed by this assembly.
    /// Implements the IVsPackage interface and registers itself with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#111", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(GUI.RecorderWindow))]
    [Guid(GuidList.guidRecorderPackageString)]
    public sealed class RecorderPackage : Package
    {
        public RecorderPackage()
        {
        }

        private void ShowWorkRecorderWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(GUI.RecorderWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        /////////////////////////////////////////////////////////////////////////////
        // Override Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Add command handlers for menu (commands from the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs != null)
            {
                CommandID menuCommandID = new CommandID(GuidList.guidShowWorkRecorder,
                    (int)PackageCommandIDs.cmdidShowWorkRecorderWindowCommand);

                MenuCommand menuItem = new MenuCommand(ShowWorkRecorderWindow, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }
        #endregion
        // Override Package Implementation
        /////////////////////////////////////////////////////////////////////////////

    }
}
