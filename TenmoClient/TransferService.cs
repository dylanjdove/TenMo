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
    public class TransferService
    {
        private readonly string API_BASE_URL = "https://localhost:44315";
        private readonly IRestClient client = new RestClient();

        public string UNAUTHORIZED_MSG { get { return "Authorization is required for this endpoint. Please log in."; } }
        public string FORBIDDEN_MSG { get { return "You do not have permission to perform the requested action"; } }
        public string OTHER_4XX_MSG { get { return "Error occurred - received non-success response: "; } }

        public API_Transfer SendTransfer(API_Transfer transfer)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}/transfer");
            request.AddJsonBody(transfer);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<API_Transfer> response = client.Post<API_Transfer>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }

        public List<API_Transfer> ListPendingTransfers(int userId)
        {
            List<API_Transfer> transfers = new List<API_Transfer>();
            RestRequest request = new RestRequest($"{API_BASE_URL}/transfer/pending/{userId}");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<List<API_Transfer>> response = client.Get<List<API_Transfer>>(request);
            transfers = response.Data;
            return transfers;
        }

        public List<API_Transfer> ListTransfers(int userId)
        {
            List<API_Transfer> transfers = new List<API_Transfer>();
            RestRequest request = new RestRequest($"{API_BASE_URL}/transfer/all/{userId}");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<List<API_Transfer>> response = client.Get<List<API_Transfer>>(request);
            transfers = response.Data;
            return transfers;
        }

        public API_Transfer GetTransfer(int id)
        {
            API_Transfer transfer = new API_Transfer();
            RestRequest request = new RestRequest($"{API_BASE_URL}/transfer/{id}");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<API_Transfer> response = client.Get<API_Transfer>(request);
            transfer = response.Data;
            return transfer;
        }

        public API_Transfer UpdateTransfer(API_Transfer transfer, int approveOrReject)
        {
            int transferId = transfer.TransferID;
            transfer.TransferStatusId = approveOrReject;
            RestRequest request = new RestRequest($"{API_BASE_URL}/transfer/{transferId}");
            request.AddJsonBody(transfer);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<API_Transfer> response = client.Put<API_Transfer>(request);
            transfer = response.Data;
            return transfer;
        }

        public string ProcessErrorResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                return "Error occurred - unable to reach server.";
            }
            else if (!response.IsSuccessful)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return UNAUTHORIZED_MSG;
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return FORBIDDEN_MSG;
                }
                else return OTHER_4XX_MSG + (int)response.StatusCode;
            }
            return "";
        }
    }
}
