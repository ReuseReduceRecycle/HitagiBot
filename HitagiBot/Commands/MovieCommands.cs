using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HitagiBot.Services;
using HitagiBot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TMDbLib.Objects.Search;

namespace HitagiBot.Commands
{
    public static class MovieCommands
    {
        private static string FormatMovie(SearchMovie movie)
        {
            var formattedMessage = new StringBuilder();

            formattedMessage.AppendFormat("<b>{0}", movie.Title);
            if (movie.ReleaseDate != null)
                formattedMessage.AppendFormat(" ({0})</b>", movie.ReleaseDate.Value.Year);
            else
                formattedMessage.Append("</b>");

            var genres = movie.GenreIds?.Select(TheMovieDb.GetGenre);
            if (genres != null)
                formattedMessage.AppendFormat("\n{0}", string.Join(", ", genres.ToArray()));

            if (movie.VoteCount > 0)
                formattedMessage.Append($"\n★ {movie.VoteAverage}  - {movie.VoteCount} votes");

            formattedMessage.Append(GetImage(movie));
            formattedMessage.AppendFormat("\n\n{0}", movie.Overview);

            return formattedMessage.ToString();
        }

        private static string GetImage(SearchMovie movie)
        {
            if (movie.BackdropPath != null)
                return MessagerHelper.InsertInvisibleLink("https://image.tmdb.org/t/p/w780/" + movie.BackdropPath,
                    ParseMode.Html);
            if (movie.PosterPath != null)
                return MessagerHelper.InsertInvisibleLink("https://image.tmdb.org/t/p/w500/" + movie.PosterPath,
                    ParseMode.Html);
            return string.Empty;
        }

        public static async Task Movie(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            var replyText = "Which movie should I look for ( •᷄ὤ•᷅)？";

            if (!string.IsNullOrWhiteSpace(matches[2].Value))
            {
                var movies = await TheMovieDb.SearchMovie(matches[2].Value);

                replyText = movies.Count > 0 ? FormatMovie(movies.First()) : "I couldn't find this movie ¯\\_(⊙︿⊙)_/¯";
            }

            await botHandle.SendSmartTextMessageAsync(source, replyText, ParseMode.Html);
        }
    }
}