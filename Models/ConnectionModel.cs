using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Newsreader.Models
{
    public class ConnectionModel : ModelBase
    {
        public string HostName { get; }
        public int Port { get; }
        public string Username { get; }
        public string Password { get; }

        public ConnectionModel(string hostName, int port, string username, string password)
        {
            HostName = hostName;
            Port = port;
            Username = username;
            Password = password;
        }
    }
}
