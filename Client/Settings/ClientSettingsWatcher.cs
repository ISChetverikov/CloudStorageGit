using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.File;
using Core.Settings;
using System.Threading.Tasks;
using System.IO;

namespace Client
{
    public class ClientSettingsWatcher : ISettingsWatcher
    {
        private readonly FileSystemWatcher _watcher;
        private readonly string _settingsPath;
        private ClientSettings _settings;
        
        public event ClientSettingsChangedHandler ClientSettingsChanged;

        public ClientSettingsWatcher(string settingsPath, ClientSettings settings)
        {
            var namesFilter = NotifyFilters.FileName;
            var notifyFilter = namesFilter | NotifyFilters.Size | NotifyFilters.Attributes | NotifyFilters.LastWrite |
                               NotifyFilters.CreationTime | NotifyFilters.Security;

            var dir = Path.GetDirectoryName(settingsPath);
            _watcher = new FileSystemWatcher(dir)
            {
                NotifyFilter = notifyFilter,
                Filter = Path.GetFileName(settingsPath)
            };
            
            _settingsPath = settingsPath;
            _settings = settings;
            _watcher.Changed += _watcher_Changed;
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var settings = ClientSettingsReader.Read(_settingsPath);
            if (_settings != settings)
                ClientSettingsChanged?.Invoke(this, new ClientSettingsChangedArgs(settings));
        }

        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
        }
    }
}
