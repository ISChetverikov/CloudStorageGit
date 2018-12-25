using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.File
{
    public class FileWatcherEventArgs
    {
        public FileAction FileAction { get; set; }

        public FileWatcherEventArgs(FileAction fileAction)
        {
            FileAction = fileAction;
        }
    }
}
