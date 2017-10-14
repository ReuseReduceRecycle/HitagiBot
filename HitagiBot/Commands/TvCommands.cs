using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HitagiBot.Localization;
using HitagiBot.Services;
using HitagiBot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TheTVDBSharp.Models;

namespace HitagiBot.Commands
{
    public static class TvCommands
    {
        private static string MessageFormatter(Series tvShow)
        {
            var formattedMessage = new StringBuilder();

            formattedMessage.AppendFormat("<b>{0}", tvShow.Title);

            if (tvShow.FirstAired.HasValue)
                formattedMessage.AppendFormat(" ({0})</b>", tvShow.FirstAired.Value.Year);
            else
                formattedMessage.Append("</b>");

            if (tvShow.Genres != null)
                formattedMessage.AppendFormat("\n{0}", string.Join(", ", tvShow.Genres));

            if (tvShow.Rating.HasValue && tvShow.RatingCount.HasValue)
                formattedMessage.Append($"\n★ {tvShow.Rating.Value}  - {tvShow.RatingCount.Value} votes");

            formattedMessage.Append(GetImage(tvShow));
            formattedMessage.AppendFormat("\n\n{0}", tvShow.Description);

            return formattedMessage.ToString();
        }

        private static string GetImage(Series tvShow)
        {
            var bannerText = string.Empty;

            if (tvShow.Banners.Count > 0)
                bannerText =
                    MessagerHelper.InsertInvisibleLink(
                        "http://thetvdb.com/banners/" + tvShow.Banners.First().RemotePath, ParseMode.Html);
            else if (!string.IsNullOrWhiteSpace(tvShow.BannerRemotePath))
                bannerText = MessagerHelper.InsertInvisibleLink("http://thetvdb.com/banners/" + tvShow.BannerRemotePath,
                    ParseMode.Html);
            else if (!string.IsNullOrWhiteSpace(tvShow.PosterRemotePath))
                bannerText = MessagerHelper.InsertInvisibleLink("http://thetvdb.com/banners/" + tvShow.PosterRemotePath,
                    ParseMode.Html);

            return bannerText;
        }

        public static async Task TvShow(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            var replyText = Strings.TVDefault;

            if (!string.IsNullOrWhiteSpace(matches[2].Value))
            {
                var shows = await TheTvdb.SearchTvShows(matches[2].Value);

                if (shows.Count > 0)
                {
                    var firstResult = shows.First();

                    var show = await TheTvdb.GetShow(firstResult.Id);

                    replyText = MessageFormatter(show);
                }
                else
                {
                    replyText = Strings.TVNoResults;
                }
            }

            await botHandle.SendSmartTextMessageAsync(source, replyText, ParseMode.Html);
        }
    }
}