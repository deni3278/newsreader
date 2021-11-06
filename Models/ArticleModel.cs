using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsreader.Models
{
    public class ArticleModel : ModelBase
    {
        public int ArticleID { get; }
        public string From { get; }
        public string Subject { get; }
        public string Date { get; }
        public string Body { get; }

        public ArticleModel(int articleID, string from, string subject, string date, string body)
        {
            ArticleID = articleID;
            From = from;
            Subject = subject;
            Date = date;
            Body = body;
        }
    }
}
