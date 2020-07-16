using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    public class ConsoleService
    {
        private static readonly AccountService accountService = new AccountService();
        /// <summary>
        /// Prompts for transfer ID to view, approve, or reject
        /// </summary>
        /// <param name="action">String to print in prompt. Expected values are "Approve" or "Reject" or "View"</param>
        /// <returns>ID of transfers to view, approve, or reject</returns>


        public int PromptForTransferID(string action)
        {
            Console.WriteLine("");
            Console.Write("Please enter transfer ID to " + action + " (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int auctionId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return auctionId;
            }
        }

        public LoginUser PromptForLogin()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            string password = GetPasswordFromConsole("Password: ");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        private string GetPasswordFromConsole(string displayMessage)
        {
            string pass = "";
            Console.Write(displayMessage);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Remove(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return pass;
        }

        public API_Transfer PromptForUserToTransfer(int userId, decimal balance)
        {
            int accountTo = 0;
            API_Transfer transfer = new API_Transfer();
            List<API_Account> accounts = new List<API_Account>();
            accounts = accountService.GetAccounts();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Users");
            Console.WriteLine("ID\tName");
            Console.WriteLine("--------------------------------------");
            foreach(API_Account account in accounts)
            {
                Console.WriteLine($"{account.UserId}\t{account.Username}");
            }
            Console.WriteLine("Enter ID of user you are sending to (0 to cancel): ");
            while(!int.TryParse(Console.ReadLine(), out accountTo))
            {
                Console.WriteLine("Please input a valid account number.");
            }
            transfer.AccountTo = accountTo;
            transfer.AccountFrom = userId;
            transfer.TransferStatus = "Approve";

            Console.WriteLine("Enter amount: ");
            decimal amountToSend = 0;

            while (!decimal.TryParse(Console.ReadLine(), out amountToSend))
            {
                Console.WriteLine("Please input a valid amount.");
            }

            if (amountToSend > balance || amountToSend < 0)
            {
                Console.WriteLine("Transfer failed try again later.");
                return null;
            }

            transfer.Amount = amountToSend;
            transfer.TransferStatusId = 2;
            transfer.TransferType = "Send";
            transfer.TransferTypeId = 2;

            

            return transfer;
        }
        public API_Transfer PromptForUserToRequestTransfer(int userId)
        {
            int accountFrom;
            API_Transfer transfer = new API_Transfer();
            List<API_Account> accounts = new List<API_Account>();
            accounts = accountService.GetAccounts();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Users");
            Console.WriteLine("ID\tName");
            Console.WriteLine("--------------------------------------");
            foreach (API_Account account in accounts)
            {
                Console.WriteLine($"{account.UserId}\t{account.Username}");
            }
            Console.WriteLine("Enter ID of user you are requesting from (0 to cancel): ");
            while (!int.TryParse(Console.ReadLine(), out accountFrom) && accountFrom != 0)
            {
                Console.WriteLine("Please input a valid account number.");
            }
            transfer.AccountFrom = accountFrom;
            transfer.AccountTo = userId;
            transfer.TransferStatus = "Pending";

            Console.WriteLine("Enter amount: ");
            decimal amountRequested = 0;

            while (!decimal.TryParse(Console.ReadLine(), out amountRequested))
            {
                Console.WriteLine("Please input a valid amount.");
            }

            if (amountRequested < 0)
            {
                Console.WriteLine("Transfer failed try again later.");
                return null;
            }

            transfer.Amount = amountRequested;
            transfer.TransferStatusId = 1;
            transfer.TransferType = "Request";
            transfer.TransferTypeId = 1;



            return transfer;
        }
    }
}
