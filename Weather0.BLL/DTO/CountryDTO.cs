using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather0.DAL.Context;

namespace Weather0.BLL.DTO
{
    class CountryDTO
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public List<CityCls> Cities { get; set; }
    }
}
