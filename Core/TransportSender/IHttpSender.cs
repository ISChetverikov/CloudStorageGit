using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.HttpSender
{

    public interface ITransportSender
    {
        Task SendCreate(byte[] file_bytes, string filename);
        void SendDelete();

        Task SendChange(string filename, byte[] deltaFile);
        void SendRename();
    }
}
