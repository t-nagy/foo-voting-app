namespace AdminAPI
{
    public class ConfigHelper
    {
        private readonly IConfigurationRoot _config;
        public string VoteDbConnectionStringName { get { return "VotingConnection"; } }

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
