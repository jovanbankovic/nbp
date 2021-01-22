using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soundable.Models
{
    public class Author
    {
        //public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string nickname { get; set; }
        public DateTime birthdate { get; set;  }
        public int ages { get; set; }
        public string citizenship { get; set; }
        public string networth { get; set; }
        public int childrens { get; set; }
        public string bornplace { get; set; }
        public Label label { get; set; }
        public ICollection<Album> albums { get; set; }
        public ICollection<Awards> awards { get; set; }

        public Author()
        {
           
        }
    }
}
