using Newsreader.Commands;
using Newsreader.Models;
using Newsreader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Newsreader.ViewModels
{
    public class ArticlesViewModel : ViewModelBase
    {
        private ArticleModel selectedItem;
        private string filter;

        public ArticleModel SelectedItem
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
                FilteredArticles.Refresh();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ArticleModel> Articles { get; }
        public DelegateCommand OpenCommand { get; }
        public DelegateCommand NewCommand { get; }

        public ICollectionView FilteredArticles
        {
            get
            {
                CollectionViewSource.GetDefaultView(Articles).Filter = p => FilterArticles(p as ArticleModel);

                return CollectionViewSource.GetDefaultView(Articles);
            }
        }

        public ArticlesViewModel(IConnectionService connectionService, IMessageService messageService)
        {
            Articles = new ObservableCollection<ArticleModel>();

            OpenCommand = new DelegateCommand(() => MessageBox.Show(SelectedItem.Body), () => SelectedItem != null);
            NewCommand = new DelegateCommand(() => MessageBox.Show("Unimplemented."));

            messageService.Register((GroupModel groupModel) => Open(connectionService, groupModel));
        }

        private void Open(IConnectionService connectionService, GroupModel groupModel)
        {
            Articles.Clear();

            if (connectionService.Write("LISTGROUP " + groupModel.Name))
            {
                string response = connectionService.ReadMultiLine();
                string[] lines = response.Split(new string[] { ConnectionService.CRLF }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length == 0)
                {
                    MessageBox.Show("No articles in the selected group.");
                }
                else if (lines[0].StartsWith("211"))
                {
                    if (Articles.Count > 0)
                    {
                        Articles.Clear();
                    }

                    for (int i = 1; i < lines.Length; i++)
                    {
                        ArticleModel article = ParseArticle(connectionService, int.Parse(lines[i]));

                        if (article != null)
                        {
                            Articles.Add(article);
                        }
                    }
                }
            }
        }

        private ArticleModel ParseArticle(IConnectionService connectionService, int articleID)
        {
            string subject = "";
            string from = "";
            string date = "";
            string body = "";

            if (connectionService.Write("HDR SUBJECT " + articleID))
            {
                string response = connectionService.ReadMultiLine();
                string[] lines = response.Split(new string[] { ConnectionService.CRLF }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length >= 2 &&
                    lines[0].Contains("221"))
                {
                    subject = lines[1].Substring(lines[1].IndexOf(' ') + 1);
                }
            }

            if (connectionService.Write("HDR FROM " + articleID))
            {
                string response = connectionService.ReadMultiLine();
                string[] lines = response.Split(new string[] { ConnectionService.CRLF }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length >= 2 &&
                    lines[0].Contains("221"))
                {
                    from = lines[1].Substring(lines[1].IndexOf('<') + 1, lines[1].Length - 1 - lines[1].IndexOf('<'));
                }
            }

            if (connectionService.Write("HDR DATE " + articleID))
            {
                string response = connectionService.ReadMultiLine();
                string[] lines = response.Split(new string[] { ConnectionService.CRLF }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length >= 2 &&
                    lines[0].Contains("221"))
                {
                    date = lines[1].Substring(lines[1].IndexOf(' ') + 1);
                }
            }

            if (connectionService.Write("BODY " + articleID))
            {
                string response = connectionService.ReadMultiLine();
                body = response.Substring(response.IndexOf(ConnectionService.CRLF) + 1);
            }

            return new ArticleModel(articleID, from, subject, date, body);
        }

        private bool FilterArticles(ArticleModel article)
        {
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                return article.Subject.Contains(Filter.ToLower());
            }

            return true;
        }
    }
}
