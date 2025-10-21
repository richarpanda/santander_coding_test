using CodingTest.Models;
using CodingTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class StoriesController : ControllerBase
    {
        private readonly IHackerNewsService _hackerNewsService;
        private readonly ILogger<StoriesController> _logger;

        public StoriesController(IHackerNewsService hackerNewsService, ILogger<StoriesController> logger)
        {
            _logger = logger;
            _hackerNewsService = hackerNewsService;
        }

        [HttpGet("beststories/{n:int}")]
        public async Task<IActionResult> GetBestStories([FromRoute] int n, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Request received for {Count} best stories", n);

            if (n <= 0)
            {
                return BadRequest(new ErrorResponse
                {
                    Error = "Parameter 'n' must be greater than 0",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            try
            {
                var stories = await _hackerNewsService.GetBestStoriesAsync(n, cancellationToken);
                _logger.LogInformation("Successfully retrieved {Count} stories", stories.Count);

                return Ok(stories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching stories");

                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Error = "An error occurred while retrieving stories",
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Details = ex.Message
                });
            }
        }


    }
}