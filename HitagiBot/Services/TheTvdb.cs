using System.Collections.Generic;
using System.Threading.Tasks;
using TheTVDBSharp;
using TheTVDBSharp.Models;

namespace HitagiBot.Services
{
    public static class TheTvdb
    {
        private static readonly TheTVDBManager TelevisionHandle = new TheTVDBManager("CEBA8E78DC4B6BF4");

        public static async Task<IReadOnlyCollection<Series>> SearchTvShows(string searchQuery)
        {
            return await TelevisionHandle.SearchSeries(searchQuery, Language.English);
        }

        public static async Task<Series> GetShow(uint seriesId)
        {
            return await TelevisionHandle.GetSeries(seriesId, Language.English);
        }
    }
}