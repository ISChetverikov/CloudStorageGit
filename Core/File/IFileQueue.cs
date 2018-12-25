using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.File
{
    public interface IFileQueue : IObservable<FileAction>
    {
        void Enqueue(FileAction file);
    }
}
