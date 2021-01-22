using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soundable.Models
{
    public class Playlist
    {
        public string name { get; set; }
        public ICollection<Album> albums { get; set; }
        public User user { get; set; }
    }
}
