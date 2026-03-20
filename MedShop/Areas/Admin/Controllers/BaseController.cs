using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MedShop.Areas.Admin.AdminConstants;

namespace MedShop.Areas.Admin.Controllers
{
    [Area(AreaName)]
    [Route("Admin/[controller]/[Action]/{id?}")]
    [Authorize(Roles = AdminRoleName)]
    public class BaseController : Controller
    {
    }
}
