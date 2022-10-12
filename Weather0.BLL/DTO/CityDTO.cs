using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather0.BLL.DTO
{
    class CityDTO
    {
        public int Id { get; set; }
        public string City { get; set; }
        public int CountryId { get; set; }
        public string Country { get; set; }
    }
}
