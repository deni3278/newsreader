using Newsreader.Services;
using Newsreader.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace Newsreader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IUnityContainer container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            container = new UnityContainer();
            container.RegisterSingleton<IMessageService, MessageService>();
            container.RegisterSingleton<IConnectionService, ConnectionService>();
            container.RegisterSingleton<ArticlesViewModel>();
            container.RegisterSingleton<SubscriptionsViewModel>();
            container.RegisterSingleton<GroupsViewModel>();
            container.RegisterSingleton<ConnectViewModel>();
            container.RegisterSingleton<MainViewModel>();
            container.RegisterSingleton<MainWindow>();

            MainWindow = container.Resolve<MainWindow>();
            MainWindow.Show();
        }
    }
}
