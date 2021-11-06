using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsreader.Models
{
    public class GroupModel : ModelBase
    {
        public string Name { get; set; }
        public int Last { get; set; }
        public int First { get; set; }
        public int Total { get; set; }
        public string Type { get; set; }

        public GroupModel(string name, int last, int first, string type)
        {
            Name = name;
            Last = last;
            First = first;
            Total = first > last ? 0 : last - first;
            Type = type;
        }
    }
}
