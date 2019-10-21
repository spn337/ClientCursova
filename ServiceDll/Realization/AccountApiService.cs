using Newtonsoft.Json;
using ServiceDll.Interfaces;
using ServiceDll.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDll.Realization
{
    public class AccountApiService : IAccountService
    {
        private readonly string _url = "https://localhost:44329/api/account";
        public UserModel Login(AccountModel model)
        {
            var http = (HttpWebRequest)WebRequest.Create(new Uri(_url + "/login"));
            // тип відправлення
            http.Accept = "application/json";
            // тип прийому
            http.ContentType = "application/json";
            // тип запиту на сервер
            http.Method = "POST";

            // посилаємо запит
            string parsedContent = JsonConvert.SerializeObject(model);
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            // отримаємо відповідь
            string content = "";
            UserModel newUser = new UserModel();

            try
            {
                var response = http.GetResponse();

                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                content = sr.ReadToEnd();
                var anon = new
                {
                    value = String.Empty
                };
                var tokenObj = JsonConvert.DeserializeAnonymousType(content, anon);

                //var jwtHandler = new JwtSecurityTokenHandler();

                //var token = jwtHandler.ReadJwtToken(tokenObj.value);

                //var jwtHeader = JsonConvert.SerializeObject(token.Header.Select(h => new { h.Key, h.Value }));
                //var jwtOutput = $"{{\r\n\"Header\":\r\n{JToken.Parse(jwtHeader)},";

                //var jwtPayload = JsonConvert.SerializeObject(token.Claims.Select(c => new { c.Type, c.Value }));
                //jwtOutput += $"\r\n\"Payload\":\r\n{JToken.Parse(jwtPayload)}\r\n}}";
            }
            catch (Exception ex)
            {
                if (ex.Message == "The remote server returned an error: (400) Bad Request.")
                {
                    return null;
                }
            }

            return newUser;
        }
        public Task<UserModel> LoginAsync(AccountModel model)
        {
            return Task.Run(() => Login(model));
        }

        public List<string> Registration(UserModel model)
        {
            var http = (HttpWebRequest)WebRequest.Create(new Uri(_url + "/registration"));
            // тип відправлення
            http.Accept = "application/json";
            // тип прийому
            http.ContentType = "application/json";
            // тип запиту на сервер
            http.Method = "POST";

            // посилаємо запит
            string parsedContent = JsonConvert.SerializeObject(model);
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            // отримаємо відповідь
            string content = "";

            try
            {
                var response = http.GetResponse();

                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                content = sr.ReadToEnd();
                var anon = new
                {
                    token = String.Empty
                };
                var tokenObj = JsonConvert.DeserializeAnonymousType(content, anon);
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                {
                    if (ex.Response != null)
                    {
                        using (var errorResponse = (HttpWebResponse)ex.Response)
                        {
                            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                            {
                                string errorsString = reader.ReadToEnd();
                                var errorsObj = JsonConvert.DeserializeObject<List<string>>(errorsString);

                                return errorsObj;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public Task<List<string>> RegistrationAsync(UserModel model)
        {
            return Task.Run(() => Registration(model));
        }
    }
}
