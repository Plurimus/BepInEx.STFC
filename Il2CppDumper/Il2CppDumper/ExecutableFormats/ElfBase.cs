using System;
using System.IO;

namespace Il2CppDumper
{
    public abstract class ElfBase : Il2Cpp
    {
        public bool IsDumped;
        public ulong DumpAddr;

        protected ElfBase(Stream stream, Action<string> reportProgressAction) : base(stream, reportProgressAction) { }

        public void GetDumpAddress()
        {
            reportProgressAction("Detected this may be a dump file.");
            reportProgressAction("Forcing continuation...");
        }
    }
}
