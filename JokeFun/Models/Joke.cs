using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JokeFun.Models
{
    public class Joke
    {
        public int Id { get; set; }
        public string Tekst { get; set; }

        public DateTime DateTime { get; set; }

        public string Kilde { get; set; }
        public Tag Tag { get; set; }
        public int TagId { get; set; }
    }
}
