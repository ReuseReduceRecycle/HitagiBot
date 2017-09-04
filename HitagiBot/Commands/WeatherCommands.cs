using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DarkSky.Models;
using Geocoding.Google;
using HitagiBot.Data;
using HitagiBot.Services;
using HitagiBot.Utilities;
using HitagiBot.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HitagiBot.Commands
{
    public static class WeatherCommands
    {
        private static string FormatWeather<T>(Forecast forecast, T locationInfo)
        {
            if (!forecast.Currently.Temperature.HasValue)
                return Strings.WeatherError;

            string formattedAddress;
            double temperature;
            char unit;

            switch (locationInfo)
            {
                case GoogleAddress address:
                    unit = address.IsAmerica() ? 'F' : 'C';
                    formattedAddress = address.FormattedAddress;
                    break;
                case UserLocation userLocation:
                    unit = userLocation.IsAmerica ? 'F' : 'C';
                    formattedAddress = userLocation.FormattedAddress;
                    break;
                default:
                    throw new ArgumentException("locationInfo is neither GoogleAddress or UserLocation...");
            }

            if (unit == 'C')
                temperature = 5.0 / 9.0 * (forecast.Currently.Temperature.Value - 32);
            else
                temperature = forecast.Currently.Temperature.Value;

            return string.Format(Strings.WeatherResult, temperature, unit, forecast.Currently.Icon.IconToString(), formattedAddress);
        }

        private static async Task<string> GetUserWeather(int id)
        {
            using (var context = new TelegramContext())
            {
                await context.Database.EnsureCreatedAsync();
                var location = await context.UserLocations.FindAsync(id);

                if (location != null)
                {
                    var weatherInfo = await Weather.GetForecast(location.Latitude, location.Longitude);

                    return FormatWeather(weatherInfo, location);
                }
                return Strings.WeatherDefault;
            }
        }

        private static async Task<string> GetLocationWeather(string location)
        {
            var addressResults = await Geocoder.GetAddresses(location);
            var address = addressResults.FirstOrDefault();

            if (address != null)
            {
                var weatherInfo = await Weather.GetForecast(address.Coordinates.Latitude,
                    address.Coordinates.Longitude);

                return FormatWeather(weatherInfo, address);
            }
            return Strings.WeatherNotFound;
        }

        public static async Task Forecast(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            await botHandle.SendSmartTextMessageAsync(source, "Coming soon!");
        }

        public static async Task WeatherCommand(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            string messageText;

            if (string.IsNullOrWhiteSpace(matches[2].Value))
                messageText = await GetUserWeather(source.From.Id);
            else
                messageText = await GetLocationWeather(matches[2].Value);

            await botHandle.SendSmartTextMessageAsync(source, messageText);
        }
    }
}