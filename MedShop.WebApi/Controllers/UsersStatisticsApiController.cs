using MedShop.Core.Contracts;
using MedShop.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedShop.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersStatisticsApiController : ControllerBase
    {
        private readonly IUserStatisticsService userStatisticsService;

        public UsersStatisticsApiController(IUserStatisticsService _userStatisticsService)
        {
            userStatisticsService = _userStatisticsService;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(UserStatisticsService))]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUsersStatistics()
        {
            var model = await userStatisticsService.UsersInfo();

            return Ok(model);
        }
    }
}
