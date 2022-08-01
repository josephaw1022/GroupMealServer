
namespace GroupMealApi.Utilities;



public static class AppEnv
{

    private static readonly string? _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");


    public static string ConnectionString
    {
        get
        {
            if (_connectionString == null)
            {
                return string.Format(Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "");
            }
            return _connectionString;
        }
    }


    public static string DatabaseName
    {
        get
        {
            return Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "";
        }
    }





}
