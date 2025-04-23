using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.DataManagers
{
    public class ShufflerApiWakeupManager : ShufflerApiCaller
    {
        public async Task WakeShuffler()
        {
            HttpResponseMessage response;
            try
            {
                var content = new HttpRequestMessage();
                response = await _client.GetAsync("/Wake");
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                return;
            }

            throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage);
        }
    }
}
