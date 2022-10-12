using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather0.DAL.Context
{
    public class WeatherDataCls
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
        public int CityId { get; set; }
        public int? CountryId { get; set; }
        public virtual CityCls City { get; set; }
        public virtual CountryCls Country { get; set; }
    }
}
