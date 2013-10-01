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
        /// Insert whole string.
        /// <para>Format: operation code (1 byte), string length (4 bytes), raw string data (char[]).</para>
        /// </summary>
        InsertString,

        /// <summary>
        /// Remove whole string.
        /// <para>Format: operation code (1 byte), string length (4 bytes), raw string data (char[]).</para>
        /// </summary>
        RemoveString,
        
        /// <summary>
        /// Normal typing.
        /// <para>Format: operation code (1 byte), string length (4 bytes), raw string data (char[]).</para>
        /// </summary>
        TypeString,
        /// <summary>
        /// TODO: Erasing string using Backspace key (opposite to TypeString).
        /// <para>Format: operation code (1 byte), string length (4 bytes), raw string data (char[]).</para>
        /// </summary>
        UntypeString,

        /// <summary>
        /// TODO: Erasing string using Delete key.
        /// <para>Format: operation code (1 byte), string length (4 bytes), raw string data (char[]).</para>
        /// </summary>
        PullString,
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
            writer.Write((byte)OperationCode.DelimiterFlag);
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

            writer.Write((byte)OperationCode.DelimiterFlag);
        }
    };

    /// <summary>
    /// Class for OperationCode.InsertString
    /// </summary>
    internal class OpCodeInsertString : OpCodeStringBased
    {
        public OpCodeInsertString() : base() { }
        public OpCodeInsertString(string value) : base(value) { }

        public override OperationCode getOpCode()
        {
            return OperationCode.InsertString;
        }
    };

    /// <summary>
    /// Class for OperationCode.RemoveString
    /// </summary>
    internal class OpCodeRemoveString : OpCodeStringBased
    {
        public OpCodeRemoveString() : base() { }
        public OpCodeRemoveString(string value) : base(value) { }

        public override OperationCode getOpCode()
        {
            return OperationCode.RemoveString;
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
    /// Class for OperationCode.UntypeString
    /// </summary>
    internal class OpCodeUntypeString : OpCodeStringBased
    {
        public OpCodeUntypeString() : base() { }
        public OpCodeUntypeString(string value) : base(value) { }

        public override OperationCode getOpCode()
        {
            return OperationCode.UntypeString;
        }
    };

    /// <summary>
    /// Class for OperationCode.PullString
    /// </summary>
    internal class OpCodePullString : OpCodeStringBased
    {
        public OpCodePullString() : base() { }
        public OpCodePullString(string value) : base(value) { }

        public override OperationCode getOpCode()
        {
            return OperationCode.PullString;
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
            writer.Write((byte)OperationCode.DelimiterFlag);
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
            writer.Write((byte)OperationCode.DelimiterFlag);
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

    // This is just flag, not operation
    // /// <summary>
    // /// Class for OperationCode.DelimiterFlag
    // /// </summary>
    // internal class OpCodeDelimiterFlag : OpCodeSimple
    // {
    //     public OpCodeDelimiterFlag() : base() { }
    // 
    //     public override OperationCode getOpCode()
    //     {
    //         return OperationCode.DelimiterFlag;
    //     }
    // };

} //namespace Community.WorkRecorder