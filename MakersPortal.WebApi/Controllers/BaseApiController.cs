using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MakersPortal.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public abstract class BaseApiController : Controller
    {
    }
}