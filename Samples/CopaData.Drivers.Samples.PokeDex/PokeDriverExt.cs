using CopaData.Drivers.Contracts;
using PokeApiNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var pokename = "bulbasaur"; //defaultwert
            if (advicedvariables.ContainsKey("inname")
                && advicedvariables["inname"] != null)
            {
                pokename = (string)advicedvariables["inname"];
            }
            ValueCallback.SetValue("Type2", " ");
            ValueCallback.SetValue("Description", " ");
            ValueCallback.SetValue("Ability0", " ");
            ValueCallback.SetValue("Ability1", " ");
            ValueCallback.SetValue("Ability2", " ");
            ValueCallback.SetValue("Flavor0", " ");
            ValueCallback.SetValue("Flavor1", " ");
            ValueCallback.SetValue("Flavor2", " ");
            Pokemon pokemon = await pokeclient.GetResourceAsync<Pokemon>(pokename);
            List<Ability> allAbilities = await pokeclient.GetResourceAsync(pokemon.Abilities.Select(ability => ability.Ability));
            PokemonSpecies species = await pokeclient.GetResourceAsync<PokemonSpecies>(pokename);

            ValueCallback.SetValue("Name", pokemon.Name);
            ValueCallback.SetValue("Height", pokemon.Height*10);
            ValueCallback.SetValue("ID", pokemon.Id);
            ValueCallback.SetValue("Weight", pokemon.Weight/10);
            ValueCallback.SetValue("Type1", pokemon.Types[0].Type.Name);
            if (pokemon.Types.Count > 1)
            {
                ValueCallback.SetValue("Type2", pokemon.Types[1].Type.Name); //->> NUll exception
            }

            for (int i=0;i < species.FlavorTextEntries.Count; i++)
            {
                if (species.FlavorTextEntries[i].Language.Name == "en"&&species.FlavorTextEntries[i].FlavorText !=null)
                {
                    ValueCallback.SetValue("Description", species.FlavorTextEntries[i].FlavorText);
                    break;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if ( i >= allAbilities.Count) { break; }
                else
                {
                    string inst1 = "Ability";
                    inst1 += i;
                    ValueCallback.SetValue(inst1, allAbilities[i].Name);
                    for (int ii = 0; ii < allAbilities.Count; ii++)
                    {
                        if (allAbilities[i].FlavorTextEntries[ii].Language.Name == "en" && allAbilities[i].FlavorTextEntries[ii].FlavorText != null)
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
            if (advicedvariables.ContainsKey(symbolicAddress))
            {
                advicedvariables[symbolicAddress] = value;
            }
            return Task.FromResult(true);
        }
    }
}
