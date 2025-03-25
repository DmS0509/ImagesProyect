using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace ImagesProyect.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

         public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Connection string is missing in appsettings.json");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
