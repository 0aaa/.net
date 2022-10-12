using System;
using System.Data.Entity;
using System.Linq;

namespace Weather0.DAL.Context
{
    public class WeatherContext : DbContext
    {
        public WeatherContext() : base("name=WeatherContext") { }
        public virtual DbSet<WeatherDataCls> WeatherData { get; set; }
        public virtual DbSet<CountryCls> Countries { get; set; }
        public virtual DbSet<CityCls> Cities { get; set; }
    }
}