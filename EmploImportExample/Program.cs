using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace EmploImportExample
{
    class Program
    {
        static string Login = "example@example.com";
        static string Password = "Ex@mplePa$sw0RD";
        static string EmploUrl = "https://[mycompany].emplo.com";
        static string ApiPath = "apiv2";

        static string ImportUsersUrl => EmploUrl + "/" + ApiPath + "/Users/Import";
        static string FinishImportUrl => EmploUrl + "/" + ApiPath + "/Users/FinishImport";
        static string BlockUserUrl => EmploUrl + "/" + ApiPath + "/Users/Block";
        static string TokenEndpoint => EmploUrl + "/identity/connect/token";

        static void Main(string[] args)
        {
            Console.WriteLine("Tworzenie klienta OAuth2...");
            TokenClient companyTokenClient = new TokenClient(TokenEndpoint, "ResourceOwnerClient", "6D359719-149A-4011-91D4-01CBA687DBBF");

            Console.WriteLine("Logowanie do emplo...");
            TokenResponse token = companyTokenClient.RequestResourceOwnerPasswordAsync(Login, Password,
                "read write offline_access").Result;

            ImportUsersRequestModel model = new ImportUsersRequestModel("CreateOrUpdate", "False")
            {
                Rows = new List<UserDataRow>()
                {
                    new UserDataRow()
                    {
                        { "NameId", "111" },
                        { "Email", "przyklad1@email.com" },
                        { "FirstName", "Konrad" },
                        { "LastName", "Nowacki" },
                        { "Position", "Stanowisko 1" }
                    },
                    new UserDataRow()
                    {
                        { "NameId", "112" },
                        { "Email", "przyklad2@email.com" },
                        { "FirstName", "Agata" },
                        { "LastName", "Kalinowska" },
                        { "Position", "Stanowisko 2" }
                    }
                }
            };

            Console.WriteLine($"Import pracowników {JsonConvert.SerializeObject(model.Rows)} ...");
            var result = Send<ImportUsersResponseModel>(JsonConvert.SerializeObject(model), ImportUsersUrl, token).Result;
            model.ImportId = result.ImportId;
            model.Rows.Clear();
            model.Rows.Add(new UserDataRow()
            {
                { "NameId", "113" },
                { "Email", "przyklad3@email.com" },
                { "FirstName", "Kacper" },
                { "LastName", "Jasiński" },
                { "Position", "Stanowisko 3" }
            });
            Console.WriteLine($"Import pracownika {JsonConvert.SerializeObject(model.Rows)} ...");
            result = Send<ImportUsersResponseModel>(JsonConvert.SerializeObject(model), ImportUsersUrl, token).Result;



            FinishImportRequestModel requestModel = new FinishImportRequestModel("False");
            requestModel.ImportId = result.ImportId;
            Console.WriteLine($"Finalizowanie operacji importu (Id {result.ImportId})...");
            var finishImportResponse = Send<FinishImportResponseModel>(JsonConvert.SerializeObject(requestModel), FinishImportUrl, token).Result;

            if (finishImportResponse.ImportStatusCode == ImportStatusCode.Ok)
            {
                Console.WriteLine("Import zakończony sukcesem");
            }


            string externalIdentifier = "113";
            Console.WriteLine($"Blokowanie dostępu do emplo użytkownikowi o Id {externalIdentifier}...");
            var statusCode = Send<HttpStatusCode>(externalIdentifier, BlockUserUrl, token).Result;
            Console.WriteLine($"Status operacji blokowania: {statusCode}");
            Console.ReadLine();
        }

        private static async Task<T> Send<T>(string json, string url, TokenResponse token)
        {
            Uri uri = new Uri(url);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage message = new HttpRequestMessage() { RequestUri = uri, Content = stringContent, Method = HttpMethod.Post };
            message.Headers.Add("Authorization", "Bearer " + token.AccessToken);
            var httpClient = new HttpClient();
            var response = httpClient.SendAsync(message).Result;

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
    }
}
