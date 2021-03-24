using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CopaData.Drivers.Samples.WeatherForecast
{
  public class AzureWeatherApiClient : IWeatherFrog
  {
    #region primary Key for the API - not to show!

    private static readonly string primaryKey = "6c3_xdoIFm5Npmb6baI6EOiCOoANhC6aTPBsuhK9shY";

    #endregion

    private static readonly string WEATHER_MAP_API_ENDPOINT =
      "https://atlas.microsoft.com/weather/currentConditions/json?api-version=1.0&query={0},{1}&subscription-key={2}";

    //Copadata Headquarter coordinates are used as fallback if input coordinates are invalid.
    private static readonly string CD_HQ_LATITUDE = "47.7942531";
    private static readonly string CD_HQ_LONGITUDE = "13.0119902";

    public string[] GetWeatherParameterKeys()
    {
      return new[]
      { //Add any key here if more weather information is needed.
        //Any data is stored in a dictionary in weather data with that key. 
        //The key needs to match the variable symbolic address in Engineering Studio
        //to match the value to the variable
        "Phrase",
        "IconCode",
        "Temperature",
        "TemperatureUnit",
        "RelativeHumidity",
        "WindDirectionDegrees",
        "WindSpeed",
        "WindSpeedUnit",
        "CloudCover",
        "Pressure",
        "PressureUnit",
      };
    }

    public async Task<WeatherData> GetCurrentWeatherData(string latitude, string longitude)
    {
      var weatherData = new WeatherData();
      #region InputValidation
      try
      {
        if (double.TryParse(latitude, out var latitudeValue) && double.TryParse(longitude, out var longitudeValue))
        {
          if (latitudeValue < -90.0 || latitudeValue > 90.0)
          {
            weatherData.Error +=
              $" Latitude not within -90° and +90°. Value: '{latitude}'. Instead, location of CD-HQ is used.";
            latitude = CD_HQ_LATITUDE;
            longitude = CD_HQ_LONGITUDE;
          }
          if (longitudeValue < -180.0 || longitudeValue > 180.0)
          {
            weatherData.Error +=
              $" Longitude not within -180° and +180°. Value: '{longitude}'. Instead, location of CD-HQ is used.";
            latitude = CD_HQ_LATITUDE;
            longitude = CD_HQ_LONGITUDE;
          }
        }
        else
        {
          weatherData.Error =
            $"latitude and/or longitude are not in a parseable string. string for latitude: '{latitude}' for longitude: '{longitude}'. Instead, location of CD-HQ is used ";
          latitude = CD_HQ_LATITUDE;
          longitude = CD_HQ_LONGITUDE;
        }
      }
      catch (Exception e)
      {
        weatherData.Error = "Error in parsing parameters for latitude and longitude " + e.Message;
      }
      #endregion
      var uri = string.Format(WEATHER_MAP_API_ENDPOINT, latitude, longitude, primaryKey);

      try
      {
        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage())
        {
          request.Method = HttpMethod.Get;
          request.RequestUri = new Uri(uri);
          var response = await client.SendAsync(request);
          if (!response.IsSuccessStatusCode)
          {
            weatherData.Error = "The request to the weather API failed! Code: '" +
                                response.StatusCode +
                                "' Reason Phrase: '" +
                                response.ReasonPhrase +
                                "' request: '" +
                                response.RequestMessage + "'";
            return weatherData;
          }
          
          //Parsing JSON result. If any key is added, add parsing for the value here
          var responseBody = await response.Content.ReadAsStringAsync();
          var jsonData = (JObject)JsonConvert.DeserializeObject(responseBody);
          var firstResult = jsonData["results"][0];
          weatherData.DateTime = firstResult["dateTime"].Value<DateTime>();
          weatherData.Data.Add("Phrase", firstResult["phrase"].Value<string>());
          weatherData.Data.Add("IconCode", firstResult["iconCode"].Value<double>());
          weatherData.Data.Add("Temperature", firstResult["temperature"]["value"].Value<double>());
          weatherData.Data.Add("TemperatureUnit", firstResult["temperature"]["unit"].Value<string>());
          weatherData.Data.Add("RelativeHumidity", firstResult["relativeHumidity"].Value<double>());
          weatherData.Data.Add("WindDirectionDegrees", firstResult["wind"]["direction"]["degrees"].Value<double>());
          weatherData.Data.Add("WindSpeed", firstResult["wind"]["speed"]["value"].Value<double>());
          weatherData.Data.Add("WindSpeedUnit", firstResult["wind"]["speed"]["unit"].Value<string>());
          weatherData.Data.Add("CloudCover", firstResult["cloudCover"].Value<double>());
          weatherData.Data.Add("Pressure", firstResult["pressure"]["value"].Value<double>());
          weatherData.Data.Add("PressureUnit", firstResult["pressure"]["unit"].Value<string>());
        }
      }
      catch (Exception e)
      {
        weatherData.Error = "Error in reading weather data! " + e.Message;
      }

      return weatherData;
    }
  }
}
