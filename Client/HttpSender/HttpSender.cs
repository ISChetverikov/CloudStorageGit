using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.HttpSender;

namespace Client
{
    class HttpSender : ITransportSender, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public HttpSender(string url)
        {
            _httpClient = new HttpClient();
            _url = url;
        }

        public async Task SendCreate(byte[] file, string filename)
        {
            //Console.WriteLine("I will send create action");
            var form = new MultipartFormDataContent();

            form.Add(new StringContent("Create"), "Method");
            form.Add(new ByteArrayContent(file, 0, file.Length), "fileData", filename);

            var response = await _httpClient.PostAsync(_url, form);

            response.EnsureSuccessStatusCode();
        }
        public async Task SendChange(string filename, byte[] deltaFile)
        {
            //Console.WriteLine("I will send change action");
            var form = new MultipartFormDataContent();

            form.Add(new StringContent("Change"), "Method");
            form.Add(new ByteArrayContent(deltaFile, 0, deltaFile.Length), "deltaData", filename);

            var response = await _httpClient.PostAsync(_url, form);

            response.EnsureSuccessStatusCode();
        }

        public void SendDelete()
        {
            Console.WriteLine("I will send delete");
        }

        public void SendRename()
        {
            Console.WriteLine("I will send rename");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _httpClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~HttpSender() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
