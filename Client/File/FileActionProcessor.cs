using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.File;
using Core.HttpSender;
using Core.Octodiff;
using System.IO;
using System.Collections.Concurrent;

namespace Client
{
    class FileActionProcessor : IFileActionProcessor
    {
        private readonly ITransportSender _httpSender;
        private readonly string _tempDirectory;
        private object _syncObject = new object();
        private ConcurrentDictionary<string, FileActionType> _filesInTheWork;

        public FileActionProcessor(ITransportSender httpSender, string tempDirectory)
        {
            _httpSender = httpSender;
            _tempDirectory = tempDirectory;
            _filesInTheWork = new ConcurrentDictionary<string, FileActionType>();
        }

        public void Process(FileAction fileAction)
        {

            lock (_syncObject)
            {
                switch (fileAction.ActionType)
                {
                    case FileActionType.Created:
                        Console.WriteLine($"Created {fileAction.FilePath}");
                        CreateRoutine(fileAction);
                        break;
                    case FileActionType.Deleted:
                        Console.WriteLine($"Deleted {fileAction.FilePath}");
                        _httpSender.SendDelete();
                        break;
                    case FileActionType.Changed:
                        Console.WriteLine($"Changed {fileAction.FilePath}");
                        ChangeRoutine(fileAction);
                        break;
                    case FileActionType.Renamed:
                        Console.WriteLine($"Renamed {fileAction.FilePath}");
                        _httpSender.SendRename();
                        break;
                    default:
                        break;
                }
            }
        }

        //private void AddAtWorkStatus(FileAction fileAction)
        //{
        //    while (!_filesInTheWork.TryAdd(fileAction.FilePath, fileAction.ActionType))
        //    {
        //        Task.Delay(1000);
        //    }
        //}

        //private bool IsOperationNeedToContinue(FileAction fileAction)
        //{
        //    if (!_filesInTheWork.ContainsKey(fileAction.FilePath))
        //        return true;

        //    FileActionType actionType;
        //    while (!_filesInTheWork.TryGetValue(FileAction.File));
        //    return true;
        //}

        //private void RemoveAtWorkStatus(FileAction fileAction)
        //{
        //    while (!_filesInTheWork.TryRemove(fileAction.FilePath, out FileActionType type))
        //    {
        //        Task.Delay(1000);
        //    }
        //}

        private void CreateRoutine(FileAction fileAction)
        {
            var filePath = fileAction.FilePath;

            var signatureFilename = OctodiffHelper.GetSignatureName(filePath);
            var signatureFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _tempDirectory, signatureFilename);

            if (File.Exists(signatureFilePath))
            {
                ChangeRoutine(new FileAction(filePath, FileActionType.Changed));
                return;
            }

            OctodiffHelper.BuildSignature(filePath, signatureFilePath);
            
            byte[] file;
            using (var SourceStream = File.Open(fileAction.FilePath, FileMode.Open))
            {
                file = new byte[SourceStream.Length];
                SourceStream.Read(file, 0, (int)SourceStream.Length);
            }

            _httpSender.SendCreate(file, filePath);
        }

        private void ChangeRoutine(FileAction fileAction)
        {
            var filePath = fileAction.FilePath;

            var deltaFilename = OctodiffHelper.GetDeltaName(filePath);
            var deltaFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _tempDirectory, deltaFilename);

            var signatureFilename = OctodiffHelper.GetSignatureName(filePath);
            var signatureFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _tempDirectory, signatureFilename);

            if (!File.Exists(signatureFilePath))
            {
                CreateRoutine(new FileAction(filePath, FileActionType.Created));
                return;
            }

            OctodiffHelper.CreateDelta(filePath, signatureFilePath, deltaFilePath);

            byte[] deltaFile;
            using (var SourceStream = File.Open(deltaFilePath, FileMode.Open))
            {
                deltaFile = new byte[SourceStream.Length];
                SourceStream.Read(deltaFile, 0, (int)SourceStream.Length);
            }

            _httpSender.SendChange(filePath, deltaFile);
        }
    }
}
