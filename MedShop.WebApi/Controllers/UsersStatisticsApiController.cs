using MedShop.Core.Contracts;
using MedShop.Core.Models.Admin;
using MedShop.Core.Models.User;
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
        [ProducesResponseType(200, Type = typeof(StatisticsViewModel))]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUsersStatistics()
        {
            var model = await userStatisticsService.UsersInfo();

            return Ok(model);
        }

        [HttpGet("categories")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryStatModel>))]
        public async Task<IActionResult> GetCategoryStats()
        {
            var model = await userStatisticsService.GetProductsByCategory();
            return Ok(model);
        }
    }
}
