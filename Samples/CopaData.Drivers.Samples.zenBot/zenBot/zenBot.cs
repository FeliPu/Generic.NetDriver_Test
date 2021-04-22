using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CopaData.Drivers.Contracts;

namespace zenBot
{
  public class zenBot : IDriverExtension
  {
    private ILogger _logger;
    private IValueCallback _valueCallback;

    public Task InitializeAsync(ILogger logger, IValueCallback valueCallback, string configFilePath)
    {
      _logger = logger;
      _valueCallback = valueCallback;
      _logger.Info("zenBot driver extension started.");
      return Task.CompletedTask;
    }

    private Dictionary<string, object> _subscriptions = new Dictionary<string, object>();

    public Task ShutdownAsync()
    {
      _subscriptions.Clear();
      _logger.DeepDebug("All variables have been removed from subscription");
      return Task.CompletedTask;
    }

    public Task<bool> SubscribeAsync(string symbolicAddress)
    {
      _subscriptions.Add(symbolicAddress,null);
      _logger.DeepDebug($"Variable '{symbolicAddress}' advised.");
      return Task.FromResult(true);
    }

    public Task UnsubscribeAsync(string symbolicAddress)
    {
      _subscriptions.Remove(symbolicAddress);
      _logger.DeepDebug($"Variable '{symbolicAddress}' unadvised.");
      return Task.CompletedTask;
    }

    public Task ReadAllAsync()
    {
      throw new NotImplementedException();
    }

    public Task<bool> WriteStringAsync(string symbolicAddress, string value, DateTime dateTime, StatusBits statusBits)
    {
      return Task.FromResult(false);
    }

    public Task<bool> WriteNumericAsync(string symbolicAddress, double value, DateTime dateTime, StatusBits statusBits)
    {
      return Task.FromResult(false);
    }
  }
}
