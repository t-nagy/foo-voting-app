using SharedLibrary;

namespace AdminAPI
{
    public class ConfigHelper
    {
        private readonly IConfigurationRoot _config;
        public string VoteDbConnectionStringName { 
            get 
            { 
                return AddressService.LocalMode ? "LocalVotingConnection" : "VotingConnection"; 
            } 
        }
        public string IdentityDbConnectionStringName 
        { 
            get 
            { 
                return AddressService.LocalMode ? "LocalIdentityConnection" : "IdentityConnection"; 
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

        public string GetSigningKeyPem()
        {
            return _config["SigningKeyPem"]!;
        }
    }
}
