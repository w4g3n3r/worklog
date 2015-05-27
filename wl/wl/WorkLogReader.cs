using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wl
{
    public class WorkLogReader : StreamReader
    {
        private WorkLog nextLog = null;

        public WorkLogReader(Stream stream) : base(stream) {}
        public WorkLogReader(string path) : base(path) { }

        public WorkLog ReadWorkLog()
        {
            if (EndOfStream) throw new InvalidOperationException("Cant read past the end of the work log.");

            string line = base.ReadLine();
            long position = BaseStream.Position;

            var wl = WorkLog.FromString(line);
            
            return wl;
        }
    }
}
