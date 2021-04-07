using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CopaData.Drivers.Contracts;

namespace CopaData.Drivers.Samples.WeatherForecast
{
  public class DriverExtension : IDriverExtension
  {
    private ILogger _logger;
    private IValueCallback _valueCallback;

    // default: position of Copadata headquarter
    private double latitude { get; set; } = 47.7942531;
    private double longitude { get; set; } = 13.0119902;

    //Variables are created in Engineering Studio. If advised, variables are stored in this dictionary
    private Dictionary<string, object> _subscriptions = new Dictionary<string, object>();

    /// <summary>
    /// Is called when the driver is initialized.
    /// </summary>
    public Task InitializeAsync(ILogger logger, IValueCallback valueCallback, string configFilePath)
    {
       _logger = logger;
      _valueCallback = valueCallback;

      _logger.Info("WeatherForecast driver extension started.");

      return Task.CompletedTask;
    }

    /// <summary>
    /// Is called when the driver is shutting down.
    /// </summary>
    public Task ShutdownAsync()
    {
      //remove all variables from the subscriptions. 
      _subscriptions.Clear();
      _logger.DeepDebug("All variables have been removed from subscription");
      return Task.CompletedTask;
    }

    /// <summary>
    /// Is called when a variable is requested
    /// </summary>
    public Task<bool> SubscribeAsync(string symbolicAddress)
    {
      _subscriptions.Add(symbolicAddress, null);
      _logger.DeepDebug($"Variable '{symbolicAddress}' advised.");
      return Task.FromResult(true);
    }

    /// <summary>
    /// Is called when a variable is not needed anymore
    /// </summary>
    public Task UnsubscribeAsync(string symbolicAddress)
    {
      _subscriptions.Remove(symbolicAddress);
      _logger.DeepDebug($"Variable '{symbolicAddress}' unadvised.");
      return Task.CompletedTask;
    }

    /// <summary>
    /// is called when the value of the variables are read
    /// </summary>
    public Task ReadAllAsync()
    {
      if (_subscriptions.Keys.Count <= 0) return Task.CompletedTask;

      var latitudeKey = "Latitude";
      if (_subscriptions.ContainsKey(latitudeKey))
      {
        _subscriptions[latitudeKey] = latitude;
        _valueCallback.SetValue(latitudeKey,latitude,DateTime.Now,StatusBits.Spontaneous);
      }

      var longitudeKey = "Longitude";
      if (_subscriptions.ContainsKey(longitudeKey))
      {
        _subscriptions[longitudeKey] = longitude;
        _valueCallback.SetValue(longitudeKey,longitude,DateTime.Now,StatusBits.Spontaneous);
      }

      //Take care on the subscription! for the example, a free developer key is used. If used excessively, the costs can be increased drastically.
      //Also think setting the update time of the driver to a long interval. after all, weather does not need to be queried multiple times a second, right?
      var weatherFrog = new AzureWeatherApiClient();
      var weatherKeys = weatherFrog.GetWeatherParameterKeys();
      var weatherDataTask = weatherFrog.GetCurrentWeatherData(latitude,longitude);
      weatherDataTask.Wait();
      var weatherData = weatherDataTask.Result;

      var statusBit = StatusBits.Spontaneous;
      if (weatherData.HasErrors())
      {
        //If you defined a variable with the symbolic address 'ErrorText' the error message is set to the value of that variable.
        if (_subscriptions.ContainsKey("ErrorText"))
        {
          _valueCallback.SetValue("ErrorText", (string) weatherData.Error, DateTime.Now, StatusBits.Invalid);
        }

        statusBit &= StatusBits.Invalid;
      }

      //The symbolic address of the variable has to match the keys given in weather data object!
      foreach (var key in weatherKeys)
      {
        if (_subscriptions.ContainsKey(key))
        {
          if (weatherData.Data[key] is double)
          {
            _valueCallback.SetValue(key, (double) weatherData.Data[key], weatherData.DateTime, statusBit);
          }
          else
          {
            _valueCallback.SetValue(key, weatherData.Data[key].ToString(), weatherData.DateTime, statusBit);
          }
        }
      }
      return Task.CompletedTask;
    }

    /// <summary>
    /// Is called when the string value of a variable is set.
    /// </summary>
    public Task<bool> WriteStringAsync(string symbolicAddress, string value, DateTime dateTime, StatusBits statusBits)
    {
      //writing values is not allowed
      return Task.FromResult(false);
    }

    /// <summary>
    /// Is called when the numeric value of a variable is set.
    /// </summary>
    public Task<bool> WriteNumericAsync(string symbolicAddress, double value, DateTime dateTime, StatusBits statusBits)
    {
      if (symbolicAddress == "Latitude")
      {
        latitude = value;
        return Task.FromResult(true);
      }

      if (symbolicAddress == "Longitude")
      {
        longitude = value;
        return Task.FromResult(true);
      }
      //writing values is not allowed
      return Task.FromResult(false);
    }
  }
}
