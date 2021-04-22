using System;
using System.Collections.Generic;
using System.Text;

namespace CopaData.Drivers.Samples.WeatherForecast
{
  public class WeatherData
  {
    public string Error { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    public bool HasErrors()
    {
      return Error != string.Empty;
    }
  }
}
