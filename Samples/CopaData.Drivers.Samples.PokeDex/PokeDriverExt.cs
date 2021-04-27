using CopaData.Drivers.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PokeApiNet;
using System.Linq;

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
            var pokename = "ho-ho"; //defaultwert
            if(advicedvariables.ContainsKey("inname")
                && advicedvariables["inname"] != null)
            {
                pokename = (string) advicedvariables["inname"];
            }
            Pokemon pokemon = await pokeclient.GetResourceAsync<Pokemon>(pokename);
            List<Ability> allAbilities = await pokeclient.GetResourceAsync(pokemon.Abilities.Select(ability => ability.Ability));
            ValueCallback.SetValue("Name", pokemon.Name);
            ValueCallback.SetValue("Height", pokemon.Height);
            ValueCallback.SetValue("ID", pokemon.Id);

            for (int i = 0; i < 3; i++)
            {
                if (allAbilities[i] == null) { break; }
                else {
                    string inst1 = "Ability";
                    inst1 += i;
                    ValueCallback.SetValue(inst1 += i, allAbilities[i].Name);
                    for (int ii = 0; ii < allAbilities.Count; ii++)
                    {
                        if (allAbilities[i].FlavorTextEntries[ii].Language.Name == "en")
                        {
                            string inst2 = "Flavor";
                            inst2 += i;
                            ValueCallback.SetValue(inst2, allAbilities[i].FlavorTextEntries[ii].FlavorText);
                            break;
                        }
                    }
                }
            }

            return;
        }

        public Task ShutdownAsync()
        {
            return Task.CompletedTask;
        }
        public Dictionary<string, object> advicedvariables { get; set; } = new Dictionary<string, object>();
        public Task<bool> SubscribeAsync(string symbolicAddress)
        {
            advicedvariables.Add(symbolicAddress, null);
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
            if(advicedvariables.ContainsKey(symbolicAddress))
            {
                advicedvariables[symbolicAddress] = value;
            }
            return Task.FromResult(true);
        }
    }
}
