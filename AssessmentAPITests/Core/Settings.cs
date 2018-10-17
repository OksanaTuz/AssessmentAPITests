using System.Configuration;

namespace Core
{
    public static class Settings
    {
        public static string ApiBasePath => ConfigurationManager.AppSettings["ApiBasePath"];
    }
}
