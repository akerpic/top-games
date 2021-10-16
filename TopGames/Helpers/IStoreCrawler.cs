using System.Collections.Generic;
using System.Threading.Tasks;
using TopGames.Models;

namespace TopGames.Helpers
{
    public interface IStoreCrawler
    {
        public Task<List<Game>> GetTopGames();
    }
}
