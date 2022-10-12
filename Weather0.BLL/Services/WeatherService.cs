using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Weather0.BLL.DTO;
using Weather0.DAL.Context;
using Weather0.DAL.Repositories;

// Класс WeatherService получает из интернета, далее помещает в БД данные по погоде через 24 ч.

namespace Weather0.BLL.Services
{
    public class WeatherService
    {
        private const string DEFAULT_UNITS = "metric";
        private const int TIMESTAMPS_NUMBER = 8;
        private GenericRepository<WeatherDataCls> WeatherDataRpstry { get; set; }
        private GenericRepository<CityCls> CityRpstry { get; set; }
        private GenericRepository<CountryCls> CountryRpstry { get; set; }
        private string RequestStr { get; set; }
        private WebClient WbClnt { get; set; }
        private string CurrentLocation { get; set; }
        private string CurrentUnits { get; set; }
        private JObject ResultJsn { get; set; }
        private WeatherDataDTO[] Forecast5Days { get; set; }
        public WeatherService()
        {
            WeatherDataRpstry = new GenericRepository<WeatherDataCls>();
            CityRpstry = new GenericRepository<CityCls>();
            CountryRpstry = new GenericRepository<CountryCls>();
            WbClnt = new WebClient();
            Forecast5Days = new WeatherDataDTO[5];
        }
        public IEnumerable<WeatherDataDTO> GetAllWeatherData()
        => WeatherDataRpstry.GetAll().Select(dt => new WeatherDataDTO
        {
            Id = dt.Id,
            Date = dt.Date,
            Temperature = dt.Temperature,
            Precipitation = dt.Precipitation,
            WindSpeed = dt.WindSpeed,
            Pressure = dt.Pressure,
            Cloudiness = dt.Cloudiness,
            PerceptibleTemperature = dt.PerceptibleTemperature,
            Humidity = dt.Humidity,
            ConditionsIconPngUrl = dt.ConditionsIconPngUrl,
            City = CityRpstry.Get(dt.CityId).CityName,
            Country = CountryRpstry.Get(dt.CountryId ?? default).CountryName
        });
        // Возвращение данных по погоде на последующие 5 дней с помещением данных, соответствующих текущему моменту времени, в БД.
        public WeatherDataDTO[] GetNewWeatherData(string location, string units = DEFAULT_UNITS)
        {
            CurrentLocation = location;
            CurrentUnits = units;
            try
            {
                ApiHandling();
            }
            catch (Exception excptn)
            {
                MessageBox.Show(excptn.Message, "Caramba!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            AddCountry();
            AddCity();
            AddWeatherDataNow();
            return GetForecast5Days();
        }
        private void ApiHandling()
        {
            RequestStr = $"http://api.openweathermap.org/data/2.5/forecast?" +
                $"units={CurrentUnits}" +
                $"&appid=804c83acf3e31d36ce7aeebd60f0844c" +
                $"&q={CurrentLocation}";
            ResultJsn = JObject.Parse(WbClnt.DownloadString(RequestStr));
        }

        private void AddCountry()
        {
            if (!CountryRpstry.GetAll().Any(cntry => cntry.CountryName == $"{ResultJsn["city"]["country"]}"))
            {
                CountryRpstry.AddUpdate(new CountryCls
                {
                    CountryName = $"{ResultJsn["city"]["country"]}"
                });
                CountryRpstry.Save();
            }
        }

        private void AddCity()
        {
            if (!CityRpstry.GetAll().Any(cty => cty.CityName == CurrentLocation))
            {
                CityRpstry.AddUpdate(new CityCls
                {
                    CityName = CurrentLocation,
                    CountryId = CountryRpstry.GetAll().FirstOrDefault(cntry => cntry.CountryName == $"{ResultJsn["city"]["country"]}").Id
                });
                CityRpstry.Save();
            }
        }

        private void AddWeatherDataNow()
        {
            WeatherDataRpstry.AddUpdate(new WeatherDataCls()
            {
                Date = DateTime.Now,
                Temperature = Convert.ToSingle(ResultJsn["list"][0]["main"]["temp"]),
                Precipitation = $"{ResultJsn["list"][0]["weather"][0]["main"]}",
                WindSpeed = Convert.ToSingle(ResultJsn["list"][0]["wind"]["speed"]),
                Pressure = Convert.ToInt32(ResultJsn["list"][0]["main"]["pressure"]),
                Cloudiness = $"{ResultJsn["list"][0]["weather"][0]["description"]}",
                PerceptibleTemperature = Convert.ToSingle(ResultJsn["list"][0]["main"]["feels_like"]),
                Humidity = Convert.ToInt32(ResultJsn["list"][0]["main"]["humidity"]),
                ConditionsIconPngUrl = $"http://openweathermap.org/img/wn/" +
                                            $"{ResultJsn["list"][0]["weather"][0]["icon"]}" +
                                            $"@2x.png",
                CityId = CityRpstry.GetAll().FirstOrDefault(cty => cty.CityName == CurrentLocation).Id,
                CountryId = CityRpstry.GetAll().FirstOrDefault(cty => cty.CityName == CurrentLocation).CountryId
            });
            WeatherDataRpstry.Save();
        }

        private WeatherDataDTO[] GetForecast5Days()
        {
            for (int i = 0; i < 5; i++)
            {
                Forecast5Days[i] = new WeatherDataDTO()
                {
                    Id = WeatherDataRpstry.GetAll().Last().Id,
                    Date = Convert.ToDateTime($"{ResultJsn["list"][i * TIMESTAMPS_NUMBER]["dt_txt"]}"),
                    Temperature = Convert.ToSingle(ResultJsn["list"][i * TIMESTAMPS_NUMBER]["main"]["temp"]),
                    Precipitation = $"{ResultJsn["list"][i * TIMESTAMPS_NUMBER]["weather"][0]["main"]}",
                    WindSpeed = Convert.ToSingle(ResultJsn["list"][i * TIMESTAMPS_NUMBER]["wind"]["speed"]),
                    Pressure = Convert.ToInt32(ResultJsn["list"][i * TIMESTAMPS_NUMBER]["main"]["pressure"]),
                    Cloudiness = $"{ResultJsn["list"][i * TIMESTAMPS_NUMBER]["weather"][0]["description"]}",
                    PerceptibleTemperature = Convert.ToSingle(ResultJsn["list"][i * TIMESTAMPS_NUMBER]["main"]["feels_like"]),
                    Humidity = Convert.ToInt32(ResultJsn["list"][i * TIMESTAMPS_NUMBER]["main"]["humidity"]),
                    ConditionsIconPngUrl = $"http://openweathermap.org/img/wn/" +
                                            $"{ResultJsn["list"][i * TIMESTAMPS_NUMBER]["weather"][0]["icon"]}" +
                                            $"@2x.png",
                    City = $"{ResultJsn["city"]["name"]}",
                    Country = $"{ResultJsn["city"]["country"]}"
                };
            }
            return Forecast5Days;
        }

        public void RemoveWeatherData(int id)
        {
            WeatherDataRpstry.Delete(WeatherDataRpstry.Get(id));
            WeatherDataRpstry.Save();
        }

        public void ClearWeatherData()
        {
            WeatherDataRpstry.GetAll().ToList().ForEach(dt => WeatherDataRpstry.Delete(dt));
            WeatherDataRpstry.Save();
        }
    }
}
