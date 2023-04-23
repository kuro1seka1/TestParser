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
       public string Response (string url)
       {
            var client = new RestClient(url);

            var request = new RestRequest();

            var response = client.Get(request);
            

            return response.Content;

            
       }
    }
}
