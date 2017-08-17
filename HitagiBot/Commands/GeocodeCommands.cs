using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Geocoding.Google;
using HitagiBot.Data;
using HitagiBot.Services;
using HitagiBot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HitagiBot.Commands
{
    public static class GeocodeCommands
    {
        private static async Task AddOrUpdateLocation(int id, GoogleAddress address)
        {
            using (var telegramContext = new TelegramContext())
            {
                await telegramContext.Database.EnsureCreatedAsync();
                var locationProfile = await telegramContext.UserLocations.FindAsync(id);

                if (locationProfile != null)
                {
                    locationProfile.Latitude = address.Coordinates.Latitude;
                    locationProfile.Longitude = address.Coordinates.Longitude;
                    locationProfile.FormattedAddress = address.FormattedAddress;
                    locationProfile.IsAmerica = address.IsAmerica();
                }
                else
                {
                    await telegramContext.UserLocations.AddAsync(
                        new UserLocation
                        {
                            Id = id,
                            Latitude = address.Coordinates.Latitude,
                            Longitude = address.Coordinates.Longitude,
                            FormattedAddress = address.FormattedAddress,
                            IsAmerica = address.IsAmerica()
                        }
                    );
                }

                await telegramContext.SaveChangesAsync();
            }
        }

        private static string FormatGeolocation(GoogleAddress address)
        {
            var geolocationText = new StringBuilder();

            geolocationText.AppendFormat("<b>{0}</b>", address.FormattedAddress);
            geolocationText.AppendFormat("\n<b>Latitude</b>: {0}", address.Coordinates.Latitude);
            geolocationText.AppendFormat("\n<b>Longitude</b>: {0}", address.Coordinates.Longitude);

            return geolocationText.ToString();
        }

        public static async Task SetLocation(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            var responseText = "What location should I set it to (・∧‐)ゞ?";

            if (!string.IsNullOrWhiteSpace(matches[2].Value))
            {
                var addresses = await Geocoder.GetAddresses(matches[2].Value);
                var address = addresses?.FirstOrDefault();

                if (address != null)
                {
                    await AddOrUpdateLocation(source.From.Id, address);

                    responseText = $"Alright, I've set your location to {address.FormattedAddress}.";
                }
                else
                {
                    responseText = "I couldn't find this location ┐(‘～`；)┌";
                }
            }

            await botHandle.SendSmartTextMessageAsync(source, responseText);
        }

        public static async Task Geocode(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            var responseText = "Where would you like coordinates for (・∧‐)ゞ?";

            if (!string.IsNullOrWhiteSpace(matches[2].Value))
            {
                var addresses = await Geocoder.GetAddresses(matches[2].Value);
                var address = addresses?.FirstOrDefault();

                responseText = address != null ? FormatGeolocation(address) : "I couldn't find this location ┐(‘～`；)┌";
            }

            await botHandle.SendSmartTextMessageAsync(source, responseText, ParseMode.Html);
        }
    }
}