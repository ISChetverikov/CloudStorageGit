using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.File
{
    public class FileAction
    {
        public string FilePath { get; }
        public FileActionType ActionType { get; }
        public string OldFilePath { get; }

        public FileAction(string filePath, FileActionType type)
        {
            ActionType = type;
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public FileAction(string filePath, FileActionType type, string oldFilePath) : this(filePath, type)
        {
            OldFilePath = oldFilePath;
        }
    }
    
}
