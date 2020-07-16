using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Data;

namespace TenmoClient
{
    public class AccountService
    {
        private readonly string API_BASE_URL = "https://localhost:44315";
        private readonly IRestClient client = new RestClient();

        public API_Account GetAccount(int id)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}/account/{id}");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<API_Account> response = client.Get<API_Account>(request);
            
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Error occured communicating with the server.");
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("Error was recieved from the server. The status code is " + (int)response.StatusCode);
            }

            API_Account account = response.Data;
            return account;
        }

        public API_Account UpdateBalance(API_Account account)
        {
            int id = account.UserId;
            RestRequest request = new RestRequest($"{API_BASE_URL}/account/{id}");
            request.AddJsonBody(account);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<API_Account> response = client.Put<API_Account>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Error occured communicating with the server.");
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("Error was recieved from the server. The status code is " + (int)response.StatusCode);
            }
            return response.Data;
        }

        public List<API_Account> GetAccounts()
        {
            List<API_Account> accounts = new List<API_Account>();
            RestRequest request = new RestRequest($"{API_BASE_URL}/account");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<List<API_Account>> response = client.Get<List<API_Account>>(request);
            accounts = response.Data;
            return accounts;
        }
    }
}
