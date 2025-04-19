namespace ShufflerAPI
{
    internal class ConfigHelper
    {
        private readonly IConfigurationRoot _config;
        public string ShufflerDbConnectionStringName { get { return "ShufflerDataConnection"; } }

        public ConfigHelper()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            _config = builder.Build();
        }

        public string GetConnectionString(string name)
        {
            return _config.GetConnectionString(name)!;
        }
    }
}
