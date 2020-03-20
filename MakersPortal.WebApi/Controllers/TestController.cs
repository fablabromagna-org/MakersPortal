using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MakersPortal.WebApi.Controllers
{ 
    [ApiController]
    public class TestController : Controller
    {
        [Route("/Index")]
        [Authorize]
        public string Index()
        {
            return "Hello, World.";
        }
    }
}