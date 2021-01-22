using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soundable.Models
{
    public class Genre
    {
        public string name { get; set; }
        public ICollection<Album> albums { get; set; }

    }
}
