using System;
using System.Net.Http;

namespace BLL.Helper
{
    public static class Util
    {
        public static string RequestJson(string uri)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(GlobalVariables.WebApiURL + uri);
            request.Method = HttpMethod.Get;//Get Put Post Delete
            request.Headers.Add("Accept", "aplication/json");//we would like JSON as response
            var client = new HttpClient();
            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK) { }

            HttpContent content = response.Content;
            var json = content.ReadAsStringAsync().Result;

            return json;
        }
    }
}