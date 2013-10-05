using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using System.ComponentModel.Composition;
//using System.Runtime.InteropServices;
//using Microsoft.VisualStudio;
//using Microsoft.VisualStudio.Editor;
//using Microsoft.VisualStudio.Utilities;
using System.IO;
using Microsoft.VisualStudio.Text;
//using Microsoft.VisualStudio.Text.Editor;
//using Microsoft.VisualStudio.TextManager.Interop;
//using Microsoft.VisualStudio.OLE.Interop;

namespace Community.WorkRecorder.Engine
{
    class Recorder
    {
        private bool recordingInProgress = false;

        private int pos = 0;

        private string buffer;
        private MemoryStream output;
        private BinaryWriter writer;
        private string recordFullPath;

        public Recorder()
        {
        }

        ~Recorder()
        {
            if (recordingInProgress)
            {
                uninit();
            }
        }

        public void init(string text, string recordFullPath)
        {
            recordingInProgress = true;
            this.recordFullPath = recordFullPath;

            pos = 0;

            buffer = "";
            output = new MemoryStream();
            writer = new BinaryWriter(output);

            if (text.Length != 0)
            {
                OpCodeInsertString opCode = new OpCodeInsertString(text);
                opCode.serialize(writer);
            }
        }

        public void uninit()
        {
            recordingInProgress = false;

            var recordFile = new FileStream(recordFullPath, FileMode.OpenOrCreate, FileAccess.Write);
            recordFile.SetLength(0);

            writeString();
            flush(recordFile);

            recordFile.Flush();
        }

        public bool isRecording()
        {
            return recordingInProgress;
        }

        public void onTextChanged(ITextChange change)
        {
            if (recordingInProgress)
            {
                int currentPos = change.NewPosition;
                if (currentPos != pos)
                {
                    writeString();
                    setNewCursorPosition(currentPos);
                }

                String newText = change.NewText;
                String oldText = change.OldText;

                if (oldText.Length == 0)
                {
                    buffer += newText;
                    pos += newText.Length;

                    if (pos != change.NewEnd)
                    {
                        writeString();
                        setNewCursorPosition(change.NewEnd);
                    }
                }
            }
        }

        public void flush(Stream receiver)
        {
            // flush current buffer
            writeString();

            output.WriteTo(receiver);
            output.SetLength(0);
        }

        private void setNewCursorPosition(int currentPos)
        {
            // flush current buffer
            writeString();

            OpCodeSetCursorPosition opCode = new OpCodeSetCursorPosition((uint)currentPos);
            opCode.serialize(writer);

            pos = currentPos;
        }

        private void writeString()
        {
            int length = buffer.Length;
            if (length != 0)
            {
                OpCodeTypeString opCode = new OpCodeTypeString(buffer);
                opCode.serialize(writer);

                buffer = "";
            }
        }
    }
}
