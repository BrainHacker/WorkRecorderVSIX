using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;

namespace Community.WorkRecorder
{
    [Export(typeof(IVsTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [ContentType("text")]
    internal sealed class VsTextViewCreationListener : IVsTextViewCreationListener
    {
        [Import(typeof(IVsEditorAdaptersFactoryService))]
        internal IVsEditorAdaptersFactoryService editorFactory = null;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView textView = editorFactory.GetWpfTextView(textViewAdapter);
            if (textView != null)
            {
                var filter = new CommandFilter(textViewAdapter, textView);
            }
        }
    }

    internal sealed class CommandFilter : IOleCommandTarget
    {
        private IWpfTextView textView;
        internal IOleCommandTarget nextFilter;
        bool listenMode;

        System.IO.TextWriter log;

        internal CommandFilter(IVsTextView textViewAdapter, IWpfTextView textView)
        {
            log = new System.IO.StreamWriter("work_log.txt");
            listenMode = true;

            this.textView = textView;
            textViewAdapter.AddCommandFilter(this, out nextFilter);

            ITextBuffer buffer = this.textView.TextBuffer;
            buffer.Changed += new EventHandler<TextContentChangedEventArgs>(bufferChanged);
        }

        void bufferChanged(object sender, TextContentChangedEventArgs args)
        {
            INormalizedTextChangeCollection changeCollection = args.Changes;
            foreach ( ITextChange change in changeCollection)
            {
                log.WriteLine("Delta = {0}", change.Delta);
                log.WriteLine("LineCountDelta = {0}", change.LineCountDelta);
                log.WriteLine("NewEnd = {0}", change.NewEnd);
                log.WriteLine("NewLength = {0}", change.NewLength);
                log.WriteLine("NewPosition = {0}", change.NewPosition);
                log.WriteLine("NewSpan = {0}", change.NewSpan);
                log.WriteLine("NewText = {0}", change.NewText);
                log.WriteLine("OldEnd = {0}", change.OldEnd);
                log.WriteLine("OldLength = {0}", change.OldLength);
                log.WriteLine("OldPosition = {0}", change.OldPosition);
                log.WriteLine("OldSpan = {0}", change.OldSpan);
                log.WriteLine("OldText = {0}", change.OldText);
                log.WriteLine("=====================================");
            }
            log.Flush();
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds,
            OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return nextFilter.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID,
            uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            //char typedChar = char.MinValue;

            ////if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            ////{
            ////    typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            ////    log.WriteByte((byte)typedChar);
            ////}

            //if (pguidCmdGroup == VSConstants.VSStd2K
            //    && nCmdID <= (uint)VSConstants.VSStd2KCmdID.END_EXT)
            //{
            //    if (listenMode)
            //    {
            //        if (pvaIn != IntPtr.Zero)
            //        {
            //            typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            //            log.WriteLine("cmdId = {0}, value = {1}", nCmdID, typedChar);
            //        }
            //        else
            //        {
            //            log.WriteLine("cmdId = {0}", nCmdID);
            //        }

            //        log.Flush();
            //    }
            //    else
            //    {
            //        // TODO: send to text editor
            //    }
            //}

            return nextFilter.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }
    }
}
