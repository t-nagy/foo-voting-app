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
        private const bool localMode = true;

        public static string AdminAddress
        {
            get
            {
                return localMode ? "https://localhost:7119" : "";
            }
        }

        public static string ShufflerAddress
        {
            get
            {
                return localMode ? "https://localhost:7193" : "";
            }
        }

        public static string CounterAddress
        {
            get
            {
                return localMode ? "https://localhost:7009" : "";
            }
        }
    }
}
