using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.File;
using Core.Settings;

namespace Client
{
    public class FileWatcher : IWatcher
    {

        private readonly FileSystemWatcher _watcher;
        private readonly IFileQueue _queue;

        private object _syncObject;
        private ConcurrentDictionary<string, DateTime> _filesInTheWork;

        public FileWatcher(string inputFolder, IFileQueue queue)
        {
            var namesFilter = NotifyFilters.FileName;
            var notifyFilter = namesFilter | NotifyFilters.Size | NotifyFilters.Attributes | NotifyFilters.LastWrite |
                               NotifyFilters.CreationTime | NotifyFilters.Security;

            _watcher = new FileSystemWatcher(inputFolder)
            {
                NotifyFilter = notifyFilter
            };
            _queue = queue;
            _syncObject = new object();
            _filesInTheWork = new ConcurrentDictionary<string, DateTime>();

            _watcher.Changed += CreatedAndChangedHandler;
            _watcher.Created += CreatedAndChangedHandler;
            _watcher.Deleted += DeletedHandler;
            _watcher.Renamed += RenamedHandler;
        }

        private void RenamedHandler(object sender, RenamedEventArgs e)
        {
            _queue.Enqueue(
                    new FileAction(e.FullPath, FileActionTypesConverter.Convert(e.ChangeType), e.OldFullPath)
                    );
        }

        private void DeletedHandler(object sender, FileSystemEventArgs e)
        {
            _queue.Enqueue(new FileAction(e.FullPath, FileActionTypesConverter.Convert(e.ChangeType)));
        }

        private void CreatedAndChangedHandler(object sender, FileSystemEventArgs e)
        {
            if (!_filesInTheWork.ContainsKey(e.FullPath))
            {
                while (!_filesInTheWork.TryAdd(e.FullPath, DateTime.Now));
                _queue.Enqueue(new FileAction(e.FullPath, FileActionTypesConverter.Convert(e.ChangeType)));
                return;
            }

            DateTime datetime;
            while(!_filesInTheWork.TryGetValue(e.FullPath, out datetime));
            _filesInTheWork.AddOrUpdate(e.FullPath, DateTime.Now, (path, date) => date);
            if (DateTime.Now.Subtract(datetime).Seconds < 2)
                return;

            _queue.Enqueue(new FileAction(e.FullPath, FileActionTypesConverter.Convert(e.ChangeType)));
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
