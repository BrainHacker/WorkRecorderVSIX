using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;

namespace Community.WorkRecorder.Engine
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
                new CommandFilter(textViewAdapter, textView);
            }
        }
    }

    internal sealed class CommandFilter : IOleCommandTarget
    {
        private IWpfTextView textView;
        internal IOleCommandTarget nextFilter;
        
        private bool listening = false;

        private Recorder recorder;
        private string recordFullPath = "";

        internal CommandFilter(IVsTextView textViewAdapter, IWpfTextView textView)
        {
            recorder = new Recorder();
            listening = false;

            this.textView = textView;
            textViewAdapter.AddCommandFilter(this, out nextFilter);

            // trying to get document path from TextBuffer
            ITextBuffer buffer = this.textView.TextBuffer;
            ITextDocument document;

            var result = buffer.Properties.TryGetProperty<ITextDocument>(
                typeof(ITextDocument), out document);

            if (result)
            {
                string documentFullPath = document.FilePath;
                recordFullPath = documentFullPath + ".rec";
            }
            else
            {
                // TODO: save to some folder
                recordFullPath = "document.rec";
            }

            // subscribe to RecorderControl event
            RecorderControl.RecordStateChanged += new EventHandler<RecordStateChangedArgs>(OnRecordStateChanged);
        }

        ~CommandFilter()
        {
        }

        public void OnRecordStateChanged(object sender, RecordStateChangedArgs args)
        {
            bool recording = args.Recording;
            if (recording)
            {
                startListening();
                initRecorder();
            }
            else
            {
                uninitRecorder();
                stopListening();
            }
        }

        private void startListening()
        {
            if (!listening)
            {
                listening = true;

                ITextBuffer buffer = textView.TextBuffer;
                buffer.Changed += new EventHandler<TextContentChangedEventArgs>(bufferChanged);
            }
        }

        private void stopListening()
        {
            if (listening)
            {
                listening = false;

                ITextBuffer buffer = textView.TextBuffer;
                buffer.Changed -= new EventHandler<TextContentChangedEventArgs>(bufferChanged);
            }
        }

        private void initRecorder()
        {
            uninitRecorder();

            ITextBuffer buffer = textView.TextBuffer;
            ITextSnapshot snapshot = buffer.CurrentSnapshot;

            string currentText = snapshot.GetText();
            recorder.init(currentText, recordFullPath);
        }

        private void uninitRecorder()
        {
            if (recorder.isRecording())
            {
                recorder.uninit();
            }
        }

        private void bufferChanged(object sender, TextContentChangedEventArgs args)
        {
            INormalizedTextChangeCollection changeCollection = args.Changes;
            foreach ( ITextChange change in changeCollection)
            {
                recorder.onTextChanged(change);
            }
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds,
            OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return nextFilter.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID,
            uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            return nextFilter.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }
    }
}
