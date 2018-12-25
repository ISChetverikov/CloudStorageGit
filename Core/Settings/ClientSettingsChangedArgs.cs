using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Settings
{
    public class ClientSettingsChangedArgs : EventArgs
    {
        public ClientSettings Settings { get; set; }

        public ClientSettingsChangedArgs(ClientSettings settings)
        {
            Settings = settings;
        }
    }
}
