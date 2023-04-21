



namespace TestParser
{
    public class Programm
    {
        private static HttpClient httpClient = new();

        static async Task Main()
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://monerohash.com/api/stats_address?address=49Suh9bksbqE8igcs6u7B42hb4zqtjyfM7TfkRL8s6a9X9oT8sCD7YoA5mRuHtSRUWXdgqXsqhuhiiUekfcMLHwgMbHam2Z&longpoll=true");
            using HttpResponseMessage response = await httpClient.SendAsync(request);
             
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("headers");

            foreach (var header in response.Headers)
            {
                Console.WriteLine(header.Key);
                foreach (var headerValue in header.Value)
                {
                    Console.WriteLine(headerValue);
                }
            }
            Console.WriteLine("\nContent");
            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
    }
}
