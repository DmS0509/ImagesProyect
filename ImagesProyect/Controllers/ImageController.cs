using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ImagesProyect.Services;
using ImagesProyect.Models;
using Microsoft.AspNetCore.Http;

namespace ImagesProyect.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageController : ControllerBase
    {

        private readonly StorageNodeService _storageNodeService;
        private readonly ImageService _imageService;
        private readonly HttpClient _httpClient;

        public ImageController(StorageNodeService storageNodeService, ImageService imageService)
        {
            _storageNodeService = storageNodeService;
            _imageService = imageService;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Obtener la lista de imágenes almacenadas.
        /// </summary>
        [HttpGet]
        public IActionResult GetAllImages()
        {
            var images = _imageService.GetAllImages();
            return Ok(images);
        }

        /// <summary>
        /// Sube una imagen al sistema.
        /// </summary>
        [HttpPost]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Debe proporcionar un archivo válido.");

            // Obtener el mejor nodo de almacenamiento disponible
            int bestNodeId = await _storageNodeService.GetBestStorageNodeAsync();
            if (bestNodeId == -1)
                return StatusCode(500, "No hay nodos de almacenamiento disponibles.");

            // Obtener la IP del nodo usando el NodeId
            StorageNode? bestNode = await _storageNodeService.GetNodeByIdAsync(bestNodeId);
            if (bestNode == null)
                return StatusCode(500, "No se pudo encontrar la IP del nodo de almacenamiento.");

            // Enviar la imagen al nodo seleccionado
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var content = new MultipartFormDataContent
        {
            { new ByteArrayContent(stream.ToArray()), "file", file.FileName }
        };

                HttpResponseMessage response = await _httpClient.PostAsync($"http://{bestNode.IP}/api/storage/upload", content);

                if (!response.IsSuccessStatusCode)
                    return StatusCode(500, "No se pudo almacenar la imagen en el nodo seleccionado.");
            }

            // Registrar la imagen en la base de datos con el NodeId
            bool success = _imageService.SaveImage(file.FileName, bestNodeId);
            if (!success) return StatusCode(500, "No se pudo registrar la imagen en la base de datos.");

            return Ok(new { message = "Imagen subida exitosamente." });
        }


        /// <summary>
        /// Elimina una imagen por ID.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeleteImage(int id)
        {
            bool success = _imageService.DeleteImage(id);
            if (!success) return NotFound("Imagen no encontrada.");

            return Ok(new { message = "Imagen eliminada correctamente." });
        }

        [HttpGet("uploads/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var filePath = Path.Combine("wwwroot/uploads", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Imagen no encontrada.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "image/jpeg"); // Cambia el tipo MIME según sea necesario
        }
    }
}
