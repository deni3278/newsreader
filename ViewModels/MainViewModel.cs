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
    public class MainViewModel : ViewModelBase
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

        public DelegateCommand GroupsCommand { get; }
        public DelegateCommand SubscriptionsCommand { get; }
        public DelegateCommand ArticlesCommand { get; }

        public MainViewModel(IUnityContainer container, IMessageService messageService)
        {
            messageService.Register("GroupsViewModel", () => CurrentViewModel = container.Resolve<GroupsViewModel>());
            messageService.Register("SubscriptionsViewModel", () => CurrentViewModel = container.Resolve<SubscriptionsViewModel>());
            messageService.Register("ArticlesViewModel", () => CurrentViewModel = container.Resolve<ArticlesViewModel>());

            GroupsCommand = new DelegateCommand(() => messageService.Send("GroupsViewModel"));
            SubscriptionsCommand = new DelegateCommand(() => messageService.Send("SubscriptionsViewModel"));
            ArticlesCommand = new DelegateCommand(() => messageService.Send("ArticlesViewModel"));

            GroupsCommand.Execute(null);
        }
    }
}