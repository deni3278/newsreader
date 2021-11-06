using Newsreader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsreader.Services
{
    public interface IConnectionService
    {
        bool Connected { get; }
        ConnectionModel ConnectionModel { get; }

        void Connect(ConnectionModel connectionModel, int readTimeout = 30000, int writeTimeout = 30000);
        string Read(int bufferSize = 4096);
        string ReadMultiLine(int bufferSize = 4096);
        bool Write(string command);
    }
}
