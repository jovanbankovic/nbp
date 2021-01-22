using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soundable.Models
{
    public class Label
    {
        public string name { get; set; }
        public DateTime founded { get; set;  }
        public string founder { get; set;  }
        public string country { get; set; }
        public ICollection<Author> authors { get; set; }
    }
}
