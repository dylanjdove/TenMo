using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly AccountService accountService = new AccountService();
        private static readonly TransferService transferService = new TransferService();


        static void Main(string[] args)
        {
            Run();
        }
        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int userID = UserService.GetUserId();
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    decimal balance = accountService.GetAccount(userID).Balance;
                    Console.WriteLine($"Your current account balance is: ${balance}");
                }
                else if (menuSelection == 2)
                {
                    int newSelection;
                    List<API_Transfer> transfers = transferService.ListTransfers(userID);
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("Transfers");
                    Console.WriteLine("ID\tFrom/To\tAmount");
                    Console.WriteLine("--------------------------------------");
                    foreach (API_Transfer transfer in transfers)
                    {
                        if(transfer.AccountFrom == userID)
                        {
                            Console.WriteLine($"{transfer.TransferID}\tTo:\t{transfer.UserToName}\t$\t{transfer.Amount}");
                        }
                        else
                        {
                            Console.WriteLine($"{transfer.TransferID}\tFrom:\t{transfer.UserFromName}\t$\t{transfer.Amount}");
                        }                        
                    }
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("Please enter transfer ID to view details (0 to cancel): ");
                    while(!int.TryParse(Console.ReadLine(), out newSelection))
                    {
                        Console.WriteLine("Invalid input please enter only a valid number.");
                    }
                    if(newSelection != 0)
                    {
                        API_Transfer transfer = transferService.GetTransfer(newSelection);
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine("Transfer Details");
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine($"Id: {transfer.TransferID}");
                        Console.WriteLine($"From:  {transfer.UserFromName}");
                        Console.WriteLine($"To:  {transfer.UserToName}");
                        Console.WriteLine($"Type: {transfer.TransferType}");
                        Console.WriteLine($"Status:  {transfer.TransferStatus}");
                        Console.WriteLine($"Amount:  ${transfer.Amount}");
                        Console.WriteLine("--------------------------------------");
                    }
                }
                else if (menuSelection == 3)
                {
                    int newSelection;
                    List<API_Transfer> transfers = transferService.ListPendingTransfers(userID);
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("Pending Transfers");
                    Console.WriteLine("ID\tTo\tAmount");
                    Console.WriteLine("--------------------------------------");
                    foreach (API_Transfer transfer in transfers)
                    {
                        Console.WriteLine($"{transfer.TransferID}\t{transfer.UserToName}\t${transfer.Amount}");
                        Console.WriteLine("--------------------------------------");
                    }
                    Console.WriteLine("Please enter transfer ID to approve/reject (0 to cancel): ");
                    while (!int.TryParse(Console.ReadLine(), out newSelection))
                    {
                        Console.WriteLine("Invalid input please enter only a valid number.");
                    }
                    if(newSelection != 0)
                    {
                        int approveOrReject;
                        API_Transfer transfer = transferService.GetTransfer(newSelection);
                        Console.WriteLine("1: Approve");
                        Console.WriteLine("2: Reject");
                        Console.WriteLine("0: Don't approve or reject");
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine("Please choose an option: ");
                        while (!int.TryParse(Console.ReadLine(), out approveOrReject))
                        {
                            Console.WriteLine("Invalid input please enter only a valid number.");
                        }
                        if (approveOrReject == 1)
                        {
                            API_Account accountFrom = accountService.GetAccount(transfer.AccountFrom);
                            if(transfer.Amount < accountFrom.Balance)
                            {
                                transfer = transferService.UpdateTransfer(transfer, 2);
                                accountFrom.Balance -= transfer.Amount;
                                accountService.UpdateBalance(accountFrom);

                                API_Account accountTo = accountService.GetAccount(transfer.AccountTo);
                                accountTo.Balance += transfer.Amount;
                                accountService.UpdateBalance(accountTo);
                                Console.WriteLine("Transfer approved!");
                            }
                            else Console.WriteLine("You do not have enough money to approve this transfer");

                        }
                        else if (approveOrReject == 2)
                        {
                            transfer = transferService.UpdateTransfer(transfer, 3);
                            Console.WriteLine("Transfer rejected!");
                        }
                    }
                }
                else if (menuSelection == 4)
                {
                    API_Account accountFrom = accountService.GetAccount(userID);
                    API_Transfer transfer = consoleService.PromptForUserToTransfer(userID, accountFrom.Balance);

                    if (transfer != null)
                    {
                        transferService.SendTransfer(transfer);

                        accountFrom.Balance -= transfer.Amount;
                        accountService.UpdateBalance(accountFrom);

                        API_Account accountTo = accountService.GetAccount(transfer.AccountTo);
                        accountTo.Balance += transfer.Amount;
                        accountService.UpdateBalance(accountTo);
                        Console.WriteLine("Transfer successful");
                    }

                }
                else if (menuSelection == 5)
                {
                    API_Account accountTo = accountService.GetAccount(userID);
                    API_Transfer transfer = consoleService.PromptForUserToRequestTransfer(userID);

                    if (transfer != null)
                    {
                        transferService.SendTransfer(transfer);
                        Console.WriteLine("Transfer pending");
                    }
                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
