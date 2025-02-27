namespace backend.Classes.Data
{
    public static class dbConstants
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionString"];
            if (connectionString == null)
            {
                throw new Exception("ConnectionString secret was null");
            }
            return connectionString;
        }
    }
}