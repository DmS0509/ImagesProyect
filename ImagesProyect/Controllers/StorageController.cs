using Microsoft.AspNetCore.Mvc;

namespace ImagesProyect.Controllers
{
    [Route("api/storage")]
    [ApiController]
    public class StorageController : Controller
    {
        [HttpPost("upload")]
        public IActionResult UploadImage()
        {
            return Ok("Imagen subida correctamente.");
        }
    }
}
