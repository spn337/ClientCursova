using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceDll.Interfaces;
using ServiceDll.Models;

namespace ServiceDll.Realization
{
    public class ProductApiService : IProductService
    {
        private string _url = "https://localhost:44329/api/product";
        public List<ProductModel> GetProducts()
        {
            //Клієнт посилає запити на API
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;

            //витягуємо дані з сервера по URL
            string data = client.DownloadString(_url);

            // перетворюємо string в object за допомогою json
            var list = JsonConvert.DeserializeObject<List<ProductModel>>(data);
            return list;
        }
        public Task<List<ProductModel>> GetProductsAsync()
        {
            return Task.Run(() => GetProducts());
        }


        public int Create(ProductAddModel product)
        {
            var http = (HttpWebRequest)WebRequest.Create(new Uri(_url));
            // тип відправлення
            http.Accept = "application/json";
            // тип прийому
            http.ContentType = "application/json";
            // тип запиту на сервер
            http.Method = "POST";

            // посилаємо запит
            string parsedContent = JsonConvert.SerializeObject(product);
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            // отримаємо відповідь
            var response = http.GetResponse();

            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();

            return 0;
        }    
        public Task<int> CreateAsync(ProductAddModel product)
        {
            return Task.Run(() => Create(product));
        }
    }
}
