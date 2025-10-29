namespace WebApplication1.Data
{
    public class DatabaseConfig
    {
        public string DatabasePath { get; set; } = "Data Source=multas.db";
        public string DatabaseName { get; set; } = "multas.db";

        public string GetConnectionString()
        {
            return DatabasePath;
        }
    }
}
