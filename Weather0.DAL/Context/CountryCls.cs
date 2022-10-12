using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather0.DAL.Context
{
    public class CountryCls
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        public virtual HashSet<CityCls> Cities { get; set; }
        public CountryCls() => Cities = new HashSet<CityCls>();
    }
}
