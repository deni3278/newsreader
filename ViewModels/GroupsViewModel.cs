using Newsreader.Commands;
using Newsreader.Models;
using Newsreader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Unity;

namespace Newsreader.ViewModels
{
    public class GroupsViewModel : ViewModelBase
    {
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
                FilteredGroups.Refresh();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<GroupModel> Groups { get; }
        public DelegateCommand RefreshCommand { get; }
        public DelegateCommand OpenCommand { get; }
        public DelegateCommand SubscribeCommand { get; }

        public ICollectionView FilteredGroups
        {
            get
            {
                CollectionViewSource.GetDefaultView(Groups).Filter = p => FilterGroups(p as GroupModel);

                return CollectionViewSource.GetDefaultView(Groups);
            }
        }

        public GroupsViewModel(IConnectionService connectionService, IMessageService messageService)
        {
            Groups = new ObservableCollection<GroupModel>();
            RefreshCommand = new DelegateCommand(() => Refresh(connectionService));
            OpenCommand = new DelegateCommand(() =>
            {
                messageService.Send("ArticlesViewModel");
                messageService.Send(SelectedItem);
            }, () => SelectedItem != null);
            SubscribeCommand = new DelegateCommand(() =>
            {
                messageService.Send<GroupModel>("Subscribe", SelectedItem);
            }, () => SelectedItem != null);

            RefreshCommand.Execute(null);
        }
        private void Refresh(IConnectionService connectionService)
        {
            if (connectionService.Write("LIST ACTIVE"))
            {
                string response = connectionService.ReadMultiLine();
                string[] lines = response.Split(new string[] { ConnectionService.CRLF }, StringSplitOptions.RemoveEmptyEntries);

                if (lines != null && lines[0].Contains("215"))
                {
                    Groups.Clear();

                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] group = lines[i].Split(' ');

                        Groups.Add(new GroupModel(group[0], int.Parse(group[1]), int.Parse(group[2]), group[3]));
                    }
                }
            }
        }

        private bool FilterGroups(GroupModel group)
        {
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                return group.Name.Contains(Filter.ToLower());
            }

            return true;
        }
    }
}