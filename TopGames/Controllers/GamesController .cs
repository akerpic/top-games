using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TopGames.Helpers;
using TopGames.Models;
using TopGames.Services;

namespace TopGames.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class GamesController : BaseController
    {
        private IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet()]
        public async Task<IActionResult> Get(string date)
        {
            if(date is not null)
            {
                try
                {
                    var gamesByDate = await _gameService.GetGamesByDate(date);
                    return gamesByDate.Any() ? Success("Game records are listed.", gamesByDate) : 
                        Success(string.Format("No record found for requested date {0}", date), gamesByDate);
                }
                catch (FormatException)
                {
                    return BadRequest<Game>("Date format is wrong. It should be like this 'YYYY-MM-DD'", null);
                }
            }
            var games = await _gameService.GetAllGames();
            return games.Any() ? Success("Game records are listed.", games) : Success(string.Format("No record found!"), games);
        }

        [HttpGet("{trackid?}")]
        public async Task<IActionResult> GetByTrackId(string trackid)
        {
            var games = await _gameService.GetByTrackId(trackid);
            return games.Any() ? Success("Game records are listed.", games) : Success(String.Format("No record found for {0}",trackid), games);
        }

        [HttpGet("{trackid?}/Latest")]
        public async Task<IActionResult> GetLatestByTrackId(string trackid)
        {
            var latestGame = await _gameService.GetLatestByTrackId(trackid);
            return latestGame != null ? Success("Latest Game is returned.", latestGame) : Success(String.Format("No record found for {0}", trackid), latestGame);
        }

    }
}
