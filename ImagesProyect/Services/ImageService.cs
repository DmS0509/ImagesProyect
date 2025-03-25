using System.Data;
using ImagesProyect.Data;
using ImagesProyect.Models;
using MySql.Data.MySqlClient;

namespace ImagesProyect.Services
{
    public class ImageService
    {
        private readonly DatabaseHelper _dbHelper;

        public ImageService(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        /// <summary>
        /// Obtiene la lista de imágenes almacenadas en la base de datos.
        /// </summary>
        public List<ImageModel> GetAllImages()
        {
            var images = new List<ImageModel>();

            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                using (var cmd = new MySqlCommand("SELECT * FROM Images", connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            images.Add(new ImageModel
                            {
                                Id = reader.GetInt32("Id"),
                                FileName = reader.GetString("FileName"),
                                FilePath = reader.GetString("FilePath"),
                                UploadDate = reader.GetDateTime("UploadDate"),
                                NodeId = reader.GetInt32("NodeId")
                            });
                        }
                    }
                }
            }

            return images;
        }

        /// <summary>
        /// Guarda una nueva imagen en el nodo con más espacio disponible.
        /// </summary>
        public bool SaveImage(string fileName, string filePath)
        {
            int nodeId = GetBestStorageNode();
            if (nodeId == -1) return false; // No hay nodos disponibles

            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                using (var cmd = new MySqlCommand("INSERT INTO Images (FileName, FilePath, NodeId) VALUES (@FileName, @FilePath, @NodeId)", connection))
                {
                    cmd.Parameters.AddWithValue("@FileName", fileName);
                    cmd.Parameters.AddWithValue("@FilePath", filePath);
                    cmd.Parameters.AddWithValue("@NodeId", nodeId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Encuentra el nodo de almacenamiento con más espacio disponible.
        /// </summary>
        private int GetBestStorageNode()
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                using (var cmd = new MySqlCommand("SELECT Id FROM StorageNodes ORDER BY AvailableSpace DESC LIMIT 1", connection))
                {
                    object? result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }

        /// <summary>
        /// Elimina una imagen por ID.
        /// </summary>
        public bool DeleteImage(int imageId)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                using (var cmd = new MySqlCommand("DELETE FROM Images WHERE Id = @ImageId", connection))
                {
                    cmd.Parameters.AddWithValue("@ImageId", imageId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
