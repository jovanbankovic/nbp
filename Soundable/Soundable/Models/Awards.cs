using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soundable.Models
{
    public class Awards
    {
        public string name { get; set; }
        public DateTime date { get; set; }
        public string city { get; set; }
        public ICollection<Author> authors { get; set; }
        public ICollection<Song> songs { get; set; }
    }
}
