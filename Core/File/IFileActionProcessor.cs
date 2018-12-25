using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.File
{
    public interface IFileActionProcessor
    {
        void Process(FileAction fileAction);

    }
}
