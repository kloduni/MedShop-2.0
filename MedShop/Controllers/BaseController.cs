using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedShop.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
    }
}
