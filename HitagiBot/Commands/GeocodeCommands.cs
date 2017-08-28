using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Geocoding.Google;
using HitagiBot.Data;
using HitagiBot.Localization;
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
            return string.Format(Strings.GeocodeResult, address.FormattedAddress, address.Coordinates.Latitude,
                address.Coordinates.Longitude);
        }

        public static async Task SetLocation(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            var responseText = Strings.GeocodeDefault;

            if (!string.IsNullOrWhiteSpace(matches[2].Value))
            {
                var addresses = await Geocoder.GetAddresses(matches[2].Value);
                var address = addresses?.FirstOrDefault();

                if (address != null)
                {
                    await AddOrUpdateLocation(source.From.Id, address);

                    responseText = string.Format(Strings.GeocodeLocationSet, address.FormattedAddress);
                }
                else
                {
                    responseText = Strings.GeocodeNotFound;
                }
            }

            await botHandle.SendSmartTextMessageAsync(source, responseText);
        }

        public static async Task Geocode(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            var responseText = Strings.GeocodeDefault;

            if (!string.IsNullOrWhiteSpace(matches[2].Value))
            {
                var addresses = await Geocoder.GetAddresses(matches[2].Value);
                var address = addresses?.FirstOrDefault();

                responseText = address != null ? FormatGeolocation(address) : Strings.GeocodeNotFound;
            }

            await botHandle.SendSmartTextMessageAsync(source, responseText, ParseMode.Html);
        }
    }
}