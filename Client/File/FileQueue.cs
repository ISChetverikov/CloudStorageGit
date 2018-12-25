using Core.File;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
    public class FileQueue : IFileQueue
    {
        private IObserver<FileAction> _observer;
        private readonly ConcurrentQueue<FileAction> _queue = new ConcurrentQueue<FileAction>();

        public IDisposable Subscribe(IObserver<FileAction> observer)
        {
            _observer = observer;
            return new Unsubscriber(observer);
        }

        public void Enqueue(FileAction file)
        {
            _queue.Enqueue(file);
            Task.Run(() => NotifyObservers());
        }

        private void NotifyObservers()
        {
            while (_queue.TryDequeue(out var fileAction))
            {
                _observer.OnNext(fileAction);
            }
        }
       
        private sealed class Unsubscriber : IDisposable
        {
            private IObserver<FileAction> _observer;

            public Unsubscriber(IObserver<FileAction> observer)
            {
                _observer = observer;
            }

            public void Dispose()
            {
                _observer = null;
            }
        }
    }
}