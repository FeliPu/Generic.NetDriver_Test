using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CopaData.Drivers.Samples.WeatherForecast
{
  public interface IWeatherFrog
  {
    Task<WeatherData> GetCurrentWeatherData(string longitude, string latitude);
    string[] GetWeatherParameterKeys();
  }
}
