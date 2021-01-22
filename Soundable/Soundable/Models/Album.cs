using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soundable.Models
{
    public class Album
    {
        public string name { get; set; }
        public DateTime releasedate { get; set; }
        public decimal price { get; set; }
        public string length { get; set; }
        public string studio { get; set; }
        public string picture { get; set; }
        public string link { get; set; }
        public ICollection<Song> songs { get; set; }
        public Author author { get; set; }
        public Genre genre { get; set; }
    }
}
