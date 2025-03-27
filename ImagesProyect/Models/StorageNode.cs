using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImagesProyect.Models
{
    public class StorageNode
    {
        public int Id { get; set; }  // Identificador en la BD
        public string IP { get; set; } = ""; // Dirección IP del nodo
        public string? Name { get; set; }  // Nombre del nodo
        public string? BaseUrl { get; set; }  // URL base donde está expuesto el nodo
        public long AvailableSpace { get; set; } // Espacio en KB o MB
    }

    public class StorageManager
    {
        private readonly List<string> _nodes;
        private readonly HttpClient _httpClient;

        public StorageManager(List<string> nodes)
        {
            _nodes = nodes;
            _httpClient = new HttpClient();
        }

        public async Task<List<StorageNode>> GetNodesWithAvailableSpace()
        {
            var nodesWithSpace = new List<StorageNode>();

            foreach (var node in _nodes)
            {
                try
                {
                    string url = $"http://{node}/api/storage/available"; // Endpoint en cada nodo
                    HttpResponseMessage response = await _httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        int availableSpace = JsonConvert.DeserializeObject<int>(json);
                        nodesWithSpace.Add(new StorageNode { IP = node, AvailableSpace = availableSpace });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error consultando el nodo {node}: {ex.Message}");
                }
            }

            return nodesWithSpace;
        }

        public async Task<string?> GetBestStorageNode()
        {
            var nodesWithSpace = await GetNodesWithAvailableSpace();

            if (nodesWithSpace.Count == 0)
                return null;

            var bestNode = nodesWithSpace.OrderByDescending(n => n.AvailableSpace).First();
            return bestNode.IP;
        }
    }

}
