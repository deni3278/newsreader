using Newsreader.Commands;
using Newsreader.Models;
using Newsreader.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace Newsreader.ViewModels
{
    public class ConnectViewModel : ViewModelBase
    {
        private readonly string path = Path.Combine(Environment.CurrentDirectory, "savedConnection.json");

        private string hostName;
        private int port;
        private string username;
        private string password;

        public string HostName
        {
            get => hostName;
            set
            {
                hostName = value;
                OnPropertyChanged();
            }
        }

        public int Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand ConnectCommand { get; }

        public ConnectViewModel(IConnectionService connectionService, IMessageService messageService)
        {
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    ConnectionModel connectionModel = JsonConvert.DeserializeObject<ConnectionModel>(json);

                    HostName = connectionModel.HostName;
                    Port = connectionModel.Port;
                    Username = connectionModel.Username;
                    Password = connectionModel.Password;
                }
                catch (Exception) { }
            }

            ConnectCommand = new DelegateCommand(() => Connect(connectionService, messageService), CanConnect);
        }

        private void Connect(IConnectionService connectionService, IMessageService messageService)
        {
            connectionService.Connect(new ConnectionModel(HostName, Port, Username, Password));

            if (connectionService.Connected)
            {
                string json = JsonConvert.SerializeObject(connectionService.ConnectionModel);
                File.WriteAllText(path, json);

                messageService.Send("MainViewModel");
            }
            else
            {
                MessageBox.Show("Cannot connect to " + HostName + ':' + Port + " with the information provided.");
            }
        }

        private bool CanConnect()
        {
            return !string.IsNullOrWhiteSpace(HostName) 
                && !string.IsNullOrWhiteSpace(Username) 
                && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
