using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HitagiBot.Exceptions;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;

namespace HitagiBot.Services
{
    public static class LastFm
    {
        private static readonly LastfmClient LastfmHandle =
            new LastfmClient(Program.Config["Tokens:LastFM:ApiKey"], Program.Config["Tokens:LastFM:ApiSecret"]);

        public static bool IsValidUsername(string username)
        {
            return username.Length < 15 && username.Length > 2 &&
                   username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-');
        }

        public static async Task<IReadOnlyList<LastTrack>> GetRecentScrobbles(string username)
        {
            var result = await LastfmHandle.User.GetRecentScrobbles(username);

            if (result.Success)
                return result.Content;
            throw new ServiceException(result.Status.ToString());
        }
    }
}