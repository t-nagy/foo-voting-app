using SharedLibrary;

namespace CounterAPI
{
    public class ConfigHelper
    {
        private readonly IConfigurationRoot _config;
        public string VoteDataConnectionStringName { 
            get 
            { 
                return AddressService.LocalMode ? "LocalVoteDataConnection" : "VoteDataConnection"; 
            } 
        }

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

        public string GetAPIUsageKey()
        {
            return _config["UsageKey"]!;
        }

        public string GetTransportEncryptionKeyPem()
        {
            return _config["TransportEncryptionKeyPem"]!;
        }
    }
}
