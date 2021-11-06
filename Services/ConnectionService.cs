using Newsreader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Newsreader.Services
{
    public class ConnectionService : IConnectionService
    {
        private IPAddress ip;
        private IPEndPoint endPoint;
        private TcpClient client;
        private NetworkStream stream;

        public const string CRLF = "\r\n";

        public bool Connected { get; private set; }
        public ConnectionModel ConnectionModel { get; private set; }

        ~ConnectionService()
        {
            stream?.Close();
            client?.Close();
        }

        public void Connect(ConnectionModel connectionModel, int readTimeout = 30000, int writeTimeout = 30000)
        {
            ConnectionModel = connectionModel;

            try
            {
                ip = Dns.GetHostEntry(ConnectionModel.HostName).AddressList.First();
                endPoint = new IPEndPoint(ip, ConnectionModel.Port);
                client = new TcpClient();
                client.Connect(endPoint);
                stream = client.GetStream();
                stream.ReadTimeout = readTimeout;
                stream.WriteTimeout = writeTimeout;

                string response = Read();

                if (response.StartsWith("200") &&
                   (response.Contains("NNTP") ||
                    response.Contains("NNRP")))
                {
                    Write("AUTHINFO USER " + ConnectionModel.Username + CRLF);

                    if (Read().StartsWith("381"))
                    {
                        Write("AUTHINFO PASS " + ConnectionModel.Password + CRLF);

                        if (Read().StartsWith("281"))
                        {
                            Connected = true;

                            return;
                        }
                    }
                }
            }
            catch (IOException) { }
        }

        public string Read(int bufferSize = 1024)
        {
            if (stream.CanRead)
            {
                try
                {
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    char[] chars = new char[bytesRead];
                    Encoding.UTF8.GetDecoder().GetChars(buffer, 0, bytesRead, chars, 0);

                    return new string(chars);
                }
                catch (IOException)
                {
                    return "";
                }
            }

            return null;
        }

        public string ReadMultiLine(int bufferSize = 1024)
        {
            if (stream.CanRead)
            {
                StringBuilder builder = new StringBuilder();
                byte[] buffer = new byte[bufferSize];
                int bytesRead;
                string chunk;

                do
                {
                    try
                    {
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        builder.Append(chunk);
                    }
                    catch (IOException)
                    {
                        return "";
                    }
                }
                while (!string.IsNullOrWhiteSpace(chunk) && !chunk.EndsWith(CRLF + '.' + CRLF));

                string response = builder.ToString();

                return response.EndsWith(CRLF + '.' + CRLF) ? response.Remove(response.Length - 3) : response;
            }

            return null;
        }

        public bool Write(string command)
        {
            if (stream.CanWrite)
            {
                try
                {
                    if (!command.EndsWith(CRLF))
                    {
                        command += CRLF;
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(command);
                    stream.Write(buffer, 0, buffer.Length);

                    return true;
                }
                catch (IOException)
                {
                    return false;
                }
            }

            return false;
        }
    }
}
