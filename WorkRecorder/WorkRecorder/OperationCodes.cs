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

} //namespace Community.WorkRecorder