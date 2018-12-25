using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.File
{
    public enum FileActionType
    {
        Created = 1,
        Deleted = 2,
        Changed = 4,
        Renamed = 8
    }
}
