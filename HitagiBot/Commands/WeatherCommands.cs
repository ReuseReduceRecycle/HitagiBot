using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DarkSky.Models;
using Geocoding.Google;
using HitagiBot.Data;
using HitagiBot.Services;
using HitagiBot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HitagiBot.Commands
{
    public static class WeatherCommands
    {
        private static string FormatWeather(Forecast forecast, GoogleAddress address)
        {
            var formattedWeather = new StringBuilder();

            if (forecast.Currently.Temperature.HasValue)
            {
                var temperature = forecast.Currently.Temperature.Value;

                if (address.IsAmerica())
                    formattedWeather.AppendFormat("It's {0:0.00}°F and ", temperature);
                else
                    formattedWeather.AppendFormat("It's {0:0.00}°C and ", 5.0 / 9.0 * (temperature - 32));

                formattedWeather.AppendFormat("{0} in {1}", forecast.Currently.Icon.ToEmoji(),
                    address.FormattedAddress);

                return formattedWeather.ToString();
            }
            return "Something broke while I was looking at the weather【・ヘ・】";
        }

        private static string FormatWeather(Forecast forecast, UserLocation userLocation)
        {
            var formattedWeather = new StringBuilder();

            if (forecast.Currently.Temperature.HasValue)
            {
                var temperature = forecast.Currently.Temperature.Value;

                if (userLocation.IsAmerica)
                    formattedWeather.AppendFormat($"It's {0:0.00}°F and ", temperature);
                else
                    formattedWeather.AppendFormat("It's {0:0.00}°C and ", 5.0 / 9.0 * (temperature - 32));

                formattedWeather.AppendFormat("{0} in {1}", forecast.Currently.Icon.ToEmoji(),
                    userLocation.FormattedAddress);

                return formattedWeather.ToString();
            }
            return "Something broke while I was looking at the weather【・ヘ・】";
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
                return "Where should I look? (・∧‐)ゞ";
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
            return "I don't know where that is ＼| ￣ヘ￣|／";
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