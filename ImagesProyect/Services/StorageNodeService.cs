using System;
using System.Collections.Generic;
using System.IO;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using ImagesProyect.Models;
using System.Data;
using ImagesProyect.Data;
using System.Threading.Tasks; // Asegura que esta línea esté presente

namespace ImagesProyect.Services
{
    public class StorageNodeService
    {
        private readonly DatabaseHelper _dbHelper; // <-- Usar DatabaseHelper

        public StorageNodeService(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<List<StorageNode>> GetAllStorageNodesAsync()
        {
            using var connection = _dbHelper.GetConnection();
            await connection.OpenAsync();

            using var cmd = new MySqlCommand("SELECT Id, BaseUrl, AvailableSpace FROM StorageNodes", connection);

            using var reader = await cmd.ExecuteReaderAsync();

            var nodes = new List<StorageNode>();
            while (await reader.ReadAsync())
            {
                nodes.Add(new StorageNode
                {
                    Id = reader.GetInt32("Id"),
                    BaseUrl = reader.GetString("BaseUrl"), // Cambio de "IP" a "BaseUrl"
                    AvailableSpace = reader.GetInt64("AvailableSpace") // Cambiado a GetInt64
                });
            }

            return nodes;
        }


        public async Task<int> GetBestStorageNodeAsync()
        {
            var nodes = await GetAllStorageNodesAsync();
            if (!nodes.Any()) return -1;

            var bestNode = nodes.OrderByDescending(n => n.AvailableSpace).FirstOrDefault();
            return bestNode?.Id ?? -1;
        }

        public async Task<StorageNode?> GetNodeByIdAsync(int nodeId)
        {
            using var connection = _dbHelper.GetConnection();
            await connection.OpenAsync();

            using var cmd = new MySqlCommand("SELECT Id, BaseUrl, AvailableSpace FROM StorageNodes WHERE Id = @NodeId", connection);
            cmd.Parameters.AddWithValue("@NodeId", nodeId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new StorageNode
                {
                    Id = reader.GetInt32("Id"),
                    BaseUrl = reader.GetString("BaseUrl"), // Cambio de "IP" a "BaseUrl"
                    AvailableSpace = reader.GetInt64("AvailableSpace") // Cambiado a GetInt64
                };
            }
            return null;
        }

    }
}
