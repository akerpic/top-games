using TopGames.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace TopGames.Controllers
{
	[ApiController]
	[Route("[controller]")]
    public class BaseController : ControllerBase
    {
		[NonAction]
		public IActionResult Success<T>(string message, T data)
		{
			return Success(new Response<T>
			{
				Success = true,
				Message = message,
				Data = data
			});
		}

		[NonAction]
		public IActionResult Success<T>(Response<T> data)
		{
			return Ok(data);
		}

		[NonAction]
		protected IActionResult NoContent<T>(Response<T> data)
		{
			return StatusCode(204, data);
		}

		[NonAction]
		protected IActionResult BadRequest<T>(string message, T data)
		{
			return BadRequest(new Response<T>
			{
				Success = false,
				Message = message,
				Data = data
			});
		}

		[NonAction]
		protected IActionResult BadRequest<T>(Response<T> data)
		{
			return StatusCode(400, data);
		}

		[NonAction]
		protected IActionResult Unauthorized<T>(string message, T data)
		{
			return Unauthorized(new Response<T>
			{
				Success = false,
				Message = message,
				Data = data
			});
		}

		[NonAction]
		protected IActionResult Unauthorized<T>(Response<T> data)
		{
			return StatusCode(401, data);
		}


		[NonAction]
		protected IActionResult NotFound<T>(Response<T> data)
		{
			return StatusCode(404, data);
		}

		[NonAction]
		protected IActionResult Error<T>(string message, T data)
		{
			return Error(new Response<T>
			{
				Success = false,
				Message = message,
				Data = data
			});
		}

		[NonAction]
		protected IActionResult Error<T>(Response<T> data)
		{
			return StatusCode(500, data);
		}
	}
}
