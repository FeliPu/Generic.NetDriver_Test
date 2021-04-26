using CopaData.Drivers.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PokeApiNet;

namespace CopaData.Drivers.Samples.PokeDex
{
    class PokeDriverExt : IDriverExtension
    {
        private ILogger Logger { get; set; }
        private IValueCallback ValueCallback { get; set; }
        private PokeApiClient pokeclient { get; set; }
        public Task InitializeAsync(ILogger logger, IValueCallback valueCallback, string configFilePath)
        {
            Logger = logger;
            ValueCallback = valueCallback;
            pokeclient = new PokeApiClient();
            return Task.CompletedTask;
        }

        public async Task ReadAllAsync()
        {
            Pokemon hoOh = await pokeclient.GetResourceAsync<Pokemon>("ho-oh");
            ValueCallback.SetValue("test", hoOh.Height);
            return;
        }

        public Task ShutdownAsync()
        {
            return Task.CompletedTask;
        }

        public Task<bool> SubscribeAsync(string symbolicAddress)
        {
            return Task.FromResult(true);
        }

        public Task UnsubscribeAsync(string symbolicAddress)
        {
            return Task.FromResult(true);
        }

        public Task<bool> WriteNumericAsync(string symbolicAddress, double value, DateTime dateTime, StatusBits statusBits)
        {
            return Task.FromResult(true);
        }

        public Task<bool> WriteStringAsync(string symbolicAddress, string value, DateTime dateTime, StatusBits statusBits)
        {
            throw new NotImplementedException();
        }
    }
}
