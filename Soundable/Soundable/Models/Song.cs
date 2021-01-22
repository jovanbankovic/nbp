using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soundable.Models
{
    public class Song
    {
        public string name { get; set; }
        public string duration { get; set; }
        public string released { get; set; }
        public Album album { get; set;  }
        public ICollection<Awards> awards { get; set; }

    }
}
