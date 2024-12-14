namespace backend.Classes.Data
{
    /// <summary>
    /// Defines the <see cref="dbConstants" />
    /// </summary>
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
