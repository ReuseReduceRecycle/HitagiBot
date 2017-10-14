using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HitagiBot.Data;
using HitagiBot.Exceptions;
using HitagiBot.Localization;
using HitagiBot.Services;
using HitagiBot.Utilities;
using IF.Lastfm.Core.Objects;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HitagiBot.Commands
{
    public static class LastFmCommands
    {
        private static async Task<string> GetUsername(int id)
        {
            using (var context = new TelegramContext())
            {
                await context.Database.EnsureCreatedAsync();

                var profile = await context.UserProfiles.FindAsync(id);


                return profile?.LastFm;
            }
        }

        private static async Task AddOrUpdateUsername(int id, string newUsername)
        {
            using (var context = new TelegramContext())
            {
                await context.Database.EnsureCreatedAsync();

                var profile = await context.UserProfiles.FindAsync(id);

                if (profile == null)
                {
                    var newProfile = new UserProfile {Id = id, LastFm = newUsername};
                    await context.UserProfiles.AddAsync(newProfile);
                    await context.SaveChangesAsync();
                }
                else
                {
                    profile.LastFm = newUsername;
                    await context.SaveChangesAsync();
                }
            }
        }

        private static string FormatLastPlayed(string username, LastTrack track)
        {
            var formattedMessage = new StringBuilder();

            formattedMessage.AppendFormat(
                track.IsNowPlaying.HasValue ? Strings.LastFMCurrentPlaying : Strings.LastFMLastPlayed, username);

            formattedMessage.AppendFormat("\n{0} ♪ {1}", track.ArtistName, track.Name);

            return formattedMessage.ToString();
        }

        private static async Task<LastTrack> GetLastTrack(string username)
        {
            try
            {
                var tracks = await LastFm.GetRecentScrobbles(username);

                return tracks.FirstOrDefault();
            }
            catch (ServiceException)
            {
                return null;
            }
        }

        private static async Task<string> SafeLastTrackString(string username)
        {
            var track = await GetLastTrack(username);

            return track != null
                ? FormatLastPlayed(username, track)
                : Strings.LastFMScrobbleError;
        }

        public static async Task SetLastFm(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            var responseText = Strings.LastFMUsername;

            if (!string.IsNullOrWhiteSpace(matches[2].Value))
                if (LastFm.IsValidUsername(matches[2].Value))
                {
                    await AddOrUpdateUsername(source.From.Id, matches[2].Value);
                    responseText = string.Format(Strings.LastFMSetUsername, matches[2].Value);
                }
                else
                {
                    responseText = Strings.LastFMInvalidUsername;
                }

            await botHandle.SendSmartTextMessageAsync(source, responseText);
        }

        public static async Task LastPlayed(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            var responseText = Strings.LastFMDefault;

            if (string.IsNullOrWhiteSpace(matches[2].Value))
            {
                var username = await GetUsername(source.From.Id);

                if (!string.IsNullOrWhiteSpace(username))
                    responseText = await SafeLastTrackString(username);
            }
            else
            {
                var username = matches[2].Value;

                responseText = LastFm.IsValidUsername(username)
                    ? await SafeLastTrackString(username)
                    : Strings.LastFMInvalidUsername;
            }

            await botHandle.SendSmartTextMessageAsync(source, responseText);
        }
    }
}