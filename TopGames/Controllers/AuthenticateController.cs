using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TopGames.Models;
using TopGames.Services;


namespace TopGames.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticateController : BaseController
    {
        private IUserService _userService;

        public AuthenticateController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return Unauthorized("Username or password is incorrect!", response);

            return Success("Successfully logged in!", response);
        }
    }
}
