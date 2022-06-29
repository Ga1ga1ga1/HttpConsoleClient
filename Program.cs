using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace HttpConsoleClient
{
    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    class Program
    {
        static HttpClient client = new HttpClient();

        static void ShowItem(Item item)
        {
            Console.WriteLine(item.Name);
        }

        static async Task<Uri> CreateItemAsync(Item item)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/items", item);
            response.EnsureSuccessStatusCode();
            return response.Headers.Location;
        }

        static async Task<Item> GetItemAsync(string path)
        {
            Item item = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if(response.IsSuccessStatusCode)
            {
                item = await response.Content.ReadAsAsync<Item>();
            }
            return item;
        }

        static async Task<Item> UpdateItemAsync(Item item)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"api/items/{item.Id}", item);
            response.EnsureSuccessStatusCode();

            item = await response.Content.ReadAsAsync<Item>();
            return item;
        }

        static async Task<HttpStatusCode> DeleteItemAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/items/{id}");
            return response.StatusCode;
        }

        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("сейчас отсутствует");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application.json"));

            try
            {
                Item item = new Item
                {
                    Id = "1",
                    Name = "Cake",
                };

                var url = await CreateItemAsync(item);
                Console.WriteLine($"Created at {url}");

                //GET
                item = await GetItemAsync(url.PathAndQuery);
                ShowItem(item);

                //UPDATE
                Console.WriteLine("Updating item...");
                item.Name = item.Name + item.Name;
                await UpdateItemAsync(item);

                //GET UPDATED
                item = await GetItemAsync(url.PathAndQuery);
                ShowItem(item);

                //DELETE
                var statusCode = await DeleteItemAsync(item.Id);
                Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

            Console.ReadLine();
        }
    }
}
