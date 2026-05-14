using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    // Shared API controller configuration. Persistence is accessed only through application services.

    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
    }
}
