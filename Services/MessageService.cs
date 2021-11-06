using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsreader.Services
{
    public class MessageService : IMessageService
    {
        private readonly List<KeyValuePair<string, Action>> stringRegistrations = new List<KeyValuePair<string, Action>>();
        private readonly List<KeyValuePair<Type, object>> typeRegistrations = new List<KeyValuePair<Type, object>>();
        private readonly List<KeyValuePair<string, object>> stringTypeRegistrations = new List<KeyValuePair<string, object>>();

        public void Register(string message, Action callback)
        {
            stringRegistrations.Add(new KeyValuePair<string, Action>(message, callback));
        }

        public void Register<T>(Action<T> callback)
        {
            typeRegistrations.Add(new KeyValuePair<Type, object>(typeof(T), callback));
        }

        public void Register<T>(string message, Action<T> callback)
        {
            stringTypeRegistrations.Add(new KeyValuePair<string, object>(message, callback));
        }

        public void Send(string message)
        {
            foreach (KeyValuePair<string, Action> registration in stringRegistrations.Where(p => p.Key.Equals(message)).ToList())
            {
                registration.Value.Invoke();
            }
        }

        public void Send<T>(T message)
        {
            foreach (KeyValuePair<Type, object> registration in typeRegistrations.Where(p => p.Key.Equals(typeof(T))).ToList())
            {
                (registration.Value as Action<T>).Invoke(message);
            }
        }
        public void Send<T>(string message, T arg)
        {
            foreach (KeyValuePair<string, object> registration in stringTypeRegistrations.Where(p => p.Key.Equals(message)).ToList())
            {
                (registration.Value as Action<T>).Invoke(arg);
            }
        }
    }
}
