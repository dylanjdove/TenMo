using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountsSqlDAO : IAccountsDAO
    {
        private readonly string connectionString;

        public AccountsSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Account GetAccount(int userID)
        {
            Account returnAccount = null;
            try
            {
                using (SqlConnection sql = new SqlConnection(connectionString))
                {
                    sql.Open();

                    SqlCommand sqlCommand = new SqlCommand("SELECT accounts.user_id, username, account_id, balance FROM accounts JOIN users ON users.user_id = accounts.user_id WHERE accounts.user_id = @userID", sql);
                    sqlCommand.Parameters.AddWithValue("@userID", userID);
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    if(reader.HasRows && reader.Read())
                    {
                        returnAccount = GetAccountFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                //Maybe change later
                throw;
            }

            return returnAccount;
        }

        public Account GetAccountFromReader(SqlDataReader reader)
        {
            Account account = new Account()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                AccountID = Convert.ToInt32(reader["account_id"]),
                Balance = Convert.ToDecimal(reader["balance"]),
                Username = Convert.ToString(reader["username"])
            };

            return account;
        }

        public Account UpdateBalance(Account accountToUpdate)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(connectionString))
                {
                    sql.Open();

                    SqlCommand sqlCommand = new SqlCommand("UPDATE accounts SET balance = @balance WHERE account_id = @account_id", sql);
                    sqlCommand.Parameters.AddWithValue("@balance", accountToUpdate.Balance);
                    sqlCommand.Parameters.AddWithValue("@account_id", accountToUpdate.UserId);
                    int numberOfRowsAffected = sqlCommand.ExecuteNonQuery();

                    if (numberOfRowsAffected > 0)
                    {
                        return accountToUpdate;
                    }
                }
            }
            catch (SqlException)
            {
                //Maybe change later
                throw;
            }
            return null;
        }

        public List<Account> GetAccounts()
        {
            List<Account> returnAccounts = new List<Account>();
            try
            {
                using (SqlConnection sql = new SqlConnection(connectionString))
                {
                    sql.Open();

                    SqlCommand sqlCommand = new SqlCommand("SELECT accounts.user_id, username, account_id, balance FROM accounts JOIN users ON users.user_id = accounts.user_id", sql);
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Account newAccount = GetAccountFromReader(reader);
                            returnAccounts.Add(newAccount);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                //Maybe change later
                throw;
            }

            return returnAccounts;
        }
    }
}
