using HttpMultipartParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Octodiff;

namespace Server
{
    public class SyncServer
    {
        string[] _prefixes = { "http://localhost:4567/" };
        string _syncDir = @"Sync\";
        string _tmpDir = @"Temp\";
        HttpListener _listener;
        
        public SyncServer()
        {
            _listener = new HttpListener();
        }

        public void Start()
        {
            
            foreach (string s in _prefixes)
            {
                _listener.Prefixes.Add(s);
            }
            _listener.Start();
            Console.WriteLine("Listening...");
            
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var contextOut = _listener.GetContext();
                    Task.Factory.StartNew((contextObj) =>
                    {
                        if (!(contextObj is HttpListenerContext context))
                            return;

                        ProccesRequest(context.Request);

                    }, contextOut, TaskCreationOptions.LongRunning);
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            _listener.Stop();
            return;
        }

        private void ProccesRequest(HttpListenerRequest request)
        {
            Console.WriteLine("I have an request: " + request.ContentType);

            var parser = new MultipartFormDataParser(request.InputStream, Encoding.UTF8);

            string method = parser.Parameters[0].Data;
            
            var file = parser.Files.First();
            var filename = file.FileName;
            Stream data = file.Data;

            switch (method)
            {
                case "Create":
                    ProcessCreate(filename, data);
                    break;
                case "Change":
                    ProcessChange(filename, data);
                    break;
                default:
                    break;
            }
            
        }

        private void ProcessChange(string filename, Stream data)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _syncDir, filename);

            var deltaFilename = OctodiffHelper.GetDeltaName(filePath);
            var deltaFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _tmpDir, filename);

            using (var fileStream = File.Create(deltaFilePath))
            {
                data.Seek(0, SeekOrigin.Begin);
                data.CopyTo(fileStream);
            }

            var signatureFilename = OctodiffHelper.GetSignatureName(filePath);
            var signaturePath = Path.Combine(_tmpDir, signatureFilename);

            OctodiffHelper.ApplyDelta(filePath, signaturePath, deltaFilePath);
        }

        private void ProcessCreate(string filename, Stream data)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _syncDir, filename);
            
            using (var fileStream = File.Create(filePath))
            {
                data.Seek(0, SeekOrigin.Begin);
                data.CopyTo(fileStream);
            }

            var signatureFilename = OctodiffHelper.GetSignatureName(filePath);
            var signaturePath = Path.Combine(_tmpDir, signatureFilename);
            OctodiffHelper.BuildSignature(filePath, signaturePath);
        }
    }
}
