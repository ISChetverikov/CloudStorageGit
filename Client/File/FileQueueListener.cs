using Core.File;
using Core.HttpSender;
using Core.Octodiff;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class FileQueueListener : IFileQueueListener
    {
        private readonly IFileActionProcessor _processor;

        public FileQueueListener(IFileActionProcessor processor)
        {
            _processor = processor;
        }

        public void OnNext(FileAction fileAction)
        {

            _processor.Process(fileAction);
        }
        
        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }
    }
}
