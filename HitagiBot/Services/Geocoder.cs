using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geocoding.Google;

namespace HitagiBot.Services
{
    public static class Geocoder
    {
        private static readonly GoogleGeocoder GeocoderHandle =
            new GoogleGeocoder(Program.Config["Tokens:Google Geocoder"]);

        public static async Task<IEnumerable<GoogleAddress>> GetAddresses(string query)
        {
            return await GeocoderHandle.GeocodeAsync(query);
        }

        public static async Task<IEnumerable<GoogleAddress>> GetAddressFromCoordinates(int latitude, int longitude)
        {
            return await GeocoderHandle.ReverseGeocodeAsync(latitude, longitude);
        }

        public static bool IsAmerica(this GoogleAddress address)
        {
            var americaCheck = from component in address.Components
                where component.ShortName.Equals("US")
                      && component.Types.Contains(GoogleAddressType.Country)
                      && component.Types.Contains(GoogleAddressType.Political)
                select component;

            return americaCheck.Any();
        }
    }
}