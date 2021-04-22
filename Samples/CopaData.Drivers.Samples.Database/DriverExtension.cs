using CopaData.Drivers.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GenericNetDb_Driver10;

namespace DataBaseGenericNetDriver
{
  public class DriverExtension : IDriverExtension
  {
    private ILogger _logger;
    private IValueCallback _valueCallback;
    private readonly Dictionary<string, object> _subscriptions;

    public DriverExtension()
    {
      _subscriptions = new Dictionary<string, object>();
    }

    /// <summary>
    /// The driver is initialized (Called when Runtime started)
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="valueCallback"></param>
    /// <param name="configFilePath"></param>
    /// <returns></returns>
    public Task InitializeAsync(ILogger logger, IValueCallback valueCallback, string configFilePath)
    {
      _logger = logger;
      _valueCallback = valueCallback;

      return Task.CompletedTask;
    }

    /// <summary>
    /// The driver is shutting down (Called when Runtime exit)
    /// </summary>
    /// <returns></returns>
    public Task ShutdownAsync()
    {
      _subscriptions.Clear();
      _logger.DeepDebug("Shutdown");
      return Task.CompletedTask;
    }

    /// <summary>
    /// This method is called by zenon, when a variable (by using the symbolic address) 
    /// is requested (variable is adviced by showing on a screen, using in archives, etc.)
    /// Called on Runtime Start for all variables under the Generic Net driver
    /// </summary>
    /// <param name="symbolicAddress"></param>
    /// <returns></returns>
    public Task<bool> SubscribeAsync(string symbolicAddress)
    {
      _logger.DeepDebug($"Subscribe '{symbolicAddress}'");
      _subscriptions.Add(symbolicAddress, null);
      return Task.FromResult(true);
    }

    /// <summary>
    /// This method is called by zenon, when a variable (by using the symbolic address) 
    /// is not needed any more (e. g. a screen switch to another screen or stopping an archive)
    /// </summary>
    /// <param name="symbolicAddress"></param>
    /// <returns></returns>
    public Task UnsubscribeAsync(string symbolicAddress)
    {
      _logger.DeepDebug($"Unsubscribe '{symbolicAddress}'");
      _subscriptions.Remove(symbolicAddress);
      return Task.CompletedTask;
    }

    /// <summary>
    /// is called when the value of the variables are read
    /// </summary>
    public Task ReadAllAsync()
    {
      if (_subscriptions.Keys.Count <= 0) return Task.CompletedTask;

      foreach (var variable in _subscriptions)
      {
        if (variable.Value != null)
        {
          if (variable.Value is string stringValue)
          {
            // get the value from the database
            var dbManager = new DbManager();
            var result = dbManager.GetValue(1);
            _logger.DeepDebug($"Current Read Value: {result}");
            _valueCallback.SetValue(variable.Key, result);
          }
          else if (variable.Value is double value)
          {
            _valueCallback.SetValue(variable.Key, value);
          }
        }
      }

      return Task.CompletedTask;
    }

    /// <summary>
    /// This method is called by zenon, when the string value of a variable is set.
    /// </summary>
    /// <returns></returns>
    public Task<bool> WriteStringAsync(string symbolicAddress, string value, DateTime dateTime, StatusBits status)
    {
      _logger.DeepDebug($"WriteString '{symbolicAddress}'");
      _subscriptions[symbolicAddress] = value;
      return Task.FromResult(true);
    }

    /// <summary>
    /// This method is called by zenon, when the numeric/boolean value of a variable is set.
    /// </summary>
    /// <param name="symbolicAddress"></param>
    /// <param name="value"></param>
    /// <param name="dateTime"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public Task<bool> WriteNumericAsync(string symbolicAddress, double value, DateTime dateTime, StatusBits status)
    {
      _logger.DeepDebug($"WriteNumeric '{symbolicAddress}'");
      Console.WriteLine($"Numeric variable changed and the new value is: {value}");
      _subscriptions[symbolicAddress] = value;
      return Task.FromResult(true);
    }

    #region Helpers

    private async Task<string> ReadCurrentValue(int id)
    {
      var dbManager = new DbManager();
      return dbManager.GetValue(id);

    }

    //private IConfigurationRoot GetConfiguration(string configFilePath)
    //{
    //    var configurationBuilder = new ConfigurationBuilder()
    //        .AddJsonFile(configFilePath, optional: true);

    //    var configuration = configurationBuilder.Build();
    //    return configuration;
    //}

    #endregion
  }
}
