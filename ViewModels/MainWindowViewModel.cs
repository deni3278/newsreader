using Newsreader.Commands;
using Newsreader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Newsreader.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => currentViewModel;
            set
            {
                currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel(IUnityContainer container, IMessageService messageService)
        {
            CurrentViewModel = container.Resolve<ConnectViewModel>();

            messageService.Register("MainViewModel", () => CurrentViewModel = container.Resolve<MainViewModel>());
        }
    }
}