using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopGames.EntityFramework;
using TopGames.Helpers;
using TopGames.Models;

namespace TopGames.Services
{
    public interface IGameService
    {
        Task<IList<Game>> GetAllGames();
        Task<IList<Game>> GetGamesByDate(string date);
        Task<IList<Game>> GetByTrackId(string trackid);
        Task<Game> GetLatestByTrackId(string trackid);
    }

    public class GameService : IGameService
    {
        private readonly HerokuDbContext _context;
        public GameService( HerokuDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Game>> GetAllGames()
        {
            return await _context.game.ToListAsync();
        }
        
        public async Task<IList<Game>> GetGamesByDate(string date)
        {
            try
            {
                var requestedDate = Convert.ToDateTime(date);
                return await _context.game.Where(x => x.created_date.Date == requestedDate.Date).ToListAsync();
            }
            catch(FormatException)
            {
                throw;
            }
        }

        public async Task<IList<Game>> GetByTrackId(string trackid)
        {
            return await _context.game.Where(x => x.track_id == trackid).ToListAsync();
        }

        public async Task<Game> GetLatestByTrackId(string trackid)
        {
            return await _context.game.Where(g => g.track_id == trackid).OrderByDescending(g => g.created_date).FirstOrDefaultAsync();
        }
    }
}
