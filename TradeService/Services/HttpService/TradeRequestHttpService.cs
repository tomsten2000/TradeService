using InventorySerivce.Models.Protobufs;
using System.Net;
using System.Text;

namespace TradeService.Services.HttpService
{
    public class TradeRequestHttpService : ITradeRequestHttpService
    {
        private readonly HttpClient __httpClient;

        public TradeRequestHttpService(HttpClient httpClient)
        {
            __httpClient = httpClient;
        }

        public async Task SendTradeRequestAsync(SendTradeDto trade)
        {
            try
            {
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler
                {
                    CookieContainer = cookieContainer,
                    UseCookies = true
                };

                // Add cookies
                cookieContainer.Add(new Uri("https://steamcommunity.com"), new Cookie("sessionid", "db27969699439376a2280274"));
                cookieContainer.Add(new Uri("https://steamcommunity.com"), new Cookie("steamLoginSecure", "76561199501984919||eyAidHlwIjogIkpXVCIsICJhbGciOiAiRWREU0EiIH0.eyAiaXNzIjogInI6MTBCMV8yNTI2MTkwQl9FMUUzMSIsICJzdWIiOiAiNzY1NjExOTk1MDE5ODQ5MTkiLCAiYXVkIjogWyAid2ViOmNvbW11bml0eSIgXSwgImV4cCI6IDE3MjgwNzExODIsICJuYmYiOiAxNzE5MzQzNTYxLCAiaWF0IjogMTcyNzk4MzU2MSwgImp0aSI6ICIxMEFFXzI1MjYxOTA2XzIwNjg2IiwgIm9hdCI6IDE3Mjc5ODM1NjAsICJydF9leHAiOiAxNzQ2NTM4MTYwLCAicGVyIjogMCwgImlwX3N1YmplY3QiOiAiNjIuMTk4LjIwNi4yNSIsICJpcF9jb25maXJtZXIiOiAiNjIuMTk4LjIwNi4yNSIgfQ.I1ArQRZIfXq2GnAZYvVUGHEPyljtuuBqPg1WI2H-hcJbrpZVvJ3oG1Jv1q5e2s9qOiAzy_vF-ahlbD5-GD1IBg"));

                // Create a separate HttpClient with the custom handler
                using var _httpClient = new HttpClient(handler);

                // The URL for the POST request
                string url = "https://steamcommunity.com/tradeoffer/new/send";

                // Set headers (these headers are required for the request)
                _httpClient.DefaultRequestHeaders.Clear(); // Clear any default headers
                _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                _httpClient.DefaultRequestHeaders.Add("Accept-Language", "da-DK,da;q=0.9");
                _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                _httpClient.DefaultRequestHeaders.Add("Origin", "https://steamcommunity.com");
                _httpClient.DefaultRequestHeaders.Add("Referer", "https://steamcommunity.com/tradeoffer/new/");

                // Prepare the body content (assuming 'message' is a part of the request, adapt as needed)
                string assets = "";
                foreach (SendTradeItemDto item in trade.Items)
                {
                    assets += "{\"appid\":730,\"contextid\":\"2\",\"amount\":1,\"assetid\":\""+ item.ItemSteamId +"\"}";
                }
                var formData = new Dictionary<string, string>
                {
                    { "sessionid", "db27969699439376a2280274" },
                    { "serverid", "1" },
                    { "partner", "76561198048620998" },
                    { "tradeoffermessage", trade.TradeId.ToString() },
                    { "json_tradeoffer", "{\"newversion\":true,\"version\":2,\"me\":{\"assets\":[],\"currency\":[],\"ready\":false},\"them\":{\"assets\":[" + assets + "],\"currency\":[],\"ready\":false}}" },
                    { "captcha", ""},
                    { "trade_offer_create_params", "{}" }
                };
                foreach (var item in formData.Values)
                {
                    Console.WriteLine("Body: " + item);
                }

                // Automatically URL-encode the body content
                var content = new FormUrlEncodedContent(formData);
                // Send the POST request
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                // Check and log the response status and content
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Trade offer sent successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to send trade offer. Status Code: {response.StatusCode}");
                }

            }
            catch (Exception ex) 
            {
                Console.WriteLine("error: " + ex.Message);
            }
        }
    }
}
