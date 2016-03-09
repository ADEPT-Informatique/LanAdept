using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptCore.Service.Challonge
{
    /// <summary>
    /// A service that sends requests to an API
    /// </summary>
    /// <remarks>
    /// In each query, the service add the ApiKey
    /// </remarks>
    public static class HttpService
    {
        /// <summary>
        /// Gets or sets the authentication key of api
        /// </summary>
        public static string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the base address of api
        /// </summary>
        public static Uri BaseAddress { get; set; }

        static HttpService()
        {
            ApiKey = "IzhNqaRgUvjHbQb4iiXCk3f76AsTyFwZod4R6eeM";
            BaseAddress = new Uri("https://api.challonge.com/v1/");
        }

        /// <summary>
        /// Send a Get query 
        /// </summary>
        /// <param name="url">Url query</param>
        /// <param name="filter">The additional parameters</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Get(string url, string filter)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = BaseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string request = url + "?api_key=" + ApiKey + filter;
                return await client.GetAsync(request);
            }
        }

        /// <summary>
        /// Send a Put query
        /// </summary>
        /// <param name="url">Url query</param>
        /// <param name="data">The information in the query</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Put(string url, JObject data)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = BaseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                data.Add("api_key", ApiKey);

                return await client.PutAsJsonAsync(url, data);
            }
        }

        /// <summary>
        /// Send a Post query
        /// </summary>
        /// <param name="url">Url query</param>
        /// <param name="data">The information in the query</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Post(string url, JObject data)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = BaseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                data.Add("api_key", ApiKey);

                return await client.PostAsJsonAsync(url, data);
            }
        }

        /// <summary>
        /// Send a Delete query
        /// </summary>
        /// <param name="url">Url query</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Delete(string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = BaseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string request = url + "?api_key=" + ApiKey;
                return await client.DeleteAsync(request);
            }
        }
    }
}
