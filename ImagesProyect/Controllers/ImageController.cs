﻿using Microsoft.AspNetCore.Mvc;
using ImagesProyect.Services;
using ImagesProyect.Models;

namespace ImagesProyect.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageController : ControllerBase
    {

        private readonly ImageService _imageService;

        public ImageController(ImageService imageService)
        {
            _imageService = imageService;
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
        public IActionResult UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Debe proporcionar un archivo válido.");

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = Path.GetFileName(file.FileName);
            string filePath = Path.Combine("wwwroot/uploads", fileName);

            try
            {
                // Guardar físicamente el archivo en el servidor
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                // Guardar la imagen en la base de datos
                bool success = _imageService.SaveImage(fileName, filePath);
                if (!success) return StatusCode(500, "No se pudo guardar la imagen.");

                return Ok(new { message = "Imagen subida exitosamente." });
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Error al subir la imagen: {ex.Message}");
            }

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
