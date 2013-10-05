// Guids.cs
// MUST match guids.h
using System;

namespace Community.WorkRecorder
{
    static class GuidList
    {
        public const string guidRecorderPackageString = "78bdc9f1-2810-4189-9ca7-7af7bca5101b";
        public const string guidShowWorkRecorderString = "2d760672-e494-4717-9e74-460a3b247fee";

        public static readonly Guid guidShowWorkRecorder = new Guid(guidShowWorkRecorderString);

        public const string guidRecorderWindowString = "31e05ac0-f004-4ebe-bf3b-3c3bcb29f384";
    };
}