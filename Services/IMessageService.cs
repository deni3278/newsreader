using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsreader.Services
{
    public interface IMessageService
    {
        void Register(string message, Action callback);
        void Register<T>(Action<T> callback);
        void Register<T>(string message, Action<T> callback);
        void Send(string message);
        void Send<T>(T message);
        void Send<T>(string message, T arg);
    }
}
