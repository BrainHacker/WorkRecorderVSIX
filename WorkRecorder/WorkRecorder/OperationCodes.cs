using System.IO;

namespace Community.WorkRecorder
{
    /// <summary>
    /// Enumeration of supported operations codes and they brief description
    /// </summary>
    internal enum OperationCode
    {
        // common operations

        /// <summary>
        /// Set cursor to specified position.
        /// <para>Format: operation code (1 byte), cursor position (4 bytes).</para>
        /// </summary>
        SetCursorPosition = 0,

        /// <summary>
        /// Normal typing.
        /// <para>Format: operation code (1 byte), string length (4 bytes), raw string data (char[]).</para>
        /// </summary>
        TypeString,
        /// <summary>
        /// TODO: Erasing string using Backspace key (opposite to TypeString).
        /// <para>Format: operation code (1 byte), string length (4 bytes), raw string data (char[]).</para>
        /// </summary>
        BackspaceString,

        /// <summary>
        /// TODO: Erasing string using Delete key.
        /// <para>Format: operation code (1 byte), string length (4 bytes), raw string data (char[]).</para>
        /// </summary>
        DeleteString,
        /// <summary>
        /// TODO: Type string without changing cursor position (opposite to DeleteString).
        /// <para>Format: operation code (1 byte), string length (4 bytes), raw string data (char[]).</para>
        /// </summary>
        PushString,

        // additional operations

        /// <summary>
        /// TODO: Sleep specified amount of time in ms before proceeding.
        /// <para>Format: operation code (1 byte), sleep time in ms (4 bytes).</para>
        /// </summary>
        SleepTime,
        /// <summary>
        /// TODO: Pause playback until user continue it.
        /// <para>Format: operation code (1 byte).</para>
        /// </summary>
        PausePlayback,

        /// <summary>
        /// Total amount of supported operations.
        /// </summary>
        TotalCount,

        /// <summary>
        /// TODO: Flag to be inserted between operations (for synchronization purpose).
        /// </summary>
        DelimiterFlag = 0xFF,
    };

    /// <summary>
    /// Abstract class, representing operation code
    /// </summary>
    internal abstract class OpCodeIface
    {
        public abstract OperationCode getOpCode();
        public abstract void serialize(BinaryWriter writer);
    }

    /// <summary>
    /// Class for OperationCode.SetCursorPosition
    /// </summary>
    internal class OpCodeSetCursorPosition : OpCodeIface
    {
        private uint position = 0;

        public OpCodeSetCursorPosition() { }
        public OpCodeSetCursorPosition(uint pos)
        {
            position = pos;
        }

        public override OperationCode getOpCode()
        {
            return OperationCode.SetCursorPosition;
        }

        public override void serialize(BinaryWriter writer)
        {
            writer.Write((byte)getOpCode());
            writer.Write(position);
        }
    };

    /// <summary>
    /// Common class for OperationCode(s) having string buffer inside
    /// </summary>
    internal abstract class OpCodeStringBased : OpCodeIface
    {
        private string buffer = "";

        public OpCodeStringBased() { }
        public OpCodeStringBased(string value)
        {
            buffer = value;
        }

        public abstract override OperationCode getOpCode();

        public override void serialize(BinaryWriter writer)
        {
            writer.Write((byte)getOpCode());
            writer.Write(buffer.Length);

            char[] symbols = buffer.ToCharArray();
            writer.Write(symbols);
        }
    };

    /// <summary>
    /// Class for OperationCode.TypeString
    /// </summary>
    internal class OpCodeTypeString : OpCodeStringBased
    {
        public OpCodeTypeString() : base() { }
        public OpCodeTypeString(string value) : base(value) { }

        public override OperationCode getOpCode()
        {
            return OperationCode.TypeString;
        }
    };

    /// <summary>
    /// Class for OperationCode.BackspaceString
    /// </summary>
    internal class OpCodeBackspaceString : OpCodeStringBased
    {
        public OpCodeBackspaceString() : base() { }
        public OpCodeBackspaceString(string value) : base(value) { }

        public override OperationCode getOpCode()
        {
            return OperationCode.BackspaceString;
        }
    };

    /// <summary>
    /// Class for OperationCode.DeleteString
    /// </summary>
    internal class OpCodeDeleteString : OpCodeStringBased
    {
        public OpCodeDeleteString() : base() { }
        public OpCodeDeleteString(string value) : base(value) { }

        public override OperationCode getOpCode()
        {
            return OperationCode.DeleteString;
        }
    };

    /// <summary>
    /// Class for OperationCode.PushString
    /// </summary>
    internal class OpCodePushString : OpCodeStringBased
    {
        public OpCodePushString() : base() { }
        public OpCodePushString(string value) : base(value) { }

        public override OperationCode getOpCode()
        {
            return OperationCode.PushString;
        }
    };

    /// <summary>
    /// Class for OperationCode.SleepTime
    /// </summary>
    internal class OpCodeSleepTime : OpCodeIface
    {
        private uint time = 0;

        public OpCodeSleepTime() { }
        /// <summary>
        /// Constructor with sleep time parameter
        /// </summary>
        /// <param name="sleepTime">sleep time in ms</param>
        public OpCodeSleepTime(uint sleepTime)
        {
            time = sleepTime;
        }

        public override OperationCode getOpCode()
        {
            return OperationCode.SleepTime;
        }

        public override void serialize(BinaryWriter writer)
        {
            writer.Write((byte)getOpCode());
            writer.Write(time);
        }
    };

    /// <summary>
    /// Common class for OperationCode(s) having only operation code inside
    /// </summary>
    internal abstract class OpCodeSimple : OpCodeIface
    {
        public OpCodeSimple() { }

        public abstract override OperationCode getOpCode();

        public override void serialize(BinaryWriter writer)
        {
            writer.Write((byte)getOpCode());
        }
    };

    /// <summary>
    /// Class for OperationCode.PausePlayback
    /// </summary>
    internal class OpCodePausePlayback : OpCodeSimple
    {
        public OpCodePausePlayback() : base() { }

        public override OperationCode getOpCode()
        {
            return OperationCode.PausePlayback;
        }
    };

    /// <summary>
    /// Class for OperationCode.DelimiterFlag
    /// </summary>
    internal class OpCodeDelimiterFlag : OpCodeSimple
    {
        public OpCodeDelimiterFlag() : base() { }

        public override OperationCode getOpCode()
        {
            return OperationCode.DelimiterFlag;
        }
    };

} //namespace Community.WorkRecorder