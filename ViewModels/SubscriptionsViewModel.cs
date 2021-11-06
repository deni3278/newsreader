using Newsreader.Commands;
using Newsreader.Models;
using Newsreader.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Newsreader.ViewModels
{
    public class SubscriptionsViewModel : ViewModelBase
    {
        private readonly string path = Path.Combine(Environment.CurrentDirectory, "subscriptions.json");

        private GroupModel selectedItem;
        private string filter;

        public GroupModel SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }

        public string Filter
        {
            get => filter;
            set
            {
                filter = value;
                FilteredSubscriptions.Refresh();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<GroupModel> Subscriptions { get; }
        public DelegateCommand OpenCommand { get; }
        public DelegateCommand UnsubscribeCommand { get; }

        public ICollectionView FilteredSubscriptions
        {
            get
            {
                CollectionViewSource.GetDefaultView(Subscriptions).Filter = p => FilterSubscriptions(p as GroupModel);

                return CollectionViewSource.GetDefaultView(Subscriptions);
            }
        }

        public SubscriptionsViewModel(IMessageService messageService)
        {
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    Subscriptions = JsonConvert.DeserializeObject<ObservableCollection<GroupModel>>(json);
                }
                catch (Exception)
                {
                    Subscriptions = new ObservableCollection<GroupModel>();
                }
            }
            else
            {
                Subscriptions = new ObservableCollection<GroupModel>();
            }

            Subscriptions.CollectionChanged += (sender, args) => FilteredSubscriptions.Refresh();
            Subscriptions.CollectionChanged += (sender, args) => File.WriteAllText(path, JsonConvert.SerializeObject(Subscriptions));

            OpenCommand = new DelegateCommand(() =>
            {
                messageService.Send("ArticlesViewModel");
                messageService.Send(SelectedItem);
            }, () => SelectedItem != null);
            UnsubscribeCommand = new DelegateCommand(() => Subscriptions.Remove(SelectedItem), () => SelectedItem != null);

            messageService.Register<GroupModel>("Subscribe", (groupModel) => Subscriptions.Add(groupModel));
        }

        private bool FilterSubscriptions(GroupModel group)
        {
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                return group.Name.Contains(Filter.ToLower());
            }

            return true;
        }
    }
}
