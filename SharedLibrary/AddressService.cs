using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public static class AddressService
    {
        public static bool LocalMode { get; } = false;

        public static string AdminAddress
        {
            get
            {
                return LocalMode ? "https://localhost:7119" : "https://foo-voting-app-admi-api-g6cjaag8c9ead2fe.polandcentral-01.azurewebsites.net";
            }
        }

        public static string ShufflerAddress
        {
            get
            {
                return LocalMode ? "https://localhost:7193" : "https://foo-voting-app-shuffler-api-htftedbadkf9cjh4.polandcentral-01.azurewebsites.net/";
            }
        }

        public static string CounterAddress
        {
            get
            {
                return LocalMode ? "https://localhost:7009" : "https://foo-app-counter-api-dxb6brbkcwafbbe9.polandcentral-01.azurewebsites.net";
            }
        }
    }
}
