using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather0.BLL.DTO
{
    public class WeatherDataDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public float Temperature { get; set; }
        public string Precipitation { get; set; }
        public float WindSpeed { get; set; }
        public int Pressure { get; set; }
        public string Cloudiness { get; set; }
        public float PerceptibleTemperature { get; set; }
        public int Humidity { get; set; }
        public string ConditionsIconPngUrl { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
