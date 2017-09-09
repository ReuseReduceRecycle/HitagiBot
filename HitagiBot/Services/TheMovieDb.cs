using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Search;

namespace HitagiBot.Services
{
    public static class TheMovieDb
    {
        private static readonly TMDbClient MovieHandle = new TMDbClient(Program.Config["Tokens:TheMovieDB"]);

        private static readonly Dictionary<int, string> GenreMapping = InitGenres().Result;

        public static async Task<List<SearchMovie>> SearchMovie(string searchQuery)
        {
            var movieResponse = await MovieHandle.SearchMovieAsync(searchQuery, CultureInfo.CurrentCulture.ToString());

            return movieResponse?.Results;
        }

        public static string GetGenre(int genreId)
        {
            return GenreMapping[genreId];
        }

        private static async Task<Dictionary<int, string>> InitGenres()
        {
            var genreDictionary = new Dictionary<int, string>();
            var genres = await MovieHandle.GetMovieGenresAsync();

            foreach (var genre in genres)
                genreDictionary.Add(genre.Id, genre.Name);

            return genreDictionary;
        }
    }
}