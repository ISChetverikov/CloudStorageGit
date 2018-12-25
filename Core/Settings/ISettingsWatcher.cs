using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using System.Threading.Tasks;
using Core.File;

namespace Core.Settings
{
    public interface ISettingsWatcher : IWatcher
    {
        
        event ClientSettingsChangedHandler ClientSettingsChanged;
        
    }
}
