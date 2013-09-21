// Guids.cs
// MUST match guids.h
using System;

namespace Community.WorkRecorder
{
    static class GuidList
    {
        public const string guidWorkRecorderPkgString = "7755ccf9-3c67-4037-ba98-7e7d6e796e76";
        public const string guidWorkRecorderCmdSetString = "5cf0ee0a-c426-4bd6-a919-d390301289dc";
        public const string guidToolWindowPersistanceString = "1ab66290-52cc-4bc1-9959-3ce317a7416d";

        public static readonly Guid guidWorkRecorderCmdSet = new Guid(guidWorkRecorderCmdSetString);
    };
}