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

namespace Community.WorkRecorder
{
    class Recorder
    {
        private enum OperationType
        {
            SetPosition = 1,
            WriteString = 2,
        };

        private int pos = 0;

        private String buffer;
        private MemoryStream output;
        private BinaryWriter writer;

        public Recorder()
        {
            pos = 0;

            buffer = "";
            output = new MemoryStream();
            writer = new BinaryWriter(output);
        }

        ~Recorder()
        {
            writeString();
        }

        public void onTextChanged(ITextChange change)
        {
            int currentPos = change.NewPosition;
            
            if (currentPos != pos)
            {
                setNewCursorPosition(currentPos);
            }

            String newText = change.NewText;
            String oldText = change.OldText;

            if (oldText.Length == 0 && newText.Length != 0)
            {
                buffer += newText;
                pos    += newText.Length;
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

            writer.Write((byte)OperationType.SetPosition);
            writer.Write(currentPos);

            pos = currentPos;
        }

        private void writeString()
        {
            int length = buffer.Length;
            if (length != 0)
            {
                writer.Write((byte)OperationType.WriteString);
                writer.Write(length);

                char[] symbols = buffer.ToArray();
                writer.Write(symbols);

                buffer = "";
            }
        }
    }
}
