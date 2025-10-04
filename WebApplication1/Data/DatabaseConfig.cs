namespace WebApplication1.Data
{
    public class DatabaseConfig
    {
        public string Host { get; set; } = "localhost";
        public string Database { get; set; } = "multasdb";
        public string Username { get; set; } = "postgres";
        public string Password { get; set; } = "postgres";
        public int Port { get; set; } = 5432;
        public bool SslMode { get; set; } = false;

        public string GetConnectionString()
        {
            return $"Host={Host};Database={Database};Username={Username};Password={Password};Port={Port};SSL Mode={(SslMode ? "Require" : "Disable")};Trust Server Certificate=true;";
        }
    }
}
