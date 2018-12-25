using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Core.File;
using Core.Settings;
using Core;
using Serilog;
using System.IO;
using Newtonsoft.Json;
using Core.HttpSender;

namespace Client
{
    public class Client : IClient
    {
        private const string _settingsFilename = "settings.json";
        private ClientSettings _settings;

        private readonly ILogger _logger;
        private readonly IWatcher _fileWatcher;
        private readonly ISettingsWatcher _settingsWatcher;

        public Client()
        {
            _settings = LoadSettings();
            var container = BuildService(_settings);

            _logger = container.Resolve<ILogger>();
            

            var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _settingsFilename);
            _settingsWatcher = container.Resolve<ISettingsWatcher>(
                new NamedParameter("settingsPath", settingsPath)
            );
            _settingsWatcher.ClientSettingsChanged += _settingsWatcher_ClientSettingsChanged;

            var inputFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _settings.InputFolder);
            Console.WriteLine(inputFolderPath);
            _fileWatcher = container.Resolve<IWatcher>(new NamedParameter("inputFolder", inputFolderPath));

            var url = _settings.ServerAddress;
            container.Resolve<ITransportSender>(new NamedParameter("url", url));

            var tempDirectory = _settings.TempFolder;
            container.Resolve<IFileActionProcessor>(new NamedParameter("tempDirectory", tempDirectory))
                ;
            var queue = container.Resolve<IFileQueue>();
            var listener = container.Resolve<IFileQueueListener>();
            queue.Subscribe(listener);
        }

        private void _settingsWatcher_ClientSettingsChanged(object sender, ClientSettingsChangedArgs e)
        {
            _logger.Information("Settings has changed");
            _settings = e.Settings;
        }

        public void Start()
        {
            _fileWatcher.Start();
            _settingsWatcher.Start();
            _logger.Information("Client started.");
        }

        public void Stop()
        {
            _settingsWatcher.Stop();
            _fileWatcher.Stop();
            _logger.Information("Client stopped.");
        }

        public void SendMessage(string message)
        {
            _logger.Information("Message: {message}", message);
        }

        private static IContainer BuildService(ClientSettings settings)
        {
            var builder = new ContainerBuilder();

            var logger = CreateLogger(settings);
            builder.RegisterInstance(logger);
    
            builder.RegisterInstance(settings);

            builder.RegisterType<ClientSettingsWatcher>().As<ISettingsWatcher>().SingleInstance();
            builder.RegisterType<FileWatcher>().As<IWatcher>().SingleInstance();
            builder.RegisterType<HttpSender>().As<ITransportSender>().SingleInstance();
            builder.RegisterType<FileQueue>().As<IFileQueue>().SingleInstance();
            builder.RegisterType<FileQueueListener>().As<IFileQueueListener>().SingleInstance();
            builder.RegisterType<FileActionProcessor>().As<IFileActionProcessor>().SingleInstance();
            builder.RegisterType<FileWatcher>().As<IWatcher>().SingleInstance();

            return builder.Build();
        }

        private static ILogger CreateLogger(ClientSettings settings)
        {
          
            var logPathFormat = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settings.LogFolder, @"log-{Date}.log");
            return new LoggerConfiguration()
                .WriteTo.RollingFile(logPathFormat)
                .CreateLogger();
        }

        private static ClientSettings LoadSettings()
        {
            return ClientSettingsReader.Read(_settingsFilename);
        }
    }
}
