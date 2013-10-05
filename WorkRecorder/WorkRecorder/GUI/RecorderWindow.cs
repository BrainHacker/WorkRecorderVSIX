using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Community.WorkRecorder.GUI
{
    [Guid(GuidList.guidRecorderWindowString)]
    public class RecorderWindow : ToolWindowPane
    {
        public RecorderWindow()
            : base(null)
        {
            this.Caption = Resources.ToolWindowTitle;

            base.Content = new RecorderControl();
        }
    }
}
