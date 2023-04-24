using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace TestParser
{
    public class HttpClient
    {
       public async Task<string> Response(string url)
       {
            var client = new RestClient(url);

            var request = new RestRequest();
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));
            var response = await client.GetAsync(request,cts.Token);

            return response.Content.ToString();

            
       }
    }
}
