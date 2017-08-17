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

        private static readonly Dictionary<Icon, string> EmojiMapping = new Dictionary<Icon, string>
        {
            {Icon.ClearDay, "☀️"},
            {Icon.ClearNight, "🌕"},
            {Icon.PartlyCloudyDay, "⛅️"},
            {Icon.PartlyCloudyNight, "⛅️"},
            {Icon.Cloudy, "☁️"},
            {Icon.Rain, "🌧"},
            {Icon.Sleet, "🌨"},
            {Icon.Snow, "❄️"},
            {Icon.Wind, "💨"},
            {Icon.Fog, "🌫️"}
        };

        public static async Task<Forecast> GetForecast(double latitude, double longitude)
        {
            var result = await DarkSkyHandle.GetForecast(latitude, longitude);

            if (result.IsSuccessStatus)
                return result.Response;
            throw new ServiceException(result.ResponseReasonPhrase);
        }

        public static string ToEmoji(this Icon icon)
        {
            return EmojiMapping[icon];
        }
    }
}