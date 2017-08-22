using System.Collections.Generic;
using System.Threading.Tasks;
using DarkSky.Models;
using DarkSky.Services;
using HitagiBot.Exceptions;

namespace HitagiBot.Services
{
    public static class Weather
    {
        private static readonly DarkSkyService DarkSkyHandle = new DarkSkyService(Program.Config["Tokens:DarkSky"]);

        private static readonly Dictionary<Icon, string> StringMapping = new Dictionary<Icon, string>
        {
            {Icon.ClearDay, "clear"},
            {Icon.ClearNight, "clear"},
            {Icon.PartlyCloudyDay, "partly cloudy"},
            {Icon.PartlyCloudyNight, "partly cloudy"},
            {Icon.Cloudy, "cloudy"},
            {Icon.Rain, "raining"},
            {Icon.Sleet, "sleeting"},
            {Icon.Snow, "snowing"},
            {Icon.Wind, "windy"},
            {Icon.Fog, "foggy"}
        };

        public static async Task<Forecast> GetForecast(double latitude, double longitude)
        {
            var result = await DarkSkyHandle.GetForecast(latitude, longitude);

            if (result.IsSuccessStatus)
                return result.Response;
            throw new ServiceException(result.ResponseReasonPhrase);
        }

        public static string IconToString(this Icon icon)
        {
            return StringMapping[icon];
        }
    }
}