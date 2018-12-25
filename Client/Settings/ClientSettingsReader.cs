using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Settings;
using Newtonsoft.Json;

namespace Client
{
    public static class ClientSettingsReader 
    {
        public static ClientSettings Read(string settingsPath)
        {
            //var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            var json = File.ReadAllText(settingsPath);
            return JsonConvert.DeserializeObject<ClientSettings>(json);
        }
    }
}
