using TenmoClient.Data;
using System.Collections.Generic;
using RestSharp;

namespace TenmoClient
{
    public static class UserService
    {
        private static API_User user = new API_User();
        private static readonly IRestClient client = new RestClient();

        public static void SetLogin(API_User u)
        {
            user = u;
        }

        public static int GetUserId()
        {
            return user.UserId;
        }

        public static bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(user.Token);
        }

        public static string GetToken()
        {
            return user?.Token ?? string.Empty;
        }
    }
}
